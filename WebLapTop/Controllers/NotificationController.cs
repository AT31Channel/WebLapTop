using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebLapTop.Data;
using WebLapTop.Models.Entities;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace WebLapTop.Controllers
{
    [Authorize]
    public class NotificationController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<NotificationController> _logger;

        public NotificationController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            ILogger<NotificationController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        // GET: /Notification
        public async Task<IActionResult> Index()
        {
            try
            {
                var userId = _userManager.GetUserId(User);
                if (string.IsNullOrEmpty(userId))
                {
                    return RedirectToAction("Login", "Account");
                }

                var notifications = await _context.CustomerNotifications
                    .Where(n => n.UserId == userId)
                    .OrderByDescending(n => n.CreateDate)
                    .ToListAsync();

                // Đếm số thông báo chưa đọc
                ViewBag.UnreadCount = notifications.Count(n => !n.IsRead);

                return View(notifications);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving notifications for user");
                TempData["ErrorMessage"] = "Đã xảy ra lỗi khi tải thông báo. Vui lòng thử lại sau.";
                return View(new List<CustomerNotification>());
            }
        }

        // GET: /Notification/Details/5
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var userId = _userManager.GetUserId(User);
                if (string.IsNullOrEmpty(userId))
                {
                    return RedirectToAction("Login", "Account");
                }

                var notification = await _context.CustomerNotifications
                    .FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);

                if (notification == null)
                {
                    _logger.LogWarning("Notification with ID {NotificationId} not found for user {UserId}", id, userId);
                    TempData["ErrorMessage"] = "Không tìm thấy thông báo này.";
                    return RedirectToAction(nameof(Index));
                }

                // Đánh dấu thông báo đã đọc
                if (!notification.IsRead)
                {
                    notification.IsRead = true;
                    _context.Update(notification);
                    await _context.SaveChangesAsync();
                }

                // Nếu thông báo có liên kết đến đơn hàng
                if (notification.OrderId.HasValue && !string.IsNullOrEmpty(notification.Link))
                {
                    return Redirect(notification.Link);
                }

                return View(notification);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving notification details for ID {NotificationId}", id);
                TempData["ErrorMessage"] = "Đã xảy ra lỗi khi tải chi tiết thông báo. Vui lòng thử lại sau.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: /Notification/MarkAsRead/5
        [HttpPost]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            try
            {
                var userId = _userManager.GetUserId(User);
                if (string.IsNullOrEmpty(userId))
                {
                    return Json(new { success = false, message = "Bạn cần đăng nhập để thực hiện hành động này." });
                }

                var notification = await _context.CustomerNotifications
                    .FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);

                if (notification == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy thông báo." });
                }

                notification.IsRead = true;
                _context.Update(notification);
                await _context.SaveChangesAsync();

                var unreadCount = await _context.CustomerNotifications
                    .CountAsync(n => n.UserId == userId && !n.IsRead);

                return Json(new { success = true, unreadCount = unreadCount });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking notification {NotificationId} as read", id);
                return Json(new { success = false, message = "Đã xảy ra lỗi. Vui lòng thử lại sau." });
            }
        }

        // POST: /Notification/MarkAllAsRead
        [HttpPost]
        public async Task<IActionResult> MarkAllAsRead()
        {
            try
            {
                var userId = _userManager.GetUserId(User);
                if (string.IsNullOrEmpty(userId))
                {
                    return Json(new { success = false, message = "Bạn cần đăng nhập để thực hiện hành động này." });
                }

                var unreadNotifications = await _context.CustomerNotifications
                    .Where(n => n.UserId == userId && !n.IsRead)
                    .ToListAsync();

                if (unreadNotifications.Count == 0)
                {
                    return Json(new { success = true, message = "Không có thông báo chưa đọc." });
                }

                foreach (var notification in unreadNotifications)
                {
                    notification.IsRead = true;
                    _context.Update(notification);
                }

                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Đã đánh dấu tất cả thông báo là đã đọc." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking all notifications as read");
                return Json(new { success = false, message = "Đã xảy ra lỗi. Vui lòng thử lại sau." });
            }
        }

        // Get unread notification count for the current user
        [HttpGet]
        public async Task<IActionResult> GetUnreadCount()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Unauthorized();
                }

                var unreadCount = await _context.CustomerNotifications
                    .Where(n => n.UserId == user.Id && !n.IsRead)
                    .CountAsync();

                return Json(new { success = true, count = unreadCount });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting unread notification count");
                return Json(new { success = false, count = 0 });
            }
        }
    }
} 