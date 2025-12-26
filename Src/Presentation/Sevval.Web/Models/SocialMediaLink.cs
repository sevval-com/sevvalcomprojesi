namespace sevvalemlak.Models // Projenizin namespace'ini kontrol edin
{
    public class SocialMediaLink
    {
        public string Platform { get; set; } // Sosyal medya platformunun adı (örn. YouTube)
        public string Url { get; set; } // Sosyal medya URL'si
        public string IconClass { get; set; } // İkon için CSS sınıfı (örn. FontAwesome)
    }
}
