using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebLapTop.Models.Entities
{
    /// <summary>
    /// Review
    /// </summary>
    [Table("Reviews")]
    public class Review
    {
        public Review()
        {
            UserId = string.Empty;
            Comment = string.Empty;
            CreatedDate = DateTime.Now;
        }

        [Key]
        public int Id { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "Đánh giá phải từ 1 đến 5 sao")]
        public int Rating { get; set; }

        [StringLength(1000)]
        public string Comment { get; set; }

        public DateTime CreatedDate { get; set; }

        [Required]
        public int OrderId { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; } = null!;

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; } = null!;

        [ForeignKey("OrderId")]
        public virtual OrderDetail OrderDetail { get; set; } = null!;

        // Tương thích ngược cho giao diện tiếng Việt
        [NotMapped]
        public int MaDanhGia => Id;

        [NotMapped]
        public int MaSanPham => ProductId;

        [NotMapped]
        public string MaNguoiDung => UserId;

        [NotMapped]
        public string BinhLuan => Comment;

        [NotMapped]
        public DateTime NgayTao => CreatedDate;
    }
}
