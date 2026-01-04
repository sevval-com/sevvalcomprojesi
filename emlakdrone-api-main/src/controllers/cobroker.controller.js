const Marketplace = require('../models/Marketplace');
const CoBrokerMessage = require('../models/CoBrokerMessage');
const User = require('../models/User');
const notificationService = require('../services/notificationService');

class CoBrokerController {
  
  // Co-broker isteği gönder
  async sendCoBrokerRequest(req, res) {
    try {
      console.log('🚀 Co-broker isteği alındı:', { 
        body: req.body, 
        userId: req.userId,
        headers: req.headers.authorization 
      });
      
      const { marketplaceId, message } = req.body;
      const fromAgentId = req.userId;
      
      // Marketplace listing'ini bul
      const marketplace = await Marketplace.findById(marketplaceId)
        .populate('agentId', 'name surname company phone')
        .populate('propertyId'); 
        
      if (!marketplace) {
        return res.status(404).json({ message: 'İlan bulunamadı' });
      }
      
      // Kendi ilanına istek gönderemez
      if (marketplace.agentId._id.toString() === fromAgentId) {
        return res.status(400).json({ message: 'Kendi ilanınıza işbirliği isteği gönderemezsiniz' });
      }
      
      // Zaten istek gönderilmiş mi kontrol et (sadece bekleyen durum engellenir)
      const existingPending = marketplace.coBrokerRequests.find(
        req => req.fromAgentId.toString() === fromAgentId && req.status === 'pending'
      );
      if (existingPending) {
        return res.status(400).json({ message: 'Bu ilan için bekleyen bir işbirliği isteğiniz var' });
      }
      
      // Zaten co-broker mı kontrol et
      const isAlreadyCoBroker = marketplace.coBrokers.some(
        cb => cb.agentId.toString() === fromAgentId
      );
      
      if (isAlreadyCoBroker) {
        return res.status(400).json({ message: 'Bu ilan için zaten işbirliği yapıyorsunuz' });
      }
      
      // İstek ekle
      marketplace.coBrokerRequests.push({
        fromAgentId,
        message: message || 'Bu ilan için işbirliği yapmak istiyorum.'
      });
      
      // Location field'ını kontrol et ve düzelt
      if (!marketplace.location || !marketplace.location.coordinates || marketplace.location.coordinates.length === 0) {
        marketplace.location = {
          type: 'Point',
          coordinates: [0, 0] // Default koordinatlar
        };
      }
      
      await marketplace.save();
      
      // Bildirim gönder
      try {
        const fromAgent = await User.findById(fromAgentId);
        await notificationService.sendCoBrokerRequestNotification(
          marketplace.agentId,
          fromAgent,
          marketplace.title
        );
      } catch (notificationError) {
        console.error('Bildirim gönderme hatası:', notificationError);
      }
      
      res.json({ 
        message: 'İşbirliği isteği başarıyla gönderildi',
        requestId: marketplace.coBrokerRequests[marketplace.coBrokerRequests.length - 1]._id
      });
      
    } catch (error) {
      console.error('Co-broker istek gönderme hatası:', error);
      res.status(500).json({ message: 'Sunucu hatası' });
    }
  }
  
  // Co-broker isteğini onayla/reddet
  async respondToCoBrokerRequest(req, res) {
    try {
      const { marketplaceId, requestId, action } = req.body; // action: 'approve' veya 'reject'
      const agentId = req.userId;
      
      const marketplace = await Marketplace.findById(marketplaceId);
      
      if (!marketplace) {
        return res.status(404).json({ message: 'İlan bulunamadı' });
      }
      
      // İlan sahibi mi kontrol et
      if (marketplace.agentId.toString() !== agentId) {
        return res.status(403).json({ message: 'Bu işlem için yetkiniz yok' });
      }
      
      // İsteği bul
      const request = marketplace.coBrokerRequests.id(requestId);
      
      if (!request) {
        return res.status(404).json({ message: 'İstek bulunamadı' });
      }
      
      if (action === 'approve') {
        // Co-broker olarak ekle
        marketplace.coBrokers.push({
          agentId: request.fromAgentId,
          joinedAt: new Date()
        });
        
        // İsteği onaylandı olarak işaretle
        request.status = 'approved';
        
        // İlk mesajı gönder
        await CoBrokerMessage.create({
          marketplaceId,
          fromAgentId: agentId,
          toAgentId: request.fromAgentId,
          message: `Hoş geldiniz! ${marketplace.title} ilanı için işbirliği yapmaya başlayabiliriz.`,
          type: 'system'
        });
        
        // Onay bildirimi gönder
        try {
          const toAgent = await User.findById(request.fromAgentId);
          const fromAgent = await User.findById(agentId);
          await notificationService.sendCoBrokerApprovalNotification(
            request.fromAgentId,
            fromAgent,
            marketplace.title
          );
        } catch (notificationError) {
          console.error('Onay bildirimi gönderme hatası:', notificationError);
        }
        
      } else if (action === 'reject') {
        request.status = 'rejected';
      }
      
      await marketplace.save();
      
      res.json({ 
        message: `İstek ${action === 'approve' ? 'onaylandı' : 'reddedildi'}` 
      });
      
    } catch (error) {
      console.error('Co-broker istek yanıtlama hatası:', error);
      res.status(500).json({ message: 'Sunucu hatası' });
    }
  }
  
  // Co-broker isteğini geri çek
  async cancelCoBrokerRequest(req, res) {
    try {
      const { marketplaceId, requestId } = req.body;
      const fromAgentId = req.userId;
      
      const marketplace = await Marketplace.findById(marketplaceId);
      
      if (!marketplace) {
        return res.status(404).json({ message: 'İlan bulunamadı' });
      }
      
      // İsteği bul
      const request = marketplace.coBrokerRequests.id(requestId);
      
      if (!request) {
        return res.status(404).json({ message: 'İstek bulunamadı' });
      }
      
      // Sadece isteği gönderen geri çekebilir
      if (request.fromAgentId.toString() !== fromAgentId) {
        return res.status(403).json({ message: 'Bu işlem için yetkiniz yok' });
      }
      
      // Sadece pending durumundaki istekler geri çekilebilir
      if (request.status !== 'pending') {
        return res.status(400).json({ message: 'Sadece bekleyen istekler geri çekilebilir' });
      }
      
      // İsteği sil
      marketplace.coBrokerRequests = marketplace.coBrokerRequests.filter(
        req => req._id.toString() !== requestId
      );
      
      await marketplace.save();
      
      res.json({ message: 'İstek başarıyla geri çekildi' });
      
    } catch (error) {
      console.error('Co-broker istek geri çekme hatası:', error);
      res.status(500).json({ message: 'Sunucu hatası' });
    }
  }
  
  // Co-broker isteklerini getir
  async getCoBrokerRequests(req, res) {
    try {
      const agentId = req.userId;
      
      // Gönderilen istekler
      const sentRequests = await Marketplace.find({
        'coBrokerRequests.fromAgentId': agentId
      })
      .populate('agentId', 'name surname company')
      .populate('propertyId', 'il ilce mahalle adaNo')
      .select('title status coBrokerRequests agentId propertyId');
      
      // Gelen istekler
      const receivedRequests = await Marketplace.find({
        agentId: agentId,
        'coBrokerRequests.status': 'pending'
      })
      .populate('coBrokerRequests.fromAgentId', 'name surname company')
      .populate('propertyId', 'il ilce mahalle adaNo')
      .select('title status coBrokerRequests propertyId');
      
      res.json({
        sent: sentRequests.map(marketplace => ({
          id: marketplace._id,
          title: marketplace.title,
          property: marketplace.propertyId,
          agent: marketplace.agentId,
          status: marketplace.status,
          requests: marketplace.coBrokerRequests.filter(req => 
            req.fromAgentId.toString() === agentId
          )
        })),
        received: receivedRequests.map(marketplace => ({
          id: marketplace._id,
          title: marketplace.title,
          property: marketplace.propertyId,
          status: marketplace.status,
          requests: marketplace.coBrokerRequests.filter(req => 
            req.status === 'pending'
          )
        }))
      });
      
    } catch (error) {
      console.error('Co-broker istekleri getirme hatası:', error);
      res.status(500).json({ message: 'Sunucu hatası' });
    }
  }
  
  // Aktif co-broker işbirliklerini getir
  async getActiveCollaborations(req, res) {
    try {
      const agentId = req.userId;
      
      const collaborations = await Marketplace.find({
        $or: [
          { agentId: agentId }, // Kendi ilanları
          { 'coBrokers.agentId': agentId } // Co-broker olduğu ilanlar
        ],
        'coBrokers.0': { $exists: true } // Sadece co-broker'ları olan ilanlar
      })
      .populate('agentId', 'name surname company')
      .populate('coBrokers.agentId', 'name surname company')
      .populate('propertyId', 'il ilce mahalle adaNo')
      .select('title status agentId coBrokers propertyId');
      
      res.json(collaborations);
      
    } catch (error) {
      console.error('Aktif işbirlikleri getirme hatası:', error);
      res.status(500).json({ message: 'Sunucu hatası' });
    }
  }
  
  // Mesaj gönder
  async sendMessage(req, res) {
    try {
      const { marketplaceId, toAgentId, message, type = 'text', attachments = [] } = req.body;
      const fromAgentId = req.userId;
      
      // Co-broker işbirliği var mı kontrol et
      const marketplace = await Marketplace.findOne({
        _id: marketplaceId,
        $or: [
          { agentId: fromAgentId },
          { 'coBrokers.agentId': fromAgentId }
        ]
      });
      
      if (!marketplace) {
        return res.status(403).json({ message: 'Bu ilan için mesaj gönderme yetkiniz yok' });
      }
      
      const newMessage = await CoBrokerMessage.create({
        marketplaceId,
        fromAgentId,
        toAgentId,
        message,
        type,
        attachments
      });
      
      // Mesajı populate ile döndür
      const populatedMessage = await CoBrokerMessage.findById(newMessage._id)
        .populate('fromAgentId', 'name surname company')
        .populate('toAgentId', 'name surname company');
      
      // Mesaj bildirimi gönder
      try {
        const toAgent = await User.findById(toAgentId);
        const fromAgent = await User.findById(fromAgentId);
        await notificationService.sendCoBrokerMessageNotification(
          toAgentId,
          fromAgent,
          message,
          marketplace.title
        );
      } catch (notificationError) {
        console.error('Mesaj bildirimi gönderme hatası:', notificationError);
      }
      
      res.json(populatedMessage);
      
    } catch (error) {
      console.error('Mesaj gönderme hatası:', error);
      res.status(500).json({ message: 'Sunucu hatası' });
    }
  }
  
  // Mesajları getir
  async getMessages(req, res) {
    try {
      const { marketplaceId, page = 1, limit = 50 } = req.query;
      const agentId = req.userId;
      
      // Co-broker işbirliği var mı kontrol et
      const marketplace = await Marketplace.findOne({
        _id: marketplaceId,
        $or: [
          { agentId: agentId },
          { 'coBrokers.agentId': agentId }
        ]
      });
      
      if (!marketplace) {
        return res.status(403).json({ message: 'Bu ilan için mesaj görme yetkiniz yok' });
      }
      
      const messages = await CoBrokerMessage.find({
        marketplaceId,
        $or: [
          { fromAgentId: agentId },
          { toAgentId: agentId }
        ],
        isDeleted: false
      })
      .populate('fromAgentId', 'name surname company')
      .populate('toAgentId', 'name surname company')
      .populate('replyTo')
      .sort({ createdAt: -1 })
      .limit(limit * 1)
      .skip((page - 1) * limit);
      
      // Okunmamış mesajları okundu olarak işaretle
      await CoBrokerMessage.updateMany({
        marketplaceId,
        toAgentId: agentId,
        status: { $ne: 'read' }
      }, {
        status: 'read',
        readAt: new Date()
      });
      
      res.json({
        messages: messages.reverse(), // Eski mesajlar üstte
        hasMore: messages.length === limit
      });
      
    } catch (error) {
      console.error('Mesajları getirme hatası:', error);
      res.status(500).json({ message: 'Sunucu hatası' });
    }
  }
  
  // Mesajı sil
  async deleteMessage(req, res) {
    try {
      const { messageId } = req.params;
      const agentId = req.userId;
      
      const message = await CoBrokerMessage.findById(messageId);
      
      if (!message) {
        return res.status(404).json({ message: 'Mesaj bulunamadı' });
      }
      
      // Sadece mesajı gönderen silebilir
      if (message.fromAgentId.toString() !== agentId) {
        return res.status(403).json({ message: 'Bu mesajı silme yetkiniz yok' });
      }
      
      await message.deleteMessage();
      
      res.json({ message: 'Mesaj silindi' });
      
    } catch (error) {
      console.error('Mesaj silme hatası:', error);
      res.status(500).json({ message: 'Sunucu hatası' });
    }
  }
  
  // Co-broker işbirliğini sonlandır
  async endCollaboration(req, res) {
    try {
      const { marketplaceId, coBrokerId } = req.body;
      const agentId = req.userId;
      
      const marketplace = await Marketplace.findById(marketplaceId);
      
      if (!marketplace) {
        return res.status(404).json({ message: 'İlan bulunamadı' });
      }
      
      // Sadece ilan sahibi işbirliğini sonlandırabilir
      if (marketplace.agentId.toString() !== agentId) {
        return res.status(403).json({ message: 'Bu işlem için yetkiniz yok' });
      }
      
      // Co-broker'ı kaldır
      marketplace.coBrokers = marketplace.coBrokers.filter(
        cb => cb.agentId.toString() !== coBrokerId
      );
      
      await marketplace.save();
      
      res.json({ message: 'İşbirliği sonlandırıldı' });
      
    } catch (error) {
      console.error('İşbirliği sonlandırma hatası:', error);
      res.status(500).json({ message: 'Sunucu hatası' });
    }
  }

  // Okunmamış mesaj sayısını getir
  async getUnreadCount(req, res) {
    try {
      const agentId = req.userId;
      
      // Gelen isteklerdeki okunmamış sayısı
      const sentRequests = await Marketplace.find({
        'coBrokerRequests.fromAgentId': agentId
      });
      
      let unreadCount = 0;
      
      // Gönderilen isteklerdeki pending durumları
      sentRequests.forEach(marketplace => {
        marketplace.coBrokerRequests.forEach(request => {
          if (request.fromAgentId.toString() === agentId && request.status === 'pending') {
            unreadCount++;
          }
        });
      });
      
      // Gelen isteklerdeki pending durumları
      const receivedRequests = await Marketplace.find({
        'agentId': agentId,
        'coBrokerRequests.status': 'pending'
      });
      
      receivedRequests.forEach(marketplace => {
        marketplace.coBrokerRequests.forEach(request => {
          if (request.status === 'pending') {
            unreadCount++;
          }
        });
      });
      
      // Aktif işbirliklerindeki okunmamış mesajlar
      const collaborations = await Marketplace.find({
        $or: [
          { 'agentId': agentId },
          { 'coBrokers.agentId': agentId }
        ]
      }).populate('coBrokers.agentId', 'name surname');
      
      for (const collaboration of collaborations) {
        try {
          const messages = await CoBrokerMessage.find({
            marketplaceId: collaboration._id,
            $or: [
              { fromAgentId: agentId },
              { toAgentId: agentId }
            ]
          }).sort({ timestamp: -1 }).limit(1);
          
          if (messages.length > 0) {
            const lastMessage = messages[0];
            if (!lastMessage.readBy?.includes(agentId)) {
              unreadCount++;
            }
          }
        } catch (error) {
          console.log('Mesaj kontrol hatası:', error);
        }
      }
      
      res.json({ unreadCount });
    } catch (error) {
      console.error('Okunmamış sayı getirme hatası:', error);
      res.status(500).json({ message: 'Sunucu hatası' });
    }
  }
}

module.exports = new CoBrokerController();
