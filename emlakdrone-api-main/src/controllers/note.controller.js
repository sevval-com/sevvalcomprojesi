const Note = require('../models/Note');
const jwt = require('jsonwebtoken');

// Not oluştur veya güncelle
exports.createOrUpdateNote = async (req, res) => {
  try {
    const token = req.headers.authorization.split(' ')[1];
    const decoded = jwt.verify(token, process.env.JWT_SECRET);
    const userId = decoded.id;

    const { propertyId, title, content, tags, color } = req.body;

    if (!propertyId || !title || !content) {
      return res.status(400).json({
        message: 'PropertyId, title ve content alanları zorunludur'
      });
    }

    // PropertyId string kontrolü
    if (typeof propertyId !== 'string' || propertyId.trim() === '') {
      return res.status(400).json({
        message: 'Geçersiz propertyId formatı'
      });
    }

    // Mevcut not var mı kontrol et
    let note = await Note.findOne({ propertyId, userId });

    if (note) {
      // Güncelle
      note.title = title;
      note.content = content;
      note.tags = tags || [];
      note.color = color || '#ffd200';
      note.updatedAt = new Date();
      await note.save();
    } else {
      // Yeni oluştur
      note = new Note({
        propertyId,
        userId,
        title,
        content,
        tags: tags || [],
        color: color || '#ffd200'
      });
      await note.save();
    }

    console.log('Not kaydedildi:', note);
    res.status(201).json({
      message: 'Not başarıyla kaydedildi',
      note
    });
  } catch (error) {
    console.error('Not kaydetme hatası:', error);
    res.status(500).json({
      message: 'Not kaydedilirken bir hata oluştu',
      error: error.message
    });
  }
};

// Property için not getir
exports.getNoteByProperty = async (req, res) => {
  try {
    const token = req.headers.authorization.split(' ')[1];
    const decoded = jwt.verify(token, process.env.JWT_SECRET);
    const userId = decoded.id;

    const { propertyId } = req.params;

    const note = await Note.findOne({ propertyId, userId });

    if (!note) {
      // Not yoksa 404 hatası yerine boş response döndür
      return res.status(200).json(null);
    }

    res.status(200).json(note);
  } catch (error) {
    console.error('Not getirme hatası:', error);
    res.status(500).json({
      message: 'Not getirilirken bir hata oluştu',
      error: error.message
    });
  }
};

// Kullanıcının tüm notlarını getir
exports.getUserNotes = async (req, res) => {
  try {
    const token = req.headers.authorization.split(' ')[1];
    const decoded = jwt.verify(token, process.env.JWT_SECRET);
    const userId = decoded.id;

    const notes = await Note.find({ userId })
      .populate('propertyId', 'il ilce mahalle adaNo parseller')
      .sort({ updatedAt: -1 });

    res.status(200).json(notes);
  } catch (error) {
    console.error('Notlar getirme hatası:', error);
    res.status(500).json({
      message: 'Notlar getirilirken bir hata oluştu',
      error: error.message
    });
  }
};

// Not sil
exports.deleteNote = async (req, res) => {
  try {
    const token = req.headers.authorization.split(' ')[1];
    const decoded = jwt.verify(token, process.env.JWT_SECRET);
    const userId = decoded.id;

    const { propertyId } = req.params;

    const note = await Note.findOneAndDelete({ propertyId, userId });

    if (!note) {
      return res.status(404).json({
        message: 'Not bulunamadı'
      });
    }

    console.log('Not silindi:', note);
    res.status(200).json({
      message: 'Not başarıyla silindi',
      deletedNote: note
    });
  } catch (error) {
    console.error('Not silme hatası:', error);
    res.status(500).json({
      message: 'Not silinirken bir hata oluştu',
      error: error.message
    });
  }
};

// Property silindiğinde notları da sil (PropertyRequest model'inde kullanılacak)
exports.deleteNotesByProperty = async (propertyId) => {
  try {
    const result = await Note.deleteMany({ propertyId });
    console.log(`${result.deletedCount} not silindi (Property: ${propertyId})`);
    return result;
  } catch (error) {
    console.error('Property notları silinirken hata:', error);
    throw error;
  }
};
