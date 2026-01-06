const express = require('express');
const router = express.Router();
const cobrokerController = require('../controllers/cobroker.controller');
const auth = require('../middleware/auth.middleware');

// Async wrapper fonksiyonu
const asyncHandler = (fn) => (req, res, next) => {
  Promise.resolve(fn(req, res, next)).catch(next);
};

// Tüm route'lar authentication gerektirir
router.use(auth);

// Co-broker isteği gönder
router.post('/request', asyncHandler(cobrokerController.sendCoBrokerRequest));

// Co-broker isteğini yanıtla (onayla/reddet)
router.post('/respond', asyncHandler(cobrokerController.respondToCoBrokerRequest));

// Co-broker isteğini geri çek
router.post('/cancel', asyncHandler(cobrokerController.cancelCoBrokerRequest));

// Co-broker isteklerini getir
router.get('/requests', asyncHandler(cobrokerController.getCoBrokerRequests));

// Aktif işbirliklerini getir
router.get('/collaborations', asyncHandler(cobrokerController.getActiveCollaborations));

// Mesaj gönder
router.post('/message', asyncHandler(cobrokerController.sendMessage));

// Mesajları getir
router.get('/messages', asyncHandler(cobrokerController.getMessages));

// Mesaj sil
router.delete('/message/:messageId', asyncHandler(cobrokerController.deleteMessage));

// İşbirliğini sonlandır
router.post('/end-collaboration', asyncHandler(cobrokerController.endCollaboration));

// Okunmamış mesaj sayısını getir
router.get('/unread-count', asyncHandler(cobrokerController.getUnreadCount));

module.exports = router;
