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
                name: "OriginalPicture_Data",
                table: "BoulderingRoute",
                type: "bytea",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<long>(
                name: "OriginalPicture_OriginalHeight",
                table: "BoulderingRoute",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "OriginalPicture_OriginalWidth",
                table: "BoulderingRoute",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "RouteHold",
                columns: table => new
                {
                    BoulderingRouteId = table.Column<Guid>(type: "uuid", nullable: false),
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SegmentedPicture = table.Column<byte[]>(type: "bytea", nullable: false),
                    X = table.Column<double>(type: "double precision", nullable: false),
                    Y = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RouteHold", x => new { x.BoulderingRouteId, x.Id });
                    table.ForeignKey(
                        name: "FK_RouteHold_BoulderingRoute_BoulderingRouteId",
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
                name: "RouteHold");

            migrationBuilder.DropColumn(
                name: "Color",
                table: "BoulderingRoute");

            migrationBuilder.DropColumn(
                name: "OriginalPicture_Data",
                table: "BoulderingRoute");

            migrationBuilder.DropColumn(
                name: "OriginalPicture_OriginalHeight",
                table: "BoulderingRoute");

            migrationBuilder.DropColumn(
                name: "OriginalPicture_OriginalWidth",
                table: "BoulderingRoute");
        }
    }
}
