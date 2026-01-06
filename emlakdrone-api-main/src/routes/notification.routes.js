const express = require('express');
const router = express.Router();
const authMiddleware = require('../middleware/auth.middleware');
const apiKeyMiddleware = require('../middleware/apiKey.middleware');
const notificationController = require('../controllers/notification.controller');

// Test endpoint'i
router.post('/test-notification',
  apiKeyMiddleware,
  async (req, res) => {
    try {
      const { userId, pushToken } = req.body;
      
      if (!userId) {
        return res.status(400).json({
          success: false,
          message: 'userId gerekli'
        });
      }

      console.log('📤 Test bildirimi isteği:', {
        userId,
        pushToken: pushToken ? pushToken.substring(0, 10) + '...' : undefined
      });

      // Bildirimi gönder
      return await notificationController.sendPushNotification({
        body: {
          userId,
          pushToken,
          title: '🧪 Test Bildirimi',
          body: 'Bu bir test bildirimidir',
          data: {
            type: 'TEST',
            screen: 'Home',
            timestamp: new Date().toISOString()
          }
        }
      }, res);

    } catch (error) {
      console.error('Test notification error:', error);
      return res.status(500).json({
        success: false,
        message: 'Test bildirimi gönderilemedi',
        error: error.message
      });
    }
  }
);

// Push token güncelleme
router.post('/update-token', 
  authMiddleware,
  notificationController.updatePushToken
);

// Push token bilgisini alma
router.get('/token/:userId',
  apiKeyMiddleware,
  notificationController.getPushToken
);

// Push notification gönderme
router.post('/send',
  apiKeyMiddleware,
  notificationController.sendPushNotification
);

module.exports = router; 