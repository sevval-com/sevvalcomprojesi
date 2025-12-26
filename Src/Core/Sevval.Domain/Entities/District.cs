using Sevval.Domain.Entities.Common;
using System.ComponentModel.DataAnnotations;


namespace Sevval.Domain.Entities;

public class District:BaseAuditableEntity<int>
{
    [StringLength(100)]
    public string Name { get; set; } // İlçe adı
    [StringLength(10)]
    public string Code { get; set; } // İlçe kodu
    public string Boundry { get; set; } // İlçe kodu
    public int ProvinceId { get; set; } // İl id
 }
