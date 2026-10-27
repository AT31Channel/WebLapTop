using System.ComponentModel.DataAnnotations;
using WebLapTop.Models.Entities;

namespace WebLapTop.Models.ViewModels
{
    public class CheckoutViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        [Display(Name = "Họ tên")]
        [StringLength(100, ErrorMessage = "Họ tên không được vượt quá {1} ký tự")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập email")]
        [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [Display(Name = "Số điện thoại")]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Số điện thoại không hợp lệ")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ giao hàng")]
        [Display(Name = "Địa chỉ giao hàng")]
        [StringLength(500, ErrorMessage = "Địa chỉ không được vượt quá {1} ký tự")]
        public string ShippingAddress { get; set; } = string.Empty;

        [Display(Name = "Ghi chú")]
        [StringLength(1000, ErrorMessage = "Ghi chú không được vượt quá {1} ký tự")]
        public string Note { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng chọn phương thức thanh toán")]
        [Display(Name = "Phương thức thanh toán")]
        public string PaymentMethod { get; set; } = "COD";

        public decimal TotalAmount { get; set; }

        public IEnumerable<Cart> CartItems { get; set; } = Enumerable.Empty<Cart>();
    }
} 