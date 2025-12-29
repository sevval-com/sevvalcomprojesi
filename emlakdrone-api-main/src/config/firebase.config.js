const admin = require('firebase-admin');

// Firebase Admin SDK'yı başlat
if (process.env.FIREBASE_PRIVATE_KEY) {
  // Environment variable'dan oku
  const serviceAccount = {
    type: 'service_account',
    project_id: process.env.FIREBASE_PROJECT_ID || 'emlakdronebybs',
    private_key: process.env.FIREBASE_PRIVATE_KEY?.replace(/\\n/g, '\n'),
    client_email: process.env.FIREBASE_CLIENT_EMAIL,
    auth_uri: 'https://accounts.google.com/o/oauth2/auth',
    token_uri: 'https://oauth2.googleapis.com/token',
    auth_provider_x509_cert_url: 'https://www.googleapis.com/oauth2/v1/certs'
  };

  try {
    admin.initializeApp({
      credential: admin.credential.cert(serviceAccount),
      projectId: process.env.FIREBASE_PROJECT_ID || 'emlakdronebybs'
    });
    console.log('✅ Firebase Admin SDK başlatıldı');
  } catch (error) {
    console.warn('⚠️ Firebase Admin SDK başlatılamadı:', error.message);
  }
} else {
  console.warn('⚠️ Firebase environment variables bulunamadı, Firebase devre dışı');
}

module.exports = admin;