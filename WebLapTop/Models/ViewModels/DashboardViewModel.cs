using WebLapTop.Models.Entities;

namespace WebLapTop.Models.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalOrders { get; set; }
        public int PendingOrders { get; set; }
        public int ConfirmedOrders { get; set; }
        public int ShippingOrders { get; set; }
        public int DeliveredOrders { get; set; }
        public int CancelledOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public List<TopProductViewModel> TopProducts { get; set; } = new List<TopProductViewModel>();
        public List<Order> RecentOrders { get; set; } = new List<Order>();
        public int TotalUsers { get; set; }
    }

    public class TopProductViewModel
    {
        public TopProductViewModel()
        {
            Name = string.Empty;
            ImageUrl = string.Empty;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public int TotalQuantity { get; set; }
    }
} 