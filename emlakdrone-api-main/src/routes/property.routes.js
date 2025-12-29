const express = require('express');
const router = express.Router();
const propertyController = require('../controllers/property.controller');

// /api/properties prefix'i ile gelen istekler
// Önce spesifik route'ları tanımla
router.post('/create', propertyController.createRequest);
router.get('/list', propertyController.getRequests);


// Sonra parametreli route'ları tanımla
router.get('/:id', propertyController.getRequestById);
router.put('/:id/status', propertyController.updateRequestStatus);
router.delete('/:id', propertyController.deleteRequest);

module.exports = router;