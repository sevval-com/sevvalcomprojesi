const mongoose = require('mongoose');

const videoRequestSchema = new mongoose.Schema({
  userId: {
    type: mongoose.Schema.Types.ObjectId,
    ref: 'User',
    required: true
  },
  status: {
    type: String,
    enum: ['pending', 'processing', 'completed', 'cancelled'],
    default: 'pending'
  },
  gender: {
    type: String,
    enum: ['male', 'female'],
    required: true
  },
  script: {
    type: String,
    required: true,
    maxlength: 1000
  },
  images: [{
    filename: String,
    originalName: String,
    mimetype: String,
    size: Number,
    uploadedAt: {
      type: Date,
      default: Date.now
    }
  }],
  videoUrl: {
    type: String,
    default: null
  },
  adminNotes: {
    type: String,
    default: null
  },
  completedAt: {
    type: Date,
    default: null
  },
  notificationSent: {
    type: Boolean,
    default: false
  }
}, {
  timestamps: true
});

// Index'ler
videoRequestSchema.index({ userId: 1, createdAt: -1 });
videoRequestSchema.index({ status: 1 });
videoRequestSchema.index({ createdAt: -1 });

// Virtual field for image count
videoRequestSchema.virtual('imageCount').get(function() {
  return this.images.length;
});

// Video tamamlandığında completedAt'i güncelle
videoRequestSchema.pre('save', function(next) {
  if (this.status === 'completed' && !this.completedAt) {
    this.completedAt = new Date();
  }
  next();
});

module.exports = mongoose.model('VideoRequest', videoRequestSchema);
