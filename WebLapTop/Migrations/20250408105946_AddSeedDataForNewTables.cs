using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WebLapTop.Migrations
{
    /// <inheritdoc />
    public partial class AddSeedDataForNewTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "ProductImages",
                columns: new[] { "Id", "DisplayOrder", "ImageUrl", "IsDefault", "ProductId" },
                values: new object[,]
                {
                    { 1, 1, "/images/products/acer-nitro-1.jpg", true, 1 },
                    { 2, 2, "/images/products/acer-nitro-2.jpg", false, 1 },
                    { 3, 1, "/images/products/dell-vostro-1.jpg", true, 2 },
                    { 4, 2, "/images/products/dell-vostro-2.jpg", false, 2 }
                });

            migrationBuilder.InsertData(
                table: "ProductSpecifications",
                columns: new[] { "Id", "DisplayOrder", "ProductId", "SpecName", "SpecValue" },
                values: new object[,]
                {
                    { 1, 1, 1, "CPU", "Intel Core i5-12500H (12 nhân, 16 luồng, 4.50 GHz)" },
                    { 2, 2, 1, "RAM", "8GB DDR4 3200MHz" },
                    { 3, 3, 1, "Ổ cứng", "512GB NVMe PCIe Gen3 x4 SSD" },
                    { 4, 1, 2, "CPU", "Intel Core i5-1135G7 (4 nhân, 8 luồng, 4.20 GHz)" },
                    { 5, 2, 2, "RAM", "8GB DDR4 3200MHz" },
                    { 6, 3, 2, "Ổ cứng", "256GB NVMe PCIe SSD" }
                });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 1,
                column: "UpdatedDate",
                value: new DateTime(2025, 4, 8, 17, 59, 45, 942, DateTimeKind.Local).AddTicks(5471));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 2,
                column: "UpdatedDate",
                value: new DateTime(2025, 4, 8, 17, 59, 45, 942, DateTimeKind.Local).AddTicks(5504));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 3,
                column: "UpdatedDate",
                value: new DateTime(2025, 4, 8, 17, 59, 45, 942, DateTimeKind.Local).AddTicks(5508));

            migrationBuilder.InsertData(
                table: "Promotions",
                columns: new[] { "Id", "Description", "DiscountType", "DiscountValue", "EndDate", "IsActive", "Name", "StartDate" },
                values: new object[,]
                {
                    { 1, "Giảm giá đặc biệt cho học sinh, sinh viên", "Percentage", 10m, new DateTime(2025, 9, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Khuyến mãi mùa tựu trường", new DateTime(2025, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, "Giảm giá đặc biệt dịp cuối năm", "FixedAmount", 2000000m, new DateTime(2025, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Giảm giá cuối năm", new DateTime(2025, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "Warranties",
                columns: new[] { "Id", "Description", "ProductId", "Terms", "WarrantyPeriod" },
                values: new object[,]
                {
                    { 1, "Bảo hành chính hãng 24 tháng", 1, "1. Bảo hành chính hãng 24 tháng tại các trung tâm bảo hành Acer toàn quốc\n2. Đổi mới trong 15 ngày đầu nếu có lỗi phần cứng từ nhà sản xuất\n3. Hỗ trợ kỹ thuật 24/7", 24 },
                    { 2, "Bảo hành chính hãng 12 tháng", 2, "1. Bảo hành chính hãng 12 tháng tại các trung tâm bảo hành Dell toàn quốc\n2. Đổi mới trong 15 ngày đầu nếu có lỗi phần cứng từ nhà sản xuất\n3. Hỗ trợ kỹ thuật trong giờ hành chính", 12 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "ProductSpecifications",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ProductSpecifications",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "ProductSpecifications",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "ProductSpecifications",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "ProductSpecifications",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "ProductSpecifications",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Promotions",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Promotions",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Warranties",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Warranties",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 1,
                column: "UpdatedDate",
                value: new DateTime(2025, 4, 8, 17, 52, 39, 193, DateTimeKind.Local).AddTicks(3231));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 2,
                column: "UpdatedDate",
                value: new DateTime(2025, 4, 8, 17, 52, 39, 193, DateTimeKind.Local).AddTicks(3265));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 3,
                column: "UpdatedDate",
                value: new DateTime(2025, 4, 8, 17, 52, 39, 193, DateTimeKind.Local).AddTicks(3269));
        }
    }
}
