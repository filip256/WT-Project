using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PoliceMaps.Migrations
{
    public partial class migration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PoliceHotspots",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExternalId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Latitude = table.Column<double>(type: "float", nullable: false),
                    Longitude = table.Column<double>(type: "float", nullable: false),
                    Severity = table.Column<int>(type: "int", nullable: false),
                    LastUpdate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FirstOcurrence = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PoliceHotspots", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SurveyAreas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StartLatitude = table.Column<double>(type: "float", nullable: false),
                    StartLongitude = table.Column<double>(type: "float", nullable: false),
                    EndLatitude = table.Column<double>(type: "float", nullable: false),
                    EndLongitude = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SurveyAreas", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PoliceHotspots");

            migrationBuilder.DropTable(
                name: "SurveyAreas");
        }
    }
}
