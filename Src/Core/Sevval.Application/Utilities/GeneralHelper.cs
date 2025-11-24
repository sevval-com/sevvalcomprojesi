namespace Sevval.Application.Utilities;

public static class GeneralHelper
{
    public static Dictionary<string, string> CityLandmarks = new Dictionary<string, string>
{
    {"İSTANBUL", "Ayasofya Camii"},
    {"KONYA", "Mevlana Türbesi"},
    {"İZMİR", "Saat Külesi"},
    {"KOCAELİ", "İzmit Simidi"},
    {"BALIKESİR", "Kaz Dağları"},
    {"KÜTAHYA", "Vazo Heykeli"},
    {"MANİSA", "Sardes Antik Kenti"},
    {"BURSA", "Kestane Şekeri"},
    {"UŞAK", "Blaundos Antik Kenti"},
    {"SAKARYA", "Sapanca Gölü"},
    {"ESKİŞEHİR", "Odunpazarı Evleri"},
    {"ANKARA", "Atakule"},
    {"AYDIN", "İncir Ağaçları"},
    {"KARABÜK", "Demir Çelik Fabrikası"},
    {"BOLU", "Abant Gölü"},
    {"BİLECİK", "Osmanlı Çadırı"},
    {"KASTAMONU", "Kastamonu Evleri"},
    {"ZONGULDAK", "Kömür Madeni"},
    {"KIRKLARELİ", "Dupnisa Mağarası"},
    {"SİVAS", "Çifte Minare Medresesi"},
    {"ÇANAKKALE", "Truva Atı"},
    {"KARAMAN", "Karaman Koyunu"},
    {"GİRESUN", "Fındık"},
    {"YOZGAT", "Roma Hamamı"},
    {"ANTALYA", "Meyve Bahçesi"},
    {"TEKİRDAĞ", "Ayçiçeği Tarlası"},
    {"ARTVİN", "Artvin Balı"},
    {"GAZİANTEP", "Baklava"},
    {"ÇORUM", "Leblebi"},
    {"BARTIN", "Gıcır"},
    {"AMASYA", "Elma"},
    {"KAYSERİ", "Mantı"},
    {"AFYONKARAHİSAR", "Patates Tarlaları"},
    {"MUĞLA", "Saklıkent Kanyonu"},
    {"BURDUR", "Lavanta Bahçesi"},
    {"ORDU", "Teleferik ve Karadeniz"},
    {"YALOVA", "Yalova Geceleri Kolonyası"},
    {"ÇANKIRI", "Kızılcık Şerbeti"},
    {"DENİZLİ", "Horoz"},
    {"ISPARTA", "Gül Bahçesi"},
    {"DÜZCE", "Tütün Kolonyası"},
    {"ERZURUM", "Palandöken Dağı"},
    {"SAMSUN", "Sevgi Gölü"},
    {"KIRIKKALE", "Dinek Dağı"},
    {"AKSARAY", "Taşpınar Halısı"},
    {"BAYBURT", "Kesme Çorbası"},
    {"MERSİN", "Muz"},
    {"BİTLİS", "Kızıl Kule"},
    {"ERZİNCAN", "Saat Kulesi"},
    {"SİNOP", "Sinop Burnu"},
    {"EDİRNE", "Selimiye Camii"},
    {"TOKAT", "Türk Hamamı"},
    {"ADIYAMAN", "Karakuş Tümüsü"},
    {"BATMAN", "Yün Çoraplar"},
    {"RİZE", "Çay Bahçesi"},
    {"MALATYA", "Kayısı"},
    {"KIRŞEHİR", "Çiçek Bahçesi"},
    {"TRABZON", "Hamsi"},
    {"AĞRI", "Ağrı Dağı"},
    {"ADANA", "Taş Köprü"},
    {"ARDAHAN", "Ardahan Kalesi"},
    {"NEVŞEHİR", "Ürgüp"},
    {"BİNGÖL", "Göller"},
    {"KARS", "Çıldır Gölü"},
    {"ELAZIĞ", "Harput Kalesi"},
    {"VAN", "Van Gölü"},
    {"KAHRAMANMARAŞ", "Maraş Dondurması"},
    {"DİYARBAKIR", "Diyarbakır Karpuzu"},
    {"MARDİN", "Kasımiye Medresesi"},
    {"ŞANLIURFA", "Balıklı Göl"}
};

    // Method to get landmark by city name
    public static string GetLandmark(this string cityName)
    {
        return CityLandmarks.TryGetValue(cityName?.ToUpper(), out string landmark)
            ? landmark
            : "-";
    }

    // Method to display all cities and their landmarks
    public static void DisplayAll()
    {
        foreach (var kvp in CityLandmarks)
        {
            Console.WriteLine($"{kvp.Key}: {kvp.Value}");
        }
    }

    public static Dictionary<string, string> categoryImagePaths = new Dictionary<string, string>
{
    { "Konut (Yaşam Alanı)", "/ImageFiles/konutilanlari.png" },
    { "İş Yeri", "/ImageFiles/isyeriilanlari2.png" },
    { "Turistik Tesis", "/ImageFiles/turistiktesisilanlari.png" },
    { "Arsa", "/ImageFiles/arsailanlari.png" },
    { "Bahçe", "/ImageFiles/bahceilanlari.png" },
    { "Tarla", "/ImageFiles/tarlailanlari.png" }
};

    public static string GetImagePath(this string category)
    {
        return categoryImagePaths.TryGetValue(category, out string landmark)
            ? landmark
            : "-";
    }
}
