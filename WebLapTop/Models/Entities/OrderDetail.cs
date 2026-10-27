using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebLapTop.Models.Entities
{
    [Table("OrderDetails")]
    public class OrderDetail
    {
        public OrderDetail()
        {
            Quantity = 1;
            UnitPrice = 0;
        }

        [Key]
        [Column("OrderDetailId")]
        public int Id { get; set; }

        [Required]
        [Column("OrderId")]
        public int OrderId { get; set; }

        [Required]
        [Column("ProductId")]
        public int ProductId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
        [Column("Quantity")]
        public int Quantity { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Đơn giá phải lớn hơn hoặc bằng 0")]
        [Column("UnitPrice", TypeName = "decimal(18,2)")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public decimal UnitPrice { get; set; }

        [Required]
        [Column("IsReviewed")]
        public bool IsReviewed { get; set; }

        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; } = null!;

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; } = null!;

        // Tương thích ngược cho giao diện tiếng Việt
        [NotMapped]
        public int MaChiTietDonHang { get => Id; set => Id = value; }

        [NotMapped]
        public int MaDonHang { get => OrderId; set => OrderId = value; }

        [NotMapped]
        public int MaSanPham { get => ProductId; set => ProductId = value; }

        [NotMapped]
        public int SoLuong { get => Quantity; set => Quantity = value; }

        [NotMapped]
        public decimal DonGia { get => UnitPrice; set => UnitPrice = value; }

        [NotMapped]
        public bool DaDanhGia { get => IsReviewed; set => IsReviewed = value; }

        [NotMapped]
        public decimal ThanhTien => Quantity * UnitPrice;
    }
}
