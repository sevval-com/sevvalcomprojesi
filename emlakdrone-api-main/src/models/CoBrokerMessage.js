const mongoose = require('mongoose');

const coBrokerMessageSchema = new mongoose.Schema({
  // Hangi marketplace listing'i için mesaj
  marketplaceId: {
    type: mongoose.Schema.Types.ObjectId,
    ref: 'Marketplace',
    required: true
  },
  
  // Mesajı gönderen emlakçı
  fromAgentId: {
    type: mongoose.Schema.Types.ObjectId,
    ref: 'User',
    required: true
  },
  
  // Mesajı alan emlakçı
  toAgentId: {
    type: mongoose.Schema.Types.ObjectId,
    ref: 'User',
    required: true
  },
  
  // Mesaj içeriği
  message: {
    type: String,
    required: true,
    maxlength: 1000
  },
  
  // Mesaj türü
  type: {
    type: String,
    enum: ['text', 'image', 'file', 'system'],
    default: 'text'
  },
  
  // Ek dosya bilgileri (resim, döküman vb.)
  attachments: [{
    filename: String,
    originalName: String,
    mimeType: String,
    size: Number,
    url: String
  }],
  
  // Mesaj durumu
  status: {
    type: String,
    enum: ['sent', 'delivered', 'read'],
    default: 'sent'
  },
  
  // Okunma zamanı
  readAt: {
    type: Date,
    default: null
  },
  
  // Mesajın silinip silinmediği
  isDeleted: {
    type: Boolean,
    default: false
  },
  
  // Yanıt verilen mesaj ID'si
  replyTo: {
    type: mongoose.Schema.Types.ObjectId,
    ref: 'CoBrokerMessage',
    default: null
  }
}, {
  timestamps: true
});

// Index'ler
coBrokerMessageSchema.index({ marketplaceId: 1, createdAt: -1 });
coBrokerMessageSchema.index({ fromAgentId: 1, toAgentId: 1 });
coBrokerMessageSchema.index({ status: 1 });

// Mesaj okundu olarak işaretle
coBrokerMessageSchema.methods.markAsRead = function() {
  this.status = 'read';
  this.readAt = new Date();
  return this.save();
};

// Mesajı sil (soft delete)
coBrokerMessageSchema.methods.deleteMessage = function() {
  this.isDeleted = true;
  return this.save();
};

module.exports = mongoose.model('CoBrokerMessage', coBrokerMessageSchema);
