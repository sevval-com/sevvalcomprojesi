const webhookAuth = (req, res, next) => {
  const authHeader = req.headers.authorization;
  
  if (!authHeader || authHeader !== `Bearer ${process.env.REVENUECAT_WEBHOOK_SECRET}`) {
    console.error('Unauthorized webhook request');
    return res.status(401).json({ error: 'Unauthorized' });
  }

  next();
};

module.exports = webhookAuth; 