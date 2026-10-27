using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace WebLapTop.Models.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            HoTen = string.Empty;
            DiaChi = string.Empty;
            FullName = string.Empty;
            Carts = new HashSet<Cart>();
            Orders = new HashSet<Order>();
            GioHangs = Carts;
            DonHangs = Orders;
        }

        [Required]
        [StringLength(100)]
        [Display(Name = "Họ tên")]
        public string HoTen { get; set; }

        [StringLength(500)]
        [Display(Name = "Địa chỉ")]
        public string DiaChi { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        // Navigation properties
        public virtual ICollection<Cart> Carts { get; set; }
        public virtual ICollection<Order> Orders { get; set; }

        // Tương thích ngược
        public virtual ICollection<Cart> GioHangs { get => Carts; set => Carts = value; }
        public virtual ICollection<Order> DonHangs { get => Orders; set => Orders = value; }
    }
}
