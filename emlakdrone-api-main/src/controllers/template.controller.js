const Template = require('../models/Template');
const axios = require('axios');
const { BlobServiceClient } = require('@azure/storage-blob');

exports.generateTemplate = async (req, res) => {
  let templateId;
  try {
    const { templateId: reqTemplateId, templateData, userId } = req.body;
    templateId = reqTemplateId;

    if (!templateId || !templateData || !userId) {
      return res.status(400).json({
        success: false,
        message: 'templateId, templateData ve userId gereklidir'
      });
    }

    // Azure Blob Storage bağlantısı
    const blobServiceClient = BlobServiceClient.fromConnectionString(
      process.env.AZURE_STORAGE_CONNECTION_STRING
    );
    const containerClient = blobServiceClient.getContainerClient('photos');
    
    // Blob adını düzenle - templates klasörü ve .jpeg uzantısı ile
    const blobName = `${userId}/templates/${templateId}.jpeg`;
    const blockBlobClient = containerClient.getBlockBlobClient(blobName);

    // Video generator servisine istek
    const serviceUrl = 'http://20.215.34.129:8291';
    const apiKey = 'vsk_live_5bdd983370a6347ae95a1a92d2cb5af81de2f83b7abcb774f1587e71e26aef82';
    
    console.log('Template isteği gönderiliyor:', {
      url: `${serviceUrl}/api/process-template`,
      templateId,
      userId,
      dataSize: JSON.stringify(templateData).length
    });

    // Template verilerini kontrol et ve logla
    console.log('Template verileri:', {
      layout: templateData.layout,
      hasBackgroundImage: !!templateData.backgroundImage,
      propertyImagesCount: templateData.propertyImages?.length || 0,
      hasContactInfo: !!templateData.propertyDetails?.contactInfo,
      location: templateData.propertyDetails?.location,
      listingType: templateData.propertyDetails?.listingType
    });

    // Video generator servisine istek gönder
    const response = await axios.post(`${serviceUrl}/api/process-template`, {
      templateId,
      templateData,
      userId,
      format: 'jpeg',
      quality: 90
    }, {
      headers: {
        'x-api-key': apiKey,
        'Content-Type': 'application/json'
      },
      timeout: 30000,
      responseType: 'arraybuffer' // Binary veri olarak al
    });

    if (!response.data) {
      throw new Error('Template verisi alınamadı');
    }

    // Metadata'yı temizle ve encode et
    const sanitizedMetadata = {
      templateid: templateId,
      userid: userId,
      status: 'completed',
      format: 'jpeg',
      quality: '90',
      location: templateData.propertyDetails?.location 
        ? encodeURIComponent(templateData.propertyDetails.location.replace(/[^a-zA-Z0-9-_]/g, '_'))
        : 'unknown',
      listingtype: templateData.propertyDetails?.listingType
        ? encodeURIComponent(templateData.propertyDetails.listingType.replace(/[^a-zA-Z0-9-_]/g, '_'))
        : 'unknown'
    };

    // Blob'u oluştur ve veriyi yükle
    try {
      // Binary veriyi doğrudan yükle
      await blockBlobClient.upload(response.data, response.data.length, {
        blobHTTPHeaders: {
          blobContentType: 'image/jpeg',
          blobCacheControl: 'public, max-age=31536000'
        }
      });

      // Metadata'yı ayarla
      await blockBlobClient.setMetadata(sanitizedMetadata);

      console.log('Template başarıyla yüklendi:', {
        blobName,
        size: response.data.length,
        metadata: sanitizedMetadata
      });

    } catch (storageError) {
      console.error('Azure Storage hatası:', storageError);
      throw new Error(`Storage hatası: ${storageError.message}`);
    }

    res.json({
      success: true,
      message: 'Template başarıyla oluşturuldu',
      data: {
        templateId,
        status: 'completed',
        downloadUrl: response.data.downloadUrl,
        format: 'jpeg',
        quality: 90
      }
    });

  } catch (error) {
    console.error('Template oluşturma hatası:', error);
    
    if (error.response) {
      console.error('Response error:', error.response.data);
    }
    
    res.status(500).json({
      success: false,
      message: 'Template oluşturulurken bir hata oluştu',
      error: error.message
    });
  }
};

exports.checkTemplateStatus = async (req, res) => {
  try {
    const { templateId } = req.params;
    const template = await Template.findOne({ templateId });

    if (!template) {
      return res.status(404).json({
        success: false,
        message: 'Template bulunamadı'
      });
    }

    res.json({
      success: true,
      data: {
        status: template.status,
        downloadUrl: template.downloadUrl
      }
    });

  } catch (error) {
    console.error('Template durum kontrolü hatası:', error);
    res.status(500).json({
      success: false,
      message: 'Template durumu kontrol edilirken bir hata oluştu'
    });
  }
};

exports.getMyTemplates = async (req, res) => {
  try {
    const userId = req.userId;

    const blobServiceClient = BlobServiceClient.fromConnectionString(
      process.env.AZURE_STORAGE_CONNECTION_STRING
    );
    const containerClient = blobServiceClient.getContainerClient('photos');

    console.log('🔍 Azure Storage\'dan template listesi alınıyor:', {
      userId,
      container: 'photos'
    });

    const blobs = containerClient.listBlobsFlat({
      prefix: `${userId}/`
    });

    const templates = [];
    
    for await (const blob of blobs) {
      const blobClient = containerClient.getBlobClient(blob.name);
      const properties = await blobClient.getProperties();
      
      // Sadece görüntü dosyalarını filtrele
      if (blob.name.endsWith('.jpg') || blob.name.endsWith('.jpeg') || blob.name.endsWith('.png')) {
        const template = {
          _id: blob.name,
          templateId: properties.metadata?.templateid || blob.name.split('/').pop().replace(/\.(jpg|jpeg|png)$/, ''),
          userId: userId,
          downloadUrl: blobClient.url,
          createdAt: properties.createdOn,
          status: properties.metadata?.status || 'completed',
          format: properties.metadata?.format || 'jpeg',
          quality: properties.metadata?.quality || '90',
          propertyDetails: {
            location: properties.metadata?.location ? decodeURIComponent(properties.metadata.location) : '',
            listingType: properties.metadata?.listingtype ? decodeURIComponent(properties.metadata.listingtype) : ''
          }
        };

        templates.push(template);
      }
    }

    templates.sort((a, b) => new Date(b.createdAt) - new Date(a.createdAt));

    console.log('✅ Template listesi başarıyla alındı:', {
      userId,
      templateCount: templates.length
    });

    res.json({
      success: true,
      data: templates
    });

  } catch (error) {
    console.error('❌ Template listesi hatası:', error);
    res.status(500).json({
      success: false,
      message: 'Template listesi alınırken bir hata oluştu',
      error: error.message
    });
  }
};

// Template silme endpoint'i
exports.deleteTemplate = async (req, res) => {
  try {
    const { templateId } = req.params;
    const userId = req.userId;

    // Template'i bul
    const template = await Template.findOne({ templateId, userId });
    if (!template) {
      return res.status(404).json({
        success: false,
        message: 'Template bulunamadı'
      });
    }

    // Azure Blob Storage'dan sil
    try {
      const blobServiceClient = BlobServiceClient.fromConnectionString(
        process.env.AZURE_STORAGE_CONNECTION_STRING
      );
      const containerClient = blobServiceClient.getContainerClient('photos');
      const blobName = `${userId}/templates/${templateId}.jpeg`;
      const blockBlobClient = containerClient.getBlockBlobClient(blobName);
      
      await blockBlobClient.delete();
    } catch (storageError) {
      console.error('Blob silme hatası:', storageError);
    }

    // MongoDB'den sil
    await Template.deleteOne({ templateId, userId });

    res.json({
      success: true,
      message: 'Template başarıyla silindi'
    });

  } catch (error) {
    console.error('Template silme hatası:', error);
    res.status(500).json({
      success: false,
      message: 'Template silinirken bir hata oluştu'
    });
  }
};

// Template güncelleme endpoint'i
exports.updateTemplate = async (req, res) => {
  try {
    const { templateId } = req.params;
    const userId = req.userId;
    const updateData = req.body;

    // Template'i bul
    const template = await Template.findOne({ templateId, userId });
    if (!template) {
      return res.status(404).json({
        success: false,
        message: 'Template bulunamadı'
      });
    }

    // Güncelleme verilerini doğrula
    if (!updateData || Object.keys(updateData).length === 0) {
      return res.status(400).json({
        success: false,
        message: 'Güncelleme verileri gerekli'
      });
    }

    // Template'i güncelle
    const updatedTemplate = await Template.findOneAndUpdate(
      { templateId, userId },
      { $set: updateData },
      { new: true }
    );

    const serviceUrl = 'http://20.215.34.129:8291';
    const apiKey = 'vsk_live_5bdd983370a6347ae95a1a92d2cb5af81de2f83b7abcb774f1587e71e26aef82';

    // Template servisine güncelleme isteği gönder
    const response = await axios.post(
      `${serviceUrl}/api/process-template`,
      {
        templateId,
        templateData: updatedTemplate.templateData,
        userId,
        isUpdate: true
      },
      {
        headers: {
          'x-api-key': apiKey,
          'Content-Type': 'application/json'
        }
      }
    );

    res.json({
      success: true,
      message: 'Template güncelleme işlemi başlatıldı',
      data: response.data
    });

  } catch (error) {
    console.error('Template güncelleme hatası:', error);
    res.status(500).json({
      success: false,
      message: 'Template güncellenirken bir hata oluştu'
    });
  }
};

// Template önizleme endpoint'i
exports.previewTemplate = async (req, res) => {
  try {
    const { templateId } = req.params;
    const userId = req.userId;

    // Template'i bul
    const template = await Template.findOne({ templateId, userId });
    if (!template) {
      return res.status(404).json({
        success: false,
        message: 'Template bulunamadı'
      });
    }

    const serviceUrl = 'http://20.215.34.129:8291';
    const apiKey = 'vsk_live_5bdd983370a6347ae95a1a92d2cb5af81de2f83b7abcb774f1587e71e26aef82';

    // Template servisine önizleme isteği gönder
    const response = await axios.post(
      `${serviceUrl}/api/preview-template`,
      {
        templateId,
        templateData: template.templateData,
        userId
      },
      {
        headers: {
          'x-api-key': apiKey,
          'Content-Type': 'application/json'
        }
      }
    );

    res.json({
      success: true,
      data: response.data
    });

  } catch (error) {
    console.error('Template önizleme hatası:', error);
    res.status(500).json({
      success: false,
      message: 'Template önizleme oluşturulurken bir hata oluştu'
    });
  }
}; 