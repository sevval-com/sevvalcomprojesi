const User = require('../models/User');

const checkVideoLimit = async (req, res, next) => {
  try {
    const user = await User.findById(req.userId);
    
    if (!user) {
      return res.status(404).json({ message: 'Kullanıcı bulunamadı' });
    }

    if (!user.canCreateVideo()) {
      return res.status(403).json({ 
        message: 'Aylık video oluşturma limitinize ulaştınız',
        videoCount: user.videoCount,
        nextResetDate: user.lastVideoResetDate
      });
    }

    req.videoLimitInfo = {
      remainingVideos: 10 - user.videoCount,
      nextResetDate: user.lastVideoResetDate
    };

    next();
  } catch (error) {
    console.error('Video limit kontrolü hatası:', error);
    res.status(500).json({ message: 'Video limit kontrolü yapılırken hata oluştu' });
  }
};

module.exports = checkVideoLimit; 