using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi.Migrations
{
    /// <inheritdoc />
    public partial class AddBoulderingRoute : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BoulderingRoute",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GymId = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                    Index = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BoulderingRoute", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BoulderingRoute_GymId_Code",
                table: "BoulderingRoute",
                columns: new[] { "GymId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BoulderingRoute_GymId_Index",
                table: "BoulderingRoute",
                columns: new[] { "GymId", "Index" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BoulderingRoute");
        }
    }
}
