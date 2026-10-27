using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Sockets;
using WebLapTop.Data;
using WebLapTop.Models.Entities;
using WebLapTop.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
// using WebLapTop.Areas.Identity;
// using WebLapTop.Areas.Identity.Data;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Logging;
using WebLapTop.Models;
using Microsoft.AspNetCore.HttpsPolicy;
using System.Linq;
using System.Collections.Generic;

// Bỏ qua cảnh báo null reference
#nullable disable

namespace WebLapTop
{
    public class Program
    {
        // Danh sách các cổng an toàn cho HTTP
        private static readonly int[] SafeHttpPorts = { 8080, 8082, 8084, 8086, 8088, 8090, 8092, 8094, 8096, 8098, 
                                                       9000, 9002, 9004, 9006, 9008, 9010, 9080, 9090 };
                                                   
        // Danh sách các cổng an toàn cho HTTPS
        private static readonly int[] SafeHttpsPorts = { 8081, 8083, 8085, 8087, 8089, 8091, 8093, 8095, 8097, 8099, 
                                                        8443, 8444, 8445, 8446, 8447, 9001, 9003, 9005, 9007, 9009 };

        // Kiểm tra xem cổng có đang được sử dụng không
        private static bool IsPortInUse(int port)
        {
            try
            {
                using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
                {
                    socket.Bind(new IPEndPoint(IPAddress.Loopback, port));
                    return false; // Port is free
                }
            }
            catch
            {
                return true; // Port is in use
            }
        }

        // Tìm cổng trống an toàn
        private static int FindAvailablePort(int startPort = 9000)
        {
            // Nếu cổng khởi điểm không đang được sử dụng, dùng nó
            if (!IsPortInUse(startPort))
            {
                return startPort;
            }

            // Thử các cổng từ danh sách cổng an toàn
            foreach (int safePort in SafeHttpPorts.OrderBy(p => Math.Abs(p - startPort)))
            {
                if (!IsPortInUse(safePort))
                {
                    return safePort;
                }
            }

            // Nếu tất cả cổng an toàn đều đang bận, tìm cổng bất kỳ có sẵn
            int currentPort = startPort;
            while (IsPortInUse(currentPort) && currentPort < 65535)
            {
                currentPort++;
            }
            
            return currentPort;
        }

        // Phương thức tương tự cho cổng HTTPS
        private static int FindAvailableHttpsPort(int httpPort)
        {
            // Thử các cổng từ danh sách cổng an toàn HTTPS
            foreach (int safePort in SafeHttpsPorts.OrderBy(p => Math.Abs(p - (httpPort + 1))))
            {
                if (!IsPortInUse(safePort) && safePort != httpPort)
                {
                    return safePort;
                }
            }

            // Nếu tất cả cổng an toàn đều đang bận, tìm cổng bất kỳ có sẵn
            int currentPort = httpPort + 1;
            while (IsPortInUse(currentPort) && currentPort < 65535)
            {
                currentPort++;
            }
            
            return currentPort;
        }

        // Đóng cổng đang được sử dụng (nếu cần)
        private static void ClosePortIfInUse(int port)
        {
            if (IsPortInUse(port))
            {
                try
                {
                    // Xác định PID của tiến trình đang sử dụng cổng
                    var process = new System.Diagnostics.Process
                    {
                        StartInfo = new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = "cmd.exe",
                            Arguments = $"/c netstat -ano | findstr :{port} | findstr LISTENING",
                            UseShellExecute = false,
                            RedirectStandardOutput = true,
                            CreateNoWindow = true
                        }
                    };
                    
                    process.Start();
                    string output = process.StandardOutput.ReadToEnd();
                    process.WaitForExit();
                    
                    // Tìm PID từ output
                    string[] lines = output.Split('\n');
                    foreach (string line in lines)
                    {
                        string trimmedLine = line.Trim();
                        if (!string.IsNullOrEmpty(trimmedLine))
                        {
                            string[] parts = trimmedLine.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            if (parts.Length > 4)
                            {
                                string pid = parts[parts.Length - 1];
                                
                                // Kết thúc tiến trình
                                var killProcess = new System.Diagnostics.Process
                                {
                                    StartInfo = new System.Diagnostics.ProcessStartInfo
                                    {
                                        FileName = "cmd.exe",
                                        Arguments = $"/c taskkill /F /PID {pid}",
                                        UseShellExecute = false,
                                        CreateNoWindow = true
                                    }
                                };
                                
                                killProcess.Start();
                                killProcess.WaitForExit();
                                Console.WriteLine($"Đã đóng tiến trình sử dụng cổng {port} (PID: {pid})");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Không thể đóng cổng {port}: {ex.Message}");
                }
            }
        }

        public static async Task Main(string[] args)
        {
            // Thiết lập encoding cho Console để hiển thị tiếng Việt
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            var builder = WebApplication.CreateBuilder(args);
            builder.Configuration.AddJsonFile("appsettings.Dev.json", optional: true, reloadOnChange: false);
            
            // Loại bỏ tất cả cảnh báo EF Core và Kestrel
            builder.Logging.AddFilter("Microsoft.EntityFrameworkCore", LogLevel.Error);
            builder.Logging.AddFilter("Microsoft.AspNetCore.Server.Kestrel", LogLevel.Error);
            builder.Logging.AddFilter("Microsoft.AspNetCore.SpaProxy", LogLevel.Error);
            builder.Logging.AddFilter("Microsoft.AspNetCore.Hosting", LogLevel.Warning);
            // Tắt cảnh báo CSS
            builder.Logging.AddFilter("Microsoft.AspNetCore.StaticFiles", LogLevel.Error);
            builder.Logging.AddFilter("Microsoft.Extensions.Hosting.Internal", LogLevel.Warning);
            
            // Cấu hình chuỗi kết nối database
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? 
                "Server=LAPTOP-6B480DAT\\THEANH;Database=WebLapTop;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true";
            Console.WriteLine($"Using connection string: {connectionString}");

            // Add services to the container
            builder.Services.AddDbContext<ApplicationDbContext>(options => 
                options.UseSqlServer(connectionString));

            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages();

            // Add services for authentication and authorization
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireUppercase = true;
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            // Cấu hình Cookie Policy
            builder.Services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.Strict;
                options.Secure = CookieSecurePolicy.Always;
            });

            // Add custom services
            builder.Services.AddSingleton<IEmailSender, EmailSender>();
            builder.Services.AddScoped<IProductService, ProductService>();

            // Cấu hình Session
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            // Thiết lập URL cho ứng dụng
            string[] defaultUrls;
            try
            {
                // Tìm port khả dụng tự động
                int httpPort = FindAvailablePort(9000);
                int httpsPort = FindAvailableHttpsPort(httpPort);

                // Đảm bảo port không bị conflict
                if (IsPortInUse(httpPort))
                {
                    ClosePortIfInUse(httpPort);
                }
                if (IsPortInUse(httpsPort))
                {
                    ClosePortIfInUse(httpsPort);
                }

                defaultUrls = new string[] 
                { 
                    $"http://localhost:{httpPort}", 
                    $"https://localhost:{httpsPort}" 
                };
            }
            catch
            {
                // Fallback nếu không thể tìm port tự động
                defaultUrls = new string[] 
                { 
                    "http://localhost:5000", 
                    "https://localhost:5001" 
                };
            }

            foreach (var url in defaultUrls)
            {
                builder.WebHost.UseUrls(url);
            }
            
            // Cấu hình để ứng dụng không crash khi không thể bind cổng
            builder.WebHost.CaptureStartupErrors(true)
                .UseSetting("detailedErrors", "true");

            // Hiển thị thông tin URL
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("====================================================");
            Console.WriteLine("Access the application at:");
            foreach (var url in defaultUrls)
            {
                Console.WriteLine($"- {url}");
            }
            Console.WriteLine("====================================================");
            Console.ResetColor();

            // Tự động mở trình duyệt HTTPS
            var httpsUrl = defaultUrls.FirstOrDefault(u => u.StartsWith("https://"));
            if (!string.IsNullOrEmpty(httpsUrl))
            {
                try
                {
                    var psi = new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = "cmd.exe",
                        Arguments = $"/c start chrome {httpsUrl}",
                        UseShellExecute = true,
                        CreateNoWindow = true
                    };
                    System.Diagnostics.Process.Start(psi);
                    Console.WriteLine($"Đã tự động mở trình duyệt tại: {httpsUrl}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Không thể tự động mở trình duyệt: {ex.Message}");
                    Console.WriteLine($"Vui lòng mở thủ công địa chỉ: {httpsUrl}");
                }
            }

            var app = builder.Build();

            // Cấu hình HTTP request pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            // Chuyển hướng tất cả traffic sang HTTPS
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseRouting();
            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();

            // Định nghĩa routing cho ứng dụng
            app.MapControllerRoute(
                name: "area",
                pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
            
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.MapRazorPages();

            // Xử lý lỗi 404
            app.Use(async (context, next) =>
            {
                await next();
                if (context.Response.StatusCode == 404 && !context.Response.HasStarted)
                {
                    // Log lỗi 404
                    app.Logger.LogWarning("404 error for path: " + context.Request.Path);
                    context.Request.Path = "/Error/NotFound";
                    await next();
                }
            });

            try
            {
                // Khởi tạo dữ liệu nếu chưa có trong database
                using (var scope = app.Services.CreateScope())
                {
                    var services = scope.ServiceProvider;
                    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

                    // Tạo các Role nếu chưa tồn tại
                    var adminRole = await roleManager.FindByNameAsync("Admin");
                    if (adminRole == null)
                    {
                        await roleManager.CreateAsync(new IdentityRole("Admin"));
                    }

                    var userRole = await roleManager.FindByNameAsync("User");
                    if (userRole == null)
                    {
                        await roleManager.CreateAsync(new IdentityRole("User"));
                    }

                    // Tạo tài khoản admin mặc định nếu chưa tồn tại
                    var adminUser = await userManager.FindByEmailAsync("admin@weblaptop.com");
                    if (adminUser == null)
                    {
                        var admin = new ApplicationUser
                        {
                            UserName = "admin@weblaptop.com",
                            Email = "admin@weblaptop.com",
                            EmailConfirmed = true,
                            HoTen = "Administrator",
                            DiaChi = "Ho Chi Minh City, Vietnam"
                        };

                        var result = await userManager.CreateAsync(admin, "Admin@123");

                        if (result.Succeeded)
                        {
                            await userManager.AddToRoleAsync(admin, "Admin");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Lỗi khi khởi tạo dữ liệu: {ex.Message}");
                Console.ResetColor();
            }

            // Chạy ứng dụng
            await app.RunAsync();
        }
    }
}

