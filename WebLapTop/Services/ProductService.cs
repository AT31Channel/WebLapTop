using Microsoft.EntityFrameworkCore;
using WebLapTop.Data;
using WebLapTop.Models.Entities;
using WebLapTop.Models.ViewModels;

namespace WebLapTop.Services
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ProductService> _logger;

        public ProductService(ApplicationDbContext context, ILogger<ProductService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<(List<Product> Products, int TotalPages)> GetProductsAsync(
            string? category = null,
            string? search = null,
            string? sort = null,
            int page = 1,
            int pageSize = 12)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .Where(p => p.IsVisible)
                .AsQueryable();

            // Filter by category
            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(p => p.Category != null && p.Category.Name == category);
            }

            // Search by name
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.Name.Contains(search));
            }

            // Sort
            query = sort switch
            {
                "price_asc" => query.OrderBy(p => p.Price),
                "price_desc" => query.OrderByDescending(p => p.Price),
                "featured" => query.Where(p => p.IsFeatured),
                _ => query.OrderByDescending(p => p.CreatedDate)
            };

            // Calculate total pages
            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            // Get paginated products
            var products = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (products, totalPages);
        }

        public async Task<List<Category>> GetCategoriesAsync()
        {
            return await _context.Categories
                .Where(c => c.IsVisible)
                .OrderBy(c => c.DisplayOrder)
                .ToListAsync();
        }

        public async Task<ProductDetailsViewModel?> GetProductDetailsAsync(int id)
        {
            try
            {
                var product = await _context.Products
                    .Include(p => p.Category)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (product == null)
                {
                    return null;
                }

                // Override image URL for specific products
                if (product.Name.Contains("Acer Nitro"))
                {
                    _logger.LogInformation($"Product ID: {product.Id}, Name: {product.Name}, ImageUrl: {product.ImageUrl}");
                    product.ImageUrl = "/images/products/acer-nitro.jpg";
                    _logger.LogInformation($"Đã ghi đè đường dẫn hình ảnh cho Acer Nitro: {product.ImageUrl}");
                }

                // Get related products
                var relatedProducts = await _context.Products
                    .Include(p => p.Category)
                    .Where(p => p.CategoryId == product.CategoryId && p.Id != product.Id && p.IsVisible)
                    .Take(4)
                    .ToListAsync();

                return new ProductDetailsViewModel
                {
                    Product = product,
                    RelatedProducts = relatedProducts
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting product details for ID: {ProductId}", id);
                return null;
            }
        }

        public async Task<int> GetCartCountAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return 0;
            }

            return await _context.Carts
                .Where(c => c.UserId == userId)
                .SumAsync(c => c.Quantity);
        }
    }
} 