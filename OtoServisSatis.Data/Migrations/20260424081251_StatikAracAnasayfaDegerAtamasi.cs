using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OtoServisSatis.Data.Migrations
{
    /// <inheritdoc />
    public partial class StatikAracAnasayfaDegerAtamasi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Araclar",
                keyColumn: "Id",
                keyValue: 1,
                column: "Anasayfa",
                value: true);

            migrationBuilder.UpdateData(
                table: "Araclar",
                keyColumn: "Id",
                keyValue: 2,
                column: "Anasayfa",
                value: true);

            migrationBuilder.UpdateData(
                table: "Araclar",
                keyColumn: "Id",
                keyValue: 3,
                column: "Anasayfa",
                value: true);

            migrationBuilder.UpdateData(
                table: "Kullanicilar",
                keyColumn: "Id",
                keyValue: 1,
                column: "EklenmeTarihi",
                value: new DateTime(2026, 4, 24, 11, 12, 47, 141, DateTimeKind.Local).AddTicks(2384));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Araclar",
                keyColumn: "Id",
                keyValue: 1,
                column: "Anasayfa",
                value: false);

            migrationBuilder.UpdateData(
                table: "Araclar",
                keyColumn: "Id",
                keyValue: 2,
                column: "Anasayfa",
                value: false);

            migrationBuilder.UpdateData(
                table: "Araclar",
                keyColumn: "Id",
                keyValue: 3,
                column: "Anasayfa",
                value: false);

            migrationBuilder.UpdateData(
                table: "Kullanicilar",
                keyColumn: "Id",
                keyValue: 1,
                column: "EklenmeTarihi",
                value: new DateTime(2026, 4, 24, 11, 2, 1, 735, DateTimeKind.Local).AddTicks(4899));
        }
    }
}
