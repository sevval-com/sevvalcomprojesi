const express = require('express');
const router = express.Router();
const videoRequestController = require('../controllers/video-request.controller');
const authMiddleware = require('../middleware/auth.middleware');
const { uploadMultiple } = require('../middleware/upload.middleware');

// Video talep oluşturma endpoint'i
router.post('/create', authMiddleware, uploadMultiple, videoRequestController.createRequest);

// Kullanıcının video taleplerini getirme endpoint'i
router.get('/my-requests', authMiddleware, videoRequestController.getMyRequests);

// Video talep durumunu kontrol endpoint'i
router.get('/status/:requestId', authMiddleware, videoRequestController.getRequestStatus);

module.exports = router;
