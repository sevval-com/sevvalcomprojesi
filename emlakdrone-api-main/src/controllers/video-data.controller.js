const VideoData = require('../models/VideoData');
const PropertyRequest = require('../models/PropertyRequest');
const User = require('../models/User');

exports.createVideoData = async (req, res) => {
  try {
    const { videoId, userId, description, language, propertyDetails } = req.body;

    if (!videoId || !userId || !propertyDetails) {
      return res.status(400).json({
        success: false,
        message: 'videoId, userId ve propertyDetails gerekli',
        error: 'MISSING_PARAMETERS'
      });
    }

    const payload = {
      videoId,
      userId,
      description,
      language,
      propertyDetails: {
        il: propertyDetails.il,
        ilce: propertyDetails.ilce,
        mahalle: propertyDetails.mahalle,
        adaNo: propertyDetails.adaNo,
        parselNo: propertyDetails.parselNo,
        tapu: propertyDetails.tapu
      }
    };

    const saved = await VideoData.findOneAndUpdate(
      { videoId },
      payload,
      { new: true, upsert: true, setDefaultsOnInsert: true }
    );

    return res.status(201).json({
      success: true,
      data: {
        videoId: saved.videoId,
        userId: saved.userId,
        propertyDetails: saved.propertyDetails,
        createdAt: saved.createdAt
      }
    });
  } catch (error) {
    console.error('❌ Video data oluşturma hatası:', error);
    return res.status(500).json({
      success: false,
      message: 'Video data oluşturulamadı',
      error: 'CREATE_FAILED'
    });
  }
};

exports.getVideoData = async (req, res) => {
  try {
    const { videoId } = req.params;
    if (!videoId) {
      return res.status(400).json({
        success: false,
        message: 'Video ID gerekli',
        error: 'VIDEO_ID_REQUIRED'
      });
    }

    const data = await VideoData.findOne({ videoId });
    if (!data) {
      return res.status(404).json({
        success: false,
        message: 'Video data bulunamadı',
        error: 'VIDEO_DATA_NOT_FOUND'
      });
    }

    const propertyQuery = {
      userId: data.userId,
      il: data.propertyDetails?.il,
      ilce: data.propertyDetails?.ilce,
      mahalle: data.propertyDetails?.mahalle,
      adaNo: data.propertyDetails?.adaNo
    };
    if (data.propertyDetails?.parselNo) {
      propertyQuery['parseller.parselNo'] = data.propertyDetails.parselNo;
    }

    const [propertyRequest, user] = await Promise.all([
      PropertyRequest.findOne(propertyQuery),
      User.findById(data.userId).select('name surname phone').lean()
    ]);

    const property = propertyRequest
      ? {
          il: propertyRequest.il,
          ilce: propertyRequest.ilce,
          mahalle: propertyRequest.mahalle,
          adaNo: propertyRequest.adaNo,
          parselNo: data.propertyDetails?.parselNo || propertyRequest.parseller?.[0]?.parselNo || '',
          location: propertyRequest.location || {
            lat: propertyRequest.parseller?.[0]?.coordinates?.lat,
            lng: propertyRequest.parseller?.[0]?.coordinates?.lng
          },
          parseller: propertyRequest.parseller || []
        }
      : {
          il: data.propertyDetails?.il || '',
          ilce: data.propertyDetails?.ilce || '',
          mahalle: data.propertyDetails?.mahalle || '',
          adaNo: data.propertyDetails?.adaNo || '',
          parselNo: data.propertyDetails?.parselNo || '',
          location: null,
          parseller: []
        };

    const title = data.propertyDetails?.il && data.propertyDetails?.ilce
      ? `${data.propertyDetails.il}, ${data.propertyDetails.ilce}`
      : '';

    return res.json({
      success: true,
      property,
      propertyDetails: {
        title,
        tapuDurumu: data.propertyDetails?.tapu || '',
        description: data.description || '',
        features: [],
        price: ''
      },
      userData: {
        name: user?.name || '',
        surname: user?.surname || '',
        phone: user?.phone || ''
      },
      logo: null,
      avatar: null
    });
  } catch (error) {
    console.error('❌ Video data getirme hatası:', error);
    return res.status(500).json({
      success: false,
      message: 'Video data getirilemedi',
      error: 'FETCH_FAILED'
    });
  }
};
