using System.Collections.Generic;

namespace Sevval
{
    public static class CityLandmarks
    {
        public static readonly Dictionary<string, string> Landmarks = new Dictionary<string, string>
        {
            { "İSTANBUL", "Ayasofya Camii" },
            { "KONYA", "Mevlana Türbesi" },
            { "İZMİR", "Saat Külesi" },
            { "KOCAELİ", "İzmit Simidi" },
            { "BALIKESİR", "Kaz Dağları" },
            { "KÜTAHYA", "Vazo Heykeli" },
            { "MANİSA", "Sardes Antik Kenti" },
            { "BURSA", "Kestane Şekeri" },
            { "UŞAK", "Blaundos Antik Kenti" },
            { "SAKARYA", "Sapanca Gölü" },
            { "ESKİŞEHİR", "Odunpazarı Evleri" },
            { "ANKARA", "Atakule" },
            { "AYDIN", "İncir Ağaçları" },
            { "KARABÜK", "Demir Çelik Fabrikası" },
            { "BOLU", "Abant Gölü" },
            { "BİLECİK", "Osmanlı Çadırı" },
            { "KASTAMONU", "Kastamonu Evleri" },
            { "ZONGULDAK", "Kömür Madeni" },
            { "KIRKLARELİ", "Dupnisa Mağarası" },
            { "SİVAS", "Çifte Minare Medresesi" },
            { "ÇANAKKALE", "Truva Atı" },
            { "KARAMAN", "Karaman Koyunu" },
            { "GİRESUN", "Fındık" },
            { "YOZGAT", "Roma Hamamı" },
            { "ANTALYA", "Meyve Bahçesi" },
            { "TEKİRDAĞ", "Ayçiçeği Tarlası" },
            { "ARTVİN", "Artvin Balı" },
            { "GAZİANTEP", "Baklava" },
            { "ÇORUM", "Leblebi" },
            { "BARTIN", "Gıcır" },
            { "AMASYA", "Elma" },
            { "KAYSERİ", "Mantı" },
            { "AFYONKARAHİSAR", "Patates Tarlaları" },
            { "MUĞLA", "Saklıkent Kanyonu" },
            { "BURDUR", "Lavanta Bahçesi" },
            { "ORDU", "Teleferik ve Karadeniz" },
            { "YALOVA", "Yalova Geceleri Kolonyası" },
            { "ÇANKIRI", "Kızılcık Şerbeti" },
            { "DENİZLİ", "Horoz" },
            { "ISPARTA", "Gül Bahçesi" },
            { "DÜZCE", "Tütün Kolonyası" },
            { "ERZURUM", "Palandöken Dağı" },
            { "SAMSUN", "Sevgi Gölü" },
            { "KIRIKKALE", "Dinek Dağı" },
            { "AKSARAY", "Taşpınar Halısı" },
            { "BAYBURT", "Kesme Çorbası" },
            { "MERSİN", "Muz" },
            { "BİTLİS", "Kızıl Kule" },
            { "ERZİNCAN", "Saat Kulesi" },
            { "SİNOP", "Sinop Burnu" },
            { "EDİRNE", "Selimiye Camii" },
            { "TOKAT", "Türk Hamamı" },
            { "ADIYAMAN", "Karakuş Tümüsü" },
            { "BATMAN", "Yün Çoraplar" },
            { "RİZE", "Çay Bahçesi" },
            { "MALATYA", "Kayısı" },
            { "KIRŞEHİR", "Çiçek Bahçesi" },
            { "TRABZON", "Hamsi" },
            { "AĞRI", "Ağrı Dağı" },
            { "ADANA", "Taş Köprü" },
            { "ARDAHAN", "Ardahan Kalesi" },
            { "NEVŞEHİR", "Ürgüp" },
            { "BİNGÖL", "Göller" },
            { "KARS", "Çıldır Gölü" },
            { "ELAZIĞ", "Harput Kalesi" },
            { "VAN", "Van Gölü" },
            { "KAHRAMANMARAŞ", "Maraş Dondurması" },
            { "DİYARBAKIR", "Diyarbakır Karpuzu" },
            { "MARDİN", "Kasımiye Medresesi" },
            { "ŞANLIURFA", "Balıklı Göl" }
        };

        /// <summary>
        /// Gets the landmark for a given city name
        /// </summary>
        /// <param name="cityName">City name in uppercase</param>
        /// <returns>Landmark name or null if city not found</returns>
        public static string GetLandmark(string cityName)
        {
            return Landmarks.TryGetValue(cityName?.ToUpper(), out string landmark) ? landmark : null;
        }
    }
}
