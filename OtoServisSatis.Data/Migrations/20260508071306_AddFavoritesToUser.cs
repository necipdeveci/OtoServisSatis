using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OtoServisSatis.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddFavoritesToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FavoriAraclarJson",
                table: "Kullanicilar",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Kullanicilar",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "EklenmeTarihi", "FavoriAraclarJson", "UserGuid" },
                values: new object[] { new DateTime(2026, 5, 8, 10, 13, 3, 597, DateTimeKind.Local).AddTicks(6941), null, new Guid("b3d40696-faa8-4309-aa18-360da7ede4e3") });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FavoriAraclarJson",
                table: "Kullanicilar");

            migrationBuilder.UpdateData(
                table: "Kullanicilar",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "EklenmeTarihi", "UserGuid" },
                values: new object[] { new DateTime(2026, 5, 3, 1, 4, 51, 349, DateTimeKind.Local).AddTicks(1777), new Guid("39201ecf-c05f-42f9-880f-e611485574a5") });
        }
    }
}
