using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebLapTop.Models.Entities
{
    [Table("Warranties")]
    public class Warranty
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        public int WarrantyPeriod { get; set; } // Số tháng bảo hành

        [StringLength(500)]
        public string? Description { get; set; }

        [StringLength(1000)]
        public string? Terms { get; set; }

        [ForeignKey(nameof(ProductId))]
        public virtual Product Product { get; set; } = null!;
    }
} 