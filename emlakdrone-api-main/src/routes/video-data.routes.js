const express = require('express');
const apiKeyMiddleware = require('../middleware/apiKey.middleware');
const videoDataController = require('../controllers/video-data.controller');

const router = express.Router();

// Video data oku (public okunabilir)
router.get('/:videoId', videoDataController.getVideoData);

// Video data olustur/guncelle (servisler icin)
router.post('/', apiKeyMiddleware, videoDataController.createVideoData);

module.exports = router;
