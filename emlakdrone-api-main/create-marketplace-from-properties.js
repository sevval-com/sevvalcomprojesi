require('dotenv').config();
const mongoose = require('mongoose');
const PropertyRequest = require('./src/models/PropertyRequest');
const Marketplace = require('./src/models/Marketplace');
const User = require('./src/models/User');

// MongoDB URI'yi environment variable'dan al
const MONGODB_URI = process.env.MONGODB_URI || 'mongodb://localhost:27017/emlakdrone';

console.log('🔌 MongoDB bağlantısı kuruluyor...');
console.log('📍 URI:', MONGODB_URI.replace(/\/\/[^:]+:[^@]+@/, '//***:***@')); // Şifreyi gizle

// MongoDB bağlantısı
mongoose.connect(MONGODB_URI, {
  useNewUrlParser: true,
  useUnifiedTopology: true,
});

const createMarketplaceListings = async () => {
  try {
    console.log('🚀 Marketplace listings oluşturuluyor...');
    
    // Tüm PropertyRequest'leri getir
    const properties = await PropertyRequest.find({}).populate('userId');
    console.log(`📊 ${properties.length} adet PropertyRequest bulundu`);
    
    if (properties.length === 0) {
      console.log('❌ PropertyRequest bulunamadı!');
      return;
    }
    
    // Marketplace collection'ını temizle
    await Marketplace.deleteMany({});
    console.log('🧹 Marketplace collection temizlendi');
    
    let createdCount = 0;
    let skippedCount = 0;
    
    for (const property of properties) {
      try {
        // userId kontrolü
        if (!property.userId) {
          console.log(`⚠️ PropertyRequest ${property._id} için userId bulunamadı, atlanıyor`);
          skippedCount++;
          continue;
        }
        
        // PropertyRequest'ten marketplace listing oluştur
        const listing = {
          propertyId: property._id,
          agentId: property.userId._id,
          title: `${property.il} ${property.ilce} ${property.mahalle}`,
          status: 'belirsiz', // Varsayılan olarak belirsiz
          price: {
            // Fiyat bilgisi PropertyRequest'ten gelecek
            amount: undefined,
            currency: 'TRY',
            type: undefined
          },
          contact: {
            phone: {
              masked: maskPhone(property.userId.phone || '5551234567'),
              original: property.userId.phone || '5551234567',
              showPhone: false
            }
          },
          stats: {
            viewCount: 0,
            favoriteCount: 0
          },
          settings: {
            isPublic: true,
            allowCoBroker: true,
            showContact: true
          },
          coBrokers: [],
          coBrokerRequests: [],
          missingInfoRequests: []
        };
        
        // Marketplace listing'i kaydet
        await Marketplace.create(listing);
        createdCount++;
        
        if (createdCount % 10 === 0) {
          console.log(`✅ ${createdCount} listing oluşturuldu...`);
        }
        
      } catch (error) {
        console.error(`❌ Listing oluşturma hatası (${property._id}):`, error.message);
        skippedCount++;
      }
    }
    
    console.log('\n🎉 Marketplace listings oluşturma tamamlandı!');
    console.log(`✅ Başarılı: ${createdCount}`);
    console.log(`❌ Başarısız: ${skippedCount}`);
    console.log(`📊 Toplam: ${properties.length}`);
    
  } catch (error) {
    console.error('❌ Genel hata:', error);
  } finally {
    mongoose.connection.close();
  }
};

// Telefon numarasını maskele
const maskPhone = (phone) => {
  if (!phone) return '*** *** ** **';
  const str = phone.toString();
  if (str.length < 10) return '*** *** ** **';
  return `${str.slice(0, 3)} *** ${str.slice(6, 8)} ${str.slice(8, 10)}`;
};

// Script'i çalıştır
createMarketplaceListings();
