using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebLapTop.Models.Entities
{
    [Table("AdminNotifications")]
    public class AdminNotification
    {
        public AdminNotification()
        {
            CreateDate = DateTime.Now;
            IsRead = false;
            Title = string.Empty;
            Message = string.Empty;
            Type = "Order";
        }

        [Key]
        [Column("NotificationId")]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        [Column("Title")]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        [Column("Message")]
        public string Message { get; set; } = string.Empty;

        [Column("CreateDate")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Column("IsRead")]
        public bool IsRead { get; set; }

        [Column("OrderId")]
        public int? OrderId { get; set; }

        [StringLength(50)]
        [Column("Type")]
        public string Type { get; set; } = "Order";

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
        public int? MaDonHang { get => OrderId; set => OrderId = value; }

        [NotMapped]
        public string Loai { get => Type; set => Type = value; }
    }
} 