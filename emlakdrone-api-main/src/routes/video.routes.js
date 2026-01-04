const express = require('express');
const router = express.Router();
const videoController = require('../controllers/video.controller');
const authMiddleware = require('../middleware/auth.middleware');

// Video oluşturma endpoint'i
router.post('/generate', authMiddleware, videoController.generateVideo);

// Video durumu kontrol endpoint'i
router.get('/status/:videoId', authMiddleware, videoController.checkVideoStatus);

// Kullanıcının videolarını getirme endpoint'i
router.get('/my-videos', authMiddleware, videoController.getMyVideos);

// videoCount güncelleme endpoint'i
router.post('/update-video-count', authMiddleware, videoController.updateVideoCount);

module.exports = router;