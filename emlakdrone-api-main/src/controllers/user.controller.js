const User = require('../models/User');

exports.getVideoLimitStatus = async (req, res) => {
  try {
    const user = await User.findById(req.userId);
    
    if (!user) {
      return res.status(404).json({ message: 'Kullanıcı bulunamadı' });
    }

    // 30 günlük süreyi kontrol et
    const now = new Date();
    const thirtyDaysAgo = new Date(now.getTime() - (30 * 24 * 60 * 60 * 1000));
    
    // Süre dolduysa sayacı sıfırla
    if (user.lastVideoResetDate < thirtyDaysAgo) {
      user.videoCount = 0;
      user.lastVideoResetDate = now;
      await user.save();
    }

    const monthlyLimit = 10;
    const baseRemaining = Math.max(0, monthlyLimit - user.videoCount);
    const totalRemainingMonthly = baseRemaining + (user.rolloverCredits || 0);

    res.status(200).json({
      videoCount: user.videoCount,
      nextResetDate: user.lastVideoResetDate,
      remainingVideos: (user.isMembership && user.membershipType === 'yearly')
        ? Number.POSITIVE_INFINITY
        : (user.isMembership && user.membershipType === 'monthly')
          ? totalRemainingMonthly
          : Math.max(0, monthlyLimit - user.videoCount),
      isMembership: user.isMembership,
      membershipType: user.membershipType,
      membershipExpireDate: user.membershipExpireDate,
      singleVideoRights: user.singleVideoRights,
      rolloverCredits: user.rolloverCredits || 0
    });
  } catch (error) {
    console.error('Video limit durumu kontrolü hatası:', error);
    res.status(500).json({ message: 'Video limit durumu alınırken hata oluştu' });
  }
};

exports.incrementVideoCount = async (req, res) => {
  try {
    const user = await User.findById(req.userId);
    
    if (!user) {
      return res.status(404).json({ message: 'Kullanıcı bulunamadı' });
    }

    if (!user.canCreateVideo()) {
      return res.status(403).json({ 
        message: 'Video oluşturma limitinize ulaştınız',
        videoCount: user.videoCount,
        nextResetDate: user.lastVideoResetDate,
        singleVideoRights: user.singleVideoRights
      });
    }

    await user.incrementVideoCount();

    res.status(200).json({
      message: 'Video sayacı güncellendi',
      videoCount: user.videoCount,
      remainingVideos: (user.isMembership && user.membershipType === 'yearly') ? Number.POSITIVE_INFINITY : Math.max(0, 10 - user.videoCount),
      singleVideoRights: user.singleVideoRights
    });
  } catch (error) {
    console.error('Video sayacı güncelleme hatası:', error);
    res.status(500).json({ message: 'Video sayacı güncellenirken hata oluştu' });
  }
};

exports.purchaseSingleVideoRight = async (req, res) => {
  try {
    const user = await User.findById(req.userId);
    
    if (!user) {
      return res.status(404).json({ message: 'Kullanıcı bulunamadı' });
    }

    // Ödeme işlemi başarılı olduktan sonra
    user.singleVideoRights += 1;
    await user.save();

    res.status(200).json({
      message: 'Tek seferlik video hakkı başarıyla satın alındı',
      singleVideoRights: user.singleVideoRights
    });
  } catch (error) {
    console.error('Video hakkı satın alma hatası:', error);
    res.status(500).json({ message: 'Video hakkı satın alınırken hata oluştu' });
  }
};

exports.updateMembership = async (req, res) => {
  try {
    // Validate request body
    const { membershipType, isMembership, membershipExpireDate } = req.body;

    if (typeof membershipType !== 'string' || typeof isMembership !== 'boolean') {
      return res.status(400).json({
        success: false,
        message: 'Geçersiz üyelik bilgileri'
      });
    }

    // Validate membershipType
    if (!['none', 'monthly', 'yearly'].includes(membershipType)) {
      return res.status(400).json({
        success: false,
        message: 'Geçersiz üyelik tipi'
      });
    }

    const user = await User.findById(req.userId);
    if (!user) {
      return res.status(404).json({
        success: false,
        message: 'Kullanıcı bulunamadı'
      });
    }

    // Update membership info
    user.membershipType = membershipType;
    user.isMembership = isMembership;
    
    // Expire date kontrolü
    if (membershipExpireDate) {
      const expireDate = new Date(membershipExpireDate);
      if (isNaN(expireDate.getTime())) {
        return res.status(400).json({
          success: false,
          message: 'Geçersiz tarih formatı'
        });
      }
      user.membershipExpireDate = expireDate;
    } else {
      user.membershipExpireDate = null;
    }

    await user.save();

    console.log('✅ Üyelik güncellendi:', {
      userId: user._id,
      membershipType,
      isMembership,
      expireDate: user.membershipExpireDate
    });

    res.status(200).json({
      success: true,
      message: 'Üyelik başarıyla güncellendi'
    });

  } catch (error) {
    console.error('❌ Üyelik güncelleme hatası:', {
      error: error.message,
      stack: error.stack,
      userId: req.userId
    });
    
    res.status(500).json({
      success: false,
      message: 'Üyelik güncellenirken bir hata oluştu',
      error: error.message
    });
  }
};

exports.handleRevenueCatWebhook = async (req, res) => {
  try {
    const event = req.body;
    console.log('RevenueCat webhook event:', event);

    switch (event.type) {
      case 'SUBSCRIPTION_CANCELLED':
      case 'SUBSCRIPTION_EXPIRED':
        await handleSubscriptionEnd(event);
        break;

      case 'SUBSCRIPTION_RENEWED':
      case 'SUBSCRIPTION_REACTIVATED':
        await handleSubscriptionRenewal(event);
        break;

      default:
        console.log('İşlenmeyen event tipi:', event.type);
    }

    res.status(200).json({ success: true });
  } catch (error) {
    console.error('Webhook işleme hatası:', error);
    res.status(500).json({ error: 'Internal server error' });
  }
};

async function handleSubscriptionEnd(event) {
  try {
    const user = await User.findOne({ rcUserId: event.app_user_id });
    if (!user) {
      console.error(`Kullanıcı bulunamadı: ${event.app_user_id}`);
      return;
    }

    user.isMembership = false;
    user.membershipType = 'none';
    user.membershipExpireDate = null;
    user.lastWebhookEvent = {
      type: event.type,
      timestamp: new Date(event.event_timestamp),
      originalEvent: event
    };

    await user.save();
    console.log(`Üyelik sonlandırıldı - Kullanıcı: ${event.app_user_id}`);
  } catch (error) {
    console.error('Üyelik sonlandırma hatası:', error);
    throw error;
  }
}

async function handleSubscriptionRenewal(event) {
  try {
    const user = await User.findOne({ rcUserId: event.app_user_id });
    if (!user) {
      console.error(`Kullanıcı bulunamadı: ${event.app_user_id}`);
      return;
    }

    // Abonelik süresini hesapla
    const expireDate = new Date(event.event_timestamp);
    const isMonthly = event.product_id.includes('monthly');
    
    if (isMonthly) {
      expireDate.setMonth(expireDate.getMonth() + 1);
    } else {
      expireDate.setFullYear(expireDate.getFullYear() + 1);
    }

    user.isMembership = true;
    user.membershipType = isMonthly ? 'monthly' : 'yearly';
    user.membershipExpireDate = expireDate;
    user.lastWebhookEvent = {
      type: event.type,
      timestamp: new Date(event.event_timestamp),
      originalEvent: event
    };

    await user.save();
    console.log(`Üyelik yenilendi - Kullanıcı: ${event.app_user_id}`);
  } catch (error) {
    console.error('Üyelik yenileme hatası:', error);
    throw error;
  }
}

exports.updatePushToken = async (req, res) => {
  try {
    const userId = req.userId;
    const { pushToken } = req.body;

    // Token validasyonu
    if (!pushToken || typeof pushToken !== 'string' || pushToken.length < 10) {
      return res.status(400).json({
        success: false,
        message: 'Geçersiz push token formatı',
        error: 'INVALID_PUSH_TOKEN'
      });
    }

    // Kullanıcıyı bul ve güncelle
    const user = await User.findById(userId);
    if (!user) {
      return res.status(404).json({
        success: false,
        message: 'Kullanıcı bulunamadı',
        error: 'USER_NOT_FOUND'
      });
    }

    // Push token'ı güncelle
    user.pushToken = pushToken;
    await user.save();

    console.log('✅ Push token güncellendi:', { userId, pushToken });

    return res.json({
      success: true,
      message: 'Push token başarıyla güncellendi'
    });

  } catch (error) {
    console.error('❌ Push token güncelleme hatası:', error);
    return res.status(500).json({
      success: false,
      message: 'Push token güncellenemedi',
      error: 'UPDATE_FAILED'
    });
  }
}; 