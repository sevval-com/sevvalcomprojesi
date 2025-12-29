const express = require('express');
const router = express.Router();
const userController = require('../controllers/user.controller');
const auth = require('../middleware/auth.middleware');
const webhookAuth = require('../middleware/webhookAuth.middleware');

router.get('/video-limit-status', auth, userController.getVideoLimitStatus);
router.post('/increment-video-count', auth, userController.incrementVideoCount);
router.post('/update-membership', auth, userController.updateMembership);
router.post('/update-push-token', auth, userController.updatePushToken);
router.post('/webhook/revenuecat', webhookAuth, userController.handleRevenueCatWebhook);

// Tek seferlik video hakkı satın alma (non-subscription ürün sonrası)
router.post('/purchase-single-video-right', auth, userController.purchaseSingleVideoRight);

module.exports = router; 