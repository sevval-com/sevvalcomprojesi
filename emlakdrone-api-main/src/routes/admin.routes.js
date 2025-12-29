const express = require('express');
const router = express.Router();
const adminController = require('../controllers/admin.controller');
const adminAuthMiddleware = require('../middleware/admin-auth.middleware');

// Admin giriş
router.post('/login', adminController.login);

// Video taleplerini listele
router.get('/video-requests', adminAuthMiddleware, adminController.getVideoRequests);

// Video talep detayını getir
router.get('/video-requests/:requestId', adminAuthMiddleware, adminController.getVideoRequestDetail);

// Video talep durumunu güncelle
router.put('/video-requests/:requestId/status', adminAuthMiddleware, adminController.updateRequestStatus);

// Video URL'i ekle
router.put('/video-requests/:requestId/video', adminAuthMiddleware, adminController.addVideoUrl);

module.exports = router;
