const express = require('express');
const router = express.Router();
const noteController = require('../controllers/note.controller');

// Not oluştur veya güncelle
router.post('/create-or-update', noteController.createOrUpdateNote);

// Property için not getir
router.get('/property/:propertyId', noteController.getNoteByProperty);

// Kullanıcının tüm notlarını getir
router.get('/user', noteController.getUserNotes);

// Not sil
router.delete('/property/:propertyId', noteController.deleteNote);

module.exports = router;
