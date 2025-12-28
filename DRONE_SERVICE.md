
## Genel Akis

1) Kullanici `Drone/ParselSorgu` sayfasinda il/ilce/mahalle/ada/parsel secer.
2) Parsel polygon bilgisi alinip haritada gosterilir.
3) Kullanici `Drone/Olustur` sayfasina yonlendirilir.
4) Kullanici logo/avatar/aciklama girdikten sonra video istegi gonderilir.
5) Web, video-processor servisine `x-api-key` ile istek gecer.

## Sayfalar

### 1) Drone/ParselSorgu

- Dosya: `Src/Presentation/Sevval.Web/Views/Drone/ParselSorgu.cshtml`
- Amac: Il/ilce/mahalle secimi + parsel polygon gosterimi.

Kullanilan endpoint'ler (Web proxy):
- `GET /api/tkgm/il`
- `GET /api/tkgm/ilce/{ilId}`
- `GET /api/tkgm/mahalle/{ilceId}`
- `GET /api/tkgm/parsel-geo?mahalleId={id}&ada={ada}&parsel={parsel}`

Notlar:
- `ilId/ilceId/mahalleId` degerleri TKGM servisinden gelir.
- `parsel-geo` cevabi GeoJSON `Feature` veya `FeatureCollection` olabilir.

### 2) Drone/Olustur

- Dosya: `Src/Presentation/Sevval.Web/Views/Drone/Olustur.cshtml`
- Amac: Drone video istegini gondermek.
- Icerik: Logo, avatar, aciklama, dil, tapu tipi.

Istek:
- `POST /api/drone/create` (form-data)

Gonderilen alanlar:
- `il`, `ilce`, `mahalle`, `ada`, `parsel`
- `lang`, `tapu`, `description`
- `logo` (file), `avatar` (file)

## Web API (Sevval Web)

### 1) Drone API

- Controller: `Src/Presentation/Sevval.Web/Controllers/DroneApiController.cs`
- Endpoint: `POST /api/drone/create`

Proxy edilen servis:
- `POST {SevvalVideo:BaseUrl}/api/sevval/process-video`

Header:
- `x-api-key: {SevvalVideo:ApiKey}`
- `Origin: https://sevval.com`

### 2) TKGM Proxy API

- Controller: `Src/Presentation/Sevval.Web/Controllers/TkgmController.cs`
- Amac: TKGM servislerini server uzerinden cagirmak ve cache etmek.

Endpoint'ler:
- `GET /api/tkgm/il`
- `GET /api/tkgm/ilce/{ilId}`
- `GET /api/tkgm/mahalle/{ilceId}`
- `GET /api/tkgm/parsel-geo?mahalleId={id}&ada={ada}&parsel={parsel}`

Cache:
- Liste endpointleri: 6 saat
- Parsel endpointi: 5 dakika

## Video Processor Servisi

Base URL:
- `http://20.215.34.129:8291/api/sevval`

Kimlik dogrulama:
- Header: `x-api-key: <SEVVAL_API_KEY>`

Onemli env ayarlari (video-processor):
- `SEVVAL_ENABLED=true`
- `SEVVAL_API_KEY=...`
- `SEVVAL_ALLOWED_ORIGINS=https://sevval.com,https://www.sevval.com`
- `SEVVAL_ALLOW_NO_ORIGIN=false`

Test:
```
curl -H "x-api-key: <SEVVAL_API_KEY>" http://20.215.34.129:8291/api/sevval/test
```

## Konfigurasyon

Web tarafi (Sevval Web):
- Dosya: `Src/Presentation/Sevval.Web/appsettings.json`
- Anahtar:
  - `SevvalVideo:BaseUrl`
  - `SevvalVideo:ApiKey`

## Sık Problemler

1) 401 Unauthorized (process-video)
- `SevvalVideo:ApiKey` bos veya yanlis
- `SEVVAL_ALLOWED_ORIGINS` localhost'u kabul etmiyor

2) Polygon gorunmuyor
- `parsel-geo` cevabi `FeatureCollection` olabilir
- Mahalle ID yanlis ise 404 gelir

3) TKGM limit/engelleme
- Proxy cache suresi limitleri azaltir
- TKGM tarafli rate limit olabilir

