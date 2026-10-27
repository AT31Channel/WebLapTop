using WebLapTop.Models.Entities;

namespace WebLapTop.Models.ViewModels
{
    public class HomeViewModel
    {
        public List<Product> Products { get; set; } = new();
        public List<Category> Categories { get; set; } = new();
        public string? CurrentCategory { get; set; }
        public string? SearchTerm { get; set; }
        public int? MinPrice { get; set; }
        public int? MaxPrice { get; set; }
        public string? SortOrder { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}
