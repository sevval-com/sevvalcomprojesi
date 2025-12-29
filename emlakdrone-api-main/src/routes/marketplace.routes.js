const express = require('express');
const router = express.Router();
const marketplaceController = require('../controllers/marketplace.controller');
const authMiddleware = require('../middleware/auth.middleware');

// Tüm routes için authentication gerekli
router.use(authMiddleware);

// Marketplace listings
router.get('/listings', marketplaceController.getAllListings);

// Eksik bilgi bildirimi
router.post('/listings/:listingId/request-missing-info', marketplaceController.requestMissingInfo);

// Eksik bilgi bildirimlerini getir (ilan sahibi için)
router.get('/listings/:listingId/missing-info-requests', marketplaceController.getMissingInfoRequests);

// Eksik bilgi bildirimini güncelle (ilan sahibi için)
router.put('/listings/:listingId/missing-info-requests/:requestId', marketplaceController.updateMissingInfoRequest);

// Telefon görüntüleme
router.post('/listings/:listingId/view-phone', marketplaceController.viewPhone);

module.exports = router;
