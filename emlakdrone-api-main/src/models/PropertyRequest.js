const mongoose = require('mongoose');

const parselSchema = new mongoose.Schema({
  parselNo: {
    type: String,
    required: true
  },
  alan: String,
  coordinates: {
    lat: Number,
    lng: Number
  },
  parselCoordinates: [{
    latitude: Number,
    longitude: Number
  }]
});

const propertyRequestSchema = new mongoose.Schema({
  userId: {
    type: mongoose.Schema.Types.ObjectId,
    ref: 'User',
    required: true
  },
  il: {
    type: String,
    required: true
  },
  ilce: {
    type: String,
    required: true
  },
  mahalle: {
    type: String,
    required: true
  },
  mevkii: String,
  nitelik: String,
  adaNo: {
    type: String,
    required: true
  },
  // Marketplace filtreleriyle uyumlu zorunlu alanlar
  status: {
    type: String,
    enum: ['satılık', 'kiralık', 'pazarlıkta'],
    required: true
  },
  price: {
    amount: {
      type: Number,
      required: true
    },
    currency: {
      type: String,
      enum: ['TRY', 'USD', 'EUR'],
      default: 'TRY'
    },
    type: {
      type: String,
      enum: ['satış', 'kiralama'],
      required: true
    }
  },
  location: {
    lat: Number,
    lng: Number
  },
  parseller: [parselSchema],
  requestStatus: {
    type: String,
    enum: ['pending', 'approved', 'rejected'],
    default: 'pending'
  }
}, { timestamps: true });

// Property silindiğinde notları da sil
propertyRequestSchema.pre('deleteOne', { document: true, query: false }, async function(next) {
  try {
    const Note = require('./Note');
    await Note.deleteMany({ propertyId: this._id });
    next();
  } catch (error) {
    next(error);
  }
});

// Property silindiğinde notları da sil (findOneAndDelete için)
propertyRequestSchema.pre('findOneAndDelete', async function(next) {
  try {
    const Note = require('./Note');
    const propertyId = this.getQuery()._id;
    if (propertyId) {
      await Note.deleteMany({ propertyId });
    }
    next();
  } catch (error) {
    next(error);
  }
});

module.exports = mongoose.model('PropertyRequest', propertyRequestSchema); 