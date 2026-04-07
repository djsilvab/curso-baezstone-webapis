using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BaezStone.MagicVilla.Api.Migrations
{
    /// <inheritdoc />
    public partial class AlimentarTablaVilla : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Villas",
                columns: new[] { "Id", "Amenidad", "Detalle", "FechaActualizacion", "FechaCreacion", "ImagenURL", "MetrosCuadrados", "Nombre", "Ocupantes", "Tarifa" },
                values: new object[,]
                {
                    { 1, "", "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.", new DateTime(2026, 4, 7, 10, 59, 53, 241, DateTimeKind.Local).AddTicks(1231), new DateTime(2026, 4, 7, 10, 59, 53, 241, DateTimeKind.Local).AddTicks(1219), "https://dotnetmasteryimages.blob.core.windows.net/bluevillaimages/villa3.jpg", 550, "Villa Real", 4, 200.0 },
                    { 2, "", "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.", new DateTime(2026, 4, 7, 10, 59, 53, 241, DateTimeKind.Local).AddTicks(1235), new DateTime(2026, 4, 7, 10, 59, 53, 241, DateTimeKind.Local).AddTicks(1234), "https://dotnetmasteryimages.blob.core.windows.net/bluevillaimages/villa1.jpg", 550, "Premium Pool Villa", 4, 300.0 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
