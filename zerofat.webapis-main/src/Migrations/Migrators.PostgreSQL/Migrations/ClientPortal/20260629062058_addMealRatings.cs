using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrators.PostgreSQL.Migrations.ClientPortal
{
    /// <inheritdoc />
    public partial class addMealRatings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BlockOption",
                schema: "Client",
                table: "Clients",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "BlockedOn",
                schema: "Client",
                table: "Clients",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "BlockedUntil",
                schema: "Client",
                table: "Clients",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MealRatings",
                schema: "Client",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ClientId = table.Column<Guid>(type: "uuid", nullable: false),
                    DailyMealSelectionId = table.Column<Guid>(type: "uuid", nullable: false),
                    MealId = table.Column<Guid>(type: "uuid", nullable: true),
                    MealName = table.Column<string>(type: "text", nullable: true),
                    MealDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Rating = table.Column<int>(type: "integer", nullable: false),
                    ImprovementTags = table.Column<string>(type: "jsonb", nullable: false),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    AdminReply = table.Column<string>(type: "text", nullable: true),
                    AdminRepliedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AdminRepliedByName = table.Column<string>(type: "text", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedByName = table.Column<string>(type: "text", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedByName = table.Column<string>(type: "text", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedByName = table.Column<string>(type: "text", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MealRatings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MealRatings_Clients_ClientId",
                        column: x => x.ClientId,
                        principalSchema: "Client",
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MealRatings_DailyMealSelections_DailyMealSelectionId",
                        column: x => x.DailyMealSelectionId,
                        principalSchema: "Client",
                        principalTable: "DailyMealSelections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MealRatings_ClientId",
                schema: "Client",
                table: "MealRatings",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_MealRatings_CreatedOn",
                schema: "Client",
                table: "MealRatings",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_MealRatings_DailyMealSelectionId",
                schema: "Client",
                table: "MealRatings",
                column: "DailyMealSelectionId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MealRatings",
                schema: "Client");

            migrationBuilder.DropColumn(
                name: "BlockOption",
                schema: "Client",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "BlockedOn",
                schema: "Client",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "BlockedUntil",
                schema: "Client",
                table: "Clients");
        }
    }
}
