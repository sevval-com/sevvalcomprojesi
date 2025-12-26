using System.ComponentModel.DataAnnotations;

namespace Sevval.Domain.Entities
{
    public class Category
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        
        [MaxLength(1000)]
        public string? Icon { get; set; }
        
        [MaxLength(100)]
        public string? Color { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
