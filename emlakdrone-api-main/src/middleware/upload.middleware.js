const multer = require('multer');
const path = require('path');

// Multer yapılandırması
const storage = multer.memoryStorage();

// Dosya filtresi
const fileFilter = (req, file, cb) => {
  // Sadece resim dosyalarını kabul et
  if (file.mimetype.startsWith('image/')) {
    cb(null, true);
  } else {
    cb(new Error('Sadece resim dosyaları kabul edilir'), false);
  }
};

// Multer middleware'i
const upload = multer({
  storage: storage,
  fileFilter: fileFilter,
  limits: {
    fileSize: 10 * 1024 * 1024, // 10MB limit
    files: 6 // Maksimum 6 dosya
  }
});

// Çoklu dosya yükleme middleware'i
const uploadMultiple = upload.array('images', 6);

module.exports = {
  uploadMultiple
};
