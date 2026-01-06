const PropertyRequest = require('../models/PropertyRequest');
const Marketplace = require('../models/Marketplace');
const User = require('../models/User');
const jwt = require('jsonwebtoken');

exports.createRequest = async (req, res) => {
  try {
    const token = req.headers.authorization.split(' ')[1];
    const decoded = jwt.verify(token, process.env.JWT_SECRET);
    const userId = decoded.id;

    console.log('Gelen veri:', req.body);

    const propertyRequests = Array.isArray(req.body) ? req.body : [req.body];
    
    const savedProperties = await Promise.all(propertyRequests.map(async (property) => {
      // Zorunlu alan doğrulaması
      if (!property.status || !['satılık', 'kiralık', 'pazarlıkta'].includes(property.status)) {
        throw new Error('Geçerli bir status zorunludur: satılık | kiralık | pazarlıkta');
      }
      if (!property.price || typeof property.price.amount !== 'number' || property.price.amount <= 0) {
        throw new Error('Geçerli bir price.amount zorunludur');
      }
      if (!property.price.type || !['satış', 'kiralama'].includes(property.price.type)) {
        throw new Error('Geçerli bir price.type zorunludur: satış | kiralama');
      }
      // Tüm parsellerin koordinatlarını kullanarak merkez noktayı hesapla
      const allCoords = property.parseller.flatMap(parsel => 
        parsel.parselCoordinates || []
      );
      
      const centerLat = allCoords.reduce((sum, coord) => 
        sum + coord.latitude, 0) / allCoords.length;
      const centerLng = allCoords.reduce((sum, coord) => 
        sum + coord.longitude, 0) / allCoords.length;

      const newProperty = new PropertyRequest({
        userId,
        il: property.il,
        ilce: property.ilce,
        mahalle: property.mahalle,
        mevkii: property.mevkii,
        nitelik: property.nitelik,
        adaNo: property.adaNo,
        status: property.status,
        price: {
          amount: property.price.amount,
          currency: property.price.currency || 'TRY',
          type: property.price.type
        },
        location: {
          lat: centerLat,
          lng: centerLng
        },
        parseller: property.parseller.map(parsel => ({
          parselNo: parsel.parselNo,
          alan: parsel.alan,
          coordinates: {
            lat: parsel.coordinates?.lat || centerLat,
            lng: parsel.coordinates?.lng || centerLng
          },
          parselCoordinates: parsel.parselCoordinates
        }))
      });

      const saved = await newProperty.save();
      console.log('Kaydedilen emlak:', saved);

      // Marketplace listing oluştur/ güncelle (idempotent)
      try {
        // Kullanıcı telefon bilgisini getir
        const user = await User.findById(saved.userId).select('phone');
        const originalPhone = user?.phone || '';
        const maskedPhone = (p => {
          if (!p) return '*** *** ** **';
          const str = p.toString();
          if (str.length < 10) return '*** *** ** **';
          return `${str.slice(0, 3)} *** ${str.slice(6, 8)} ${str.slice(8, 10)}`;
        })(originalPhone);

        const point = saved.location && saved.location.lat && saved.location.lng
          ? { type: 'Point', coordinates: [saved.location.lng, saved.location.lat] }
          : { type: 'Point', coordinates: [0, 0] }; // Default koordinatlar

        await Marketplace.findOneAndUpdate(
          { propertyId: saved._id },
          {
            $set: {
              propertyId: saved._id,
              agentId: saved.userId,
              title: `${saved.il} ${saved.ilce} ${saved.mahalle}`.trim(),
              status: saved.status || 'belirsiz',
              price: {
                amount: saved.price?.amount,
                currency: saved.price?.currency || 'TRY',
                type: saved.price?.type
              },
              contact: {
                phone: {
                  masked: maskedPhone,
                  original: originalPhone,
                  showPhone: false,
                }
              },
              location: point,
              settings: { isPublic: true, allowCoBroker: true, showContact: true }
            }
          },
          { upsert: true, new: true }
        );
      } catch (mkErr) {
        console.error('Marketplace oluşturma/güncelleme hatası:', mkErr.message);
      }
      return saved;
    }));

    res.status(201).json({
      message: 'Emlak talepleri başarıyla oluşturuldu',
      properties: savedProperties
    });
  } catch (error) {
    console.error('Emlak talebi oluşturma hatası:', error);
    res.status(500).json({
      message: 'Emlak talebi oluşturulurken bir hata oluştu',
      error: error.message
    });
  }
};

exports.getRequests = async (req, res) => {
  try {
    const token = req.headers.authorization.split(' ')[1];
    const decoded = jwt.verify(token, process.env.JWT_SECRET);
    const userId = decoded.id;

    const properties = await PropertyRequest.find({ userId })
      .sort({ createdAt: -1 });

    // Her property için location kontrolü yap
    const propertiesWithLocation = properties.map(property => {
      if (!property.location) {
        // Location yoksa parsellerin koordinatlarından hesapla
        const allCoords = property.parseller.flatMap(parsel => 
          parsel.parselCoordinates || []
        );
        
        const centerLat = allCoords.reduce((sum, coord) => 
          sum + coord.latitude, 0) / allCoords.length;
        const centerLng = allCoords.reduce((sum, coord) => 
          sum + coord.longitude, 0) / allCoords.length;

        property.location = {
          lat: centerLat,
          lng: centerLng
        };
      }
      return property;
    });

    console.log('Bulunan emlak talepleri:', propertiesWithLocation.length);
    res.status(200).json(propertiesWithLocation);
  } catch (error) {
    console.error('Emlak talepleri getirme hatası:', error);
    res.status(500).json({
      message: 'Emlak talepleri getirilirken bir hata oluştu',
      error: error.message
    });
  }
};

exports.getRequestById = async (req, res) => {
  try {
    const request = await PropertyRequest.findById(req.params.id)
      .populate('userId', 'name surname email');
    
    if (!request) {
      return res.status(404).json({ message: 'Emlak talebi bulunamadı' });
    }

    // Location kontrolü yap
    if (!request.location) {
      const allCoords = request.parseller.flatMap(parsel => 
        parsel.parselCoordinates || []
      );
      
      const centerLat = allCoords.reduce((sum, coord) => 
        sum + coord.latitude, 0) / allCoords.length;
      const centerLng = allCoords.reduce((sum, coord) => 
        sum + coord.longitude, 0) / allCoords.length;

      request.location = {
        lat: centerLat,
        lng: centerLng
      };
    }

    console.log('Emlak talebi detayları:', request);
    res.status(200).json(request);
  } catch (error) {
    console.error('Emlak talebi bulma hatası:', error);
    res.status(500).json({ 
      message: 'Emlak talebi bulunurken bir hata oluştu',
      error: error.message 
    });
  }
};

exports.updateRequestStatus = async (req, res) => {
  try {
    const { status } = req.body;
    const request = await PropertyRequest.findByIdAndUpdate(
      req.params.id, 
      { status }, 
      { new: true }
    );
    
    if (!request) {
      return res.status(404).json({ message: 'Emlak talebi bulunamadı' });
    }

    console.log('Emlak talebi durumu güncellendi:', request);
    res.status(200).json(request);
  } catch (error) {
    console.error('Emlak talebi durumu güncelleme hatası:', error);
    res.status(500).json({ 
      message: 'Emlak talebi durumu güncellenirken bir hata oluştu',
      error: error.message 
    });
  }
};

exports.deleteRequest = async (req, res) => {
  try {
    const request = await PropertyRequest.findByIdAndDelete(req.params.id);
    
    if (!request) {
      return res.status(404).json({ message: 'Emlak talebi bulunamadı' });
    }

    console.log('Emlak talebi silindi:', request);
    res.status(200).json({ 
      message: 'Emlak talebi başarıyla silindi',
      deletedProperty: request
    });
  } catch (error) {
    console.error('Emlak talebi silme hatası:', error);
    res.status(500).json({ 
      message: 'Emlak talebi silinirken bir hata oluştu',
      error: error.message 
    });
  }
}; 