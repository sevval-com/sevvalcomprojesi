const VideoData = require('../models/VideoData');

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

    return res.json({
      success: true,
      data: {
        videoId: data.videoId,
        userId: data.userId,
        description: data.description,
        language: data.language,
        propertyDetails: data.propertyDetails,
        createdAt: data.createdAt
      }
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
