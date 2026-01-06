const mongoose = require('mongoose');

const videoDataSchema = new mongoose.Schema({
  videoId: {
    type: String,
    required: true,
    unique: true
  },
  userId: {
    type: String,
    required: true
  },
  description: String,
  language: String,
  propertyDetails: {
    il: String,
    ilce: String,
    mahalle: String,
    adaNo: String,
    parselNo: String,
    tapu: String
  },
  createdAt: {
    type: Date,
    default: Date.now
  }
});

module.exports = mongoose.model('VideoData', videoDataSchema);
