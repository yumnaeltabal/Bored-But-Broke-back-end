using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bored_But_Broke_back_end.Migrations
{
    /// <inheritdoc />
    public partial class AddFavourites : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Places",
                columns: table => new
                {
                    PlaceId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PlaceName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Location_Address1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Location_Address2 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Location_Address3 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Location_City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Location_ZipCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Location_Country = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Location_State = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Location_DisplayAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<int>(type: "int", nullable: true),
                    Coordinates_Latitude = table.Column<double>(type: "float", nullable: true),
                    Coordinates_Longitude = table.Column<double>(type: "float", nullable: true),
                    Rating = table.Column<double>(type: "float", nullable: true),
                    PlaceUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Places", x => x.PlaceId);
                });

            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    PlaceId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Alias = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => new { x.PlaceId, x.Id });
                    table.ForeignKey(
                        name: "FK_Category_Places_PlaceId",
                        column: x => x.PlaceId,
                        principalTable: "Places",
                        principalColumn: "PlaceId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Favourites",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PlaceId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Favourites", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Favourites_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Favourites_Places_PlaceId",
                        column: x => x.PlaceId,
                        principalTable: "Places",
                        principalColumn: "PlaceId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OpeningHours",
                columns: table => new
                {
                    PlaceId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HoursType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsOpenNow = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpeningHours", x => new { x.PlaceId, x.Id });
                    table.ForeignKey(
                        name: "FK_OpeningHours_Places_PlaceId",
                        column: x => x.PlaceId,
                        principalTable: "Places",
                        principalColumn: "PlaceId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Hour",
                columns: table => new
                {
                    OpeningHoursPlaceId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    OpeningHoursId = table.Column<int>(type: "int", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Day = table.Column<int>(type: "int", nullable: true),
                    Start = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    End = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hour", x => new { x.OpeningHoursPlaceId, x.OpeningHoursId, x.Id });
                    table.ForeignKey(
                        name: "FK_Hour_OpeningHours_OpeningHoursPlaceId_OpeningHoursId",
                        columns: x => new { x.OpeningHoursPlaceId, x.OpeningHoursId },
                        principalTable: "OpeningHours",
                        principalColumns: new[] { "PlaceId", "Id" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Favourites_PlaceId",
                table: "Favourites",
                column: "PlaceId");

            migrationBuilder.CreateIndex(
                name: "IX_Favourites_UserId_PlaceId",
                table: "Favourites",
                columns: new[] { "UserId", "PlaceId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Category");

            migrationBuilder.DropTable(
                name: "Favourites");

            migrationBuilder.DropTable(
                name: "Hour");

            migrationBuilder.DropTable(
                name: "OpeningHours");

            migrationBuilder.DropTable(
                name: "Places");
        }
    }
}
