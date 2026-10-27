using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebLapTop.Migrations
{
    /// <inheritdoc />
    public partial class AddFullNameToApplicationUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AspNetUserTokens",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserTokens",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "AspNetUsers",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "ProviderKey",
                table: "AspNetUserLogins",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserLogins",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

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

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1",
                columns: new[] { "ConcurrencyStamp", "FullName", "PasswordHash", "SecurityStamp" },
                values: new object[] { "4b57641e-96b5-498f-a045-3a594ef280d4", "", "AQAAAAIAAYagAAAAEFYv4Gzs3jxLZWU05qJQMFOxPTpA27OceqKt2rE9P9kthi2Vb9LGzLTHvmLCsL2IVg==", "b24e92a7-04ed-4d6f-8c4f-c46c66e9a166" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "2",
                columns: new[] { "ConcurrencyStamp", "FullName", "PasswordHash", "SecurityStamp" },
                values: new object[] { "3bf8c134-65de-44a4-bc24-8ceea9772814", "", "AQAAAAIAAYagAAAAEBfnqE/h2FOKur5zVcvAlPbyLoG5SjwOW+8vSbt28u0JZm0EiCPNlad7evaFR14+lQ==", "0bc78403-3a16-4ed5-8e26-023684d33155" });

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FullName",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AspNetUserTokens",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserTokens",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "ProviderKey",
                table: "AspNetUserLogins",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserLogins",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.UpdateData(
                table: "AdminNotifications",
                keyColumn: "NotificationId",
                keyValue: 1,
                column: "CreateDate",
                value: new DateTime(2025, 4, 3, 22, 50, 49, 558, DateTimeKind.Local).AddTicks(534));

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1",
                column: "ConcurrencyStamp",
                value: "5d104da9-ea88-44c4-a2f9-8aa6efa77b17");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2",
                column: "ConcurrencyStamp",
                value: "8848c3d6-e48a-4fa2-930e-fb07d2228f1a");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "a3bd14e6-6211-496d-bfb0-35dbca20f6e7", "AQAAAAIAAYagAAAAEFEi8uZidS8zSXpMe1oTBe7S0IYkxwsRgnQSpc/R4M9lF7naYrHldU/al7JKzrV4GA==", "6df04915-9276-49cb-b201-761e44230174" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "2",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "2395b1bc-9135-424f-a3f0-0bd1cc635739", "AQAAAAIAAYagAAAAENHlmJCd6WgDptW5X2ORWV9WLObAe0ZXHhgOFnz4NspCQA5MEaNdckHSJQRP+Q3h+A==", "4d64ca75-3b40-4aae-be85-166b5b19b1bb" });

            migrationBuilder.UpdateData(
                table: "Carts",
                keyColumn: "CartId",
                keyValue: 1,
                column: "AddedDate",
                value: new DateTime(2025, 4, 8, 20, 50, 49, 558, DateTimeKind.Local).AddTicks(595));

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "OrderId",
                keyValue: 1,
                column: "OrderDate",
                value: new DateTime(2025, 4, 3, 22, 50, 49, 558, DateTimeKind.Local).AddTicks(154));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 1,
                column: "UpdatedDate",
                value: new DateTime(2025, 4, 8, 22, 50, 49, 557, DateTimeKind.Local).AddTicks(9765));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 2,
                column: "UpdatedDate",
                value: new DateTime(2025, 4, 8, 22, 50, 49, 557, DateTimeKind.Local).AddTicks(9817));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 3,
                column: "UpdatedDate",
                value: new DateTime(2025, 4, 8, 22, 50, 49, 557, DateTimeKind.Local).AddTicks(9826));

            migrationBuilder.UpdateData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2025, 4, 5, 22, 50, 49, 558, DateTimeKind.Local).AddTicks(486));
        }
    }
}
