const admin = require('firebase-admin');

// Firebase Admin SDK'yı başlat (eğer henüz başlatılmamışsa)
if (!admin.apps.length) {
  try {
    admin.initializeApp({
      credential: admin.credential.cert({
        projectId: process.env.FIREBASE_PROJECT_ID,
        clientEmail: process.env.FIREBASE_CLIENT_EMAIL,
        privateKey: process.env.FIREBASE_PRIVATE_KEY?.replace(/\\n/g, '\n'),
      }),
    });
  } catch (error) {
    console.error('Firebase Admin SDK başlatma hatası:', error);
  }
}

class NotificationService {
  
  // Co-broker istek bildirimi gönder
  async sendCoBrokerRequestNotification(toUserId, fromAgent, listingTitle) {
    try {
      const user = await require('../models/User').findById(toUserId);
      if (!user || !user.pushToken) {
        console.log('Kullanıcı push token bulunamadı:', toUserId);
        return;
      }

      const message = {
        notification: {
          title: 'Yeni Co-Broker İsteği',
          body: `${fromAgent.name} ${fromAgent.surname} "${listingTitle}" ilanı için co-broker isteği gönderdi`,
        },
        data: {
          type: 'CO_BROKER_REQUEST',
          listingId: listingTitle,
          fromAgentId: fromAgent._id.toString(),
          fromAgentName: `${fromAgent.name} ${fromAgent.surname}`,
        },
        token: user.pushToken,
      };

      const response = await admin.messaging().send(message);
      console.log('Co-broker istek bildirimi gönderildi:', response);
      return response;
    } catch (error) {
      console.error('Co-broker istek bildirimi gönderme hatası:', error);
      throw error;
    }
  }

  // Co-broker onay bildirimi gönder
  async sendCoBrokerApprovalNotification(toUserId, fromAgent, listingTitle) {
    try {
      const user = await require('../models/User').findById(toUserId);
      if (!user || !user.pushToken) {
        console.log('Kullanıcı push token bulunamadı:', toUserId);
        return;
      }

      const message = {
        notification: {
          title: 'Co-Broker İsteği Onaylandı',
          body: `${fromAgent.name} ${fromAgent.surname} co-broker isteğinizi onayladı`,
        },
        data: {
          type: 'CO_BROKER_APPROVAL',
          listingId: listingTitle,
          fromAgentId: fromAgent._id.toString(),
          fromAgentName: `${fromAgent.name} ${fromAgent.surname}`,
        },
        token: user.pushToken,
      };

      const response = await admin.messaging().send(message);
      console.log('Co-broker onay bildirimi gönderildi:', response);
      return response;
    } catch (error) {
      console.error('Co-broker onay bildirimi gönderme hatası:', error);
      throw error;
    }
  }

  // Co-broker mesaj bildirimi gönder
  async sendCoBrokerMessageNotification(toUserId, fromAgent, listingTitle, messageText) {
    try {
      const user = await require('../models/User').findById(toUserId);
      if (!user || !user.pushToken) {
        console.log('Kullanıcı push token bulunamadı:', toUserId);
        return;
      }

      const message = {
        notification: {
          title: `${fromAgent.name} ${fromAgent.surname}`,
          body: messageText.length > 50 ? messageText.substring(0, 50) + '...' : messageText,
        },
        data: {
          type: 'CO_BROKER_MESSAGE',
          listingId: listingTitle,
          fromAgentId: fromAgent._id.toString(),
          fromAgentName: `${fromAgent.name} ${fromAgent.surname}`,
        },
        token: user.pushToken,
      };

      const response = await admin.messaging().send(message);
      console.log('Co-broker mesaj bildirimi gönderildi:', response);
      return response;
    } catch (error) {
      console.error('Co-broker mesaj bildirimi gönderme hatası:', error);
      throw error;
    }
  }

  // Genel bildirim gönder
  async sendNotification(userId, title, body, data = {}) {
    try {
      const user = await require('../models/User').findById(userId);
      if (!user || !user.pushToken) {
        console.log('Kullanıcı push token bulunamadı:', userId);
        return;
      }

      const message = {
        notification: {
          title,
          body,
        },
        data: {
          ...data,
          timestamp: new Date().toISOString(),
        },
        token: user.pushToken,
      };

      const response = await admin.messaging().send(message);
      console.log('Bildirim gönderildi:', response);
      return response;
    } catch (error) {
      console.error('Bildirim gönderme hatası:', error);
      throw error;
    }
  }

  // Toplu bildirim gönder
  async sendBulkNotification(userIds, title, body, data = {}) {
    try {
      const users = await require('../models/User').find({
        _id: { $in: userIds },
        pushToken: { $exists: true, $ne: null }
      });

      if (users.length === 0) {
        console.log('Push token bulunan kullanıcı yok');
        return;
      }

      const messages = users.map(user => ({
        notification: {
          title,
          body,
        },
        data: {
          ...data,
          timestamp: new Date().toISOString(),
        },
        token: user.pushToken,
      }));

      const response = await admin.messaging().sendAll(messages);
      console.log('Toplu bildirim gönderildi:', response);
      return response;
    } catch (error) {
      console.error('Toplu bildirim gönderme hatası:', error);
      throw error;
    }
  }

  // Co-broker mesaj bildirimi gönder
  async sendCoBrokerMessageNotification(toUserId, fromAgent, message, listingTitle) {
    try {
      const user = await require('../models/User').findById(toUserId);
      if (!user || !user.pushToken) {
        console.log('Kullanıcı push token bulunamadı:', toUserId);
        return;
      }

      const messageData = {
        notification: {
          title: `Yeni Co-Broker Mesajı`,
          body: `${fromAgent.name} ${fromAgent.surname}: ${message.substring(0, 50)}${message.length > 50 ? '...' : ''}`,
        },
        data: {
          type: 'cobroker_message',
          fromAgentId: fromAgent._id.toString(),
          fromAgentName: `${fromAgent.name} ${fromAgent.surname}`,
          listingTitle: listingTitle,
          message: message
        },
        token: user.pushToken,
      };

      const response = await admin.messaging().send(messageData);
      console.log('Co-broker mesaj bildirimi gönderildi:', response);
      return response;
    } catch (error) {
      console.error('Co-broker mesaj bildirimi gönderme hatası:', error);
      throw error;
    }
  }
}

module.exports = new NotificationService();
