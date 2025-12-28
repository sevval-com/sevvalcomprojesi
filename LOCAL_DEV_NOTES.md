# Local Development Notes - GitHub'a Atılmayacak

Bu dosya local development için yapılan değişiklikleri içerir.
**ÖNEMLİ: Bu dosya ve aşağıdaki değişiklikler GitHub'a push edilmemelidir!**

---

## 1. Mock Data Dosyaları (Kök dizinde)

### `mock_gunun_ilani_seed.sql`
- Günün ilanı için mock data
- DB Browser for SQLite ile çalıştırılır
- İlan No: SVL-2024-001

### `mock_admin_user_seed.sql`
- Admin kullanıcı SQL (kullanılmıyor, seed ile oluşturuluyor)

### `seed_mock_data.ps1`
- PowerShell yardımcı script

---

## 2. Değiştirilen Dosyalar

### `Src/Infrastructure/Sevval.Persistence/Context/AppContextInitializer.cs`
**Eklenen metodlar:**
- `SeedAdminUserAsync()` - Admin kullanıcı oluşturur
- `SeedMockGununIlaniAsync()` - Mock günün ilanı oluşturur

**Admin Giriş Bilgileri:**
- Email: `sftumen41@gmail.com`
- Şifre: `Admin123!`

### `Src/Presentation/Sevval.Web/appsettings.json`
**Değişiklik:** Connection string local path'e güncellendi
```json
"DefaultConnection": "Data Source=C:\\Users\\emreg\\sevvalcomprojesi\\Src\\Presentation\\Sevval.Api\\sevvalemlak2.db;..."
```
**Orijinal değer:**
```json
"DefaultConnection": "Data Source=/Users/murat/Desktop/sevvalEmlak/sevko/Src/Presentation/Sevval.Api/sevvalemlak2.db;..."
```

---

## 3. .gitignore'a Eklenecekler

```gitignore
# Local development mock data
mock_*.sql
seed_mock_data.ps1
LOCAL_DEV_NOTES.md
```

---

## 4. GitHub'a Push Etmeden Önce Yapılacaklar

1. `AppContextInitializer.cs` dosyasındaki seed metodlarını kaldır veya yorum satırı yap
2. `appsettings.json` connection string'i orijinal haline döndür
3. Mock SQL dosyalarını silme (gitignore'da olacak)

---

## 5. Veritabanı Bilgileri

- **Dosya:** `Src/Presentation/Sevval.Api/sevvalemlak2.db`
- **Tür:** SQLite
- **Araç:** DB Browser for SQLite

### Tablolar:
- `AspNetUsers` - Kullanıcılar
- `IlanBilgileri` - İlanlar
- `GununIlanlari` - Günün ilanları
- `Photos` - İlan fotoğrafları

---

## 6. Hızlı Komutlar

```powershell
# Uygulamayı çalıştır
cd Src/Presentation/Sevval.Web
dotnet run

# Build
dotnet build Sevval.sln
```

---

*Son güncelleme: 2024-12-27*
