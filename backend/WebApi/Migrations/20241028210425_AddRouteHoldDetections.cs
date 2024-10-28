using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace WebApi.Migrations
{
    /// <inheritdoc />
    public partial class AddRouteHoldDetections : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "BoulderingRoute",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<byte[]>(
                name: "MaskedPicture",
                table: "BoulderingRoute",
                type: "bytea",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "OriginalPicture",
                table: "BoulderingRoute",
                type: "bytea",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "HoldDetection",
                columns: table => new
                {
                    BoulderingRouteId = table.Column<Guid>(type: "uuid", nullable: false),
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    X = table.Column<int>(type: "integer", nullable: false),
                    Y = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HoldDetection", x => new { x.BoulderingRouteId, x.Id });
                    table.ForeignKey(
                        name: "FK_HoldDetection_BoulderingRoute_BoulderingRouteId",
                        column: x => x.BoulderingRouteId,
                        principalTable: "BoulderingRoute",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HoldDetection");

            migrationBuilder.DropColumn(
                name: "Color",
                table: "BoulderingRoute");

            migrationBuilder.DropColumn(
                name: "MaskedPicture",
                table: "BoulderingRoute");

            migrationBuilder.DropColumn(
                name: "OriginalPicture",
                table: "BoulderingRoute");
        }
    }
}
