using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BaezStone.MagicVilla.Api.Migrations
{
    /// <inheritdoc />
    public partial class TestQuitarAtributoIdentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "FechaActualizacion", "FechaCreacion" },
                values: new object[] { new DateTime(2026, 4, 7, 11, 44, 59, 958, DateTimeKind.Local).AddTicks(8275), new DateTime(2026, 4, 7, 11, 44, 59, 958, DateTimeKind.Local).AddTicks(8258) });

            migrationBuilder.UpdateData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "FechaActualizacion", "FechaCreacion" },
                values: new object[] { new DateTime(2026, 4, 7, 11, 44, 59, 958, DateTimeKind.Local).AddTicks(8278), new DateTime(2026, 4, 7, 11, 44, 59, 958, DateTimeKind.Local).AddTicks(8278) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "FechaActualizacion", "FechaCreacion" },
                values: new object[] { new DateTime(2026, 4, 7, 10, 59, 53, 241, DateTimeKind.Local).AddTicks(1231), new DateTime(2026, 4, 7, 10, 59, 53, 241, DateTimeKind.Local).AddTicks(1219) });

            migrationBuilder.UpdateData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "FechaActualizacion", "FechaCreacion" },
                values: new object[] { new DateTime(2026, 4, 7, 10, 59, 53, 241, DateTimeKind.Local).AddTicks(1235), new DateTime(2026, 4, 7, 10, 59, 53, 241, DateTimeKind.Local).AddTicks(1234) });
        }
    }
}
