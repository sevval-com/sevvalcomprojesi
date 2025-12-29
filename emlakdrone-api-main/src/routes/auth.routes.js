const express = require('express');
const { register, login, logout, deleteUser } = require('../controllers/auth.controller');
const authMiddleware = require('../middleware/auth.middleware');

const router = express.Router();

router.post('/register', register);
router.post('/login', login);
router.post('/logout', logout);
router.delete('/delete/:userId', authMiddleware, deleteUser);

module.exports = router; 