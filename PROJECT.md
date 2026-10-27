# WebLapTop — Tài liệu dự án

> Ứng dụng web bán laptop xây dựng bằng **ASP.NET Core 8 MVC**

---

## ⚙️ Công nghệ sử dụng

| Thành phần | Chi tiết |
|---|---|
| Framework | ASP.NET Core 8 MVC |
| ORM | Entity Framework Core 8 |
| Database | SQL Server — Windows Authentication |
| Authentication | ASP.NET Core Identity — Role: Admin, User |
| UI | Bootstrap 5, Font Awesome 6, jQuery |
| Email | `IEmailSender` — SMTP Gmail |
| Session | `IDistributedMemoryCache` — 30 phút timeout |
| Validation | jquery-validation + jquery-validation-unobtrusive |
| Build / Run | `dotnet` CLI / `runapp.bat` |

---

## 📁 Cấu trúc thư mục

```
WebLapTop/
├── Areas/Admin/
│   ├── Controllers/
│   │   ├── CategoryController.cs   # CRUD danh mục
│   │   ├── ProductController.cs    # CRUD sản phẩm
│   │   └── OrderController.cs      # Quản lý đơn hàng
│   └── Views/
│       ├── Category/               # Create.cshtml
│       ├── Product/                # Index, Create, Edit
│       └── Order/                  # Index, Details
│
├── Controllers/
│   ├── HomeController.cs           # Trang chủ, tìm kiếm, lọc, phân trang
│   ├── AccountController.cs        # Đăng ký, đăng nhập, profile
│   ├── CartController.cs           # Giỏ hàng, checkout, lịch sử đơn hàng
│   ├── OrderController.cs          # Chi tiết đơn, hủy đơn, đánh giá
│   ├── NotificationController.cs   # Thông báo khách hàng
│   ├── AdminController.cs          # Dashboard, quản lý user
│   └── ErrorController.cs          # Xử lý lỗi 404/500
│
├── Data/
│   ├── ApplicationDbContext.cs     # DbContext + cấu hình + seed data
│   └── DbInitializer.cs            # Tạo role/admin lúc startup
│
├── Migrations/                     # 7 migration EF Core
│
├── Models/
│   ├── Entities/                   # 13 entity
│   │   ├── Product.cs, Category.cs
│   │   ├── Cart.cs, Order.cs, OrderDetail.cs
│   │   ├── Review.cs, Warranty.cs, Promotion.cs
│   │   ├── ProductImage.cs, ProductSpecification.cs
│   │   ├── AdminNotification.cs, CustomerNotification.cs
│   │   └── ApplicationUser.cs      # Extends IdentityUser
│   └── ViewModels/                 # 11 ViewModel
│
├── Services/
│   ├── IProductService.cs          # Interface: lọc SP, phân trang, giỏ hàng
│   ├── ProductService.cs           # Implement IProductService
│   └── EmailSender.cs              # Gửi email qua SMTP
│
├── ViewComponents/
│   └── CategoryMenuViewComponent.cs
│
├── Views/
│   ├── Account/      Login, Register, Profile
│   ├── Admin/        Dashboard, Products, Categories, Orders, Users, Notifications
│   ├── Cart/         Index, Checkout, OrderConfirmation, OrderDetails, OrderHistory
│   ├── Home/         Index, Details, About, Contact, Privacy
│   ├── Notification/ Index, Details
│   ├── Order/        Details, History
│   └── Shared/       _Layout, _AdminLayout, _LoginPartial
│
├── wwwroot/
│   ├── css/          site.css
│   ├── js/           site.js
│   ├── images/products/  3 ảnh sản phẩm mẫu
│   └── lib/          bootstrap, jquery, jquery-validation
│
├── Program.cs                      # Startup, DI, tự động tìm port
├── appsettings.json                # Cấu hình mẫu
├── appsettings.Development.json    # Cấu hình local (không commit)
└── web.config                      # IIS config
```

---

## 🗄️ Database

### Quan hệ các bảng

```
AspNetUsers (ApplicationUser)
    HoTen, DiaChi (mở rộng Identity)
    → Carts, Orders, Reviews, CustomerNotifications

Categories → Products
              → Carts
              → OrderDetails → Orders
              → Reviews
              → ProductImages
              → ProductSpecifications
              → Warranties

Orders → OrderDetails
       → AdminNotifications
       → CustomerNotifications

Promotions  ⚠️ độc lập, chưa liên kết với Order
```

### Seed data mặc định

| Loại | Dữ liệu |
|---|---|
| Roles | `Admin`, `User` |
| Admin | admin@weblaptop.com / `Admin@123` |
| User mẫu | user@weblaptop.com / `User@123` |
| Danh mục | Gaming, Văn Phòng, Đồ Họa |
| Sản phẩm | Acer Nitro 5, Dell Vostro 3520, MSI Creator Z16 |
| Khuyến mãi | 2 mẫu (tựu trường, cuối năm) |

---

## ✅ Đã làm được

### Khách hàng
- [x] Trang chủ: danh sách sản phẩm, lọc danh mục, tìm kiếm, sắp xếp giá, phân trang
- [x] Chi tiết sản phẩm + sản phẩm liên quan
- [x] Đăng ký / Đăng nhập / Đăng xuất
- [x] Xem & cập nhật profile
- [x] Giỏ hàng: thêm, cập nhật số lượng, xóa
- [x] Checkout: nhập địa chỉ, chọn phương thức thanh toán (COD)
- [x] Lịch sử đơn hàng + chi tiết đơn hàng
- [x] Hủy đơn hàng (chỉ khi trạng thái `Pending`)
- [x] Đánh giá sản phẩm (sau khi đơn `Delivered`)
- [x] Thông báo khách hàng (đặt hàng, hủy đơn)

### Admin
- [x] Dashboard tổng quan
- [x] Quản lý sản phẩm: xem, thêm, sửa, xóa
- [x] Quản lý danh mục: xem, thêm _(Edit còn thiếu view)_
- [x] Quản lý đơn hàng: xem danh sách, chi tiết, cập nhật trạng thái
- [x] Quản lý người dùng: xem, thêm, sửa, xem chi tiết
- [x] Thông báo admin khi có đơn hàng mới

### Hệ thống
- [x] ASP.NET Core Identity — đăng ký, đăng nhập, phân quyền Role
- [x] EF Core migrations + seed data
- [x] Session (30 phút)
- [x] Tự động tìm port trống khi khởi động
- [x] Xử lý lỗi 404
- [x] Transaction khi đặt hàng (rollback nếu lỗi)

---

## 🔧 Cần làm thêm

### 🔴 Ưu tiên cao
- [ ] **Bootstrap/jQuery thiếu file** — `wwwroot/lib/` chỉ có LICENSE, cần chạy `libman restore`
- [ ] **Admin Edit danh mục** — thiếu `Edit.cshtml` trong `Areas/Admin/Views/Category/`
- [ ] **Tích hợp Promotion vào Checkout** — bảng đã có nhưng chưa áp dụng khi đặt hàng
- [ ] **Thanh toán online** — hiện chỉ COD, cần tích hợp VNPay / MoMo / ZaloPay

### 🟡 Ưu tiên trung bình
- [ ] **ProductImage & ProductSpecification** — entity có, seed có, chưa hiển thị trên trang chi tiết SP
- [ ] **Warranty** — entity có nhưng chưa hiển thị
- [ ] **Phân trang Admin** — danh sách SP/đơn hàng/user chưa có phân trang
- [ ] **Tìm kiếm & lọc Admin** — chưa có filter trong các trang quản lý
- [ ] **Upload ảnh sản phẩm** — hiện chỉ nhập URL thủ công
- [ ] **Email xác nhận đơn hàng** — `EmailSender` có nhưng chưa gọi sau checkout
- [ ] **Quên mật khẩu** — `ForgotPasswordViewModel` có nhưng chưa implement flow reset

### 🟢 Ưu tiên thấp
- [ ] **Thống kê doanh thu** — Dashboard hiện là placeholder
- [ ] **Xuất báo cáo** — Excel/PDF đơn hàng
- [ ] **Wishlist** — danh sách yêu thích
- [ ] **So sánh sản phẩm**
- [ ] **SEO** — meta tags, sitemap
- [ ] **Responsive Admin** — `_AdminLayout` chưa tối ưu mobile

---

## 🚀 Chạy dự án

```bash
# 1. Tạo file cấu hình local
cp WebLapTop/appsettings.json WebLapTop/appsettings.Development.json
# Sửa YOUR_SERVER_NAME trong appsettings.Development.json

# 2. Migrate database
dotnet ef database update --project WebLapTop

# 3. Chạy app
runapp.bat
# hoặc
dotnet run --project WebLapTop
```

**Tài khoản mặc định:**

| Role | Email | Mật khẩu |
|---|---|---|
| Admin | admin@weblaptop.com | `Admin@123` |
| User | user@weblaptop.com | `User@123` |
