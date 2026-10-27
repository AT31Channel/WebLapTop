using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebLapTop.Models;

namespace WebLapTop.Controllers
{
    public class ErrorController : Controller
    {
        private readonly ILogger<ErrorController> _logger;

        public ErrorController(ILogger<ErrorController> logger)
        {
            _logger = logger;
        }

        [Route("Error/{statusCode}")]
        public IActionResult HttpStatusCodeHandler(int statusCode)
        {
            var statusCodeResult = HttpContext.Features.Get<Microsoft.AspNetCore.Diagnostics.IStatusCodeReExecuteFeature>();

            switch (statusCode)
            {
                case 404:
                    ViewBag.ErrorMessage = "Không tìm thấy trang yêu cầu";
                    _logger.LogWarning($"404 Error Occurred. Path = {statusCodeResult?.OriginalPath}");
                    break;
                case 500:
                    ViewBag.ErrorMessage = "Đã xảy ra lỗi trong quá trình xử lý";
                    _logger.LogError($"500 Error Occurred. Path = {statusCodeResult?.OriginalPath}");
                    break;
                default:
                    ViewBag.ErrorMessage = "Đã xảy ra lỗi";
                    break;
            }

            return View("Error");
        }

        [Route("Error")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var exceptionDetails = HttpContext.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerPathFeature>();

            _logger.LogError($"Error Occurred. Path = {exceptionDetails?.Path}, Error = {exceptionDetails?.Error}");

            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
