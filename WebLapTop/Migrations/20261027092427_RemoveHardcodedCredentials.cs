using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WebLapTop.Migrations
{
    /// <inheritdoc />
    public partial class RemoveHardcodedCredentials : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "1", "1" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "2", "2" });

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "2");

            migrationBuilder.UpdateData(
                table: "AdminNotifications",
                keyColumn: "NotificationId",
                keyValue: 1,
                column: "CreateDate",
                value: new DateTime(2025, 4, 8, 0, 0, 0, 0, DateTimeKind.Local));

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1",
                column: "ConcurrencyStamp",
                value: "98e21e9f-3829-47d1-98d1-786445903d75");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2",
                column: "ConcurrencyStamp",
                value: "3d170087-9abb-4757-a219-dfae06767a09");

            migrationBuilder.UpdateData(
                table: "Carts",
                keyColumn: "CartId",
                keyValue: 1,
                column: "AddedDate",
                value: new DateTime(2025, 4, 13, 21, 0, 0, 0, DateTimeKind.Local));

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "OrderId",
                keyValue: 1,
                column: "OrderDate",
                value: new DateTime(2025, 4, 8, 0, 0, 0, 0, DateTimeKind.Local));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 1,
                column: "UpdatedDate",
                value: new DateTime(2026, 10, 27, 16, 24, 26, 71, DateTimeKind.Local).AddTicks(9608));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 2,
                column: "UpdatedDate",
                value: new DateTime(2026, 10, 27, 16, 24, 26, 71, DateTimeKind.Local).AddTicks(9712));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 3,
                column: "UpdatedDate",
                value: new DateTime(2026, 10, 27, 16, 24, 26, 71, DateTimeKind.Local).AddTicks(9716));

            migrationBuilder.UpdateData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2025, 4, 10, 0, 0, 0, 0, DateTimeKind.Local));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AdminNotifications",
                keyColumn: "NotificationId",
                keyValue: 1,
                column: "CreateDate",
                value: new DateTime(2025, 4, 8, 23, 26, 54, 657, DateTimeKind.Local).AddTicks(4129));

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1",
                column: "ConcurrencyStamp",
                value: "41987450-106d-4ec6-8212-c341ac81765d");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2",
                column: "ConcurrencyStamp",
                value: "0c64d84f-7493-421e-8b03-903c2b69f523");

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "DiaChi", "Email", "EmailConfirmed", "FullName", "HoTen", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { "1", 0, "4b57641e-96b5-498f-a045-3a594ef280d4", "123 Đường Admin, Quận 1, TP.HCM", "admin@weblaptop.com", true, "", "Quản trị viên", false, null, "ADMIN@WEBLAPTOP.COM", "ADMIN@WEBLAPTOP.COM", "AQAAAAIAAYagAAAAEFYv4Gzs3jxLZWU05qJQMFOxPTpA27OceqKt2rE9P9kthi2Vb9LGzLTHvmLCsL2IVg==", "0123456789", true, "b24e92a7-04ed-4d6f-8c4f-c46c66e9a166", false, "admin@weblaptop.com" },
                    { "2", 0, "3bf8c134-65de-44a4-bc24-8ceea9772814", "456 Đường User, Quận 2, TP.HCM", "user@weblaptop.com", true, "", "Khách hàng Mẫu", false, null, "USER@WEBLAPTOP.COM", "USER@WEBLAPTOP.COM", "AQAAAAIAAYagAAAAEBfnqE/h2FOKur5zVcvAlPbyLoG5SjwOW+8vSbt28u0JZm0EiCPNlad7evaFR14+lQ==", "0987654321", true, "0bc78403-3a16-4ed5-8e26-023684d33155", false, "user@weblaptop.com" }
                });

            migrationBuilder.UpdateData(
                table: "Carts",
                keyColumn: "CartId",
                keyValue: 1,
                column: "AddedDate",
                value: new DateTime(2025, 4, 13, 21, 26, 54, 657, DateTimeKind.Local).AddTicks(4568));

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "OrderId",
                keyValue: 1,
                column: "OrderDate",
                value: new DateTime(2025, 4, 8, 23, 26, 54, 657, DateTimeKind.Local).AddTicks(3644));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 1,
                column: "UpdatedDate",
                value: new DateTime(2025, 4, 13, 23, 26, 54, 657, DateTimeKind.Local).AddTicks(2616));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 2,
                column: "UpdatedDate",
                value: new DateTime(2025, 4, 13, 23, 26, 54, 657, DateTimeKind.Local).AddTicks(2705));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 3,
                column: "UpdatedDate",
                value: new DateTime(2025, 4, 13, 23, 26, 54, 657, DateTimeKind.Local).AddTicks(2727));

            migrationBuilder.UpdateData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2025, 4, 10, 23, 26, 54, 657, DateTimeKind.Local).AddTicks(4012));

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { "1", "1" },
                    { "2", "2" }
                });
        }
    }
}
