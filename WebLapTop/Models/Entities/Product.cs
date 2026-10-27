using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace WebLapTop.Models.Entities
{
    [Table("Products")]
    public class Product(string name = "", string description = "", string imageUrl = "")
    {
        [Key]
        [Column("ProductId")]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = name;

        [Required]
        [StringLength(1000)]
        public string Description { get; set; } = description;

        [Required]
        [StringLength(200)]
        public string ImageUrl { get; set; } = imageUrl;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? DiscountPrice { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        public bool IsVisible { get; set; } = true;

        [Required]
        public bool IsAvailable { get; set; } = true;

        [Required]
        public bool IsFeatured { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime? UpdatedDate { get; set; } = DateTime.Now;

        [Required]
        public int Stock { get; set; }

        [NotMapped]
        public double Rating => Reviews.Count > 0 ? Reviews.Average(r => r.Rating) : 0;

        [ForeignKey(nameof(CategoryId))]
        public virtual Category Category { get; set; } = null!;

        public virtual ICollection<Review> Reviews { get; set; } = [];
        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = [];
        public virtual ICollection<Cart> Carts { get; set; } = [];
        public virtual ICollection<ProductImage> Images { get; set; } = [];
        public virtual ICollection<ProductSpecification> Specifications { get; set; } = [];
        public virtual ICollection<Warranty> Warranties { get; set; } = [];
        public virtual ICollection<Promotion> Promotions { get; set; } = [];

        // Tương thích ngược cho giao diện tiếng Việt
        [NotMapped]
        public int MaSanPham => Id;

        [NotMapped]
        public string TenSanPham => Name;

        [NotMapped]
        public string MoTa => Description;

        [NotMapped]
        public string HinhAnh => ImageUrl;

        [NotMapped]
        public decimal Gia => Price;

        [NotMapped]
        public decimal? GiaGiam => DiscountPrice;

        [NotMapped]
        public int MaDanhMuc => CategoryId;

        [NotMapped]
        public bool HienThi => IsVisible;

        [NotMapped]
        public bool ConHang => IsAvailable;

        [NotMapped]
        public bool NoiBat => IsFeatured;

        [NotMapped]
        public DateTime NgayTao => CreatedDate;

        [NotMapped]
        public DateTime? NgayCapNhat => UpdatedDate;

        [NotMapped]
        public int SoLuongTon => Stock;

        [NotMapped]
        public double DiemDanhGia => Rating;
    }
}
