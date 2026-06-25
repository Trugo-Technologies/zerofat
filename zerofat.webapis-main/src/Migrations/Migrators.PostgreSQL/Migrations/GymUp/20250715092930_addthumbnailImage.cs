using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrators.PostgreSQL.Migrations.GymUp
{
    /// <inheritdoc />
    public partial class addthumbnailImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ThumbnailUrl",
                schema: "GymUp",
                table: "PlanGoals",
                newName: "ImageThumbnailUrl");

            migrationBuilder.AddColumn<string>(
                name: "AvatarImageOptimzeUrl",
                schema: "GymUp",
                table: "Workouts",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AvatarImageThumbnailUrl",
                schema: "GymUp",
                table: "Workouts",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AvatarImageOptimzeUrl",
                schema: "GymUp",
                table: "Trainers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AvatarImageThumbnailUrl",
                schema: "GymUp",
                table: "Trainers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AvatarImageOptimzeUrl",
                schema: "GymUp",
                table: "Plans",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AvatarImageThumbnailUrl",
                schema: "GymUp",
                table: "Plans",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageOptimzeUrl",
                schema: "GymUp",
                table: "PlanGoals",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AvatarImageOptimzeUrl",
                schema: "GymUp",
                table: "Exercises",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AvatarImageThumbnailUrl",
                schema: "GymUp",
                table: "Exercises",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageOptimzeUrl",
                schema: "GymUp",
                table: "Equipments",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageThumbnailUrl",
                schema: "GymUp",
                table: "Equipments",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvatarImageOptimzeUrl",
                schema: "GymUp",
                table: "Workouts");

            migrationBuilder.DropColumn(
                name: "AvatarImageThumbnailUrl",
                schema: "GymUp",
                table: "Workouts");

            migrationBuilder.DropColumn(
                name: "AvatarImageOptimzeUrl",
                schema: "GymUp",
                table: "Trainers");

            migrationBuilder.DropColumn(
                name: "AvatarImageThumbnailUrl",
                schema: "GymUp",
                table: "Trainers");

            migrationBuilder.DropColumn(
                name: "AvatarImageOptimzeUrl",
                schema: "GymUp",
                table: "Plans");

            migrationBuilder.DropColumn(
                name: "AvatarImageThumbnailUrl",
                schema: "GymUp",
                table: "Plans");

            migrationBuilder.DropColumn(
                name: "ImageOptimzeUrl",
                schema: "GymUp",
                table: "PlanGoals");

            migrationBuilder.DropColumn(
                name: "AvatarImageOptimzeUrl",
                schema: "GymUp",
                table: "Exercises");

            migrationBuilder.DropColumn(
                name: "AvatarImageThumbnailUrl",
                schema: "GymUp",
                table: "Exercises");

            migrationBuilder.DropColumn(
                name: "ImageOptimzeUrl",
                schema: "GymUp",
                table: "Equipments");

            migrationBuilder.DropColumn(
                name: "ImageThumbnailUrl",
                schema: "GymUp",
                table: "Equipments");

            migrationBuilder.RenameColumn(
                name: "ImageThumbnailUrl",
                schema: "GymUp",
                table: "PlanGoals",
                newName: "ThumbnailUrl");
        }
    }
}
