using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebLapTop.Models.Entities
{
    [Table("ProductSpecifications")]
    public class ProductSpecification
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        [StringLength(100)]
        public string SpecName { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string SpecValue { get; set; } = string.Empty;

        [Required]
        public int DisplayOrder { get; set; }

        [ForeignKey(nameof(ProductId))]
        public virtual Product Product { get; set; } = null!;
    }
} 