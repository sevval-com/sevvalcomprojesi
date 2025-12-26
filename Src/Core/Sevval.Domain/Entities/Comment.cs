using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sevval.Domain.Entities;

public class Comment
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(1000)]
    public string Content { get; set; }

    public string UserFullName { get; set; }

    public string? UserId { get; set; }

    [ForeignKey("UserId")]
    public ApplicationUser User { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // Yeni eklenen alan: Yorum onay durumu. 
    // Varsayılan false, yani admin onaylayana kadar görünmez.
    public bool IsApproved { get; set; } = false;
}