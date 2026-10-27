using WebLapTop.Models.Entities;

namespace WebLapTop.Models.ViewModels
{
    public class UserDetailsViewModel
    {
        public ApplicationUser User { get; set; } = null!;
        public bool IsAdmin { get; set; }
        public List<Order> Orders { get; set; } = new();
    }
} 