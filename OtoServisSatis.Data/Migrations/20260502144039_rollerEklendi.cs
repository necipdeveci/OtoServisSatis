using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OtoServisSatis.Data.Migrations
{
    /// <inheritdoc />
    public partial class rollerEklendi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Kullanicilar",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "EklenmeTarihi", "UserGuid" },
                values: new object[] { new DateTime(2026, 5, 2, 17, 40, 35, 982, DateTimeKind.Local).AddTicks(1832), new Guid("df55ac34-e802-4e08-a851-53a760acf49e") });

            migrationBuilder.InsertData(
                table: "Roller",
                columns: new[] { "Id", "Adi" },
                values: new object[] { 5, "Customer" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roller",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.UpdateData(
                table: "Kullanicilar",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "EklenmeTarihi", "UserGuid" },
                values: new object[] { new DateTime(2026, 5, 2, 17, 22, 47, 642, DateTimeKind.Local).AddTicks(9092), new Guid("e922bbba-bc44-4a02-8427-bfe9a90528f0") });
        }
    }
}
