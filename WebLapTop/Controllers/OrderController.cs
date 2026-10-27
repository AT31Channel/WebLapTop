using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WebLapTop.Data;
using WebLapTop.Models;
using WebLapTop.Models.Entities;

namespace WebLapTop.Controllers
{
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<OrderController> _logger;

        public OrderController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, ILogger<OrderController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        [Authorize]
        public async Task<IActionResult> Details(int id)
        {
            var userId = _userManager.GetUserId(User);
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                        .ThenInclude(p => p.Category)
                .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Cancel(int id)
        {
            var userId = _userManager.GetUserId(User);
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);

            if (order == null)
            {
                return Json(new { success = false, message = "Không tìm thấy đơn hàng" });
            }

            if (order.Status.ToLower() != "pending")
            {
                return Json(new { success = false, message = "Chỉ có thể hủy đơn hàng đang chờ xử lý" });
            }

            try
            {
                // Hoàn trả số lượng sản phẩm vào kho
                foreach (var item in order.OrderDetails)
                {
                    item.Product.Stock += item.Quantity;
                }

                order.Status = "Cancelled";
                await _context.SaveChangesAsync();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi hủy đơn hàng {OrderId}", id);
                return Json(new { success = false, message = "Có lỗi xảy ra khi hủy đơn hàng" });
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Review([FromBody] ReviewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Dữ liệu không hợp lệ" });
            }

            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return Json(new { success = false, message = "Không tìm thấy thông tin người dùng" });
            }

            var orderDetail = await _context.OrderDetails
                .Include(od => od.Order)
                .Include(od => od.Product)
                .FirstOrDefaultAsync(od => od.Id == model.OrderDetailId && od.Order.UserId == userId);

            if (orderDetail == null || orderDetail.Order == null)
            {
                return Json(new { success = false, message = "Không tìm thấy sản phẩm trong đơn hàng" });
            }

            if (orderDetail.Order.Status?.ToLower() != "delivered")
            {
                return Json(new { success = false, message = "Chỉ có thể đánh giá sản phẩm khi đơn hàng đã giao thành công" });
            }

            if (orderDetail.IsReviewed)
            {
                return Json(new { success = false, message = "Bạn đã đánh giá sản phẩm này" });
            }

            try
            {
                var review = new Review
                {
                    ProductId = orderDetail.ProductId,
                    UserId = userId,
                    Rating = model.Rating,
                    Comment = model.Comment,
                    CreatedDate = DateTime.Now
                };

                orderDetail.IsReviewed = true;
                _context.Reviews.Add(review);
                await _context.SaveChangesAsync();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi thêm đánh giá cho sản phẩm {ProductId}", orderDetail.ProductId);
                return Json(new { success = false, message = "Có lỗi xảy ra khi gửi đánh giá" });
            }
        }

        public class ReviewModel
        {
            public int OrderDetailId { get; set; }
            
            [Range(1, 5)]
            public int Rating { get; set; }
            
            [Required]
            [StringLength(1000)]
            public string Comment { get; set; } = string.Empty;
        }
    }
} 