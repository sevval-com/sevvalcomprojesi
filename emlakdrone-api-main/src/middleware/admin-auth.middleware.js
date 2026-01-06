const jwt = require('jsonwebtoken');

const adminAuthMiddleware = async (req, res, next) => {
  try {
    const token = req.header('Authorization')?.replace('Bearer ', '');
    
    if (!token) {
      return res.status(401).json({
        success: false,
        message: 'Admin erişim token\'ı gerekli',
        error: 'NO_TOKEN'
      });
    }

    // Token doğrulama
    const decoded = jwt.verify(token, process.env.JWT_SECRET || 'supersecretjwtkey');
    
    // Admin rolü kontrolü
    if (decoded.role !== 'admin') {
      return res.status(403).json({
        success: false,
        message: 'Admin yetkisi gerekli',
        error: 'ADMIN_REQUIRED'
      });
    }

    req.admin = decoded;
    next();

  } catch (error) {
    console.error('Admin auth middleware hatası:', error);
    
    if (error.name === 'JsonWebTokenError') {
      return res.status(401).json({
        success: false,
        message: 'Geçersiz admin token',
        error: 'INVALID_TOKEN'
      });
    }
    
    if (error.name === 'TokenExpiredError') {
      return res.status(401).json({
        success: false,
        message: 'Admin token süresi dolmuş',
        error: 'TOKEN_EXPIRED'
      });
    }

    return res.status(500).json({
      success: false,
      message: 'Sunucu hatası',
      error: 'SERVER_ERROR'
    });
  }
};

module.exports = adminAuthMiddleware;
