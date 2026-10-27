using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebLapTop.Data;
using WebLapTop.Models.Entities;

namespace WebLapTop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CategoryController> _logger;

        public CategoryController(ApplicationDbContext context, ILogger<CategoryController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var categories = await _context.Categories
                    .OrderBy(c => c.DisplayOrder)
                    .ToListAsync();
                return View(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading categories");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải danh sách danh mục";
                return View(new List<Category>());
            }
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (await _context.Categories.AnyAsync(c => c.Name == category.Name))
                    {
                        ModelState.AddModelError("Name", "Tên danh mục đã tồn tại");
                        return View(category);
                    }

                    await _context.Categories.AddAsync(category);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Created new category: {CategoryName}", category.Name);
                    TempData["SuccessMessage"] = "Thêm danh mục thành công";
                    return RedirectToAction(nameof(Index));
                }
                return View(category);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating category");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi thêm danh mục";
                return View(category);
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var category = await _context.Categories.FindAsync(id);
                if (category == null)
                {
                    _logger.LogWarning("Category not found: {CategoryId}", id);
                    return NotFound();
                }
                return View(category);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading category for edit: {CategoryId}", id);
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải thông tin danh mục";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Category category)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (await _context.Categories.AnyAsync(c => c.Name == category.Name && c.Id != category.Id))
                    {
                        ModelState.AddModelError("Name", "Tên danh mục đã tồn tại");
                        return View(category);
                    }

                    _context.Categories.Update(category);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Updated category: {CategoryName}", category.Name);
                    TempData["SuccessMessage"] = "Cập nhật danh mục thành công";
                    return RedirectToAction(nameof(Index));
                }
                return View(category);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating category: {CategoryId}", category.Id);
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi cập nhật danh mục";
                return View(category);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var category = await _context.Categories
                    .Include(c => c.Products)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (category == null)
                {
                    _logger.LogWarning("Category not found for deletion: {CategoryId}", id);
                    return Json(new { success = false, message = "Danh mục không tồn tại" });
                }

                if (category.Products.Any())
                {
                    return Json(new { success = false, message = "Không thể xóa danh mục vì có sản phẩm liên quan" });
                }

                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Deleted category: {CategoryName}", category.Name);
                return Json(new { success = true, message = "Xóa danh mục thành công" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting category: {CategoryId}", id);
                return Json(new { success = false, message = "Có lỗi xảy ra khi xóa danh mục" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateDisplayOrder(int id, int newOrder)
        {
            try
            {
                var category = await _context.Categories.FindAsync(id);
                if (category == null)
                {
                    _logger.LogWarning("Category not found for display order update: {CategoryId}", id);
                    return Json(new { success = false, message = "Danh mục không tồn tại" });
                }

                category.DisplayOrder = newOrder;
                await _context.SaveChangesAsync();
                _logger.LogInformation("Updated display order for category: {CategoryName} to {NewOrder}", category.Name, newOrder);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating display order for category: {CategoryId}", id);
                return Json(new { success = false, message = "Có lỗi xảy ra khi cập nhật thứ tự hiển thị" });
            }
        }
    }
} 