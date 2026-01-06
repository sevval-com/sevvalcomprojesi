const Marketplace = require('../models/Marketplace');
const PropertyRequest = require('../models/PropertyRequest');
const User = require('../models/User');

// Rate limiting için basit cache
const phoneViewCache = new Map();
const RATE_LIMIT_WINDOW = 5 * 60 * 1000; // 5 dakika

class MarketplaceController {
  // Tüm ilanları getir (harita için)
  async getAllListings(req, res) {
    try {
      const { 
        status, 
        minPrice, 
        maxPrice, 
        bounds, // "minLng,minLat,maxLng,maxLat"
        page = 1,
        limit = 50
      } = req.query;

      // Query builder
      let query = { 'settings.isPublic': true };
      
      // Kullanıcının kendi ilanlarını hariç tut
      const userId = req.userId; // Auth middleware'den gelen userId
      if (userId) {
        query.agentId = { $ne: userId };
      }
      
      if (status && status !== 'all') {
        query.status = status;
      }
      
      if (minPrice || maxPrice) {
        query['price.amount'] = {};
        if (minPrice) query['price.amount'].$gte = Number(minPrice);
        if (maxPrice) query['price.amount'].$lte = Number(maxPrice);
      }

      // Geo-spatial query (bounds varsa)
      if (bounds) {
        const [minLng, minLat, maxLng, maxLat] = bounds.split(',').map(Number);
        query.location = {
          $geoWithin: {
            $box: [
              [minLng, minLat],
              [maxLng, maxLat]
            ]
          }
        };
      }

      // Pagination
      const skip = (page - 1) * limit;
      
      // Performans için sadece gerekli alanları getir
      const listings = await Marketplace.find(query)
        .select('status title price location contact.phone contact.phoneViewCount stats.viewCount createdAt settings agentId')
        .populate('propertyId', 'il ilce mahalle adaNo parseller location')
        .populate('agentId', 'name surname company')
        .sort({ createdAt: -1 })
        .skip(skip)
        .limit(limit)
        .lean();

      // Kullanıcının daha önce görüntülediği telefon numaralarını kontrol et
      const User = require('../models/User');
      const user = await User.findById(userId).select('viewedPhones').lean();
      const viewedPhones = user?.viewedPhones || [];

      // Listing'leri kullanıcı bazında düzenle
      const processedListings = listings.map(listing => {
        const hasViewedPhone = viewedPhones.includes(listing._id.toString());
        
        return {
          ...listing,
          contact: {
            ...listing.contact,
            phone: {
              ...listing.contact.phone,
              showPhone: hasViewedPhone
            }
          }
        };
      });

      // Toplam sayı
      const total = await Marketplace.countDocuments(query);

      res.json({
        success: true,
        data: processedListings,
        pagination: {
          page: Number(page),
          limit: Number(limit),
          total,
          pages: Math.ceil(total / limit)
        }
      });

    } catch (error) {
      console.error('Marketplace listings getirme hatası:', error);
      res.status(500).json({
        success: false,
        message: 'İlanlar getirilemedi'
      });
    }
  }

  // Eksik bilgi bildirimi gönder
  async requestMissingInfo(req, res) {
    try {
      const { listingId } = req.params;
      const { type, message } = req.body;
      const userId = req.userId;
      
      // IP adresini logla
      const realIP = req.headers['x-forwarded-for'] || 
                     req.headers['x-real-ip'] || 
                     req.headers['x-vercel-forwarded-for'] || 
                     req.ip || 
                     req.connection.remoteAddress;
      
      console.log(`🔍 Eksik bilgi bildirimi isteği:`, {
        listingId,
        type,
        message,
        userId,
        realIP,
        headers: req.headers
      });
      
      // Listing'i bul
      const listing = await Marketplace.findById(listingId);
      if (!listing) {
        return res.status(404).json({
          success: false,
          message: 'İlan bulunamadı'
        });
      }
      
      // Kendi ilanına bildirim gönderemez
      if (listing.agentId.toString() === userId) {
        return res.status(400).json({
          success: false,
          message: 'Kendi ilanınıza bildirim gönderemezsiniz'
        });
      }
      
      // Zaten bildirim gönderilmiş mi kontrol et
      const existingRequest = listing.missingInfoRequests.find(
        req => req.fromUserId.toString() === userId && req.status === 'pending'
      );
      
      if (existingRequest) {
        return res.status(400).json({
          success: false,
          message: 'Zaten bildirim gönderilmiş'
        });
      }
      
      // Bildirimi ekle
      listing.missingInfoRequests.push({
        fromUserId: userId,
        type,
        message: message || `${type === 'price' ? 'Fiyat' : type === 'status' ? 'Durum' : 'Fiyat ve durum'} bilgisi eklenmesi isteniyor`
      });
      
      await listing.save();
      
      res.json({
        success: true,
        message: 'Bildirim gönderildi'
      });
      
    } catch (error) {
      console.error('Eksik bilgi bildirimi hatası:', error);
      res.status(500).json({
        success: false,
        message: 'Bildirim gönderilemedi'
      });
    }
  }

  // Eksik bilgi bildirimlerini getir (ilan sahibi için)
  async getMissingInfoRequests(req, res) {
    try {
      const { listingId } = req.params;
      const userId = req.userId;
      
      // Listing'i bul
      const listing = await Marketplace.findById(listingId);
      if (!listing) {
        return res.status(404).json({
          success: false,
          message: 'İlan bulunamadı'
        });
      }
      
      // Sadece ilan sahibi eksik bilgi bildirimlerini görebilir
      if (listing.agentId.toString() !== userId) {
        return res.status(403).json({
          success: false,
          message: 'Bu işlem için yetkiniz yok'
        });
      }
      
      // Eksik bilgi bildirimlerini getir
      const missingInfoRequests = listing.missingInfoRequests || [];
      
      res.json({
        success: true,
        data: missingInfoRequests,
        count: missingInfoRequests.length
      });
      
    } catch (error) {
      console.error('Eksik bilgi bildirimleri getirme hatası:', error);
      res.status(500).json({
        success: false,
        message: 'Bildirimler getirilemedi'
      });
    }
  }

  // Eksik bilgi bildirimini güncelle (ilan sahibi için)
  async updateMissingInfoRequest(req, res) {
    try {
      const { listingId, requestId } = req.params;
      const { status, response } = req.body;
      const userId = req.userId;
      
      // Listing'i bul
      const listing = await Marketplace.findById(listingId);
      if (!listing) {
        return res.status(404).json({
          success: false,
          message: 'İlan bulunamadı'
        });
      }
      
      // Sadece ilan sahibi eksik bilgi bildirimini güncelleyebilir
      if (listing.agentId.toString() !== userId) {
        return res.status(403).json({
          success: false,
          message: 'Bu işlem için yetkiniz yok'
        });
      }
      
      // Bildirimi bul
      const requestIndex = listing.missingInfoRequests.findIndex(
        req => req._id.toString() === requestId
      );
      
      if (requestIndex === -1) {
        return res.status(404).json({
          success: false,
          message: 'Bildirim bulunamadı'
        });
      }
      
      // Bildirimi güncelle
      listing.missingInfoRequests[requestIndex].status = status;
      if (response) {
        listing.missingInfoRequests[requestIndex].response = response;
      }
      listing.missingInfoRequests[requestIndex].respondedAt = new Date();
      
      await listing.save();
      
      res.json({
        success: true,
        message: 'Bildirim güncellendi',
        data: listing.missingInfoRequests[requestIndex]
      });
      
    } catch (error) {
      console.error('Eksik bilgi bildirimi güncelleme hatası:', error);
      res.status(500).json({
        success: false,
        message: 'Bildirim güncellenemedi'
      });
    }
  }

  // Telefon görüntüleme
  async viewPhone(req, res) {
    try {
      const { listingId } = req.params;
      const userId = req.userId;
      
      // IP adresini logla
      const realIP = req.headers['x-forwarded-for'] || 
                     req.headers['x-real-ip'] || 
                     req.headers['x-vercel-forwarded-for'] || 
                     req.ip || 
                     req.connection.remoteAddress;
      
      console.log(`📞 Telefon görüntüleme isteği:`, {
        listingId,
        userId,
        realIP,
        timestamp: new Date().toISOString()
      });
      
      // Listing'i bul
      const listing = await Marketplace.findById(listingId);
      if (!listing) {
        return res.status(404).json({
          success: false,
          message: 'İlan bulunamadı'
        });
      }
      
      // Kendi ilanının telefonunu görüntüleyemez
      if (listing.agentId.toString() === userId) {
        return res.status(400).json({
          success: false,
          message: 'Kendi ilanınızın telefonunu görüntüleyemezsiniz'
        });
      }
      
      // Rate limiting kontrolü (1 dakikada maksimum 2 kez)
      const now = new Date();
      const oneMinuteAgo = new Date(now.getTime() - 1 * 60 * 1000);
      
      // Son 1 dakikada kaç kez görüntülenmiş kontrol et
      const recentViews = listing.contact?.phone?.recentViews || [];
      const viewsInLastMinute = recentViews.filter(viewTime => 
        new Date(viewTime) > oneMinuteAgo
      );
      
      if (viewsInLastMinute.length >= 2) {
        const oldestView = Math.min(...viewsInLastMinute.map(v => new Date(v).getTime()));
        const waitTime = Math.ceil((oldestView + 1 * 60 * 1000 - now.getTime()) / 1000);
        
        return res.status(429).json({
          success: false,
          message: 'Çok sık telefon görüntüleme. 1 dakikada maksimum 2 kez görüntüleyebilirsiniz.',
          retryAfter: waitTime
        });
      }
      
      // Telefon görüntüleme sayısını artır
      if (!listing.contact.phone) {
        listing.contact.phone = {};
      }
      
      if (!listing.contact.phone.phoneViewCount) {
        listing.contact.phone.phoneViewCount = 0;
      }
      
      if (!listing.contact.phone.recentViews) {
        listing.contact.phone.recentViews = [];
      }
      
      // Son görüntüleme zamanını ekle
      listing.contact.phone.recentViews.push(now);
      
      // Son 5 dakikadan eski görüntülemeleri temizle (performans için)
      const fiveMinutesAgo = new Date(now.getTime() - 5 * 60 * 1000);
      listing.contact.phone.recentViews = listing.contact.phone.recentViews.filter(
        viewTime => new Date(viewTime) > fiveMinutesAgo
      );
      
      listing.contact.phone.phoneViewCount += 1;
      listing.contact.phone.lastPhoneView = now;
      
      await listing.save();
      
      // Kullanıcının görüntülediği telefon numaralarını güncelle
      const User = require('../models/User');
      await User.findByIdAndUpdate(
        userId,
        { $addToSet: { viewedPhones: listingId } }, // $addToSet duplicates'i önler
        { upsert: true }
      );
      
      // Telefon numarasını döndür
      res.json({
        success: true,
        data: {
          phone: listing.contact.phone.original,
          maskedPhone: listing.contact.phone.masked,
          viewCount: listing.contact.phone.phoneViewCount
        },
        message: 'Telefon numarası görüntülendi'
      });
      
    } catch (error) {
      console.error('Telefon görüntüleme hatası:', error);
      res.status(500).json({
        success: false,
        message: 'Telefon görüntülenemedi'
      });
    }
  }

  // Telefon maskeleme yardımcı fonksiyonu
  maskPhoneNumber(phone) {
    if (!phone || phone.length <= 4) return phone;
    
    const visible = phone.slice(-4);
    const masked = '*'.repeat(phone.length - 4);
    return masked + visible;
  }
}

module.exports = new MarketplaceController();
