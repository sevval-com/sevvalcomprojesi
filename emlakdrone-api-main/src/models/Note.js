const mongoose = require('mongoose');

const noteSchema = new mongoose.Schema({
  propertyId: {
    type: String,
    required: true
  },
  userId: {
    type: mongoose.Schema.Types.ObjectId,
    ref: 'User',
    required: true
  },
  title: {
    type: String,
    required: true,
    trim: true
  },
  content: {
    type: String,
    required: true,
    trim: true
  },
  tags: [{
    type: String,
    trim: true
  }],
  color: {
    type: String,
    default: '#ffd200',
    validate: {
      validator: function(v) {
        return /^#[0-9A-F]{6}$/i.test(v);
      },
      message: 'Geçersiz renk formatı'
    }
  },
  updatedAt: {
    type: Date,
    default: Date.now
  }
}, {
  timestamps: true
});

// Index'ler ekle - duplicate index uyarısını önlemek için
noteSchema.index({ propertyId: 1, userId: 1 }, { unique: false });
noteSchema.index({ userId: 1 }, { unique: false });
noteSchema.index({ updatedAt: -1 }, { unique: false });

module.exports = mongoose.model('Note', noteSchema);
