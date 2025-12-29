const User = require('../models/User');
const { Expo } = require('expo-server-sdk');
const admin = require('../config/firebase.config');

// Expo istemcisini oluştur
const expo = new Expo();

// Push token validasyonu için yardımcı fonksiyon
const validatePushToken = (token) => {
  if (!token || typeof token !== 'string') {
    return { isValid: false, message: 'Token geçersiz format' };
  }
  return { isValid: true };
};

// Push token güncelleme
exports.updatePushToken = async (req, res) => {
  try {
    const { pushToken, deviceInfo } = req.body;
    const userId = req.userId;

    console.log('📱 Push token update isteği:', {
      userId,
      pushToken,
      deviceInfo
    });

    // Token validasyonu
    const tokenValidation = validatePushToken(pushToken);
    if (!tokenValidation.isValid) {
      return res.status(400).json({
        success: false,
        message: tokenValidation.message
      });
    }

    if (!deviceInfo || typeof deviceInfo !== 'object') {
      return res.status(400).json({
        success: false,
        message: 'Geçerli device bilgisi gerekli'
      });
    }

    const user = await User.findByIdAndUpdate(
      userId,
      {
        $set: {
          pushToken,
          deviceInfo: {
            ...deviceInfo,
            lastUpdated: new Date()
          },
          lastTokenUpdate: new Date()
        }
      },
      { new: true }
    );

    if (!user) {
      return res.status(404).json({
        success: false,
        message: 'Kullanıcı bulunamadı'
      });
    }

    console.log('✅ Push token kaydedildi:', {
      userId: user._id,
      token: pushToken,
      deviceInfo: user.deviceInfo
    });

    return res.json({
      success: true,
      message: 'Push token başarıyla kaydedildi',
      data: {
        userId: user._id,
        pushToken: user.pushToken,
        deviceInfo: user.deviceInfo,
        lastTokenUpdate: user.lastTokenUpdate
      }
    });

  } catch (error) {
    console.error('❌ Push token kaydetme hatası:', error);
    return res.status(500).json({
      success: false,
      message: 'Push token kaydedilirken bir hata oluştu'
    });
  }
};

// Push token bilgisini getir
exports.getPushToken = async (req, res) => {
  try {
    const { userId } = req.params;

    const user = await User.findById(userId)
      .select('pushToken deviceInfo lastTokenUpdate')
      .lean();

    if (!user) {
      return res.status(404).json({
        success: false,
        message: 'Kullanıcı bulunamadı'
      });
    }

    // Token validasyonu
    if (user.pushToken) {
      const tokenValidation = validatePushToken(user.pushToken);
      if (!tokenValidation.isValid) {
        console.warn('⚠️ Geçersiz token tespit edildi:', user.pushToken);
      }
    }

    return res.json({
      success: true,
      data: {
        userId: user._id,
        pushToken: user.pushToken,
        deviceInfo: user.deviceInfo,
        lastTokenUpdate: user.lastTokenUpdate,
        isTokenValid: user.pushToken ? validatePushToken(user.pushToken).isValid : false
      }
    });

  } catch (error) {
    console.error('❌ Push token getirme hatası:', error);
    return res.status(500).json({
      success: false,
      message: 'Push token alınırken bir hata oluştu'
    });
  }
};

// Push notification gönderme fonksiyonu
exports.sendPushNotification = async (req, res) => {
  try {
    const { userId, pushToken: directToken, title, body, data } = req.body;

    console.log('📤 Bildirim isteği alındı:', {
      userId,
      directToken: directToken ? directToken.substring(0, 10) + '...' : undefined,
      title,
      body,
      data
    });

    if (!userId) {
      return res.status(400).json({
        success: false,
        message: 'userId zorunludur'
      });
    }

    let pushToken = directToken;
    let deviceInfo = null;

    // Eğer direkt token gönderilmediyse, veritabanından al
    if (!pushToken) {
      const user = await User.findById(userId).select('pushToken deviceInfo');
      if (!user || !user.pushToken) {
        console.error('❌ Push token bulunamadı:', userId);
        return res.status(404).json({
          success: false,
          message: 'Kullanıcı push token\'ı bulunamadı'
        });
      }
      pushToken = user.pushToken;
      deviceInfo = user.deviceInfo;
    }

    // Token validasyonu
    const tokenValidation = validatePushToken(pushToken);
    if (!tokenValidation.isValid) {
      console.error('❌ Geçersiz push token formatı:', pushToken);
      return res.status(400).json({
        success: false,
        message: 'Geçersiz push token'
      });
    }

    console.log('✅ Push token doğrulandı:', {
      token: pushToken.substring(0, 10) + '...',
      device: deviceInfo?.platform
    });

    // Test bildirimi için varsayılan değerler
    const messageTitle = title || '🧪 Test Bildirimi';
    const messageBody = body || 'Bu bir test bildirimidir';

    // Expo üzerinden gönder
    const message = {
      to: pushToken,
      title: messageTitle,
      body: messageBody,
      data: {
        ...(data || {}),
        type: data?.type || 'TEST',
        screen: data?.screen || 'Home',
        timestamp: new Date().toISOString(),
        experienceId: '@emlakdrone/mobile',
        scopeKey: '@emlakdrone/mobile'
      },
      sound: deviceInfo?.platform === 'ios' ? 'notification' : 'notification.wav',
      priority: 'high',
      channelId: 'default',
      _displayInForeground: true,
      android: {
        channelId: 'default',
        sound: 'notification.wav',
        priority: 'max',
        vibrate: [0, 250, 250, 250]
      },
      ios: {
        sound: 'notification'
      }
    };

    // Expo üzerinden gönder
    const chunks = expo.chunkPushNotifications([message]);
    let success = false;
    let error = null;

    for (let chunk of chunks) {
      try {
        const tickets = await expo.sendPushNotificationsAsync(chunk);
        console.log('✅ Expo bildirimi gönderildi:', {
          userId,
          tickets
        });
        success = true;
      } catch (err) {
        error = err;
        console.error('❌ Expo bildirim hatası:', err);
      }
    }

    if (success) {
      return res.json({
        success: true,
        message: 'Bildirim başarıyla gönderildi'
      });
    } else {
      throw error || new Error('Bildirim gönderilemedi');
    }

  } catch (error) {
    console.error('❌ Push notification hatası:', error);
    return res.status(500).json({
      success: false,
      message: 'Bildirim gönderilemedi',
      error: error.message
    });
  }
};

// Video tamamlandığında bildirim gönder
exports.sendVideoCompletedNotification = async (userId, videoId, propertyDetails) => {
  try {
    const user = await User.findById(userId).select('pushToken deviceInfo');
    if (!user || !user.pushToken) {
      console.warn('⚠️ Push token bulunamadı:', userId);
      return false;
    }

    // Token validasyonu
    const tokenValidation = validatePushToken(user.pushToken);
    if (!tokenValidation.isValid) {
      console.warn('⚠️ Geçersiz token formatı:', user.pushToken);
      return false;
    }

    const message = {
      to: user.pushToken,
      sound: user.deviceInfo?.platform === 'ios' ? 'notification' : 'notification.wav',
      title: '🎥 Drone Görüntünüz Hazır!',
      body: 'Video işleminiz tamamlandı. Hemen indirebilirsiniz!',
      data: {
        type: 'NEW_VIDEO',
        screen: 'MyVideos',
        videoId,
        propertyDetails,
        timestamp: new Date().toISOString(),
        experienceId: '@emlakdrone/mobile',
        scopeKey: '@emlakdrone/mobile'
      },
      priority: 'high',
      channelId: 'default',
      _displayInForeground: true,
      android: {
        channelId: 'default',
        sound: 'notification.wav',
        priority: 'max',
        vibrate: [0, 250, 250, 250]
      },
      ios: {
        sound: 'notification'
      }
    };

    const chunks = expo.chunkPushNotifications([message]);
    let success = false;

    for (let chunk of chunks) {
      try {
        console.log('📤 Video hazır bildirimi gönderiliyor:', {
          userId,
          videoId,
          token: user.pushToken.substring(0, 10) + '...',
          device: user.deviceInfo?.platform
        });
        
        const tickets = await expo.sendPushNotificationsAsync(chunk);
        console.log('✅ Video hazır bildirimi gönderildi:', {
          userId,
          videoId,
          tickets
        });
        success = true;
      } catch (error) {
        console.error('❌ Video bildirimi gönderim hatası:', error);
      }
    }

    return success;
  } catch (error) {
    console.error('❌ Video bildirim hatası:', error);
    return false;
  }
}; 