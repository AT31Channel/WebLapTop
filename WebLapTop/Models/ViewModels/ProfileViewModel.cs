using System.ComponentModel.DataAnnotations;

namespace WebLapTop.Models.ViewModels
{
    public class ProfileViewModel
    {
        public ProfileViewModel()
        {
            Email = string.Empty;
            HoTen = string.Empty;
            PhoneNumber = string.Empty;
        }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        [StringLength(100, ErrorMessage = "Họ tên không được vượt quá {1} ký tự")]
        [Display(Name = "Họ tên")]
        public string HoTen { get; set; }

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [Display(Name = "Số điện thoại")]
        public string PhoneNumber { get; set; }
    }
}
