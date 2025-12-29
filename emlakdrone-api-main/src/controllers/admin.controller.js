const VideoRequest = require('../models/VideoRequest');
const User = require('../models/User');
const jwt = require('jsonwebtoken');
const { BlobServiceClient } = require('@azure/storage-blob');

// Admin giriş
exports.login = async (req, res) => {
  try {
    const { username, password } = req.body;
    
    // Basit admin kontrolü (gerçek uygulamada daha güvenli olmalı)
    const adminUsername = process.env.ADMIN_USERNAME || 'admin';
    const adminPassword = process.env.ADMIN_PASSWORD || 'adminpass';
    
    if (username === adminUsername && password === adminPassword) {
      const token = jwt.sign(
        { 
          userId: 'admin',
          role: 'admin',
          username: username 
        },
        process.env.JWT_SECRET || 'supersecretjwtkey',
        { expiresIn: '24h' }
      );

      return res.json({
        success: true,
        message: 'Admin girişi başarılı',
        token: token
      });
    } else {
      return res.status(401).json({
        success: false,
        message: 'Geçersiz admin bilgileri',
        error: 'INVALID_CREDENTIALS'
      });
    }
  } catch (error) {
    console.error('❌ Admin giriş hatası:', error);
    return res.status(500).json({
      success: false,
      message: 'Admin girişi başarısız',
      error: 'LOGIN_FAILED'
    });
  }
};

// Video taleplerini listele
exports.getVideoRequests = async (req, res) => {
  try {
    const { status, page = 1, limit = 20 } = req.query;
    
    const query = {};
    if (status) {
      query.status = status;
    }

    const requests = await VideoRequest.find(query)
      .populate('userId', 'firstName lastName email phone')
      .sort({ createdAt: -1 })
      .limit(limit * 1)
      .skip((page - 1) * limit);

    const total = await VideoRequest.countDocuments(query);

    return res.json({
      success: true,
      requests: requests.map(request => ({
        _id: request._id,
        user: {
          name: `${request.userId.firstName} ${request.userId.lastName}`,
          email: request.userId.email,
          phone: request.userId.phone
        },
        status: request.status,
        gender: request.gender,
        script: request.script,
        imageCount: request.imageCount,
        createdAt: request.createdAt,
        completedAt: request.completedAt,
        videoUrl: request.videoUrl,
        adminNotes: request.adminNotes
      })),
      pagination: {
        current: parseInt(page),
        pages: Math.ceil(total / limit),
        total: total
      }
    });

  } catch (error) {
    console.error('❌ Video talepleri listeleme hatası:', error);
    return res.status(500).json({
      success: false,
      message: 'Video talepleri alınamadı',
      error: 'FETCH_FAILED'
    });
  }
};

// Video talep detayını getir
exports.getVideoRequestDetail = async (req, res) => {
  try {
    const { requestId } = req.params;
    
    const request = await VideoRequest.findById(requestId)
      .populate('userId', 'firstName lastName email phone');

    if (!request) {
      return res.status(404).json({
        success: false,
        message: 'Video talebi bulunamadı',
        error: 'REQUEST_NOT_FOUND'
      });
    }

    // Fotoğrafları Azure Storage'dan al
    const imageUrls = await getImageUrls(request.images);

    return res.json({
      success: true,
      request: {
        _id: request._id,
        user: {
          name: `${request.userId.firstName} ${request.userId.lastName}`,
          email: request.userId.email,
          phone: request.userId.phone
        },
        status: request.status,
        gender: request.gender,
        script: request.script,
        images: imageUrls,
        createdAt: request.createdAt,
        completedAt: request.completedAt,
        videoUrl: request.videoUrl,
        adminNotes: request.adminNotes
      }
    });

  } catch (error) {
    console.error('❌ Video talep detayı getirme hatası:', error);
    return res.status(500).json({
      success: false,
      message: 'Video talep detayı alınamadı',
      error: 'FETCH_FAILED'
    });
  }
};

// Video talep durumunu güncelle
exports.updateRequestStatus = async (req, res) => {
  try {
    const { requestId } = req.params;
    const { status, adminNotes } = req.body;
    
    if (!['pending', 'processing', 'completed', 'cancelled'].includes(status)) {
      return res.status(400).json({
        success: false,
        message: 'Geçersiz durum',
        error: 'INVALID_STATUS'
      });
    }

    const request = await VideoRequest.findByIdAndUpdate(
      requestId,
      { 
        status: status,
        adminNotes: adminNotes || null,
        completedAt: status === 'completed' ? new Date() : null
      },
      { new: true }
    );

    if (!request) {
      return res.status(404).json({
        success: false,
        message: 'Video talebi bulunamadı',
        error: 'REQUEST_NOT_FOUND'
      });
    }

    console.log('✅ Video talep durumu güncellendi:', {
      requestId: request._id,
      status: status,
      adminNotes: adminNotes
    });

    return res.json({
      success: true,
      message: 'Video talep durumu güncellendi',
      request: {
        _id: request._id,
        status: request.status,
        adminNotes: request.adminNotes,
        completedAt: request.completedAt
      }
    });

  } catch (error) {
    console.error('❌ Video talep durum güncelleme hatası:', error);
    return res.status(500).json({
      success: false,
      message: 'Video talep durumu güncellenemedi',
      error: 'UPDATE_FAILED'
    });
  }
};

// Video URL'i ekle
exports.addVideoUrl = async (req, res) => {
  try {
    const { requestId } = req.params;
    const { videoUrl, adminNotes } = req.body;
    
    if (!videoUrl) {
      return res.status(400).json({
        success: false,
        message: 'Video URL gerekli',
        error: 'MISSING_VIDEO_URL'
      });
    }

    const request = await VideoRequest.findByIdAndUpdate(
      requestId,
      { 
        videoUrl: videoUrl,
        status: 'completed',
        adminNotes: adminNotes || null,
        completedAt: new Date()
      },
      { new: true }
    ).populate('userId', 'firstName lastName email phone');

    if (!request) {
      return res.status(404).json({
        success: false,
        message: 'Video talebi bulunamadı',
        error: 'REQUEST_NOT_FOUND'
      });
    }

    // Kullanıcıya bildirim gönder (opsiyonel)
    // await sendVideoCompletedNotification(request.userId._id, request._id);

    console.log('✅ Video URL eklendi:', {
      requestId: request._id,
      videoUrl: videoUrl,
      userId: request.userId._id
    });

    return res.json({
      success: true,
      message: 'Video URL başarıyla eklendi',
      request: {
        _id: request._id,
        status: request.status,
        videoUrl: request.videoUrl,
        adminNotes: request.adminNotes,
        completedAt: request.completedAt
      }
    });

  } catch (error) {
    console.error('❌ Video URL ekleme hatası:', error);
    return res.status(500).json({
      success: false,
      message: 'Video URL eklenemedi',
      error: 'ADD_VIDEO_FAILED'
    });
  }
};

// Fotoğraf URL'lerini Azure Storage'dan al
const getImageUrls = async (images) => {
  try {
    const blobServiceClient = BlobServiceClient.fromConnectionString(
      process.env.AZURE_STORAGE_CONNECTION_STRING
    );
    const containerClient = blobServiceClient.getContainerClient('photos');

    const imageUrls = [];
    
    for (const image of images) {
      const blobClient = containerClient.getBlobClient(image.filename);
      const url = blobClient.url;
      imageUrls.push({
        filename: image.filename,
        originalName: image.originalName,
        url: url
      });
    }

    return imageUrls;
  } catch (error) {
    console.error('❌ Fotoğraf URL alma hatası:', error);
    return [];
  }
};
