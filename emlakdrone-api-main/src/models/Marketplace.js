const mongoose = require('mongoose');

const marketplaceSchema = new mongoose.Schema({
  // PropertyRequest ile birebir bağlantı
  propertyId: {
    type: mongoose.Schema.Types.ObjectId,
    ref: 'PropertyRequest',
    required: true,
    unique: true // Her PropertyRequest için sadece 1 marketplace listing
  },
  
  // Harita filtreleri için konum (GeoJSON Point)
  location: {
    type: {
      type: String,
      enum: ['Point'],
      default: 'Point'
    },
    coordinates: {
      type: [Number], // [lng, lat]
      index: '2dsphere',
      default: [0, 0] // Default koordinatlar (Türkiye merkezi)
    }
  },
  
  // Emlakçı bilgisi
  agentId: {
    type: mongoose.Schema.Types.ObjectId,
    ref: 'User',
    required: true
  },
  
  // İlan bilgileri
  title: {
    type: String,
    required: true
  },
  
  // Durum (PropertyRequest'ten alınacak, yoksa belirsiz)
  status: {
    type: String,
    enum: ['satılık', 'kiralık', 'pazarlıkta', 'belirsiz'],
    default: 'belirsiz'
  },
  
  // Fiyat bilgisi (PropertyRequest'ten alınacak, yoksa null)
  price: {
    amount: {
      type: Number,
      required: false // Zorunlu değil, PropertyRequest'ten gelecek
    },
    currency: {
      type: String,
      enum: ['TRY', 'USD', 'EUR'],
      default: 'TRY'
    },
    type: {
      type: String,
      enum: ['satış', 'kiralama'],
      required: false // Zorunlu değil, PropertyRequest'ten gelecek
    }
  },
  
  // İletişim bilgileri
  contact: {
    phone: {
      masked: {
        type: String,
        required: true
      },
      original: {
        type: String,
        required: true
      },
      showPhone: {
        type: Boolean,
        default: false
      },
      phoneViewCount: {
        type: Number,
        default: 0
      },
      lastPhoneView: {
        type: Date,
        default: null
      },
      recentViews: [{
        type: Date,
        default: []
      }]
    }
  },
  
  // İstatistikler
  stats: {
    viewCount: {
      type: Number,
      default: 0
    },
    favoriteCount: {
      type: Number,
      default: 0
    }
  },
  
  // Ayarlar
  settings: {
    isPublic: {
      type: Boolean,
      default: true
    },
    allowCoBroker: {
      type: Boolean,
      default: true
    },
    showContact: {
      type: Boolean,
      default: true
    }
  },
  
  // Co-broker sistemi
  coBrokers: [{
    agentId: {
      type: mongoose.Schema.Types.ObjectId,
      ref: 'User'
    },
    joinedAt: {
      type: Date,
      default: Date.now
    }
  }],
  
  coBrokerRequests: [{
    fromAgentId: {
      type: mongoose.Schema.Types.ObjectId,
      ref: 'User'
    },
    message: String,
    status: {
      type: String,
      enum: ['pending', 'approved', 'rejected'],
      default: 'pending'
    },
    createdAt: {
      type: Date,
      default: Date.now
    }
  }],
  
  // Eksik bilgi bildirimleri
  missingInfoRequests: [{
    fromUserId: {
      type: mongoose.Schema.Types.ObjectId,
      ref: 'User'
    },
    type: {
      type: String,
      enum: ['price', 'status', 'both'],
      required: true
    },
    message: String,
    status: {
      type: String,
      enum: ['pending', 'resolved', 'ignored'],
      default: 'pending'
    },
    createdAt: {
      type: Date,
      default: Date.now
    }
  }],
  
  // PropertyRequest'ten otomatik güncellenen bilgiler
  lastPropertyUpdate: {
    type: Date,
    default: Date.now
  }
}, {
  timestamps: true
});

// Index'ler
marketplaceSchema.index({ "location": "2dsphere" });
marketplaceSchema.index({ "status": 1 });
marketplaceSchema.index({ "createdAt": -1 });
marketplaceSchema.index({ "agentId": 1 });
marketplaceSchema.index({ "propertyId": 1 }, { unique: true });

module.exports = mongoose.model('Marketplace', marketplaceSchema);
