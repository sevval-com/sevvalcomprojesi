const axios = require('axios');
const User = require('../models/User');
const { BlobServiceClient } = require('@azure/storage-blob');
const Video = require('../models/Video');
const { sendVideoCompletedNotification } = require('./notification.controller');

exports.generateVideo = async (req, res) => {
  try {
    const { videoId, description, language, propertyDetails, pushToken } = req.body;
    const userId = req.userId;

    // Validasyon
    if (!videoId || !description || !language || !propertyDetails || !pushToken) {
      return res.status(400).json({
        success: false,
        message: 'Eksik parametreler',
        error: 'MISSING_PARAMETERS'
      });
    }

    // PropertyDetails validasyonu
    if (!propertyDetails.il || !propertyDetails.ilce) {
      return res.status(400).json({
        success: false,
        message: 'İl ve ilçe bilgileri eksik',
        error: 'MISSING_LOCATION_INFO'
      });
    }

    // Kullanıcıyı kontrol et
    const user = await User.findById(userId);
    if (!user) {
      return res.status(404).json({
        success: false,
        message: 'Kullanıcı bulunamadı',
        error: 'USER_NOT_FOUND'
      });
    }

    // Kullanıcının video oluşturma hakkını kontrol et
    if (!user.canCreateVideo()) {
      return res.status(403).json({
        success: false,
        message: 'Video oluşturma hakkınız kalmadı',
        error: 'NO_VIDEO_RIGHTS'
      });
    }

    // Dil kontrolü
    if (!['tr', 'ru', 'ar'].includes(language)) {
      return res.status(400).json({
        success: false,
        message: 'Geçersiz dil seçimi. Desteklenen diller: tr, ru, ar',
        error: 'INVALID_LANGUAGE'
      });
    }

    // Video service'e istek gönder
    console.log('🎯 Video generation request:', {
      videoId,
      userId,
      language,
      propertyDetails: {
        il: propertyDetails.il,
        ilce: propertyDetails.ilce,
        mahalle: propertyDetails.mahalle,
        mevkii: propertyDetails.mevkii,
        adaNo: propertyDetails.adaNo
      }
    });

    // Hemen başladı bilgisi dön
    res.status(202).json({
      success: true,
      message: 'Video oluşturma işlemi başlatıldı',
      videoId: videoId,
      status: 'processing'
    });

    // Arka planda video oluşturma isteğini gönder
    try {
      const videoResponse = await axios.post(
        'http://20.215.34.129:8291/api/video/process-video',
        {
          videoId,
          description,
          language,
          userId,
          propertyDetails: {
            ...propertyDetails,
            il: propertyDetails.il?.trim(),
            ilce: propertyDetails.ilce?.trim()
          },
          pushToken
        },
        {
          headers: {
            'x-api-key': process.env.VIDEO_SERVICE_API_KEY,
            'Content-Type': 'application/json'
          }
        }
      );
      console.log('✅ Video service response:', videoResponse.data);
    } catch (videoError) {
      console.error('❌ Video service error:', {
        message: videoError.message,
        response: videoError.response?.data,
        status: videoError.response?.status,
        code: videoError.code,
        propertyDetails: propertyDetails
      });
    }

    // Hak tüketimi / sayaç güncellemesi (başlatma sonrası)
    try {
      await user.incrementVideoCount();
    } catch (countError) {
      console.warn('Video sayacı artırılamadı:', countError?.message || countError);
    }

  } catch (error) {
    console.error('❌ Video request error:', error);
    return res.status(500).json({
      success: false,
      message: 'Video oluşturma isteği başlatılamadı',
      error: 'REQUEST_FAILED'
    });
  }
};

// Video durumunu kontrol et
exports.checkVideoStatus = async (req, res) => {
  try {
    const { videoId } = req.params;
    
    if (!videoId) {
      return res.status(400).json({
        success: false,
        message: 'Video ID gerekli'
      });
    }

    // Video kaydını kontrol et
    const video = await Video.findById(videoId);
    if (!video) {
      return res.status(404).json({
        success: false,
        message: 'Video kaydı bulunamadı'
      });
    }

    // Blob kontrolü
    try {
      // Blob kontrolü yapın
      const blobExists = await checkBlobExists(video.blobPath);
      
      if (!blobExists && video.status === 'completed') {
        // Blob yoksa durumu güncelle
        video.status = 'failed';
        await video.save();
      }
    } catch (blobError) {
      console.error('Blob kontrolü hatası:', blobError);
    }

    // Eğer video tamamlandıysa ve daha önce bildirim gönderilmediyse
    if (video.status === 'completed' && !video.notificationSent) {
      console.log('🔔 Video tamamlandı, bildirim gönderiliyor:', {
        videoId: video._id,
        userId: video.userId
      });

      const notificationSent = await sendVideoCompletedNotification(
        video.userId,
        video._id,
        video.propertyDetails
      );

      if (notificationSent) {
        video.notificationSent = true;
        await video.save();
        console.log('✅ Video bildirimi gönderildi ve kaydedildi');
      }
    }

    return res.status(200).json({
      success: true,
      status: video.status,
      isNewVideo: video.isNewVideo,
      message: getStatusMessage(video.status)
    });

  } catch (error) {
    console.error('Video durum kontrolü hatası:', error);
    return res.status(500).json({
      success: false,
      message: 'Sunucu hatası'
    });
  }
};

// Yardımcı fonksiyonlar
const deleteUserVideos = async (userId, containerClient) => {
  try {
    console.log('🗑️ Kullanıcı videoları siliniyor:', userId);
    const userPrefix = `${userId}/`;
    let deletedCount = 0;

    for await (const blob of containerClient.listBlobsFlat({ prefix: userPrefix })) {
      if (!blob.deleted) {
        const blobClient = containerClient.getBlobClient(blob.name);
        await blobClient.delete();
        deletedCount++;
        
        console.log('✅ Video silindi:', {
          name: blob.name,
          userId,
          createdOn: blob.properties.createdOn
        });
      }
    }

    console.log('🗑️ Video silme tamamlandı:', {
      userId,
      deletedCount
    });

    return deletedCount;
  } catch (error) {
    console.error('❌ Video silme hatası:', {
      error: error.message,
      stack: error.stack,
      userId
    });
    throw error;
  }
};

const resetUserVideoCount = async (userId) => {
  try {
    const user = await User.findById(userId);
    if (!user) {
      console.warn('⚠️ Video sayacı sıfırlanamadı - Kullanıcı bulunamadı:', userId);
      return false;
    }

    const oldCount = user.videoCount;
    user.videoCount = 0;
    user.lastVideoResetDate = new Date();
    await user.save();

    console.log('🔄 Video sayacı sıfırlandı:', {
      userId,
      oldCount,
      newCount: 0,
      resetDate: user.lastVideoResetDate
    });

    return true;
  } catch (error) {
    console.error('❌ Sayaç sıfırlama hatası:', {
      error: error.message,
      stack: error.stack,
      userId
    });
    throw error;
  }
};

// Kullanıcının videolarını getir
exports.getMyVideos = async (req, res) => {
  try {
    const userId = req.userId;
    
    // Kullanıcı kontrolü
    const user = await User.findById(userId);
    if (!user) {
      return res.status(404).json({
        success: false,
        message: 'Kullanıcı bulunamadı',
        error: 'USER_NOT_FOUND'
      });
    }

    // Azure Storage bağlantı bilgilerini kontrol et
    if (!process.env.AZURE_STORAGE_CONNECTION_STRING) {
      console.error('❌ Azure Storage connection string bulunamadı');
      return res.status(500).json({
        success: false,
        message: 'Storage yapılandırması eksik',
        error: 'STORAGE_CONFIG_MISSING'
      });
    }

    // Azure Blob Storage bağlantısı
    const blobServiceClient = BlobServiceClient.fromConnectionString(
      process.env.AZURE_STORAGE_CONNECTION_STRING
    );
    const containerClient = blobServiceClient.getContainerClient('videos');

    // Container'ın varlığını kontrol et
    console.log('🔍 Container kontrolü yapılıyor...');
    const containerExists = await containerClient.exists();
    if (!containerExists) {
      console.error('❌ Container bulunamadı: videos');
      return res.status(500).json({
        success: false,
        message: 'Video container\'ı bulunamadı',
        error: 'CONTAINER_NOT_FOUND'
      });
    }

    // Üyelik durumu kontrolü
    const now = new Date();
    const thirtyDaysAgo = new Date(now.getTime() - (30 * 24 * 60 * 60 * 1000));
    let videosDeleted = false;
    let countReset = false;

    // Üyelik aktif değilse veya süresi dolduysa
    if (!user.isMembership || (user.membershipExpireDate && user.membershipExpireDate < now)) {
      console.log('⚠️ Üyelik durumu değişikliği:', {
        userId,
        isMembership: user.isMembership,
        membershipType: user.membershipType,
        expireDate: user.membershipExpireDate,
        action: 'video_cleanup'
      });
      try {
        await deleteUserVideos(userId, containerClient);
        await resetUserVideoCount(userId);
        videosDeleted = true;
        countReset = true;
      } catch (error) {
        console.error('❌ Üyelik sonu temizleme hatası:', error);
      }
    }
    // Yıllık üyelikte 30 gün kontrolü
    else if (user.membershipType === 'yearly' && user.lastVideoResetDate < thirtyDaysAgo) {
      console.log('⚠️ Yıllık üyelik periyodu doldu:', {
        userId,
        membershipType: user.membershipType,
        lastReset: user.lastVideoResetDate,
        action: 'period_cleanup'
      });
      try {
        await deleteUserVideos(userId, containerClient);
        await resetUserVideoCount(userId);
        videosDeleted = true;
        countReset = true;
      } catch (error) {
        console.error('❌ 30 gün temizleme hatası:', error);
      }
    }

    // Video listesini getir
    const videos = [];
    const userPrefix = `${userId}/`;
    console.log('🔍 Aranan prefix:', userPrefix);
    
    try {
      // Blob'ları listele
      console.log('📂 Blob\'lar listeleniyor...');
      let blobCount = 0;
      
      for await (const blob of containerClient.listBlobsFlat({ prefix: userPrefix })) {
        blobCount++;
        console.log('📄 Blob bulundu:', {
          name: blob.name,
          size: blob.properties.contentLength,
          createdOn: blob.properties.createdOn,
          deleted: blob.deleted
        });
        
        if (!blob.deleted) {
          if (blob.name.includes('output-')) {
            const videoId = blob.name.split('output-')[1].split('.')[0];
            const downloadUrl = `${containerClient.url}/${blob.name}`;
            
            // Blob'dan metadata bilgisini al
            const blobClient = containerClient.getBlobClient(blob.name);
            const properties = await blobClient.getProperties();
            let propertyDetails = {};
            
            try {
              // Metadata'dan il ve ilce bilgilerini al
              if (properties.metadata && properties.metadata.il && properties.metadata.ilce) {
                propertyDetails = {
                  il: properties.metadata.il,
                  ilce: properties.metadata.ilce
                };
              }
            } catch (metadataError) {
              console.error('❌ Metadata parse hatası:', {
                videoId,
                error: metadataError.message,
                metadata: properties.metadata
              });
            }
            
            videos.push({
              _id: videoId,
              videoId: videoId,
              userId: userId,
              downloadUrl: downloadUrl,
              createdAt: blob.properties.createdOn,
              status: 'completed',
              propertyDetails
            });
          }
        }
      }

      console.log('📊 Blob istatistikleri:', {
        totalBlobs: blobCount,
        validVideos: videos.length,
        userId: userId
      });

      return res.json({
        success: true,
        videos: videos.sort((a, b) => b.createdAt - a.createdAt),
        nextResetDate: calculateNextResetDate(user),
        videosDeleted,
        countReset
      });

    } catch (blobError) {
      console.error('❌ Blob listing error:', {
        error: blobError.message,
        code: blobError.code,
        userId
      });
      throw new Error('BLOB_LISTING_FAILED');
    }

  } catch (error) {
    console.error('❌ Get my videos error:', {
      error: error.message,
      stack: error.stack,
      userId: req.userId
    });
    return res.status(500).json({
      success: false,
      message: 'Video listesi alınamadı',
      error: 'FETCH_FAILED'
    });
  }
};

// Yardımcı fonksiyonlar
const calculateNextResetDate = (user) => {
  if (!user.isMembership) return null;
  
  if (user.membershipType === 'yearly') {
    return new Date(user.lastVideoResetDate.getTime() + (30 * 24 * 60 * 60 * 1000));
  }
  
  return user.membershipExpireDate;
};

// videoCount'u güncelle
exports.updateVideoCount = async (req, res) => {
  try {
    const userId = req.userId;
    const { count } = req.body;

    const user = await User.findById(userId);
    if (!user) {
      return res.status(404).json({
        success: false,
        message: 'Kullanıcı bulunamadı',
        error: 'USER_NOT_FOUND'
      });
    }

    const now = new Date();
    const thirtyDaysAgo = new Date(now.getTime() - (30 * 24 * 60 * 60 * 1000));

    // Üyelik durumu kontrolü
    if (!user.isMembership || (user.membershipExpireDate && user.membershipExpireDate < now)) {
      console.log('⚠️ Üyelik durumu nedeniyle sayaç sıfırlanıyor:', {
        userId,
        isMembership: user.isMembership,
        membershipType: user.membershipType,
        expireDate: user.membershipExpireDate,
        oldCount: user.videoCount,
        action: 'reset_counter'
      });
      user.videoCount = 0;
      user.lastVideoResetDate = now;
      
      console.log('🔄 Üyelik durumu nedeniyle sayaç sıfırlandı:', {
        userId,
        isMembership: user.isMembership,
        expireDate: user.membershipExpireDate
      });
    }
    // Yıllık üyelikte 30 gün kontrolü
    else if (user.membershipType === 'yearly' && user.lastVideoResetDate < thirtyDaysAgo) {
      console.log('⚠️ Periyot dolduğu için sayaç sıfırlanıyor:', {
        userId,
        membershipType: user.membershipType,
        lastReset: user.lastVideoResetDate,
        oldCount: user.videoCount,
        action: 'period_reset'
      });
      user.videoCount = 0;
      user.lastVideoResetDate = now;
      
      console.log('🔄 30 gün dolduğu için sayaç sıfırlandı:', {
        userId,
        oldDate: user.lastVideoResetDate,
        newDate: now
      });
    }
    // Normal güncelleme
    else if (typeof count === 'number' && count >= 0) {
      user.videoCount = count;
      
      console.log('✅ Video sayacı güncellendi:', {
        userId,
        newCount: count
      });
    }

    await user.save();

    return res.json({
      success: true,
      message: 'Video sayacı güncellendi',
      data: {
        videoCount: user.videoCount,
        nextResetDate: calculateNextResetDate(user)
      }
    });

  } catch (error) {
    console.error('❌ Video sayacı güncelleme hatası:', {
      error: error.message,
      stack: error.stack,
      userId: req.userId
    });
    return res.status(500).json({
      success: false,
      message: 'Video sayacı güncellenemedi',
      error: 'UPDATE_FAILED'
    });
  }
}; 