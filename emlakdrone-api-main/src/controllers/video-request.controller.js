const VideoRequest = require('../models/VideoRequest');
const User = require('../models/User');
const { BlobServiceClient } = require('@azure/storage-blob');
const { v4: uuidv4 } = require('uuid');
const path = require('path');

// Video talep oluşturma
exports.createRequest = async (req, res) => {
  try {
    const userId = req.userId;
    const { gender, script } = req.body;
    
    // Validasyon
    if (!gender || !script) {
      return res.status(400).json({
        success: false,
        message: 'Cinsiyet ve seslendirme metni gerekli',
        error: 'MISSING_PARAMETERS'
      });
    }

    if (!['male', 'female'].includes(gender)) {
      return res.status(400).json({
        success: false,
        message: 'Geçersiz cinsiyet seçimi',
        error: 'INVALID_GENDER'
      });
    }

    if (!req.files || req.files.length < 3) {
      return res.status(400).json({
        success: false,
        message: 'En az 3 fotoğraf gerekli',
        error: 'INSUFFICIENT_IMAGES'
      });
    }

    if (req.files.length > 6) {
      return res.status(400).json({
        success: false,
        message: 'En fazla 6 fotoğraf gönderebilirsiniz',
        error: 'TOO_MANY_IMAGES'
      });
    }

    if (script.length > 1000) {
      return res.status(400).json({
        success: false,
        message: 'Seslendirme metni 1000 karakterden uzun olamaz',
        error: 'SCRIPT_TOO_LONG'
      });
    }

    // Kullanıcı kontrolü
    const user = await User.findById(userId);
    if (!user) {
      return res.status(404).json({
        success: false,
        message: 'Kullanıcı bulunamadı',
        error: 'USER_NOT_FOUND'
      });
    }

    // Fotoğrafları Azure Storage'a yükle
    const uploadedImages = await uploadImagesToStorage(req.files, userId);

    // Video talep kaydını oluştur
    const videoRequest = new VideoRequest({
      userId: userId,
      gender: gender,
      script: script.trim(),
      images: uploadedImages,
      status: 'pending'
    });

    await videoRequest.save();

    console.log('✅ Video talep oluşturuldu:', {
      requestId: videoRequest._id,
      userId: userId,
      imageCount: uploadedImages.length,
      gender: gender,
      scriptLength: script.length
    });

    return res.status(201).json({
      success: true,
      message: 'Video talebiniz başarıyla oluşturuldu',
      data: {
        requestId: videoRequest._id,
        status: 'pending',
        estimatedCompletion: '24 saat içerisinde'
      }
    });

  } catch (error) {
    console.error('❌ Video talep oluşturma hatası:', error);
    return res.status(500).json({
      success: false,
      message: 'Video talebi oluşturulamadı',
      error: 'REQUEST_FAILED'
    });
  }
};

// Kullanıcının video taleplerini getir
exports.getMyRequests = async (req, res) => {
  try {
    const userId = req.userId;
    
    const requests = await VideoRequest.find({ userId })
      .sort({ createdAt: -1 })
      .select('-images') // Resim detaylarını dahil etme
      .limit(20);

    return res.json({
      success: true,
      requests: requests.map(request => ({
        _id: request._id,
        status: request.status,
        gender: request.gender,
        script: request.script,
        imageCount: request.imageCount,
        createdAt: request.createdAt,
        completedAt: request.completedAt,
        videoUrl: request.videoUrl,
        adminNotes: request.adminNotes
      }))
    });

  } catch (error) {
    console.error('❌ Video talepleri getirme hatası:', error);
    return res.status(500).json({
      success: false,
      message: 'Video talepleri alınamadı',
      error: 'FETCH_FAILED'
    });
  }
};

// Video talep durumunu kontrol et
exports.getRequestStatus = async (req, res) => {
  try {
    const { requestId } = req.params;
    const userId = req.userId;
    
    const request = await VideoRequest.findOne({ 
      _id: requestId, 
      userId 
    });

    if (!request) {
      return res.status(404).json({
        success: false,
        message: 'Video talebi bulunamadı',
        error: 'REQUEST_NOT_FOUND'
      });
    }

    return res.json({
      success: true,
      request: {
        _id: request._id,
        status: request.status,
        gender: request.gender,
        script: request.script,
        imageCount: request.imageCount,
        createdAt: request.createdAt,
        completedAt: request.completedAt,
        videoUrl: request.videoUrl,
        adminNotes: request.adminNotes
      }
    });

  } catch (error) {
    console.error('❌ Video talep durum kontrolü hatası:', error);
    return res.status(500).json({
      success: false,
      message: 'Video talep durumu kontrol edilemedi',
      error: 'STATUS_CHECK_FAILED'
    });
  }
};

// Fotoğrafları Azure Storage'a yükleme
const uploadImagesToStorage = async (files, userId) => {
  try {
    const blobServiceClient = BlobServiceClient.fromConnectionString(
      process.env.AZURE_STORAGE_CONNECTION_STRING
    );
    const containerClient = blobServiceClient.getContainerClient('photos');
    
    // Container'ı oluştur (yoksa)
    await containerClient.createIfNotExists();

    const uploadedImages = [];

    for (let i = 0; i < files.length; i++) {
      const file = files[i];
      const uniqueId = uuidv4();
      const fileName = `${userId}/${uniqueId}-${file.originalname}`;
      
      const blockBlobClient = containerClient.getBlockBlobClient(fileName);
      
      await blockBlobClient.upload(file.buffer, file.buffer.length, {
        blobHTTPHeaders: {
          blobContentType: file.mimetype
        }
      });

      uploadedImages.push({
        filename: fileName,
        originalName: file.originalname,
        mimetype: file.mimetype,
        size: file.size
      });

      console.log(`✅ Fotoğraf yüklendi: ${fileName}`);
    }

    return uploadedImages;

  } catch (error) {
    console.error('❌ Fotoğraf yükleme hatası:', error);
    throw error;
  }
};
