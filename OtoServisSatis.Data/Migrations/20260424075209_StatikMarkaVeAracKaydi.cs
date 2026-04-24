using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OtoServisSatis.Data.Migrations
{
    /// <inheritdoc />
    public partial class StatikMarkaVeAracKaydi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Araclar",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Resim1", "Resim2", "Resim3" },
                values: new object[] { "toyota_on.png", "toyota_arka.png", "toyota_ic.png" });

            migrationBuilder.UpdateData(
                table: "Araclar",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Resim1", "Resim2", "Resim3" },
                values: new object[] { "ford_on.png", "ford_arka.png", "ford_ic.png" });

            migrationBuilder.UpdateData(
                table: "Araclar",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Resim1", "Resim2", "Resim3" },
                values: new object[] { "honda_on.png", "honda_arka.png", "honda_ic.png" });

            migrationBuilder.UpdateData(
                table: "Kullanicilar",
                keyColumn: "Id",
                keyValue: 1,
                column: "EklenmeTarihi",
                value: new DateTime(2026, 4, 24, 10, 52, 6, 793, DateTimeKind.Local).AddTicks(1668));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Araclar",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Resim1", "Resim2", "Resim3" },
                values: new object[] { "/Img/Cars/toyota_on.png", "/Img/Cars/toyota_arka.png", "/Img/Cars/toyota_ic.png" });

            migrationBuilder.UpdateData(
                table: "Araclar",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Resim1", "Resim2", "Resim3" },
                values: new object[] { "/Img/Cars/ford_on.png", "/Img/Cars/ford_arka.png", "/Img/Cars/ford_ic.png" });

            migrationBuilder.UpdateData(
                table: "Araclar",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Resim1", "Resim2", "Resim3" },
                values: new object[] { "/Img/Cars/honda_on.png", "/Img/Cars/honda_arka.png", "/Img/Cars/honda_ic.png" });

            migrationBuilder.UpdateData(
                table: "Kullanicilar",
                keyColumn: "Id",
                keyValue: 1,
                column: "EklenmeTarihi",
                value: new DateTime(2026, 4, 24, 10, 41, 8, 180, DateTimeKind.Local).AddTicks(6279));
        }
    }
}
