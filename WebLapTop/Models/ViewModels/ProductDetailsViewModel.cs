using WebLapTop.Models.Entities;

namespace WebLapTop.Models.ViewModels
{
    public class ProductDetailsViewModel
    {
        public Product? Product { get; set; }
        public List<Product> RelatedProducts { get; set; } = new();
    }
}
