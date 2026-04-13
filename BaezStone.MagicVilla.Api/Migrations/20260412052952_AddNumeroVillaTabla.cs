using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BaezStone.MagicVilla.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddNumeroVillaTabla : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NumeroVillas",
                columns: table => new
                {
                    VillaNro = table.Column<int>(type: "int", nullable: false),
                    VillaId = table.Column<int>(type: "int", nullable: false),
                    DetalleEspecial = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NumeroVillas", x => x.VillaNro);
                    table.ForeignKey(
                        name: "FK_NumeroVillas_Villas_VillaId",
                        column: x => x.VillaId,
                        principalTable: "Villas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "FechaActualizacion", "FechaCreacion" },
                values: new object[] { new DateTime(2026, 4, 12, 0, 29, 49, 855, DateTimeKind.Local).AddTicks(2880), new DateTime(2026, 4, 12, 0, 29, 49, 855, DateTimeKind.Local).AddTicks(2870) });

            migrationBuilder.UpdateData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "FechaActualizacion", "FechaCreacion" },
                values: new object[] { new DateTime(2026, 4, 12, 0, 29, 49, 855, DateTimeKind.Local).AddTicks(2883), new DateTime(2026, 4, 12, 0, 29, 49, 855, DateTimeKind.Local).AddTicks(2882) });

            migrationBuilder.CreateIndex(
                name: "IX_NumeroVillas_VillaId",
                table: "NumeroVillas",
                column: "VillaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NumeroVillas");

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
    }
}
