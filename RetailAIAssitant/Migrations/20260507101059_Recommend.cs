using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RetailAIAssitant.Migrations
{
    /// <inheritdoc />
    public partial class Recommend : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RecommendationLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestedProduct = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RecommendedProduct = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Confidence = table.Column<double>(type: "float", nullable: false),
                    Similarity = table.Column<double>(type: "float", nullable: false),
                    FinalScore = table.Column<double>(type: "float", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecommendationLogs", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RecommendationLogs");
        }
    }
}
