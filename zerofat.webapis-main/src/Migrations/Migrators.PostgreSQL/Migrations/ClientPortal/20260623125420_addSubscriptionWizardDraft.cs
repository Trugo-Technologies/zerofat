using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrators.PostgreSQL.Migrations.ClientPortal
{
    /// <inheritdoc />
    public partial class addSubscriptionWizardDraft : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "AddOnAmount",
                schema: "Client",
                table: "ClientSubscriptions",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AddOnItems",
                schema: "Client",
                table: "ClientSubscriptions",
                type: "jsonb",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "CalorieTarget",
                schema: "Client",
                table: "ClientSubscriptions",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByAdminId",
                schema: "Client",
                table: "ClientSubscriptions",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ManualDiscountAed",
                schema: "Client",
                table: "ClientSubscriptions",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PlanVariant",
                schema: "Client",
                table: "ClientSubscriptions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PromoCode",
                schema: "Client",
                table: "ClientSubscriptions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProteinTargetG",
                schema: "Client",
                table: "ClientSubscriptions",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "VatAmount",
                schema: "Client",
                table: "ClientSubscriptions",
                type: "numeric",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SubscriptionWizardDrafts",
                schema: "Client",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ClientId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedByAdminId = table.Column<Guid>(type: "uuid", nullable: true),
                    WizardMode = table.Column<int>(type: "integer", nullable: false),
                    CurrentStep = table.Column<int>(type: "integer", nullable: false),
                    RenewalStrategy = table.Column<int>(type: "integer", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    MealPlanId = table.Column<Guid>(type: "uuid", nullable: true),
                    PlanVariant = table.Column<string>(type: "text", nullable: true),
                    CalorieTarget = table.Column<int>(type: "integer", nullable: true),
                    ProteinTargetG = table.Column<int>(type: "integer", nullable: true),
                    MealTypeSelections = table.Column<string>(type: "jsonb", nullable: false),
                    AddOnItems = table.Column<string>(type: "jsonb", nullable: false),
                    SubscriptionType = table.Column<int>(type: "integer", nullable: true),
                    SkipSaturdays = table.Column<bool>(type: "boolean", nullable: false),
                    SkipSundays = table.Column<bool>(type: "boolean", nullable: false),
                    SelectedDeliveryDates = table.Column<string>(type: "jsonb", nullable: false),
                    SelectedDeliveryDays = table.Column<string>(type: "jsonb", nullable: false),
                    PreferredDeliveryTime = table.Column<int>(type: "integer", nullable: true),
                    ClientLocationId = table.Column<Guid>(type: "uuid", nullable: true),
                    PromoCode = table.Column<string>(type: "text", nullable: true),
                    ManualDiscountAed = table.Column<decimal>(type: "numeric", nullable: false),
                    CustomerEmail = table.Column<string>(type: "text", nullable: true),
                    OptionalMessage = table.Column<string>(type: "text", nullable: true),
                    ScheduledStartDate = table.Column<DateOnly>(type: "date", nullable: true),
                    FinalizedClientSubscriptionId = table.Column<Guid>(type: "uuid", nullable: true),
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
                    table.PrimaryKey("PK_SubscriptionWizardDrafts", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SubscriptionWizardDrafts",
                schema: "Client");

            migrationBuilder.DropColumn(
                name: "AddOnAmount",
                schema: "Client",
                table: "ClientSubscriptions");

            migrationBuilder.DropColumn(
                name: "AddOnItems",
                schema: "Client",
                table: "ClientSubscriptions");

            migrationBuilder.DropColumn(
                name: "CalorieTarget",
                schema: "Client",
                table: "ClientSubscriptions");

            migrationBuilder.DropColumn(
                name: "CreatedByAdminId",
                schema: "Client",
                table: "ClientSubscriptions");

            migrationBuilder.DropColumn(
                name: "ManualDiscountAed",
                schema: "Client",
                table: "ClientSubscriptions");

            migrationBuilder.DropColumn(
                name: "PlanVariant",
                schema: "Client",
                table: "ClientSubscriptions");

            migrationBuilder.DropColumn(
                name: "PromoCode",
                schema: "Client",
                table: "ClientSubscriptions");

            migrationBuilder.DropColumn(
                name: "ProteinTargetG",
                schema: "Client",
                table: "ClientSubscriptions");

            migrationBuilder.DropColumn(
                name: "VatAmount",
                schema: "Client",
                table: "ClientSubscriptions");
        }
    }
}
