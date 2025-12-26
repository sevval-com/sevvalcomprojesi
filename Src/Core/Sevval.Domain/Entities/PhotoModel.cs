namespace Sevval.Domain.Entities;

public class PhotoModel
{
    public int Id { get; set; }  // Fotoğraf ID'si

    public string Url { get; set; }  // Fotoğrafın URL adresi

    public int IlanId { get; set; }  // İlgili ilan ID'si

}
