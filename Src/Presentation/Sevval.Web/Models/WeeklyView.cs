using Microsoft.AspNetCore.Mvc;

namespace sevvalemlak.csproj.Models
{
    public class WeeklyView
    {
        public int Id { get; set; }  // Birincil anahtar
        public string UserEmail { get; set; }  // Kullanıcı e-posta adresi
        public DateTime Date { get; set; }  // Tarih
        public int Views { get; set; }  // Günlük görüntülenme sayısı
    }
}
