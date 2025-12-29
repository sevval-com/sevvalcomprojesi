const jwt = require('jsonwebtoken');

const authMiddleware = (req, res, next) => {
  try {
    const token = req.headers.authorization?.split(' ')[1];
    
    // IP adresini logla
    const realIP = req.headers['x-forwarded-for'] || 
                   req.headers['x-real-ip'] || 
                   req.headers['x-vercel-forwarded-for'] || 
                   req.ip || 
                   req.connection.remoteAddress;
    
    console.log(`🔐 Auth middleware - Token kontrolü:`, {
      url: req.url,
      method: req.method,
      realIP,
      hasToken: !!token
    });
    
    if (!token) {
      return res.status(401).json({ message: 'Token bulunamadı' });
    }

    try {
      const decoded = jwt.verify(token, process.env.JWT_SECRET);
      req.userId = decoded.id;
      next();
    } catch (error) {
      if (error.name === 'TokenExpiredError') {
        return res.status(401).json({ 
          message: 'Token süresi doldu',
          expired: true
        });
      }
      throw error;
    }
  } catch (error) {
    console.error('Token doğrulama hatası:', error);
    res.status(401).json({ message: 'Geçersiz token' });
  }
};

module.exports = authMiddleware; 