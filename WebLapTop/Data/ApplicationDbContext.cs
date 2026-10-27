using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebLapTop.Models.Entities;

namespace WebLapTop.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Bảng sản phẩm
        public DbSet<Product> Products { get; set; } = null!;

        // Bảng danh mục
        public DbSet<Category> Categories { get; set; } = null!;

        // Bảng giỏ hàng
        public DbSet<Cart> Carts { get; set; } = null!;

        // Bảng đơn hàng
        public DbSet<Order> Orders { get; set; } = null!;

        // Bảng chi tiết đơn hàng
        public DbSet<OrderDetail> OrderDetails { get; set; } = null!;

        // Bảng đánh giá
        public DbSet<Review> Reviews { get; set; } = null!;

        // Bảng thông báo admin
        public DbSet<AdminNotification> AdminNotifications { get; set; } = null!;

        // Bảng hình ảnh sản phẩm
        public DbSet<ProductImage> ProductImages { get; set; } = null!;

        // Bảng thông số kỹ thuật
        public DbSet<ProductSpecification> ProductSpecifications { get; set; } = null!;

        // Bảng bảo hành
        public DbSet<Warranty> Warranties { get; set; } = null!;

        // Bảng khuyến mãi
        public DbSet<Promotion> Promotions { get; set; } = null!;

        // Add DbSet for CustomerNotification
        public DbSet<CustomerNotification> CustomerNotifications { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // This will be used as a fallback configuration if no options are provided via AddDbContext
                optionsBuilder.UseSqlServer("Server=LAPTOP-6B480DAT\\THEANH;Database=WebLapTop;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true");
                optionsBuilder.EnableSensitiveDataLogging();
                optionsBuilder.EnableDetailedErrors();
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cấu hình khóa chính và khóa ngoại cho các bảng
            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Products");
                entity.Property(p => p.Name).HasMaxLength(200).IsRequired();
                entity.Property(p => p.Description).HasMaxLength(1000).IsRequired();
                entity.Property(p => p.ImageUrl).HasMaxLength(200).IsRequired();
                entity.Property(p => p.Price).HasColumnType("decimal(18,2)").IsRequired();
                entity.Property(p => p.DiscountPrice).HasColumnType("decimal(18,2)");
                entity.Property(p => p.CategoryId).IsRequired();
                entity.Property(p => p.IsVisible).IsRequired();
                entity.Property(p => p.CreatedDate).IsRequired();
                entity.Property(p => p.Stock).IsRequired().HasDefaultValue(0);
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("Categories");
                entity.Property(c => c.Name).HasMaxLength(200).IsRequired();
                entity.Property(c => c.Description).HasMaxLength(500);
                entity.Property(c => c.DisplayOrder).IsRequired();
                entity.Property(c => c.IsVisible).IsRequired();
            });

            modelBuilder.Entity<Cart>(entity =>
            {
                entity.ToTable("Carts");
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Quantity).IsRequired();
                entity.Property(c => c.UnitPrice).HasColumnType("decimal(18,2)").IsRequired();
                entity.Property(c => c.AddedDate).IsRequired();
                entity.Property(c => c.UserId).HasMaxLength(450).IsRequired();
                entity.Property(c => c.ProductId).IsRequired();

                entity.HasOne(c => c.User)
                    .WithMany(u => u.Carts)
                    .HasForeignKey(c => c.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(c => c.Product)
                    .WithMany()
                    .HasForeignKey(c => c.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("Orders");
                entity.Property(o => o.UserId).HasMaxLength(450).IsRequired();
                entity.Property(o => o.OrderDate).IsRequired();
                entity.Property(o => o.TotalAmount).HasColumnType("decimal(18,2)").IsRequired();
                entity.Property(o => o.ShippingAddress).HasMaxLength(500).IsRequired();
                entity.Property(o => o.FullName).HasMaxLength(100).IsRequired();
                entity.Property(o => o.PhoneNumber).HasMaxLength(20).IsRequired();
                entity.Property(o => o.Email).HasMaxLength(100).IsRequired();
                entity.Property(o => o.Note).HasMaxLength(1000);
                entity.Property(o => o.PaymentMethod).HasMaxLength(50).IsRequired();
                entity.Property(o => o.Status).HasMaxLength(50).IsRequired();
            });

            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.ToTable("OrderDetails");
                entity.Property(od => od.OrderId).IsRequired();
                entity.Property(od => od.ProductId).IsRequired();
                entity.Property(od => od.Quantity).IsRequired();
                entity.Property(od => od.UnitPrice).HasColumnType("decimal(18,2)").IsRequired();
            });

            modelBuilder.Entity<AdminNotification>(entity =>
            {
                entity.ToTable("AdminNotifications");
                entity.Property(n => n.Title).HasMaxLength(100).IsRequired();
                entity.Property(n => n.Message).HasMaxLength(500).IsRequired();
                entity.Property(n => n.CreateDate).IsRequired();
                entity.Property(n => n.IsRead).IsRequired();
            });

            // Cấu hình mối quan hệ giữa các bảng
            modelBuilder.Entity<Category>()
                .HasMany(c => c.Products)
                .WithOne(p => p.Category)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(u => u.Orders)
                .WithOne(o => o.User)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Order>()
                .HasMany(o => o.OrderDetails)
                .WithOne(od => od.Order)
                .HasForeignKey(od => od.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Product)
                .WithMany(p => p.OrderDetails)
                .HasForeignKey(od => od.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // Seed dữ liệu mẫu
            SeedData(modelBuilder);

            // In OnModelCreating, add configuration for CustomerNotification
            modelBuilder.Entity<CustomerNotification>(entity =>
            {
                entity.ToTable("CustomerNotifications");
                entity.Property(n => n.Title).HasMaxLength(100).IsRequired();
                entity.Property(n => n.Message).HasMaxLength(500).IsRequired();
                entity.Property(n => n.CreateDate).IsRequired();
                entity.Property(n => n.IsRead).IsRequired();
                entity.Property(n => n.Type).HasMaxLength(50).IsRequired();
                entity.Property(n => n.Link).HasMaxLength(255);

                entity.HasOne(n => n.User)
                    .WithMany()
                    .HasForeignKey(n => n.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(n => n.Order)
                    .WithMany()
                    .HasForeignKey(n => n.OrderId)
                    .OnDelete(DeleteBehavior.SetNull);
            });
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed Roles (AspNetRoles)
            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole
                {
                    Id = "1",
                    Name = "Admin",
                    NormalizedName = "ADMIN",
                    ConcurrencyStamp = Guid.NewGuid().ToString()
                },
                new IdentityRole
                {
                    Id = "2",
                    Name = "User",
                    NormalizedName = "USER",
                    ConcurrencyStamp = Guid.NewGuid().ToString()
                }
            );

            // Seed Categories
            modelBuilder.Entity<Category>().HasData(
                new Category
                {
                    Id = 1,
                    Name = "Laptop Gaming",
                    Description = "Laptop chuyên dụng cho gaming",
                    IsVisible = true,
                    DisplayOrder = 1
                },
                new Category
                {
                    Id = 2,
                    Name = "Laptop Văn Phòng",
                    Description = "Laptop cho công việc văn phòng",
                    IsVisible = true,
                    DisplayOrder = 2
                },
                new Category
                {
                    Id = 3,
                    Name = "Laptop Đồ Họa",
                    Description = "Laptop cho thiết kế và đồ họa",
                    IsVisible = true,
                    DisplayOrder = 3
                }
            );

            // Seed Products
            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Name = "Acer Nitro 5",
                    Description = "CPU: Intel Core i5-12500H (12 nhân, 16 luồng, 4.50 GHz), RAM: 8GB DDR4 3200MHz, SSD: 512GB NVMe, Card: NVIDIA GeForce RTX 3050 4GB, Màn hình: 15.6 inch FHD IPS 144Hz.",
                    Price = 23990000,
                    DiscountPrice = 20990000,
                    ImageUrl = "/images/products/acer-nitro.jpg",
                    IsAvailable = true,
                    IsFeatured = true,
                    CategoryId = 1,
                    IsVisible = true,
                    CreatedDate = new DateTime(2025, 1, 1),
                    Stock = 10
                },
                new Product
                {
                    Id = 2,
                    Name = "Dell Vostro 3520",
                    Description = "CPU: Intel Core i5-1135G7 (4 nhân, 8 luồng, 4.20 GHz), RAM: 8GB DDR4 3200MHz, SSD: 256GB NVMe, Card: Intel Iris Xe Graphics, Màn hình: 15.6 inch FHD, Vỏ nhôm bền bỉ.",
                    Price = 16990000,
                    DiscountPrice = 15690000,
                    ImageUrl = "/images/products/vostro-notebook-3520.jpg",
                    IsAvailable = true,
                    IsFeatured = false,
                    CategoryId = 2,
                    IsVisible = true,
                    CreatedDate = new DateTime(2025, 1, 1),
                    Stock = 15
                },
                new Product
                {
                    Id = 3,
                    Name = "MSI Creator Z16",
                    Description = "CPU: Intel Core i7-12700H (14 nhân, 20 luồng, 4.70 GHz), RAM: 16GB DDR5 4800MHz, SSD: 1TB NVMe PCIe Gen4, Card: NVIDIA GeForce RTX 3060 6GB, Màn hình: 16 inch QHD+ 120Hz.",
                    Price = 59990000,
                    ImageUrl = "/images/products/msi-creator-z16.jpg",
                    IsAvailable = true,
                    IsFeatured = true,
                    CategoryId = 3,
                    IsVisible = true,
                    CreatedDate = new DateTime(2025, 1, 1),
                    Stock = 5
                }
            );

            // Seed ProductImages
            modelBuilder.Entity<ProductImage>().HasData(
                // Acer Nitro 5
                new ProductImage
                {
                    Id = 1,
                    ProductId = 1,
                    ImageUrl = "/images/products/acer-nitro-1.jpg",
                    DisplayOrder = 1,
                    IsDefault = true
                },
                new ProductImage
                {
                    Id = 2,
                    ProductId = 1,
                    ImageUrl = "/images/products/acer-nitro-2.jpg",
                    DisplayOrder = 2,
                    IsDefault = false
                },
                // Dell Vostro 3520
                new ProductImage
                {
                    Id = 3,
                    ProductId = 2,
                    ImageUrl = "/images/products/dell-vostro-1.jpg",
                    DisplayOrder = 1,
                    IsDefault = true
                },
                new ProductImage
                {
                    Id = 4,
                    ProductId = 2,
                    ImageUrl = "/images/products/dell-vostro-2.jpg",
                    DisplayOrder = 2,
                    IsDefault = false
                }
            );

            // Seed ProductSpecifications
            modelBuilder.Entity<ProductSpecification>().HasData(
                // Acer Nitro 5
                new ProductSpecification
                {
                    Id = 1,
                    ProductId = 1,
                    SpecName = "CPU",
                    SpecValue = "Intel Core i5-12500H (12 nhân, 16 luồng, 4.50 GHz)",
                    DisplayOrder = 1
                },
                new ProductSpecification
                {
                    Id = 2,
                    ProductId = 1,
                    SpecName = "RAM",
                    SpecValue = "8GB DDR4 3200MHz",
                    DisplayOrder = 2
                },
                new ProductSpecification
                {
                    Id = 3,
                    ProductId = 1,
                    SpecName = "Ổ cứng",
                    SpecValue = "512GB NVMe PCIe Gen3 x4 SSD",
                    DisplayOrder = 3
                },
                // Dell Vostro 3520
                new ProductSpecification
                {
                    Id = 4,
                    ProductId = 2,
                    SpecName = "CPU",
                    SpecValue = "Intel Core i5-1135G7 (4 nhân, 8 luồng, 4.20 GHz)",
                    DisplayOrder = 1
                },
                new ProductSpecification
                {
                    Id = 5,
                    ProductId = 2,
                    SpecName = "RAM",
                    SpecValue = "8GB DDR4 3200MHz",
                    DisplayOrder = 2
                },
                new ProductSpecification
                {
                    Id = 6,
                    ProductId = 2,
                    SpecName = "Ổ cứng",
                    SpecValue = "256GB NVMe PCIe SSD",
                    DisplayOrder = 3
                }
            );

            // Seed Warranties
            modelBuilder.Entity<Warranty>().HasData(
                new Warranty
                {
                    Id = 1,
                    ProductId = 1,
                    WarrantyPeriod = 24,
                    Description = "Bảo hành chính hãng 24 tháng",
                    Terms = "1. Bảo hành chính hãng 24 tháng tại các trung tâm bảo hành Acer toàn quốc\n" +
                           "2. Đổi mới trong 15 ngày đầu nếu có lỗi phần cứng từ nhà sản xuất\n" +
                           "3. Hỗ trợ kỹ thuật 24/7"
                },
                new Warranty
                {
                    Id = 2,
                    ProductId = 2,
                    WarrantyPeriod = 12,
                    Description = "Bảo hành chính hãng 12 tháng",
                    Terms = "1. Bảo hành chính hãng 12 tháng tại các trung tâm bảo hành Dell toàn quốc\n" +
                           "2. Đổi mới trong 15 ngày đầu nếu có lỗi phần cứng từ nhà sản xuất\n" +
                           "3. Hỗ trợ kỹ thuật trong giờ hành chính"
                }
            );

            // Seed Promotions
            modelBuilder.Entity<Promotion>().HasData(
                new Promotion
                {
                    Id = 1,
                    Name = "Khuyến mãi mùa tựu trường",
                    Description = "Giảm giá đặc biệt cho học sinh, sinh viên",
                    DiscountType = "Percentage",
                    DiscountValue = 10, // Giảm 10%
                    StartDate = new DateTime(2025, 8, 1),
                    EndDate = new DateTime(2025, 9, 30),
                    IsActive = true
                },
                new Promotion
                {
                    Id = 2,
                    Name = "Giảm giá cuối năm",
                    Description = "Giảm giá đặc biệt dịp cuối năm",
                    DiscountType = "FixedAmount",
                    DiscountValue = 2000000, // Giảm 2 triệu
                    StartDate = new DateTime(2025, 12, 1),
                    EndDate = new DateTime(2025, 12, 31),
                    IsActive = false
                }
            );

            // Seed Orders
            modelBuilder.Entity<Order>().HasData(
                new Order
                {
                    Id = 1,
                    UserId = "2", // Normal user
                    OrderDate = new DateTime(2025, 4, 8, 0, 0, 0, DateTimeKind.Local),
                    ShippingAddress = "456 Đường User, Quận 2, TP.HCM",
                    FullName = "Khách hàng Mẫu",
                    PhoneNumber = "0987654321",
                    Email = "user@weblaptop.com",
                    TotalAmount = 20990000,
                    PaymentMethod = "COD",
                    Status = "Đã giao hàng",
                    Note = "Giao hàng trong giờ hành chính"
                }
            );

            // Seed OrderDetails
            modelBuilder.Entity<OrderDetail>().HasData(
                new OrderDetail
                {
                    Id = 1,
                    OrderId = 1,
                    ProductId = 1,
                    Quantity = 1,
                    UnitPrice = 20990000,
                    IsReviewed = true
                }
            );

            // Seed Reviews
            modelBuilder.Entity<Review>().HasData(
                new Review
                {
                    Id = 1,
                    ProductId = 1,
                    UserId = "2",
                    Rating = 5,
                    Comment = "Sản phẩm rất tốt, đóng gói cẩn thận, giao hàng nhanh!",
                    CreatedDate = new DateTime(2025, 4, 10, 0, 0, 0, DateTimeKind.Local),
                    OrderId = 1
                }
            );

            // Seed AdminNotifications
            modelBuilder.Entity<AdminNotification>().HasData(
                new AdminNotification
                {
                    Id = 1,
                    Title = "Đơn hàng mới",
                    Message = "Có đơn hàng mới cần xử lý",
                    CreateDate = new DateTime(2025, 4, 8, 0, 0, 0, DateTimeKind.Local),
                    IsRead = true,
                    OrderId = 1
                }
            );

            // Seed Cart (Example cart item for normal user)
            modelBuilder.Entity<Cart>().HasData(
                new Cart
                {
                    Id = 1,
                    UserId = "2",
                    ProductId = 2,
                    Quantity = 1,
                    UnitPrice = 15690000,
                    AddedDate = new DateTime(2025, 4, 13, 21, 0, 0, DateTimeKind.Local)
                }
            );
        }
    }
}
