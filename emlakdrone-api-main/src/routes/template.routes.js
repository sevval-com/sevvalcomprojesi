const express = require('express');
const router = express.Router();
const templateController = require('../controllers/template.controller');
const authMiddleware = require('../middleware/auth.middleware');
const apiKeyMiddleware = require('../middleware/apiKey.middleware');

// Template oluşturma endpoint'i
router.post('/generate', authMiddleware, templateController.generateTemplate);

// Template durumu kontrol endpoint'i
router.get('/status/:templateId', authMiddleware, templateController.checkTemplateStatus);

// Kullanıcının templatelerini getirme endpoint'i
router.get('/my-templates', authMiddleware, templateController.getMyTemplates);

// Template güncelleme endpoint'i
router.put('/:templateId', authMiddleware, templateController.updateTemplate);

// Template önizleme endpoint'i
router.get('/:templateId/preview', authMiddleware, templateController.previewTemplate);

module.exports = router; 