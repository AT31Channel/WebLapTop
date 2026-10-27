using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace WebLapTop.Models.Entities
{
    /// <summary>
    /// Danh mục sản phẩm
    /// </summary>
    [Table("Categories")]
    public class Category
    {
        public Category()
        {
            Name = string.Empty;
            Description = string.Empty;
            IsVisible = true;
            DisplayOrder = 0;
            Products = new List<Product>();
            SanPhams = Products;
        }

        [Key]
        [Column("CategoryId")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên danh mục")]
        [Display(Name = "Tên danh mục")]
        [MaxLength(200, ErrorMessage = "Tên danh mục không được vượt quá 200 ký tự")]
        public string Name { get; set; }

        [Display(Name = "Mô tả")]
        [MaxLength(1000)]
        public string Description { get; set; }

        [Display(Name = "Hiển thị")]
        public bool IsVisible { get; set; }

        [Display(Name = "Thứ tự")]
        public int DisplayOrder { get; set; }

        // Navigation property
        public virtual ICollection<Product> Products { get; set; }

        // Tương thích ngược cho giao diện tiếng Việt
        [NotMapped]
        public string TenDanhMuc { get => Name; set => Name = value; }

        [NotMapped]
        public string MoTa { get => Description; set => Description = value; }

        [NotMapped]
        public bool HienThi { get => IsVisible; set => IsVisible = value; }

        [NotMapped]
        public int ThuTu { get => DisplayOrder; set => DisplayOrder = value; }

        [NotMapped]
        public int MaDanhMuc { get => Id; set => Id = value; }

        [NotMapped]
        public virtual ICollection<Product> SanPhams { get => Products; set => Products = value; }
    }
}
