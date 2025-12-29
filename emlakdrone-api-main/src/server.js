require('dotenv').config();
const express = require('express');
const cors = require('cors');
const path = require('path');
const fs = require('fs');
const { connectDB } = require('./config/database');

const authRoutes = require('./routes/auth.routes');
const propertyRoutes = require('./routes/property.routes');
const userRoutes = require('./routes/user.routes');
const notificationRoutes = require('./routes/notification.routes');
const videoRoutes = require('./routes/video.routes');
const videoDataRoutes = require('./routes/video-data.routes');
const templateRoutes = require('./routes/template.routes');
const authMiddleware = require('./middleware/auth.middleware');
const noteRoutes = require('./routes/note.routes');
const marketplaceRoutes = require('./routes/marketplace.routes');
const cobrokerRoutes = require('./routes/cobroker.routes');
const videoRequestRoutes = require('./routes/video-request.routes');
const adminRoutes = require('./routes/admin.routes');

const app = express();

// Vercel proxy için trust proxy ayarı
app.set('trust proxy', true);

const corsOptions = {
  origin: [
    'http://localhost:3000',
    'https://emlakdrone.vercel.app',
    'exp://',
    'http://localhost:19000', // Expo geliştirme sunucusu
    'http://localhost:19006', // Expo web
    'capacitor://', // Capacitor uygulamaları için
    'ionic://', // Ionic uygulamaları için
    /^https?:\/\/[a-z0-9-]+\.expo\.(io|dev)$/, // Expo Go uygulaması
    /^http:\/\/[0-9.]+:[0-9]+$/, // Yerel IP adresleri (Android emülatör ve cihazlar için)
  ],
  methods: ['GET', 'POST', 'PUT', 'DELETE', 'OPTIONS', 'PATCH'],
  allowedHeaders: [
    'Content-Type',
    'Authorization',
    'Accept',
    'Origin',
    'X-Requested-With',
    'Access-Control-Allow-Origin',
    'Access-Control-Allow-Headers'
  ],
  credentials: true,
  maxAge: 600,
  preflightContinue: false,
  optionsSuccessStatus: 204
};

app.use(cors(corsOptions));
app.use(express.json({ limit: '50mb' }));
app.use(express.urlencoded({ extended: true, limit: '50mb' }));

// Ek güvenlik başlıkları
app.use((req, res, next) => {
  res.header('Access-Control-Allow-Private-Network', 'true');
  res.header('Access-Control-Allow-Credentials', 'true');
  next();
});

// Health check endpoint'i
app.get('/health', (req, res) => {
  res.status(200).json({ 
    status: 'OK',
    timestamp: new Date(),
    uptime: process.uptime(),
    memoryUsage: process.memoryUsage()
  });
});

// IP test endpoint'i
app.get('/ip-test', (req, res) => {
  const realIP = req.headers['x-forwarded-for'] || 
                 req.headers['x-real-ip'] || 
                 req.headers['x-vercel-forwarded-for'] || 
                 req.ip || 
                 req.connection.remoteAddress;
  
  res.status(200).json({
    success: true,
    ip: {
      realIP,
      reqIP: req.ip,
      xForwardedFor: req.headers['x-forwarded-for'],
      xRealIP: req.headers['x-real-ip'],
      xVercelForwardedFor: req.headers['x-vercel-forwarded-for'],
      connectionRemoteAddress: req.connection.remoteAddress
    },
    headers: req.headers,
    timestamp: new Date().toISOString()
  });
});

// Test DELETE endpoint
app.delete('/api/test-delete', (req, res) => {
  res.status(200).json({ 
    message: 'DELETE method is working',
    timestamp: new Date()
  });
});

// Özel loglama fonksiyonu
const logToFile = (message) => {
  if (process.env.NODE_ENV === 'production') {
    try {
      // Production'da console.log kullan, dosya yazma işlemi Vercel'de sorun çıkarabilir
      console.log(`[PROD-LOG] ${message}`);
    } catch (error) {
      console.error('Loglama hatası:', error);
    }
  } else {
    console.log(message);
  }
};

// Request detaylı loglama
app.use((req, res, next) => {
  // Gerçek IP adresini al
  const realIP = req.headers['x-forwarded-for'] || 
                 req.headers['x-real-ip'] || 
                 req.headers['x-vercel-forwarded-for'] || 
                 req.ip || 
                 req.connection.remoteAddress;
  
  logToFile(`
    İstek Detayları:
    - Metod: ${req.method}
    - URL: ${req.url}
    - Headers: ${JSON.stringify(req.headers)}
    - Body: ${JSON.stringify(req.body)}
    - Query: ${JSON.stringify(req.query)}
    - IP: ${realIP}
    - X-Forwarded-For: ${req.headers['x-forwarded-for']}
    - X-Real-IP: ${req.headers['x-real-ip']}
    - X-Vercel-Forwarded-For: ${req.headers['x-vercel-forwarded-for']}
  `);
  next();
});

// Response loglama
app.use((req, res, next) => {
  const originalSend = res.send;
  res.send = function (data) {
    logToFile(`
      Yanıt Detayları:
      - URL: ${req.url}
      - Durum Kodu: ${res.statusCode}
      - Yanıt: ${JSON.stringify(data)}
    `);
    originalSend.call(this, data);
  };
  next();
});

// API routes with /api prefix
app.use('/api', (req, res, next) => {
  if (req.path === '/') {
    return res.status(200).json({ 
      message: 'EmlakDrone API',
      version: '1.0.5',
      status: 'active'
    });
  }
  next();
});

// Routes
app.use('/api/auth', authRoutes);
app.use('/api/properties', authMiddleware, propertyRoutes);
app.use('/api/user', userRoutes);
app.use('/api/notification', notificationRoutes);
app.use('/api/video', videoRoutes);
app.use('/api/video-data', videoDataRoutes);
app.use('/api/template', templateRoutes);
app.use('/api/notes', authMiddleware, noteRoutes);
app.use('/api/marketplace', marketplaceRoutes);
app.use('/api/cobroker', cobrokerRoutes);
app.use('/api/video-request', videoRequestRoutes);
app.use('/api/admin', adminRoutes);

// Debug: Log all registered routes
app._router.stack.forEach((middleware) => {
  if (middleware.route) {
    console.log(`Route: ${middleware.route.stack[0].method.toUpperCase()} ${middleware.route.path}`);
  } else if (middleware.name === 'router') {
    middleware.handle.stack.forEach((handler) => {
      if (handler.route) {
        console.log(`Route: ${handler.route.stack[0].method.toUpperCase()} ${middleware.regexp.source}${handler.route.path}`);
      }
    });
  }
});

// Catch-all route for debugging
app.use('*', (req, res, next) => {
  console.log('Request URL:', req.originalUrl);
  console.log('Request method:', req.method);
  next();
});

// MongoDB Bağlantısı ve Index Oluşturma
connectDB()
  .then(() => {
    console.log('✅ MongoDB bağlantısı ve index\'ler başarılı');
    logToFile('MongoDB bağlantısı ve index\'ler başarılı');
  })
  .catch(err => {
    console.error('❌ MongoDB bağlantı hatası:', err);
    logToFile(`MongoDB bağlantı hatası: ${err.message}`);
    
    // Production'da process.exit(1) yerine daha nazik bir yaklaşım
    if (process.env.NODE_ENV === 'production') {
      console.log('🔄 MongoDB bağlantısı başarısız, uygulama çalışmaya devam ediyor...');
      // process.exit(1) kaldırıldı - Vercel'de crash olmasın
    }
  });

// MongoDB bağlantı durumunu izle
const mongoose = require('mongoose');
mongoose.connection.on('disconnected', () => {
  console.log('📡 MongoDB bağlantısı kesildi');
  logToFile('MongoDB bağlantısı kesildi');
});

mongoose.connection.on('reconnected', () => {
  console.log('🔄 MongoDB yeniden bağlandı');
  logToFile('MongoDB yeniden bağlandı');
});

// Global hata yönetimi
app.use((err, req, res, next) => {
  logToFile(`Hata: ${err.stack}`);
  res.status(500).json({ 
    message: 'Sunucu hatası oluştu',
    error: err.message 
  });
});

// 404 yönlendirmesi
app.use((req, res) => {
  logToFile(`404 - Route bulunamadı: ${req.originalUrl}`);
  res.status(404).json({ 
    success: false,
    message: 'İstenen sayfa veya kaynak bulunamadı',
    path: req.originalUrl,
    timestamp: new Date().toISOString()
  });
});

const PORT = process.env.PORT || 5008;
app.listen(PORT, () => {
  console.log(`🚀 Server ${PORT} portunda çalışıyor`);
  console.log(`🌍 Ortam: ${process.env.NODE_ENV}`);
  if (process.env.NODE_ENV === 'production') {
    console.log(`📡 Frontend URL: ${process.env.FRONTEND_URL}`);
  }
}); 
