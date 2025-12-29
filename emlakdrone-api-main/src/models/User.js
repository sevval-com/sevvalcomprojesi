const mongoose = require('mongoose');
const bcrypt = require('bcryptjs');

const userSchema = new mongoose.Schema({
  email: {
    type: String,
    required: true,
    unique: true,
    trim: true,
    lowercase: true
  },
  password: {
    type: String,
    required: true
  },
  name: {
    type: String,
    required: true,
    trim: true
  },
  surname: {
    type: String,
    required: true,
    trim: true
  },
  phone: {
    type: String,
    required: true,
    trim: true
  },
  company: {
    type: String,
    default: ''
  },
  pushToken: {
    type: String,
    default: null
  },
  deviceInfo: {
    platform: {
      type: String,
      default: null
    },
    deviceName: {
      type: String,
      default: 'Unknown'
    },
    isDevice: {
      type: Boolean,
      default: false
    },
    isEmulator: {
      type: Boolean,
      default: false
    },
    updatedAt: {
      type: Date,
      default: Date.now
    }
  },
  userType: {
    type: String,
    enum: ['user', 'admin'],
    default: 'user'
  },
  membershipType: {
    type: String,
    enum: ['none', 'monthly', 'yearly'],
    default: 'none'
  },
  isMembership: {
    type: Boolean,
    default: false
  },
  membershipExpireDate: {
    type: Date,
    default: null
  },
  videoCount: {
    type: Number,
    default: 0
  },
  lastVideoResetDate: {
    type: Date,
    default: Date.now
  },
  // Aylık üyelikler için bir önceki aydan devreden haklar
  rolloverCredits: {
    type: Number,
    default: 0
  },
  singleVideoRights: {
    type: Number,
    default: 0  // Satın alınan tek seferlik haklar
  },
  rcUserId: {
    type: String,
    sparse: true,
    index: true
  },
  lastWebhookEvent: {
    type: {
      type: String
    },
    timestamp: Date,
    originalEvent: Object
  },
  viewedPhones: [{
    type: mongoose.Schema.Types.ObjectId,
    ref: 'Marketplace'
  }]
}, { timestamps: true });

// Video oluşturma kontrolü
userSchema.methods.canCreateVideo = function() {
  const now = new Date();
  const thirtyDaysAgo = new Date(now.getTime() - (30 * 24 * 60 * 60 * 1000));
  
  // Üyelik aktif mi ve süresi geçmemiş mi?
  const isActiveMembership = this.isMembership && (!this.membershipExpireDate || this.membershipExpireDate >= now);

  // Yıllık üyelikte limit kaldırıldı (aktifse her zaman izin ver)
  if (isActiveMembership && this.membershipType === 'yearly') {
    console.log('✅ Video oluşturma serbest - Yıllık üyelik (sınırsız)', {
      userId: this._id,
      membershipType: this.membershipType
    });
    return true;
  }

  // Aylık üyelikte 30 gün periyodu resetle
  if (isActiveMembership && this.lastVideoResetDate < thirtyDaysAgo) {
    this.videoCount = 0;
    this.lastVideoResetDate = now;
    console.log('🔄 30 günlük periyot yenilendi:', {
      userId: this._id,
      oldCount: this.videoCount,
      newResetDate: now
    });
  }

  // Aktif üyelik: aylık limit + rollover kontrolü
  if (isActiveMembership && this.membershipType === 'monthly') {
    const monthlyLimit = 10;
    const remainingBase = Math.max(0, monthlyLimit - this.videoCount);
    const totalRemaining = remainingBase + (this.rolloverCredits || 0);
    const canCreateMonthly = totalRemaining > 0;
    console.log('🎥 Video oluşturma kontrolü (aylık + rollover):', {
      userId: this._id,
      membershipType: this.membershipType,
      usedThisPeriod: this.videoCount,
      rolloverCredits: this.rolloverCredits || 0,
      baseLimit: monthlyLimit,
      totalRemaining,
      canCreate: canCreateMonthly
    });
    return canCreateMonthly;
  }

  // Üyelik yok veya aktif değil: tek seferlik hak varsa izin ver
  if (!isActiveMembership) {
    const hasSingleRight = (this.singleVideoRights || 0) > 0;
    console.log('🎥 Video oluşturma kontrolü (üye değil):', {
      userId: this._id,
      singleVideoRights: this.singleVideoRights,
      canCreate: hasSingleRight
    });
    return hasSingleRight;
  }

  return false;
};

// Video sayacını artır
userSchema.methods.incrementVideoCount = async function() {
  const now = new Date();
  const thirtyDaysAgo = new Date(now.getTime() - (30 * 24 * 60 * 60 * 1000));
  
  const isActiveMembership = this.isMembership && (!this.membershipExpireDate || this.membershipExpireDate >= now);

  // Üyelik yok/aktif değil: tek seferlik hakkı tüket
  if (!isActiveMembership) {
    if ((this.singleVideoRights || 0) > 0) {
      this.singleVideoRights -= 1;
      console.log('🟡 Tek seferlik video hakkı tüketildi:', {
        userId: this._id,
        remainingSingleVideoRights: this.singleVideoRights
      });
      await this.save();
      return true;
    }
    console.log('⚠️ Tek seferlik hakkı yok, sayaç artırılamadı:', { userId: this._id });
    return false;
  }

  // Aktif üyelik: yıllık sınırsız, sayacı artırmak opsiyonel (istatistik)
  if (this.membershipType === 'yearly') {
    this.videoCount += 1;
    console.log('➕ (Yıllık) Video sayacı artırıldı (istatistik):', {
      userId: this._id,
      newCount: this.videoCount
    });
  } else {
    // Aylık üyelik: 30 gün periyodu kontrol et ve rollover uygula
    if (this.lastVideoResetDate < thirtyDaysAgo) {
      const monthlyLimit = 10;
      const usedLastPeriod = this.videoCount;
      const unusedLastPeriod = Math.max(0, monthlyLimit - Math.min(usedLastPeriod, monthlyLimit));
      // Sadece bir önceki ayın kullanılmayan haklarını devret
      this.rolloverCredits = unusedLastPeriod;
      this.videoCount = 0; // Yeni periyot başlangıcı için sıfırla
      this.lastVideoResetDate = now;
      console.log('🔄 Yeni periyot + rollover:', {
        userId: this._id,
        carriedOver: this.rolloverCredits,
        resetDate: now
      });
    }

    // Hakkı artır: önce baz limitten, sonra rollover’dan tüket
    const monthlyLimit = 10;
    if (this.videoCount < monthlyLimit) {
      this.videoCount += 1;
      console.log('➕ Video sayacı artırıldı (baz limit):', {
        userId: this._id,
        usedThisPeriod: this.videoCount
      });
    } else if ((this.rolloverCredits || 0) > 0) {
      this.videoCount += 1;
      this.rolloverCredits -= 1;
      console.log('➕ Video sayacı artırıldı (rollover tüketildi):', {
        userId: this._id,
        usedThisPeriod: this.videoCount,
        remainingRollover: this.rolloverCredits
      });
    } else {
      console.log('⚠️ Rollover yok, sayaç artırılamadı:', { userId: this._id });
      await this.save();
      return false;
    }
  }

  await this.save();
  return true;
};

// Üyelik durumu kontrolü
userSchema.methods.checkMembershipStatus = function() {
  const now = new Date();
  
  if (!this.membershipType || this.membershipType === 'none') {
    this.isMembership = false;
    console.log('⚠️ Üyelik yok:', {
      userId: this._id,
      membershipType: this.membershipType
    });
    return false;
  }

  if (!this.membershipExpireDate) {
    console.log('⚠️ Üyelik bitiş tarihi yok:', {
      userId: this._id,
      membershipType: this.membershipType
    });
    return false;
  }

  const isActive = now < this.membershipExpireDate;
  this.isMembership = isActive;

  console.log('🔍 Üyelik durumu kontrolü:', {
    userId: this._id,
    membershipType: this.membershipType,
    expireDate: this.membershipExpireDate,
    isActive: isActive
  });

  return isActive;
};

// Şifre hash'leme
userSchema.pre('save', async function(next) {
  if (this.isModified('password')) {
    this.password = await bcrypt.hash(this.password, 10);
  }
  
  // Üyelik durumunu kontrol et
  if (this.isModified('membershipType')) {
    this.checkMembershipStatus();
  }
  
  next();
});

// Şifre doğrulama
userSchema.methods.comparePassword = async function(candidatePassword) {
  return bcrypt.compare(candidatePassword, this.password);
};

module.exports = mongoose.model('User', userSchema); 