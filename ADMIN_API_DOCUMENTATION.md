# Admin Panel API Documentation

## 📋 Genel Bakış

Mobil uygulama için kurumsal kullanıcı yönetimi API'si geliştirilmiştir. Bu API, admin kullanıcıların kurumsal hesapları görüntülemesine, onaylamasına, reddetmesine ve silmesine olanak tanır.

**Base URL:** `http://94.73.131.202:8090/api/v1/admin`  
**Authentication:** JWT Bearer Token (Admin rolü gerekli)  
**API Version:** v1

---

## 🔐 Authentication

Tüm admin endpoint'leri `Admin` rolü gerektirir.

```http
Authorization: Bearer {your-jwt-token}
```

**Not:** JWT token içinde `http://schemas.microsoft.com/ws/2008/06/identity/claims/role` claim'i olarak `Admin` değeri bulunmalıdır.

---

## 📡 API Endpoints

### 1. Kurumsal Kullanıcı Listesi

Filtrelenmiş ve sayfalandırılmış kurumsal kullanıcı listesini getirir.

**Endpoint:**
```http
GET /api/v1/admin/corporate-users
```

**Query Parameters:**

| Parametre | Tip | Zorunlu | Açıklama | Default |
|-----------|-----|---------|----------|---------|
| `userType` | string | Hayır | Kullanıcı tipi filtresi | null |
| `status` | string | Hayır | Durum filtresi | null |
| `page` | int | Hayır | Sayfa numarası | 1 |
| `pageSize` | int | Hayır | Sayfa başına kayıt | 20 |

**Request Example:**
```http
GET /api/v1/admin/corporate-users?status=pending&page=1&pageSize=20
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

**Response (200 OK):**
```json
{
  "users": [
    {
      "id": "user-guid-123",
      "email": "sirket@example.com",
      "phoneNumber": "+905551234567",
      "companyName": "Örnek Emlak A.Ş.",
      "isApproved": false,
      "isEmailConfirmed": true,
      "registrationDate": "2024-11-15T10:30:00",
      "lastLoginDate": "2024-11-28T09:15:00",
      "isActive": true,
      "accountStatus": "Active",
      "announcementStats": {
        "totalAnnouncements": 15,
        "photoAnnouncements": 0,
        "videoAnnouncements": 8,
        "noPhotoAnnouncements": 15,
        "noVideoAnnouncements": 7,
        "lastAnnouncementDate": "2024-11-27T14:20:00",
        "firstAnnouncementDate": "2024-11-16T11:00:00"
      }
    }
  ],
  "totalCount": 45,
  "page": 1,
  "pageSize": 20,
  "totalPages": 3
}
```

**Response Codes:**
- `200` - Başarılı
- `401` - Yetkisiz (Token yok veya geçersiz)
- `403` - Yasak (Admin rolü yok)

---

### 2. Kullanıcı Detay İstatistikleri

Belirli bir kullanıcının detaylı istatistiklerini getirir.

**Endpoint:**
```http
GET /api/v1/admin/corporate-users/{id}/stats
```

**Path Parameters:**

| Parametre | Tip | Zorunlu | Açıklama |
|-----------|-----|---------|----------|
| `id` | string | Evet | Kullanıcı ID (GUID) |

**Request Example:**
```http
GET /api/v1/admin/corporate-users/550e8400-e29b-41d4-a716-446655440000/stats
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

**Response (200 OK):**
```json
{
  "userId": "550e8400-e29b-41d4-a716-446655440000",
  "email": "sirket@example.com",
  "phoneNumber": "+905551234567",
  "companyName": "Örnek Emlak A.Ş.",
  "isApproved": true,
  "isEmailConfirmed": true,
  "registrationDate": "2024-11-15T10:30:00",
  "lastLoginDate": "2024-11-28T09:15:00",
  "totalAnnouncements": 15,
  "photoAnnouncements": 0,
  "videoAnnouncements": 8,
  "noPhotoAnnouncements": 15,
  "noVideoAnnouncements": 7,
  "lastAnnouncementDate": "2024-11-27T14:20:00",
  "firstAnnouncementDate": "2024-11-16T11:00:00"
}
```

**Response Codes:**
- `200` - Başarılı
- `401` - Yetkisiz
- `403` - Yasak
- `404` - Kullanıcı bulunamadı

---

### 3. Kullanıcı Onaylama/Reddetme

Kurumsal kullanıcıyı onaylar veya reddeder.

**Endpoint:**
```http
PUT /api/v1/admin/corporate-users/{id}/approve
```

**Path Parameters:**

| Parametre | Tip | Zorunlu | Açıklama |
|-----------|-----|---------|----------|
| `id` | string | Evet | Kullanıcı ID (GUID) |

**Request Body:**
```json
{
  "isApproved": true
}
```

**Request Example:**
```http
PUT /api/v1/admin/corporate-users/550e8400-e29b-41d4-a716-446655440000/approve
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
Content-Type: application/json

{
  "isApproved": true
}
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Kullanıcı başarıyla onaylandı",
  "userId": "550e8400-e29b-41d4-a716-446655440000"
}
```

**Response Codes:**
- `200` - Başarılı
- `400` - Geçersiz istek
- `401` - Yetkisiz
- `403` - Yasak
- `404` - Kullanıcı bulunamadı

**Not:** İşlem sonrası kullanıcıya email bildirimi gönderilir.

---

### 4. Kullanıcı Silme

Kullanıcıyı soft delete ile siler (veritabanından silinmez, sadece işaretlenir).

**Endpoint:**
```http
DELETE /api/v1/admin/corporate-users/{id}
```

**Path Parameters:**

| Parametre | Tip | Zorunlu | Açıklama |
|-----------|-----|---------|----------|
| `id` | string | Evet | Kullanıcı ID (GUID) |

**Request Example:**
```http
DELETE /api/v1/admin/corporate-users/550e8400-e29b-41d4-a716-446655440000
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Kullanıcı başarıyla silindi",
  "userId": "550e8400-e29b-41d4-a716-446655440000"
}
```

**Response Codes:**
- `200` - Başarılı
- `401` - Yetkisiz
- `403` - Yasak
- `404` - Kullanıcı bulunamadı

**Not:** Bu işlem geri alınamaz ancak veriler veritabanında korunur (soft delete).

---

### 5. Toplu İşlem (Bulk Action)

Birden fazla kullanıcı üzerinde aynı anda işlem yapar.

**Endpoint:**
```http
POST /api/v1/admin/corporate-users/bulk-action
```

**Request Body:**
```json
{
  "userIds": [
    "550e8400-e29b-41d4-a716-446655440000",
    "6ba7b810-9dad-11d1-80b4-00c04fd430c8",
    "7c9e6679-7425-40de-944b-e07fc1f90ae7"
  ],
  "action": "approve"
}
```

**Action Types:**
- `approve` - Kullanıcıları onayla
- `reject` - Kullanıcıları reddet
- `delete` - Kullanıcıları sil

**Request Example:**
```http
POST /api/v1/admin/corporate-users/bulk-action
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
Content-Type: application/json

{
  "userIds": ["550e8400-e29b-41d4-a716-446655440000", "6ba7b810-9dad-11d1-80b4-00c04fd430c8"],
  "action": "approve"
}
```

**Response (200 OK):**
```json
{
  "totalProcessed": 2,
  "successCount": 2,
  "failedCount": 0,
  "results": [
    {
      "userId": "550e8400-e29b-41d4-a716-446655440000",
      "success": true,
      "message": "İşlem başarılı"
    },
    {
      "userId": "6ba7b810-9dad-11d1-80b4-00c04fd430c8",
      "success": true,
      "message": "İşlem başarılı"
    }
  ],
  "failedUserIds": []
}
```

**Response Codes:**
- `200` - İşlem tamamlandı (başarılı veya başarısız detayları response'da)
- `400` - Geçersiz action veya boş userIds listesi
- `401` - Yetkisiz
- `403` - Yasak

---

## 📊 Data Models

### UserDto
```typescript
{
  id: string;                      // GUID
  email: string;
  phoneNumber: string;
  companyName: string;
  isApproved: boolean;
  isEmailConfirmed: boolean;
  registrationDate: string;        // ISO 8601
  lastLoginDate: string | null;    // ISO 8601
  isActive: boolean;
  accountStatus: string;
  announcementStats: AnnouncementStatsDto;
}
```

### AnnouncementStatsDto
```typescript
{
  totalAnnouncements: number;
  photoAnnouncements: number;      // Şu an 0 (ayrı tablo)
  videoAnnouncements: number;
  noPhotoAnnouncements: number;
  noVideoAnnouncements: number;
  lastAnnouncementDate: string | null;   // ISO 8601
  firstAnnouncementDate: string | null;  // ISO 8601
}
```

---

## ⚠️ Error Response Format

Tüm hata durumlarında standart format:

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "Bad Request",
  "status": 400,
  "errors": {
    "IsApproved": ["The IsApproved field is required."]
  }
}
```

---

## 🔧 Teknik Detaylar

### Mimari
- **Pattern:** CQRS (Command Query Responsibility Segregation)
- **Mediator:** MediatR
- **Authentication:** JWT Bearer Token
- **Authorization:** Role-based (Admin)
- **Database Access:** IApplicationDbContext (Direct DbSet access)

### Oluşturulan Dosyalar (17 adet)

#### Commands (9 dosya)
```
Features/Admin/Commands/
├── ApproveUser/
│   ├── ApproveUserCommandHandler.cs
│   ├── ApproveUserCommandRequest.cs
│   └── ApproveUserCommandResponse.cs
├── BulkAction/
│   ├── BulkActionCommandHandler.cs
│   ├── BulkActionCommandRequest.cs
│   └── BulkActionCommandResponse.cs
└── DeleteUser/
    ├── DeleteUserAdminCommandHandler.cs
    ├── DeleteUserAdminCommandRequest.cs
    └── DeleteUserAdminCommandResponse.cs
```

#### Queries (6 dosya)
```
Features/Admin/Queries/
├── GetCorporateUsers/
│   ├── GetCorporateUsersQueryHandler.cs
│   ├── GetCorporateUsersQueryRequest.cs
│   └── GetCorporateUsersQueryResponse.cs
└── GetUserStats/
    ├── GetUserStatsQueryHandler.cs
    ├── GetUserStatsQueryRequest.cs
    └── GetUserStatsQueryResponse.cs
```

#### Controller (1 dosya)
```
Controllers/
└── AdminController.cs
```

#### Interface (1 dosya)
```
Interfaces/IService/Common/
└── IApplicationDbContext.cs
```

### Düzeltilen Hatalar

1. **IUnitOfWork Sorunu**
   - Problem: `IUnitOfWork.GetRepository<T>()` method'u mevcut değildi
   - Çözüm: `IApplicationDbContext` interface'i oluşturuldu, DbSet'lere doğrudan erişim sağlandı

2. **Property İsimleri**
   - `UserId` → `Email` (IlanModel'de UserId alanı yok)
   - `CreatedDate` → `GirisTarihi`
   - `VideoUrl` → `VideoLink`
   - `Photos` navigation property → 0 (ayrı tablo)

3. **Tip Dönüşümleri**
   - `DateTimeOffset?` → `DateTime?` (.DateTime property ile)
   - `Count` property → `Count()` method

---

## 🚀 Deployment

**Production URL:** `http://94.73.131.202:8090`

API şu anda production'da çalışıyor ve test edilmeye hazır.

### Swagger UI
Endpoint'leri test etmek için:
```
http://94.73.131.202:8090/swagger
```

---

## 📝 Notlar

### Bilinen Limitasyonlar

1. **Photo Statistics:**
   - `photoAnnouncements` değeri şu an `0` olarak dönüyor
   - Sebep: PhotoModel ayrı tabloda ve navigation property yok
   - İhtiyaç durumunda JOIN query ile düzeltilebilir

2. **JWT Role Claim:**
   - Token'da mutlaka Admin role claim'i olmalı
   - Claim tipi: `http://schemas.microsoft.com/ws/2008/06/identity/claims/role`
   - Değer: `Admin`

### Güvenlik

- Tüm endpoint'ler `[Authorize(Roles = "Admin")]` ile korunuyor
- Sadece Admin kullanıcılar erişebilir
- Normal kullanıcılar 403 Forbidden alır

### Performance

- Sayfalama varsayılan: 20 kayıt/sayfa
- Maximum pageSize: Sınır belirlenmemiş (ihtiyaç halinde eklenebilir)
- Database sorguları optimize edilmiş (LINQ to SQL)

---

## 📞 Destek

Sorularınız için:
- **Repository:** sevval-com/sevvalcomprojesi
- **Branch:** dev_2
- **API Documentation:** Bu dosya

---

**Son Güncelleme:** 28 Kasım 2024  
**Versiyon:** 1.0.0  
**Durum:** ✅ Production'da Aktif
