using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebLapTop.Models.Entities
{
    [Table("CustomerNotifications")]
    public class CustomerNotification
    {
        [Key]
        [Column("NotificationId")]
        public int Id { get; set; }

        [Required]
        [Column("UserId")]
        [StringLength(450)]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [Column("Title")]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [Column("Message")]
        [StringLength(500)]
        public string Message { get; set; } = string.Empty;

        [Required]
        [Column("CreateDate")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Required]
        [Column("IsRead")]
        public bool IsRead { get; set; } = false;

        [Column("OrderId")]
        public int? OrderId { get; set; }

        [Column("Link")]
        [StringLength(255)]
        public string? Link { get; set; }

        [Column("Type")]
        [StringLength(50)]
        public string Type { get; set; } = "Order"; // Order, System, Promotion

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; } = null!;

        [ForeignKey("OrderId")]
        public virtual Order? Order { get; set; }

        // Tương thích ngược cho giao diện tiếng Việt
        [NotMapped]
        public int MaThongBao { get => Id; set => Id = value; }

        [NotMapped]
        public string TieuDe { get => Title; set => Title = value; }

        [NotMapped]
        public string NoiDung { get => Message; set => Message = value; }

        [NotMapped]
        public DateTime NgayTao { get => CreateDate; set => CreateDate = value; }

        [NotMapped]
        public bool DaDoc { get => IsRead; set => IsRead = value; }

        [NotMapped]
        public string MaNguoiDung { get => UserId; set => UserId = value; }

        [NotMapped]
        public int? MaDonHang { get => OrderId; set => OrderId = value; }

        [NotMapped]
        public string Loai { get => Type; set => Type = value; }
    }
} 