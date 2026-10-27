using WebLapTop.Models.Entities;
using WebLapTop.Models.ViewModels;

namespace WebLapTop.Services
{
    public interface IProductService
    {
        Task<(List<Product> Products, int TotalPages)> GetProductsAsync(
            string? category = null,
            string? search = null,
            string? sort = null,
            int page = 1,
            int pageSize = 12);

        Task<List<Category>> GetCategoriesAsync();
        
        Task<ProductDetailsViewModel?> GetProductDetailsAsync(int id);
        
        Task<int> GetCartCountAsync(string userId);
    }
} 