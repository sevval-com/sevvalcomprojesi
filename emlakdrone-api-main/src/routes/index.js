const express = require('express');
const router = express.Router();
const authMiddleware = require('../middleware/auth.middleware');
const authRoutes = require('./auth.routes');
const propertyRoutes = require('./property.routes');
const videoRoutes = require('./video.routes');
const revenueCatWebhook = require('./routes/webhooks/revenuecat');

router.use('/auth', authRoutes);
router.use('/properties', authMiddleware, propertyRoutes);
router.use('/videos', videoRoutes);
router.use('/api/webhooks', revenueCatWebhook);

module.exports = router; 