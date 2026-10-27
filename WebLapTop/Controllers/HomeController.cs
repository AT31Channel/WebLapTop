using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebLapTop.Models;
using WebLapTop.Services;

namespace WebLapTop.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductService _productService;

        public HomeController(ILogger<HomeController> logger, IProductService productService)
        {
            _logger = logger;
            _productService = productService;
        }

        private async Task SetCartCountAsync()
        {
            if (User.Identity?.IsAuthenticated == true && User.Identity.Name != null)
            {
                ViewBag.CartCount = await _productService.GetCartCountAsync(User.Identity.Name);
            }
            else
            {
                ViewBag.CartCount = 0;
            }
        }

        public async Task<IActionResult> Index(string? category = null, string? search = null, string? sort = null, int page = 1)
        {
            await SetCartCountAsync();

            var (products, totalPages) = await _productService.GetProductsAsync(category, search, sort, page);

            // Set ViewBag data
            ViewBag.CurrentCategory = category ?? string.Empty;
            ViewBag.CurrentSearch = search ?? string.Empty;
            ViewBag.CurrentSort = sort ?? string.Empty;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.Categories = await _productService.GetCategoriesAsync();

            return View(products);
        }

        public async Task<IActionResult> Details(int id)
        {
            await SetCartCountAsync();

            var viewModel = await _productService.GetProductDetailsAsync(id);
            if (viewModel == null)
            {
                return NotFound();
            }

            // Store related products in ViewBag
            ViewBag.RelatedProducts = viewModel.RelatedProducts;

            // Return the Product model instead of the ViewModel
            return View(viewModel.Product);
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
