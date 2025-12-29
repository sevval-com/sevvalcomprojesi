const rateLimit = require('express-rate-limit');

const pushTokenUpdateLimiter = rateLimit({
  windowMs: 15 * 60 * 1000, // 15 dakika
  max: 5, // Her IP için 15 dakikada maksimum 5 istek
  message: {
    success: false,
    message: 'Çok fazla istek gönderildi',
    error: {
      code: 'TOO_MANY_REQUESTS',
      details: 'Lütfen 15 dakika sonra tekrar deneyin'
    }
  },
  standardHeaders: true,
  legacyHeaders: false
});

const pushTokenGetLimiter = rateLimit({
  windowMs: 60 * 1000, // 1 dakika
  max: 30, // Her IP için dakikada maksimum 30 istek
  message: {
    success: false,
    message: 'Çok fazla istek gönderildi',
    error: {
      code: 'TOO_MANY_REQUESTS',
      details: 'Lütfen 1 dakika sonra tekrar deneyin'
    }
  },
  standardHeaders: true,
  legacyHeaders: false
});

module.exports = {
  pushTokenUpdateLimiter,
  pushTokenGetLimiter
}; 