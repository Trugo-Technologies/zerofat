using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrators.PostgreSQL.Migrations.Core
{
    /// <inheritdoc />
    public partial class addthumbnailImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageOptimzeUrl",
                schema: "Core",
                table: "PhysicalActivityLevels",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageThumbnailUrl",
                schema: "Core",
                table: "PhysicalActivityLevels",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageOptimzeUrl",
                schema: "Core",
                table: "Advertisements",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageThumbnailUrl",
                schema: "Core",
                table: "Advertisements",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageOptimzeUrl",
                schema: "Core",
                table: "PhysicalActivityLevels");

            migrationBuilder.DropColumn(
                name: "ImageThumbnailUrl",
                schema: "Core",
                table: "PhysicalActivityLevels");

            migrationBuilder.DropColumn(
                name: "ImageOptimzeUrl",
                schema: "Core",
                table: "Advertisements");

            migrationBuilder.DropColumn(
                name: "ImageThumbnailUrl",
                schema: "Core",
                table: "Advertisements");
        }
    }
}
