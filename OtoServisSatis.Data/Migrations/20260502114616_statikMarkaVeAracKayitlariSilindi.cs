using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace OtoServisSatis.Data.Migrations
{
    /// <inheritdoc />
    public partial class statikMarkaVeAracKayitlariSilindi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Araclar",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Araclar",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Araclar",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Markalar",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Markalar",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Markalar",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.UpdateData(
                table: "Kullanicilar",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "EklenmeTarihi", "UserGuid" },
                values: new object[] { new DateTime(2026, 5, 2, 14, 46, 9, 321, DateTimeKind.Local).AddTicks(292), new Guid("2885044f-e0a2-4f57-a328-778120fec404") });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Kullanicilar",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "EklenmeTarihi", "UserGuid" },
                values: new object[] { new DateTime(2026, 4, 27, 10, 9, 12, 268, DateTimeKind.Local).AddTicks(6638), new Guid("587705e3-31c2-4232-b0e7-7bdfcacd4d30") });

            migrationBuilder.InsertData(
                table: "Markalar",
                columns: new[] { "Id", "Adi" },
                values: new object[,]
                {
                    { 1, "Toyota" },
                    { 2, "Ford" },
                    { 3, "Honda" }
                });

            migrationBuilder.InsertData(
                table: "Araclar",
                columns: new[] { "Id", "Anasayfa", "Fiyati", "KasaTipi", "MarkaId", "ModelYili", "Modeli", "Notlar", "Renk", "Resim1", "Resim2", "Resim3", "SatistaMi" },
                values: new object[,]
                {
                    { 1, true, 1250000m, "Sedan", 1, 2023, "Corolla", "Boyasız, tramersiz.", "Beyaz", "toyota_on.png", "toyota_arka.png", "toyota_ic.png", true },
                    { 2, true, 1450000m, "Convertible", 2, 2022, "Mustang", "Yetkili servis bakımlı.", "Siyah", "ford_on.png", "ford_arka.png", "ford_ic.png", true },
                    { 3, true, 1750000m, "Sedan", 3, 2024, "Civic", "Sıfır ayarında.", "Kırmızı", "honda_on.png", "honda_arka.png", "honda_ic.png", true }
                });
        }
    }
}
