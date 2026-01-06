const User = require('../models/User');

const checkMembership = async (req, res, next) => {
  try {
    const user = await User.findById(req.userId);
    
    if (!user) {
      return res.status(404).json({ message: 'Kullanıcı bulunamadı' });
    }

    // Üyelik süresi kontrolü
    if (user.membershipExpireDate && new Date() > user.membershipExpireDate) {
      user.isMembership = false;
      user.membershipType = 'none';
      await user.save();
    }

    req.userMembership = {
      isMembership: user.isMembership,
      membershipType: user.membershipType,
      expireDate: user.membershipExpireDate
    };

    next();
  } catch (error) {
    console.error('Üyelik kontrolü hatası:', error);
    res.status(500).json({ message: 'Üyelik kontrolü yapılırken hata oluştu' });
  }
};

module.exports = checkMembership; 