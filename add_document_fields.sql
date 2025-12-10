-- Yeni Document alanlarını AspNetUsers tablosuna ekliyoruz
-- Bu script güvenli: COLUMN zaten varsa hata vermez

-- Document1Path ekliyoruz (UserTypes'a göre: Seviye 5, Müteahhitlik, İmza Sirküleri, Vakıf Senedi)
ALTER TABLE AspNetUsers ADD COLUMN Document1Path TEXT NULL;

-- Document2Path ekliyoruz (Her zaman Vergi Levhası)
ALTER TABLE AspNetUsers ADD COLUMN Document2Path TEXT NULL;

-- RecoveryToken ekliyoruz (add_recovery_token.sql ile zaten eklenmiş olabilir)
-- ALTER TABLE DeletedAccounts ADD COLUMN RecoveryToken TEXT NULL;

-- Geriye dönük veri migration: Mevcut Level5CertificatePath ve TaxPlatePath değerlerini kopyalıyoruz
UPDATE AspNetUsers 
SET Document1Path = Level5CertificatePath,
    Document2Path = TaxPlatePath
WHERE Level5CertificatePath IS NOT NULL 
   OR TaxPlatePath IS NOT NULL;
