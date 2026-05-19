using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OtoServisSatis.Data.Migrations
{
    /// <inheritdoc />
    public partial class aractabloaracbilgiguncellendi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Kullanicilar",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "EklenmeTarihi", "UserGuid" },
                values: new object[] { new DateTime(2026, 5, 19, 23, 5, 14, 733, DateTimeKind.Local).AddTicks(6368), new Guid("89085520-caec-4270-863c-8c9724532250") });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Kullanicilar",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "EklenmeTarihi", "UserGuid" },
                values: new object[] { new DateTime(2026, 5, 8, 12, 43, 16, 925, DateTimeKind.Local).AddTicks(1684), new Guid("f82893c6-7a72-40e4-b503-d48c17038cc2") });
        }
    }
}
