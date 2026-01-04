const mongoose = require('mongoose');

const templateSchema = new mongoose.Schema({
  userId: {
    type: mongoose.Schema.Types.ObjectId,
    ref: 'User',
    required: true
  },
  templateId: {
    type: String,
    required: true
  },
  status: {
    type: String,
    enum: ['processing', 'completed', 'failed'],
    default: 'processing'
  },
  downloadUrl: String,
  templateData: {
    type: Object,
    required: true
  },
  notificationSent: {
    type: Boolean,
    default: false
  },
  createdAt: {
    type: Date,
    default: Date.now
  }
});

module.exports = mongoose.model('Template', templateSchema); 