const User = require('../models/User');
const jwt = require('jsonwebtoken');

exports.register = async (req, res) => {
  try {
    const { email, password, name, surname, phone, company } = req.body;
    
    // E-posta kontrolü
    const existingUser = await User.findOne({ email });
    if (existingUser) {
      return res.status(409).json({ message: 'Bu e-posta adresi zaten kullanımda' });
    }

    const newUser = new User({
      email,
      password,
      name,
      surname,
      phone,
      company: company || '',
      membershipType: 'none',
      isMembership: false,
      videoCount: 0,
      lastVideoResetDate: new Date(),
      singleVideoRights: 0
    });

    await newUser.save();
    
    res.status(201).json({ message: 'Kullanıcı başarıyla kaydedildi' });
  } catch (error) {
    console.error('Kayıt hatası:', error);
    res.status(500).json({ message: 'Kayıt sırasında bir hata oluştu' });
  }
};

exports.login = async (req, res) => {
  try {
    const { email, password } = req.body;
    const user = await User.findOne({ email });

    if (!user || !(await user.comparePassword(password))) {
      return res.status(401).json({ message: 'Geçersiz e-posta veya şifre' });
    }

    // Üyelik durumunu kontrol et
    user.checkMembershipStatus();
    await user.save();

    const token = jwt.sign(
      { 
        id: user._id,
        email: user.email,
        name: user.name,
        surname: user.surname,
        userType: user.userType,
        company: user.company,
        isMembership: user.isMembership,
        membershipType: user.membershipType
      }, 
      process.env.JWT_SECRET,
      { expiresIn: '90d' }
    );
    
    res.status(200).json({
      token,
      user: {
        id: user._id.toString(),
        email: user.email,
        name: user.name,
        surname: user.surname,
        phone: user.phone,
        company: user.company,
        userType: user.userType,
        isMembership: user.isMembership,
        membershipType: user.membershipType,
        membershipExpireDate: user.membershipExpireDate
      }
    });
  } catch (error) {
    console.error('Giriş hatası:', error);
    res.status(500).json({ message: 'Giriş sırasında bir hata oluştu' });
  }
}; 

exports.logout = async (req, res) => {
  res.clearCookie('token');
  res.status(200).json({ message: 'Çıkış yapıldı' });
};

exports.deleteUser = async (req, res) => {
  try {
    const { userId } = req.params;
    console.log('Request userId:', userId);
    console.log('Token userId:', req.userId);
    
    // İsteği yapan kullanıcı ile silinecek hesabın aynı olduğunu kontrol et
    if (req.userId.toString() !== userId) {
      console.log('Yetki hatası: Token userId ve parametre userId eşleşmiyor');
      return res.status(403).json({ message: 'Bu işlem için yetkiniz yok' });
    }

    const deletedUser = await User.findByIdAndDelete(userId);
    
    if (!deletedUser) {
      return res.status(404).json({ message: 'Kullanıcı bulunamadı' });
    }

    res.status(200).json({ message: 'Hesap başarıyla silindi' });
  } catch (error) {
    console.error('Hesap silme hatası:', error);
    res.status(500).json({ message: 'Hesap silinirken bir hata oluştu' });
  }
};

