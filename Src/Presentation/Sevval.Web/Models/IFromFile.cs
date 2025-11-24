
namespace sevvalemlak.Models
{
    public interface IFromFile
    {
        object FileName { get; set; }

        Task CopyToAsync(FileStream? stream);
    }
}