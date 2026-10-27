using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebLapTop.Models.Entities
{
    /// <summary>
    /// Cart
    /// </summary>
    [Table("Carts")]
    public class Cart
    {
        public Cart()
        {
            UserId = string.Empty;
            AddedDate = DateTime.Now;
            Quantity = 1;
        }

        [Key]
        [Column("CartId")]
        public int Id { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
        [Column("Quantity")]
        public int Quantity { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Đơn giá phải lớn hơn hoặc bằng 0")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        [Column("AddedDate")]
        public DateTime AddedDate { get; set; } = DateTime.Now;

        [Required]
        [StringLength(450)]
        [Column("UserId")]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [Column("ProductId")]
        public int ProductId { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; } = null!;

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; } = null!;

        // Tương thích ngược cho giao diện tiếng Việt
        [NotMapped]
        public int SoLuong { get => Quantity; set => Quantity = value; }

        [NotMapped]
        public decimal DonGia { get => UnitPrice; set => UnitPrice = value; }

        [NotMapped]
        public DateTime NgayThem { get => AddedDate; set => AddedDate = value; }

        [NotMapped]
        public string MaNguoiDung { get => UserId; set => UserId = value; }

        [NotMapped]
        public int MaSanPham { get => ProductId; set => ProductId = value; }
    }
}
