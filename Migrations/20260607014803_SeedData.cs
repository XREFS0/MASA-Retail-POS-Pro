using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MASA.RetailPOS.App.Migrations
{
    /// <inheritdoc />
    public partial class SeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "CreatedAt", "IconSource", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 6, 7, 3, 48, 3, 181, DateTimeKind.Local).AddTicks(6364), null, "مشروبات", new DateTime(2026, 6, 7, 3, 48, 3, 181, DateTimeKind.Local).AddTicks(6366) },
                    { 2, new DateTime(2026, 6, 7, 3, 48, 3, 181, DateTimeKind.Local).AddTicks(6368), null, "حلويات", new DateTime(2026, 6, 7, 3, 48, 3, 181, DateTimeKind.Local).AddTicks(6369) },
                    { 3, new DateTime(2026, 6, 7, 3, 48, 3, 181, DateTimeKind.Local).AddTicks(6371), null, "أطعمة", new DateTime(2026, 6, 7, 3, 48, 3, 181, DateTimeKind.Local).AddTicks(6371) },
                    { 4, new DateTime(2026, 6, 7, 3, 48, 3, 181, DateTimeKind.Local).AddTicks(6372), null, "منتجات متنوعة", new DateTime(2026, 6, 7, 3, 48, 3, 181, DateTimeKind.Local).AddTicks(6373) }
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 7, 3, 48, 3, 181, DateTimeKind.Local).AddTicks(5078), new DateTime(2026, 6, 7, 3, 48, 3, 181, DateTimeKind.Local).AddTicks(5089) });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Barcode", "CategoryId", "CreatedAt", "ImageSource", "MinimumStockAlert", "Name", "PurchasePrice", "SalePrice", "StockQuantity", "TaxRate", "UnitOfMeasure", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, "1001", 1, new DateTime(2026, 6, 7, 3, 48, 3, 181, DateTimeKind.Local).AddTicks(6407), null, 0, "قهوة تركية", 0m, 25.00m, 100, 0m, "Piece", new DateTime(2026, 6, 7, 3, 48, 3, 181, DateTimeKind.Local).AddTicks(6408) },
                    { 2, "1002", 1, new DateTime(2026, 6, 7, 3, 48, 3, 181, DateTimeKind.Local).AddTicks(6412), null, 0, "شاي أخضر", 0m, 15.00m, 100, 0m, "Piece", new DateTime(2026, 6, 7, 3, 48, 3, 181, DateTimeKind.Local).AddTicks(6413) },
                    { 3, "1003", 1, new DateTime(2026, 6, 7, 3, 48, 3, 181, DateTimeKind.Local).AddTicks(6415), null, 0, "عصير برتقال", 0m, 20.00m, 100, 0m, "Piece", new DateTime(2026, 6, 7, 3, 48, 3, 181, DateTimeKind.Local).AddTicks(6415) },
                    { 4, "1004", 1, new DateTime(2026, 6, 7, 3, 48, 3, 181, DateTimeKind.Local).AddTicks(6417), null, 0, "مياه معدنية", 0m, 5.00m, 100, 0m, "Piece", new DateTime(2026, 6, 7, 3, 48, 3, 181, DateTimeKind.Local).AddTicks(6417) },
                    { 5, "1005", 1, new DateTime(2026, 6, 7, 3, 48, 3, 181, DateTimeKind.Local).AddTicks(6419), null, 0, "مشروب غازي", 0m, 7.00m, 100, 0m, "Piece", new DateTime(2026, 6, 7, 3, 48, 3, 181, DateTimeKind.Local).AddTicks(6419) },
                    { 6, "1006", 1, new DateTime(2026, 6, 7, 3, 48, 3, 181, DateTimeKind.Local).AddTicks(6421), null, 0, "كابتشينو", 0m, 30.00m, 100, 0m, "Piece", new DateTime(2026, 6, 7, 3, 48, 3, 181, DateTimeKind.Local).AddTicks(6422) },
                    { 7, "1007", 2, new DateTime(2026, 6, 7, 3, 48, 3, 181, DateTimeKind.Local).AddTicks(6423), null, 0, "كيك شوكولاتة", 0m, 45.00m, 100, 0m, "Piece", new DateTime(2026, 6, 7, 3, 48, 3, 181, DateTimeKind.Local).AddTicks(6424) },
                    { 8, "1008", 2, new DateTime(2026, 6, 7, 3, 48, 3, 181, DateTimeKind.Local).AddTicks(6426), null, 0, "كرواسون", 0m, 18.00m, 100, 0m, "Piece", new DateTime(2026, 6, 7, 3, 48, 3, 181, DateTimeKind.Local).AddTicks(6426) },
                    { 9, "1009", 3, new DateTime(2026, 6, 7, 3, 48, 3, 181, DateTimeKind.Local).AddTicks(6464), null, 0, "ساندويتش الدجاج", 0m, 35.00m, 100, 0m, "Piece", new DateTime(2026, 6, 7, 3, 48, 3, 181, DateTimeKind.Local).AddTicks(6465) },
                    { 10, "1010", 3, new DateTime(2026, 6, 7, 3, 48, 3, 181, DateTimeKind.Local).AddTicks(6467), null, 0, "بيتزا خضار", 0m, 40.00m, 100, 0m, "Piece", new DateTime(2026, 6, 7, 3, 48, 3, 181, DateTimeKind.Local).AddTicks(6467) },
                    { 11, "1011", 3, new DateTime(2026, 6, 7, 3, 48, 3, 181, DateTimeKind.Local).AddTicks(6469), null, 0, "بطاطس مقلية", 0m, 15.00m, 100, 0m, "Piece", new DateTime(2026, 6, 7, 3, 48, 3, 181, DateTimeKind.Local).AddTicks(6469) },
                    { 12, "1012", 2, new DateTime(2026, 6, 7, 3, 48, 3, 181, DateTimeKind.Local).AddTicks(6471), null, 0, "آيس كريم", 0m, 25.00m, 100, 0m, "Piece", new DateTime(2026, 6, 7, 3, 48, 3, 181, DateTimeKind.Local).AddTicks(6471) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 7, 3, 41, 27, 449, DateTimeKind.Local).AddTicks(4454), new DateTime(2026, 6, 7, 3, 41, 27, 449, DateTimeKind.Local).AddTicks(4466) });
        }
    }
}
