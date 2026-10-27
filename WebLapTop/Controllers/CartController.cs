using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text;
using WebLapTop.Data;
using WebLapTop.Models.Entities;
using WebLapTop.Models.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace WebLapTop.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CartController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private const string LogTemplate = "Cart operation: {Operation}";

        public CartController(
            ApplicationDbContext context, 
            ILogger<CartController> logger,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
        }

        private async Task SetCartCountAsync()
        {
            try
            {
                if (User.Identity?.IsAuthenticated == true)
                {
                    var user = await _userManager.GetUserAsync(User);
                    if (user != null)
                    {
                        var cartCount = await _context.Carts
                            .Where(c => c.UserId == user.Id)
                            .SumAsync(c => c.Quantity);
                        ViewBag.CartCount = cartCount;
                    }
                    else
                    {
                        ViewBag.CartCount = 0;
                    }
                }
                else
                {
                    ViewBag.CartCount = 0;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting cart count");
                ViewBag.CartCount = 0;
            }
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                if (!User.Identity?.IsAuthenticated == true)
                {
                    return RedirectToAction("Login", "Account");
                }

                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    _logger.LogWarning("User is authenticated but UserManager returned null");
                    return RedirectToAction("Login", "Account");
                }

                var cartItems = await _context.Carts
                    .Include(c => c.Product)
                    .Where(c => c.UserId == user.Id)
                    .ToListAsync();

                await SetCartCountAsync();
                return View(cartItems ?? new List<Cart>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading cart items");
                TempData["Error"] = "Có lỗi xảy ra khi tải giỏ hàng.";
                return View(new List<Cart>());
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartRequest request)
        {
            try
            {
                if (!User.Identity?.IsAuthenticated == true)
                {
                    return Json(new { success = false, message = "Vui lòng đăng nhập để thêm sản phẩm vào giỏ hàng" });
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Json(new { success = false, message = "Không tìm thấy thông tin người dùng" });
                }

                var product = await _context.Products
                    .FirstOrDefaultAsync(p => p.Id == request.ProductId && p.IsVisible);

                if (product == null)
                {
                    return Json(new { success = false, message = "Sản phẩm không tồn tại hoặc đã bị ẩn" });
                }

                if (request.Quantity <= 0 || request.Quantity > product.Stock)
                {
                    return Json(new { success = false, message = "Số lượng không hợp lệ" });
                }

                var cartItem = await _context.Carts
                    .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == request.ProductId);

                if (cartItem != null)
                {
                    cartItem.Quantity += request.Quantity;
                    if (cartItem.Quantity > product.Stock)
                    {
                        return Json(new { success = false, message = "Số lượng vượt quá tồn kho" });
                    }
                }
                else
                {
                    cartItem = new Cart
                    {
                        UserId = userId,
                        ProductId = request.ProductId,
                        Quantity = request.Quantity,
                        UnitPrice = product.DiscountPrice ?? product.Price
                    };
                    _context.Carts.Add(cartItem);
                }

                await _context.SaveChangesAsync();
                await SetCartCountAsync();

                return Json(new { 
                    success = true, 
                    message = "Đã thêm sản phẩm vào giỏ hàng",
                    cartCount = ViewBag.CartCount
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi thêm sản phẩm vào giỏ hàng");
                return Json(new { success = false, message = "Có lỗi xảy ra khi thêm sản phẩm vào giỏ hàng" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateQuantity([FromBody] UpdateQuantityRequest request)
        {
            try
            {
                if (!User.Identity?.IsAuthenticated == true)
                {
                    return Json(new { success = false, message = "Vui lòng đăng nhập để cập nhật giỏ hàng" });
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Json(new { success = false, message = "Không tìm thấy thông tin người dùng" });
                }

                var cartItem = await _context.Carts
                    .Include(c => c.Product)
                    .FirstOrDefaultAsync(c => c.Id == request.Id && c.UserId == userId);

                if (cartItem == null || cartItem.Product == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy sản phẩm trong giỏ hàng" });
                }

                if (request.Quantity <= 0 || request.Quantity > cartItem.Product.Stock)
                {
                    return Json(new { success = false, message = "Số lượng không hợp lệ" });
                }

                cartItem.Quantity = request.Quantity;
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Đã cập nhật số lượng" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật số lượng sản phẩm");
                return Json(new { success = false, message = "Có lỗi xảy ra khi cập nhật số lượng" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromCart([FromBody] RemoveFromCartRequest request)
        {
            try
            {
                if (!User.Identity?.IsAuthenticated == true)
                {
                    return Json(new { success = false, message = "Vui lòng đăng nhập để xóa sản phẩm khỏi giỏ hàng" });
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Json(new { success = false, message = "Không tìm thấy thông tin người dùng" });
                }

                var cartItem = await _context.Carts
                    .FirstOrDefaultAsync(c => c.Id == request.Id && c.UserId == userId);

                if (cartItem == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy sản phẩm trong giỏ hàng" });
                }

                _context.Carts.Remove(cartItem);
                await _context.SaveChangesAsync();
                await SetCartCountAsync();

                return Json(new { success = true, message = "Đã xóa sản phẩm khỏi giỏ hàng" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa sản phẩm khỏi giỏ hàng");
                return Json(new { success = false, message = "Có lỗi xảy ra khi xóa sản phẩm khỏi giỏ hàng" });
            }
        }

        [Authorize]
        public async Task<IActionResult> Checkout()
        {
            await SetCartCountAsync();

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var cartItems = await _context.Carts
                .Include(c => c.Product)
                .Where(c => c.UserId == user.Id)
                .ToListAsync();

            if (!cartItems.Any())
            {
                TempData["Error"] = "Giỏ hàng của bạn đang trống. Vui lòng thêm sản phẩm vào giỏ hàng trước khi thanh toán.";
                return RedirectToAction(nameof(Index));
            }

            var model = new CheckoutViewModel
            {
                CartItems = cartItems,
                TotalAmount = cartItems.Sum(item => item.Quantity * (item.Product?.DiscountPrice ?? item.Product?.Price ?? 0)),
                FullName = user.HoTen,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                ShippingAddress = user.DiaChi
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(CheckoutViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                _logger.LogError("Không tìm thấy thông tin người dùng khi thanh toán");
                TempData["Error"] = "Vui lòng đăng nhập lại để tiếp tục.";
                return RedirectToAction("Login", "Account");
            }

            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Model không hợp lệ khi checkout: {errors}", 
                        string.Join(", ", ModelState.Values
                            .SelectMany(v => v.Errors)
                            .Select(e => e.ErrorMessage)));
                            
                    var cart = await _context.Carts
                        .Include(c => c.Product)
                        .Where(c => c.UserId == user.Id)
                        .ToListAsync();
                    
                    model.CartItems = cart;
                    TempData["Error"] = "Vui lòng kiểm tra lại thông tin đặt hàng.";
                    return View(model);
                }

                // Kiểm tra giỏ hàng
                var cartItems = await _context.Carts
                    .Include(c => c.Product)
                    .Where(c => c.UserId == user.Id)
                    .ToListAsync();

                if (!cartItems.Any())
                {
                    _logger.LogWarning("Giỏ hàng trống khi thanh toán");
                    TempData["Error"] = "Giỏ hàng của bạn đang trống.";
                    return RedirectToAction(nameof(Index));
                }

                // Kiểm tra số lượng tồn kho
                foreach (var item in cartItems)
                {
                    if (item.Product == null || item.Product.Stock < item.Quantity)
                    {
                        _logger.LogWarning("Sản phẩm {ProductId} không đủ số lượng trong kho", item.ProductId);
                        TempData["Error"] = $"Sản phẩm {item.Product?.Name} không đủ số lượng trong kho.";
                        return RedirectToAction(nameof(Index));
                    }
                }

                // Bắt đầu transaction
                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        _logger.LogInformation("Bắt đầu xử lý đơn hàng cho user {UserId}", user.Id);
                        
                        // Tạo đơn hàng mới
                        var order = new Order
                        {
                            UserId = user.Id,
                            OrderDate = DateTime.Now,
                            FullName = model.FullName,
                            Email = model.Email,
                            PhoneNumber = model.PhoneNumber,
                            ShippingAddress = model.ShippingAddress,
                            Note = model.Note,
                            Status = "Pending",
                            TotalAmount = cartItems.Sum(item => item.Quantity * (item.Product?.DiscountPrice ?? item.Product?.Price ?? 0)),
                            PaymentMethod = model.PaymentMethod
                        };

                        _context.Orders.Add(order);
                        await _context.SaveChangesAsync();
                        _logger.LogInformation("Đã tạo đơn hàng {OrderId}", order.Id);

                        // Tạo chi tiết đơn hàng và cập nhật số lượng tồn kho
                        foreach (var item in cartItems)
                        {
                            if (item.Product != null)
                            {
                                var orderDetail = new OrderDetail
                                {
                                    OrderId = order.Id,
                                    ProductId = item.ProductId,
                                    Quantity = item.Quantity,
                                    UnitPrice = item.Product.DiscountPrice ?? item.Product.Price
                                };

                                _context.OrderDetails.Add(orderDetail);

                                // Cập nhật số lượng tồn kho
                                item.Product.Stock -= item.Quantity;
                                _context.Entry(item.Product).State = EntityState.Modified;
                            }
                        }
                        await _context.SaveChangesAsync();
                        _logger.LogInformation("Đã lưu chi tiết đơn hàng và cập nhật tồn kho cho đơn hàng {OrderId}", order.Id);

                        // Tạo thông báo cho admin
                        var adminNotification = new AdminNotification
                        {
                            Title = "Đơn hàng mới",
                            Message = $"Đơn hàng mới #{order.Id} từ {user.HoTen} với tổng giá trị {order.TotalAmount:N0}đ",
                            OrderId = order.Id,
                            CreateDate = DateTime.Now,
                            IsRead = false,
                            Type = "Order"
                        };
                        _context.AdminNotifications.Add(adminNotification);
                        await _context.SaveChangesAsync();
                        _logger.LogInformation("Đã tạo thông báo admin cho đơn hàng {OrderId}", order.Id);

                        // Tạo thông báo cho khách hàng
                        var customerNotification = new CustomerNotification
                        {
                            UserId = user.Id,
                            Title = "Đơn hàng mới",
                            OrderId = order.Id,
                            Message = $"Đơn hàng #{order.Id} của bạn đã được tạo thành công và đang chờ xử lý",
                            CreateDate = DateTime.Now,
                            IsRead = false,
                            Type = "Order",
                            Link = Url.Action("OrderDetails", "Cart", new { id = order.Id }, Request.Scheme)
                        };
                        _context.CustomerNotifications.Add(customerNotification);
                        await _context.SaveChangesAsync();
                        _logger.LogInformation("Đã tạo thông báo khách hàng cho đơn hàng {OrderId}", order.Id);

                        // Xóa giỏ hàng
                        _context.Carts.RemoveRange(cartItems);
                        await _context.SaveChangesAsync();
                        _logger.LogInformation("Đã xóa giỏ hàng sau khi đặt hàng thành công");

                        // Commit transaction
                        await transaction.CommitAsync();
                        _logger.LogInformation("Đã hoàn tất đơn hàng {OrderId}", order.Id);

                        TempData["Success"] = $"Đặt hàng thành công! Mã đơn hàng của bạn là #{order.Id}";
                        return RedirectToAction(nameof(OrderConfirmation), new { id = order.Id });
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        _logger.LogError(ex, "Lỗi khi xử lý đơn hàng");
                        ModelState.AddModelError(string.Empty, "Có lỗi xảy ra khi xử lý đơn hàng. Vui lòng thử lại sau.");
                        
                        model.CartItems = cartItems;
                        return View(model);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi không mong muốn khi xử lý đơn hàng");
                ModelState.AddModelError(string.Empty, "Có lỗi xảy ra. Vui lòng thử lại sau.");
                return View(model);
            }
        }

        [Authorize]
        public async Task<IActionResult> OrderConfirmation(int id)
        {
            try
            {
                _logger.LogInformation("Đang hiển thị trang xác nhận đơn hàng ID: {OrderId}", id);
                
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    _logger.LogWarning("Người dùng không tìm thấy khi hiển thị OrderConfirmation");
                    return RedirectToAction("Login", "Account");
                }

                var order = await _context.Orders
                    .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                    .FirstOrDefaultAsync(o => o.Id == id && o.UserId == user.Id);

                if (order == null)
                {
                    _logger.LogWarning("Không tìm thấy đơn hàng {OrderId} cho người dùng {UserId}", id, user.Id);
                    TempData["Error"] = "Không tìm thấy đơn hàng. Vui lòng kiểm tra lại.";
                    return RedirectToAction("OrderHistory");
                }

                // Chuyển thông báo từ OrderSuccess sang Success nếu có
                if (TempData["OrderSuccess"] != null)
                {
                    TempData["Success"] = TempData["OrderSuccess"];
                }
                else if (TempData["Success"] == null)
                {
                    TempData["Success"] = $"Đơn hàng #{id} đã được đặt thành công và đang chờ xử lý!";
                }

                // Kiểm tra đã có thông báo đơn hàng cho admin hay chưa
                var adminNotification = await _context.AdminNotifications
                    .FirstOrDefaultAsync(n => n.OrderId == id && n.Type == "Order");
                
                // Nếu chưa có thông báo, tạo thông báo cho admin
                if (adminNotification == null)
                {
                    _logger.LogWarning("Không tìm thấy thông báo admin cho đơn hàng {OrderId}, tạo thông báo mới", id);
                    
                    adminNotification = new AdminNotification
                    {
                        Title = "Đơn hàng mới",
                        Message = $"Đơn hàng mới #{order.Id} từ {user.HoTen} với tổng giá trị {order.TotalAmount:N0}đ",
                        OrderId = order.Id,
                        CreateDate = DateTime.Now,
                        IsRead = false,
                        Type = "Order"
                    };
                    _context.AdminNotifications.Add(adminNotification);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Đã tạo thông báo admin mới cho đơn hàng {OrderId}", id);
                }

                // Kiểm tra đã có thông báo đơn hàng cho khách hàng hay chưa
                var customerNotification = await _context.CustomerNotifications
                    .FirstOrDefaultAsync(n => n.OrderId == id && n.UserId == user.Id && n.Type == "Order");
                
                // Nếu chưa có thông báo, tạo thông báo cho khách hàng
                if (customerNotification == null)
                {
                    _logger.LogWarning("Không tìm thấy thông báo khách hàng cho đơn hàng {OrderId}, tạo thông báo mới", id);
                    
                    customerNotification = new CustomerNotification
                    {
                        UserId = user.Id,
                        Title = "Đơn hàng mới",
                        OrderId = order.Id,
                        Message = $"Đơn hàng #{order.Id} của bạn đã được tạo thành công và đang chờ xử lý",
                        CreateDate = DateTime.Now,
                        IsRead = false,
                        Type = "Order",
                        Link = Url.Action("OrderDetails", "Cart", new { id = order.Id }, Request.Scheme)
                    };
                    _context.CustomerNotifications.Add(customerNotification);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Đã tạo thông báo khách hàng mới cho đơn hàng {OrderId}", id);
                }

                return View(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi hiển thị trang xác nhận đơn hàng ID: {OrderId}", id);
                TempData["Error"] = "Có lỗi xảy ra khi hiển thị thông tin đơn hàng.";
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize]
        public async Task<IActionResult> OrderHistory()
        {
            await SetCartCountAsync();

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var orders = await _context.Orders
                .Where(o => o.UserId == user.Id)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return View(orders);
        }

        [Authorize]
        public async Task<IActionResult> OrderDetails(int id)
        {
            await SetCartCountAsync();

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(o => o.Id == id && o.UserId == user.Id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelOrder([FromBody] CancelOrderRequest request)
        {
            try
            {
                if (User?.Identity?.IsAuthenticated != true)
                {
                    return Json(new { success = false, message = "Bạn cần đăng nhập để thực hiện hành động này." });
                }

                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy thông tin người dùng." });
                }

                var order = await _context.Orders
                    .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                    .FirstOrDefaultAsync(o => o.Id == request.OrderId && o.UserId == user.Id);

                if (order == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy đơn hàng." });
                }

                // Kiểm tra xem đơn hàng có thể hủy không (chỉ cho phép hủy đơn hàng ở trạng thái Chờ xác nhận)
                if (order.Status.ToLower() != "pending")
                {
                    return Json(new { success = false, message = "Không thể hủy đơn hàng ở trạng thái hiện tại." });
                }

                // Hoàn trả số lượng sản phẩm vào kho
                foreach (var item in order.OrderDetails)
                {
                    if (item.Product != null)
                    {
                        item.Product.Stock += item.Quantity;
                    }
                }

                // Cập nhật trạng thái đơn hàng
                order.Status = "Cancelled";
                await _context.SaveChangesAsync();

                // Gửi thông báo cho khách hàng
                var notification = new CustomerNotification
                {
                    UserId = user.Id,
                    Title = "Đơn hàng đã bị hủy",
                    Message = $"Đơn hàng #{order.Id} đã bị hủy theo yêu cầu của bạn.",
                    OrderId = order.Id,
                    Link = Url.Action("OrderDetails", "Cart", new { id = order.Id }, Request.Scheme),
                    Type = "Order"
                };

                _context.CustomerNotifications.Add(notification);

                // Gửi thông báo cho admin
                var adminNotification = new AdminNotification
                {
                    Title = "Đơn hàng bị hủy",
                    Message = $"Đơn hàng #{order.Id} đã bị hủy bởi khách hàng {user.UserName}.",
                    OrderId = order.Id,
                    IsRead = false,
                    Type = "Order"
                };

                _context.AdminNotifications.Add(adminNotification);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Đơn hàng đã được hủy thành công." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi hủy đơn hàng {OrderId}", request.OrderId);
                return Json(new { success = false, message = "Có lỗi xảy ra khi hủy đơn hàng. Vui lòng thử lại sau." });
            }
        }

        public class AddToCartRequest
        {
            public int ProductId { get; set; }
            public int Quantity { get; set; }
        }

        public class UpdateQuantityRequest
        {
            public int Id { get; set; }
            public int Quantity { get; set; }
        }

        public class RemoveFromCartRequest
        {
            public int Id { get; set; }
        }

        public class CancelOrderRequest
        {
            public int OrderId { get; set; }
        }
    }
}