namespace sevvalemlak.Models
{
    public class Ad
    {
        public int Id { get; set; }
        public string UserEmail { get; set; }

        // Haftanın günlerine göre günlük görüntülenme sayıları (örneğin Pazardan Cumartesi'ye)
        public int[] Views { get; set; }
    }
}