using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebLapTop.Data;
using WebLapTop.Models.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Logging;
using System;

namespace WebLapTop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<ProductController> _logger;

        public ProductController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment, ILogger<ProductController> logger)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
        }

        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            try
            {
                var totalProducts = await _context.Products.CountAsync();
                var totalPages = (int)Math.Ceiling(totalProducts / (double)pageSize);
                page = Math.Max(1, Math.Min(page, totalPages));

                var products = await _context.Products
                    .Include(p => p.Category)
                    .OrderByDescending(p => p.CreatedDate)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                ViewBag.CurrentPage = page;
                ViewBag.TotalPages = totalPages;
                ViewBag.PageSize = pageSize;

                return View(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while loading products");
                TempData["ErrorMessage"] = "Đã xảy ra lỗi khi tải danh sách sản phẩm";
                return View(new List<Product>());
            }
        }

        public async Task<IActionResult> Create()
        {
            try
            {
                ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name");
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while loading create product form");
                TempData["ErrorMessage"] = "Đã xảy ra lỗi khi tải form tạo sản phẩm";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, IFormFile imageFile)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Kiểm tra tên sản phẩm đã tồn tại (case-insensitive)
                    if (await _context.Products.AnyAsync(p => string.Equals(p.Name, product.Name, StringComparison.OrdinalIgnoreCase)))
                    {
                        ModelState.AddModelError("Name", "Tên sản phẩm đã tồn tại");
                        ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name");
                        return View(product);
                    }

                    // Xử lý ảnh
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "products");
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        var uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(fileStream);
                        }

                        product.ImageUrl = "/images/products/" + uniqueFileName;
                    }

                    product.CreatedDate = DateTime.Now;
                    product.UpdatedDate = DateTime.Now;

                    _context.Add(product);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Product created successfully: {ProductId}", product.Id);
                    TempData["SuccessMessage"] = "Thêm sản phẩm thành công";
                    return RedirectToAction(nameof(Index));
                }

                ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name");
                return View(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating product");
                TempData["ErrorMessage"] = "Đã xảy ra lỗi khi thêm sản phẩm";
                ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name");
                return View(product);
            }
        }

        public async Task<IActionResult> Edit(int? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                var product = await _context.Products.FindAsync(id);
                if (product == null)
                {
                    return NotFound();
                }

                ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name");
                return View(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while loading edit product form");
                TempData["ErrorMessage"] = "Đã xảy ra lỗi khi tải form chỉnh sửa sản phẩm";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product, IFormFile imageFile)
        {
            try
            {
                if (id != product.Id)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    // Kiểm tra tên sản phẩm đã tồn tại (trừ sản phẩm hiện tại, case-insensitive)
                    if (await _context.Products.AnyAsync(p => string.Equals(p.Name, product.Name, StringComparison.OrdinalIgnoreCase) && p.Id != id))
                    {
                        ModelState.AddModelError("Name", "Tên sản phẩm đã tồn tại");
                        ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name");
                        return View(product);
                    }

                    var existingProduct = await _context.Products.FindAsync(id);
                    if (existingProduct == null)
                    {
                        return NotFound();
                    }

                    // Xử lý ảnh
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "products");
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        // Xóa ảnh cũ nếu có
                        if (!string.IsNullOrEmpty(existingProduct.ImageUrl))
                        {
                            var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, existingProduct.ImageUrl.TrimStart('/'));
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }

                        var uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(fileStream);
                        }

                        existingProduct.ImageUrl = "/images/products/" + uniqueFileName;
                    }

                    // Cập nhật thông tin sản phẩm
                    existingProduct.Name = product.Name;
                    existingProduct.Description = product.Description;
                    existingProduct.Price = product.Price;
                    existingProduct.Stock = product.Stock;
                    existingProduct.CategoryId = product.CategoryId;
                    existingProduct.UpdatedDate = DateTime.Now;

                    _context.Update(existingProduct);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Product updated successfully: {ProductId}", id);
                    TempData["SuccessMessage"] = "Cập nhật sản phẩm thành công";
                    return RedirectToAction(nameof(Index));
                }

                ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name");
                return View(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating product");
                TempData["ErrorMessage"] = "Đã xảy ra lỗi khi cập nhật sản phẩm";
                ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name");
                return View(product);
            }
        }

        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                var product = await _context.Products
                    .Include(p => p.Category)
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (product == null)
                {
                    return NotFound();
                }

                return View(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while loading delete product form");
                TempData["ErrorMessage"] = "Đã xảy ra lỗi khi tải form xóa sản phẩm";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var product = await _context.Products
                    .Include(p => p.OrderDetails)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (product == null)
                {
                    return NotFound();
                }

                // Kiểm tra xem sản phẩm có trong đơn hàng nào không
                if (product.OrderDetails.Any())
                {
                    TempData["ErrorMessage"] = "Không thể xóa sản phẩm vì đã có trong đơn hàng";
                    return RedirectToAction(nameof(Index));
                }

                // Xóa ảnh sản phẩm
                if (!string.IsNullOrEmpty(product.ImageUrl))
                {
                    var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, product.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }

                _context.Products.Remove(product);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Product deleted successfully: {ProductId}", id);
                TempData["SuccessMessage"] = "Xóa sản phẩm thành công";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting product");
                TempData["ErrorMessage"] = "Đã xảy ra lỗi khi xóa sản phẩm";
                return RedirectToAction(nameof(Index));
            }
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
} 