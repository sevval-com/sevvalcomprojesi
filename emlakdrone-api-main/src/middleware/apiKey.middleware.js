const apiKeyMiddleware = (req, res, next) => {
  const apiKey = req.headers['x-api-key'];

  if (!apiKey) {
    return res.status(401).json({
      success: false,
      message: 'API key gerekli',
      error: 'API_KEY_REQUIRED'
    });
  }

  if (apiKey !== process.env.MOBILE_API_KEY) {
    return res.status(401).json({
      success: false,
      message: 'Geçersiz API key',
      error: 'INVALID_API_KEY'
    });
  }

  next();
};

module.exports = apiKeyMiddleware; 