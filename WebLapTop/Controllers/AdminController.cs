using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebLapTop.Data;
using WebLapTop.Models.Entities;
using WebLapTop.Models.ViewModels;

namespace WebLapTop.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AdminController> _logger;

        public AdminController(
            ApplicationDbContext context, 
            UserManager<ApplicationUser> userManager,
            ILogger<AdminController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                // Kiểm tra xem người dùng hiện tại có phải là admin không
                if (!User.IsInRole("Admin"))
                {
                    _logger.LogWarning($"Unauthorized access attempt to admin dashboard by user {User.Identity?.Name}");
                    return RedirectToAction("AccessDenied", "Account");
                }

                // Đếm số thông báo chưa đọc
                ViewBag.UnreadNotifications = await _context.AdminNotifications
                    .Where(n => !n.IsRead)
                    .CountAsync();

                // Đơn hàng chờ xử lý
                ViewBag.PendingOrders = await _context.Orders
                    .Where(o => o.Status.ToLower() == "pending")
                    .CountAsync();

                // Tổng số sản phẩm
                ViewBag.TotalProducts = await _context.Products
                    .CountAsync();

                // Tổng số khách hàng (không bao gồm Admin)
                var adminRole = await _context.Roles
                    .Where(r => r.Name == "Admin")
                    .FirstOrDefaultAsync();
                
                if (adminRole != null)
                {
                    var adminUserIds = await _context.UserRoles
                        .Where(ur => ur.RoleId == adminRole.Id)
                        .Select(ur => ur.UserId)
                        .ToListAsync();
                    
                    ViewBag.TotalUsers = await _userManager.Users
                        .Where(u => !adminUserIds.Contains(u.Id))
                        .CountAsync();
                }
                else
                {
                    ViewBag.TotalUsers = await _userManager.Users.CountAsync();
                }

                // Doanh thu tháng này
                var firstDayOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                ViewBag.MonthlyRevenue = await _context.Orders
                    .Where(o => o.Status.ToLower() == "delivered" && o.OrderDate >= firstDayOfMonth)
                    .SumAsync(o => o.TotalAmount);

                // Thông báo gần đây
                var recentNotifications = await _context.AdminNotifications
                    .OrderByDescending(n => n.CreateDate)
                    .Take(5)
                    .ToListAsync();
                ViewBag.RecentNotifications = recentNotifications ?? new List<AdminNotification>();

                // Lấy danh sách sản phẩm mới nhất
                var products = await _context.Products
                    .Include(p => p.Category)
                    .OrderByDescending(p => p.CreatedDate)
                    .Take(5)
                    .ToListAsync();

                _logger.LogInformation("Loaded admin dashboard successfully");
                return View(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading admin dashboard");
                TempData["Error"] = "Có lỗi xảy ra khi tải trang tổng quan";
                ViewBag.RecentNotifications = new List<AdminNotification>(); // Ensure ViewBag.RecentNotifications is never null
                return View(new List<Product>());
            }
        }

        // QUẢN LÝ SẢN PHẨM
        public async Task<IActionResult> Products(int page = 1, int pageSize = 10)
        {
            if (!User.IsInRole("Admin"))
            {
                _logger.LogWarning($"Unauthorized access attempt to Products by user {User.Identity?.Name}");
                return RedirectToAction("AccessDenied", "Account");
            }

            ViewBag.UnreadNotifications = await GetUnreadNotificationsCountAsync();

            try
            {
                var totalProducts = await _context.Products.CountAsync();
                var totalPages = (int)Math.Ceiling(totalProducts / (double)pageSize);
                
                // Đảm bảo trang hợp lệ
                page = Math.Max(1, Math.Min(page, totalPages > 0 ? totalPages : 1));
                
                var products = await _context.Products
                    .Include(p => p.Category)
                    .OrderByDescending(p => p.CreatedDate)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var viewModel = new AdminProductsViewModel
                {
                    Products = products ?? new List<Product>(),
                    CurrentPage = page,
                    TotalPages = totalPages,
                    TotalItems = totalProducts,
                    PageSize = pageSize
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tải danh sách sản phẩm");
                TempData["Error"] = "Có lỗi xảy ra khi tải danh sách sản phẩm";
                
                return View(new AdminProductsViewModel
                {
                    Products = new List<Product>(),
                    CurrentPage = 1,
                    TotalPages = 1,
                    TotalItems = 0,
                    PageSize = pageSize
                });
            }
        }

        public async Task<IActionResult> CreateProduct()
        {
            if (!User.IsInRole("Admin"))
            {
                _logger.LogWarning($"Unauthorized access attempt to CreateProduct by user {User.Identity?.Name}");
                return RedirectToAction("AccessDenied", "Account");
            }

            ViewBag.UnreadNotifications = await GetUnreadNotificationsCountAsync();
            try 
            {
                ViewBag.Categories = await _context.Categories
                    .Where(c => c.IsVisible)
                    .OrderBy(c => c.DisplayOrder)
                    .ToListAsync() ?? new List<Category>();

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading categories for product creation");
                TempData["Error"] = "Có lỗi xảy ra khi tải danh mục sản phẩm";
                ViewBag.Categories = new List<Category>();
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProduct(Product product)
        {
            if (!User.IsInRole("Admin"))
            {
                _logger.LogWarning($"Unauthorized access attempt to CreateProduct POST by user {User.Identity?.Name}");
                return RedirectToAction("AccessDenied", "Account");
            }

            try
            {
                ViewBag.Categories = await _context.Categories.ToListAsync() ?? new List<Category>();

                if (ModelState.IsValid)
                {
                    product.CreatedDate = DateTime.Now;
                    _context.Products.Add(product);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"Product {product.Name} created successfully by {User.Identity?.Name}");
                    TempData["Success"] = "Thêm sản phẩm thành công!";
                    return RedirectToAction(nameof(Products));
                }
                return View(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating product: {product.Name}");
                TempData["Error"] = "Có lỗi xảy ra khi thêm sản phẩm";
                return View(product);
            }
        }

        public async Task<IActionResult> EditProduct(int id)
        {
            if (!User.IsInRole("Admin"))
            {
                _logger.LogWarning($"Unauthorized access attempt to EditProduct by user {User.Identity?.Name}");
                return RedirectToAction("AccessDenied", "Account");
            }

            ViewBag.UnreadNotifications = await GetUnreadNotificationsCountAsync();
            
            try
            {
                var product = await _context.Products.FindAsync(id);
                if (product == null)
                {
                    _logger.LogWarning($"Product with ID {id} not found during edit attempt");
                    return NotFound();
                }

                ViewBag.Categories = await _context.Categories
                    .OrderBy(c => c.DisplayOrder)
                    .ToListAsync() ?? new List<Category>();

                return View(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error loading product {id} for editing");
                TempData["Error"] = "Có lỗi xảy ra khi tải thông tin sản phẩm";
                return RedirectToAction(nameof(Products));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProduct(Product product)
        {
            if (!User.IsInRole("Admin"))
            {
                _logger.LogWarning($"Unauthorized access attempt to EditProduct POST by user {User.Identity?.Name}");
                return RedirectToAction("AccessDenied", "Account");
            }

            try
            {
                ViewBag.Categories = await _context.Categories.ToListAsync() ?? new List<Category>();

                if (ModelState.IsValid)
                {
                    var existingProduct = await _context.Products.FindAsync(product.Id);
                    if (existingProduct == null)
                    {
                        _logger.LogWarning($"Product with ID {product.Id} not found during update");
                        return NotFound();
                    }

                    // Cập nhật các trường
                    existingProduct.Name = product.Name;
                    existingProduct.Description = product.Description;
                    existingProduct.Price = product.Price;
                    existingProduct.DiscountPrice = product.DiscountPrice;
                    existingProduct.ImageUrl = product.ImageUrl;
                    existingProduct.CategoryId = product.CategoryId;
                    existingProduct.IsAvailable = product.IsAvailable;
                    existingProduct.IsFeatured = product.IsFeatured;
                    existingProduct.IsVisible = product.IsVisible;
                    existingProduct.UpdatedDate = DateTime.Now;

                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"Product {product.Name} updated successfully by {User.Identity?.Name}");
                    TempData["Success"] = "Cập nhật sản phẩm thành công!";
                    return RedirectToAction(nameof(Products));
                }
                return View(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating product: {product.Name}");
                TempData["Error"] = "Có lỗi xảy ra khi cập nhật sản phẩm";
                return View(product);
            }
        }

        public async Task<IActionResult> DeleteProduct(int id)
        {
            if (!User.IsInRole("Admin"))
            {
                _logger.LogWarning($"Unauthorized access attempt to DeleteProduct by user {User.Identity?.Name}");
                return RedirectToAction("AccessDenied", "Account");
            }

            ViewBag.UnreadNotifications = await GetUnreadNotificationsCountAsync();
            
            try
            {
                var product = await _context.Products
                    .Include(p => p.Category)
                    .FirstOrDefaultAsync(p => p.Id == id);
                    
                if (product == null)
                {
                    _logger.LogWarning($"Product with ID {id} not found during delete attempt");
                    return NotFound();
                }
                return View(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error loading product {id} for deletion");
                TempData["Error"] = "Có lỗi xảy ra khi tải thông tin sản phẩm";
                return RedirectToAction(nameof(Products));
            }
        }

        [HttpPost, ActionName("DeleteProduct")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProductConfirmed(int id)
        {
            if (!User.IsInRole("Admin"))
            {
                _logger.LogWarning($"Unauthorized access attempt to DeleteProduct POST by user {User.Identity?.Name}");
                return RedirectToAction("AccessDenied", "Account");
            }

            try
            {
                var product = await _context.Products.FindAsync(id);
                if (product != null)
                {
                    _context.Products.Remove(product);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Xóa sản phẩm thành công!";
                }
                return RedirectToAction(nameof(Products));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting product");
                TempData["Error"] = "Có lỗi xảy ra khi xóa sản phẩm";
                return RedirectToAction(nameof(Products));
            }
        }

        [HttpPost]
        public async Task<IActionResult> ToggleAvailability(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                product.IsAvailable = !product.IsAvailable;
                await _context.SaveChangesAsync();
                return Json(new { success = true, isAvailable = product.IsAvailable });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public async Task<IActionResult> ToggleFeatured(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                product.IsFeatured = !product.IsFeatured;
                await _context.SaveChangesAsync();
                return Json(new { success = true, isFeatured = product.IsFeatured });
            }
            return Json(new { success = false });
        }

        // QUẢN LÝ DANH MỤC
        public async Task<IActionResult> Categories()
        {
            ViewBag.UnreadNotifications = await GetUnreadNotificationsCountAsync();
            var categories = await _context.Categories.OrderBy(c => c.DisplayOrder).ToListAsync();
            return View(categories);
        }

        public async Task<IActionResult> CreateCategory()
        {
            ViewBag.UnreadNotifications = await GetUnreadNotificationsCountAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCategory(Category category)
        {
            if (ModelState.IsValid)
            {
                _context.Categories.Add(category);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Thêm danh mục thành công!";
                return RedirectToAction(nameof(Categories));
            }
            return View(category);
        }

        public async Task<IActionResult> EditCategory(int id)
        {
            ViewBag.UnreadNotifications = await GetUnreadNotificationsCountAsync();
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCategory(Category category)
        {
            if (ModelState.IsValid)
            {
                _context.Categories.Update(category);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Cập nhật danh mục thành công!";
                return RedirectToAction(nameof(Categories));
            }
            return View(category);
        }

        // QUẢN LÝ TÀI KHOẢN
        public async Task<IActionResult> Users()
        {
            try
            {
                var users = await _userManager.Users.ToListAsync();
                var userViewModels = new List<UserViewModel>();

                foreach (var user in users)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    userViewModels.Add(new UserViewModel
                    {
                        Id = user.Id,
                        UserName = user.UserName,
                        Email = user.Email,
                        PhoneNumber = user.PhoneNumber,
                        FullName = user.FullName,
                        Roles = roles.ToList()
                    });
                }

                return View(userViewModels);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while loading users list");
                TempData["Error"] = "Có lỗi xảy ra khi tải danh sách người dùng";
                return View(new List<UserViewModel>());
            }
        }

        public IActionResult CreateUser()
        {
            return View(new CreateUserViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser(CreateUserViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var user = new ApplicationUser
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    FullName = model.FullName,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    if (model.IsStaff)
                    {
                        await _userManager.AddToRoleAsync(user, "Staff");
                    }
                    else
                    {
                        await _userManager.AddToRoleAsync(user, "User");
                    }

                    TempData["Success"] = "Tạo tài khoản thành công";
                    return RedirectToAction(nameof(Users));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating new user");
                ModelState.AddModelError("", "Có lỗi xảy ra khi tạo tài khoản");
            }

            return View(model);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                TempData["Error"] = "Không tìm thấy người dùng";
                return RedirectToAction(nameof(Users));
            }

            var roles = await _userManager.GetRolesAsync(user);
            var model = new EditUserViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                FullName = user.FullName,
                IsStaff = roles.Contains("Staff")
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var user = await _userManager.FindByIdAsync(model.Id);
                if (user == null)
                {
                    TempData["Error"] = "Không tìm thấy người dùng";
                    return RedirectToAction(nameof(Users));
                }

                user.Email = model.Email;
                user.PhoneNumber = model.PhoneNumber;
                user.FullName = model.FullName;

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    var currentRoles = await _userManager.GetRolesAsync(user);
                    if (model.IsStaff && !currentRoles.Contains("Staff"))
                    {
                        await _userManager.RemoveFromRoleAsync(user, "User");
                        await _userManager.AddToRoleAsync(user, "Staff");
                    }
                    else if (!model.IsStaff && currentRoles.Contains("Staff"))
                    {
                        await _userManager.RemoveFromRoleAsync(user, "Staff");
                        await _userManager.AddToRoleAsync(user, "User");
                    }

                    TempData["Success"] = "Cập nhật thông tin thành công";
                    return RedirectToAction(nameof(Users));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating user");
                ModelState.AddModelError("", "Có lỗi xảy ra khi cập nhật thông tin");
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStaffRole(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    TempData["Error"] = "Không tìm thấy người dùng";
                    return RedirectToAction(nameof(Users));
                }

                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Contains("Admin"))
                {
                    TempData["Error"] = "Không thể thay đổi quyền của tài khoản Admin";
                    return RedirectToAction(nameof(Users));
                }

                if (roles.Contains("Staff"))
                {
                    await _userManager.RemoveFromRoleAsync(user, "Staff");
                    await _userManager.AddToRoleAsync(user, "User");
                    TempData["Success"] = "Đã hủy quyền nhân viên";
                }
                else
                {
                    await _userManager.RemoveFromRoleAsync(user, "User");
                    await _userManager.AddToRoleAsync(user, "Staff");
                    TempData["Success"] = "Đã cấp quyền nhân viên";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while toggling staff role");
                TempData["Error"] = "Có lỗi xảy ra khi thay đổi quyền";
            }

            return RedirectToAction(nameof(Users));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    TempData["Error"] = "Không tìm thấy người dùng";
                    return RedirectToAction(nameof(Users));
                }

                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Contains("Admin"))
                {
                    TempData["Error"] = "Không thể xóa tài khoản Admin";
                    return RedirectToAction(nameof(Users));
                }

                var result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    TempData["Success"] = "Xóa tài khoản thành công";
                }
                else
                {
                    TempData["Error"] = "Có lỗi xảy ra khi xóa tài khoản";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting user");
                TempData["Error"] = "Có lỗi xảy ra khi xóa tài khoản";
            }

            return RedirectToAction(nameof(Users));
        }

        // QUẢN LÝ ĐƠN HÀNG
        public async Task<IActionResult> Orders()
        {
            ViewBag.UnreadNotifications = await GetUnreadNotificationsCountAsync();

            var orders = await _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return View(orders);
        }

        public async Task<IActionResult> OrderDetails(int id)
        {
            ViewBag.UnreadNotifications = await GetUnreadNotificationsCountAsync();

            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateOrderStatus(int id, string status)
        {
            if (string.IsNullOrEmpty(status))
            {
                return BadRequest("Status cannot be empty");
            }

            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            var oldStatus = order.Status;
            order.Status = status;
            await _context.SaveChangesAsync();

            // Create notification
            var notification = new AdminNotification
            {
                Title = $"Đơn hàng #{id} đã được cập nhật",
                Message = $"Trạng thái đơn hàng đã thay đổi từ {GetStatusText(oldStatus)} sang {GetStatusText(status)}",
                CreateDate = DateTime.Now,
                OrderId = id,
                Type = "Order"
            };

            _context.AdminNotifications.Add(notification);
            await _context.SaveChangesAsync();

            // Create customer notification
            var customerNotification = new CustomerNotification
            {
                UserId = order.UserId,
                Title = $"Đơn hàng #{id} đã được cập nhật",
                Message = $"Trạng thái đơn hàng của bạn đã thay đổi từ {GetStatusText(oldStatus)} sang {GetStatusText(status)}",
                CreateDate = DateTime.Now,
                OrderId = id,
                Type = "Order"
            };

            _context.CustomerNotifications.Add(customerNotification);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Cập nhật trạng thái đơn hàng thành công!" });
        }

        private string GetStatusText(string status)
        {
            return status.ToLower() switch
            {
                "pending" => "Chờ xử lý",
                "confirmed" => "Đã xác nhận",
                "shipping" => "Đang giao hàng",
                "delivered" => "Đã giao hàng",
                "cancelled" => "Đã hủy",
                _ => status
            };
        }

        // QUẢN LÝ THÔNG BÁO
        public async Task<IActionResult> Notifications()
        {
            try
            {
                var notifications = await _context.AdminNotifications
                    .Include(n => n.Order)
                    .OrderByDescending(n => n.CreateDate)
                    .ToListAsync();

                // Đánh dấu tất cả thông báo đã đọc
                var unreadNotifications = await _context.AdminNotifications
                    .Where(n => !n.IsRead)
                    .ToListAsync();

                foreach (var notification in unreadNotifications)
                {
                    notification.IsRead = true;
                }
                await _context.SaveChangesAsync();

                // Cập nhật số thông báo chưa đọc
                ViewBag.UnreadNotifications = 0;

                return View(notifications);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tải thông báo");
                TempData["ErrorMessage"] = "Đã xảy ra lỗi khi tải thông báo. Vui lòng thử lại sau.";
                return View(new List<AdminNotification>());
            }
        }

        // BẢNG ĐIỀU KHIỂN
        public async Task<IActionResult> Dashboard()
        {
            ViewBag.UnreadNotifications = await GetUnreadNotificationsCountAsync();
            
            // Số lượng đơn hàng
            var totalOrders = await _context.Orders.CountAsync();
            var pendingOrders = await _context.Orders.CountAsync(o => o.Status.ToLower() == "pending");
            var confirmedOrders = await _context.Orders.CountAsync(o => o.Status.ToLower() == "confirmed");
            var shippingOrders = await _context.Orders.CountAsync(o => o.Status.ToLower() == "shipping");
            var deliveredOrders = await _context.Orders.CountAsync(o => o.Status.ToLower() == "delivered");
            var cancelledOrders = await _context.Orders.CountAsync(o => o.Status.ToLower() == "cancelled");
            
            // Tổng doanh thu
            var totalRevenue = await _context.Orders
                .Where(o => o.Status.ToLower() == "delivered")
                .SumAsync(o => o.TotalAmount);
            
            // Sản phẩm bán chạy
            var topProducts = await _context.OrderDetails
                .Include(od => od.Product)
                .GroupBy(od => od.ProductId)
                .Select(g => new
                {
                    ProductId = g.Key,
                    Product = g.First().Product,
                    TotalQuantity = g.Sum(od => od.Quantity)
                })
                .OrderByDescending(x => x.TotalQuantity)
                .Take(5)
                .ToListAsync();
            
            // Đơn hàng gần đây
            var recentOrders = await _context.Orders
                .OrderByDescending(o => o.OrderDate)
                .Take(5)
                .ToListAsync();
            
            // Số lượng khách hàng
            var totalUsers = await _userManager.Users.CountAsync();
            
            var dashboardViewModel = new DashboardViewModel
            {
                TotalOrders = totalOrders,
                PendingOrders = pendingOrders,
                ConfirmedOrders = confirmedOrders,
                ShippingOrders = shippingOrders,
                DeliveredOrders = deliveredOrders,
                CancelledOrders = cancelledOrders,
                TotalRevenue = totalRevenue,
                TopProducts = topProducts.Select(p => new TopProductViewModel 
                { 
                    Id = p.ProductId,
                    Name = p.Product.Name,
                    ImageUrl = p.Product.ImageUrl,
                    TotalQuantity = p.TotalQuantity
                }).ToList(),
                RecentOrders = recentOrders,
                TotalUsers = totalUsers
            };
            
            return View(dashboardViewModel);
        }

        // Helper method to get unread notifications count
        private async Task<int> GetUnreadNotificationsCountAsync()
        {
            try
            {
                return await _context.AdminNotifications
                    .Where(n => !n.IsRead)
                    .CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi đếm số thông báo chưa đọc");
                return 0;
            }
        }
    }
}
