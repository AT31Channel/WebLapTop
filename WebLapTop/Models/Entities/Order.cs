using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace WebLapTop.Models.Entities
{
    [Table("Orders")]
    public class Order
    {
        public Order()
        {
            OrderDate = DateTime.Now;
            Status = "Pending";
            OrderDetails = new HashSet<OrderDetail>();
        }

        [Key]
        [Column("OrderId")]
        public int Id { get; set; }

        [Required]
        [Column("UserId")]
        public string UserId { get; set; } = string.Empty;

        [Column("OrderDate")]
        public DateTime OrderDate { get; set; }

        [Required]
        [StringLength(500)]
        [Column("ShippingAddress")]
        public string ShippingAddress { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [Column("FullName")]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        [Column("PhoneNumber")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [Column("Email")]
        public string Email { get; set; } = string.Empty;

        [Column("TotalAmount")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public decimal TotalAmount { get; set; }

        [StringLength(1000)]
        [Column("Note")]
        public string? Note { get; set; }

        [Required]
        [StringLength(50)]
        [Column("PaymentMethod")]
        public string PaymentMethod { get; set; } = "COD";

        [Required]
        [StringLength(50)]
        [Column("Status")]
        public string Status { get; set; } = "Pending";

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; } = null!;

        public virtual ICollection<OrderDetail> OrderDetails { get; set; }

        // Tương thích ngược cho giao diện tiếng Việt
        [NotMapped]
        public string MaDonHang { get => Id.ToString("D6"); }

        [NotMapped]
        public string DiaChiGiaoHang { get => ShippingAddress; set => ShippingAddress = value; }

        [NotMapped]
        public string GhiChu { get => Note ?? string.Empty; set => Note = value; }

        [NotMapped]
        public decimal TongTien { get => TotalAmount; set => TotalAmount = value; }

        [NotMapped]
        public DateTime NgayDatHang { get => OrderDate; set => OrderDate = value; }

        [NotMapped]
        public string TrangThai { get => Status; set => Status = value; }

        [NotMapped]
        public string MaNguoiDung { get => UserId; set => UserId = value; }
    }

    public enum OrderStatus
    {
        [Display(Name = "Chờ xử lý")]
        Pending = 0,

        [Display(Name = "Đã xác nhận")]
        Confirmed = 1,

        [Display(Name = "Đang giao hàng")]
        Shipping = 2,

        [Display(Name = "Đã giao hàng")]
        Delivered = 3,

        [Display(Name = "Đã hủy")]
        Cancelled = 4
    }
}
