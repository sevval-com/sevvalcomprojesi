const mongoose = require('mongoose');

// MongoDB bağlantısı
const connectDB = async () => {
  try {
    const conn = await mongoose.connect(process.env.MONGODB_URI || 'mongodb://localhost:27017/emlakdrone');
    console.log('✅ MongoDB bağlandı:', conn.connection.host);
    
    // Index'leri oluştur
    await createIndexes();
    
    return conn;
  } catch (error) {
    console.error('❌ MongoDB bağlantı hatası:', error);
    process.exit(1);
  }
};

// Marketplace index'lerini oluştur
const createIndexes = async () => {
  try {
    console.log('🔧 Marketplace index\'leri kontrol ediliyor...');
    
    const db = mongoose.connection.db;
    
    // Mevcut index'leri kontrol et
    const existingIndexes = await db.collection('marketplaces').indexes();
    const indexNames = existingIndexes.map(idx => idx.name);
    
    // 2dsphere index (geo-spatial queries için - ZORUNLU)
    if (!indexNames.includes('location_2dsphere')) {
      await db.collection('marketplaces').createIndex({ "location": "2dsphere" });
      console.log('✅ location 2dsphere index oluşturuldu');
    } else {
      console.log('ℹ️ location 2dsphere index zaten mevcut');
    }
    
    // Status index (filtreleme performansı için)
    if (!indexNames.includes('status_1')) {
      await db.collection('marketplaces').createIndex({ "status": 1 });
      console.log('✅ status index oluşturuldu');
    } else {
      console.log('ℹ️ status index zaten mevcut');
    }
    
    // CreatedAt index (sıralama performansı için)
    if (!indexNames.includes('createdAt_-1')) {
      await db.collection('marketplaces').createIndex({ "createdAt": -1 });
      console.log('✅ createdAt index oluşturuldu');
    } else {
      console.log('ℹ️ createdAt index zaten mevcut');
    }
    
    // AgentId index (kullanıcı listeleri için)
    if (!indexNames.includes('agentId_1')) {
      await db.collection('marketplaces').createIndex({ "agentId": 1 });
      console.log('✅ agentId index oluşturuldu');
    } else {
      console.log('ℹ️ agentId index zaten mevcut');
    }
    
    // Phone index (arama için)
    if (!indexNames.includes('contact.phone.original_1')) {
      await db.collection('marketplaces').createIndex({ "contact.phone.original": 1 });
      console.log('✅ phone index oluşturuldu');
    } else {
      console.log('ℹ️ phone index zaten mevcut');
    }
    
    // Compound index (performans için)
    if (!indexNames.includes('status_1_createdAt_-1')) {
      await db.collection('marketplaces').createIndex({ "status": 1, "createdAt": -1 });
      console.log('✅ compound index (status + createdAt) oluşturuldu');
    } else {
      console.log('ℹ️ compound index zaten mevcut');
    }
    
    // Text search index (başlık ve açıklama araması için)
    if (!indexNames.includes('title_text_description_text')) {
      await db.collection('marketplaces').createIndex({ 
        "title": "text", 
        "description": "text" 
      });
      console.log('✅ text search index oluşturuldu');
    } else {
      console.log('ℹ️ text search index zaten mevcut');
    }
    
    console.log('🎉 Tüm marketplace index\'leri hazır!');
    
  } catch (error) {
    console.error('❌ Index oluşturma hatası:', error);
    // Index hatası kritik değil, uygulama çalışmaya devam edebilir
  }
};

module.exports = { connectDB, createIndexes };
