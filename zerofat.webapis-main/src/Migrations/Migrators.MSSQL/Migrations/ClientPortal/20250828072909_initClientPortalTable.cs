using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrators.MSSQL.Migrations.ClientPortal
{
    /// <inheritdoc />
    public partial class initClientPortalTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Client");

            migrationBuilder.CreateTable(
                name: "ClientChats",
                schema: "Client",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChatDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ChatId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChatType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChannelId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChannelName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BaseDeviceId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClientId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeviceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientChats", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Clients",
                schema: "Client",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StripeId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Mobile = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Gender = table.Column<int>(type: "int", nullable: true),
                    BirthDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ClientAllergicIds = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HeightInCM = table.Column<double>(type: "float", nullable: true),
                    WeightInKG = table.Column<double>(type: "float", nullable: true),
                    CurrentWeightInKG = table.Column<double>(type: "float", nullable: true),
                    TargetWeightInKG = table.Column<double>(type: "float", nullable: true),
                    DietitianGoal = table.Column<int>(type: "int", nullable: false),
                    ActivityValue = table.Column<double>(type: "float", nullable: false),
                    ClientSubscriptionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    NewGoalStart = table.Column<DateOnly>(type: "date", nullable: true),
                    SubscriptionStatus = table.Column<int>(type: "int", nullable: false),
                    BMI = table.Column<double>(type: "float", nullable: false),
                    BMR = table.Column<double>(type: "float", nullable: false),
                    BodyFat = table.Column<double>(type: "float", nullable: false),
                    TDEE = table.Column<double>(type: "float", nullable: false),
                    TimeToReachGoalInDays = table.Column<int>(type: "int", nullable: false),
                    NeededCaloriesToReachGoal = table.Column<double>(type: "float", nullable: false),
                    AccountIsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedByName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedByName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedByName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ActivationChangedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ActivationChangedByName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActivationChangedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DiscountRules",
                schema: "Client",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DescriptionEn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DescriptionAr = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StripeId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PercentOff = table.Column<long>(type: "bigint", nullable: true),
                    AmountOff = table.Column<long>(type: "bigint", nullable: true),
                    Duration = table.Column<int>(type: "int", nullable: false),
                    DurationInMonths = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: true),
                    ExpirationDate = table.Column<DateOnly>(type: "date", nullable: true),
                    MaxRedemptions = table.Column<int>(type: "int", nullable: false),
                    RedemptionsUsed = table.Column<int>(type: "int", nullable: false),
                    MaxRedemptionsPerClient = table.Column<int>(type: "int", nullable: false),
                    FirstTimeClientsOnly = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedByName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedByName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedByName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ActivationChangedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ActivationChangedByName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActivationChangedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscountRules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ClientGoals",
                schema: "Client",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HeightInCM = table.Column<double>(type: "float", nullable: true),
                    WeightInKG = table.Column<double>(type: "float", nullable: true),
                    TargetWeightInKG = table.Column<double>(type: "float", nullable: true),
                    DietitianGoal = table.Column<int>(type: "int", nullable: false),
                    HeightMeasurement = table.Column<int>(type: "int", nullable: true),
                    WeightMeasurement = table.Column<int>(type: "int", nullable: true),
                    TargetWeightMeasurement = table.Column<int>(type: "int", nullable: true),
                    ActivityValue = table.Column<double>(type: "float", nullable: false),
                    PhysicalActivityLevelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BMI = table.Column<double>(type: "float", nullable: false),
                    BMR = table.Column<double>(type: "float", nullable: false),
                    BodyFat = table.Column<double>(type: "float", nullable: false),
                    TDEE = table.Column<double>(type: "float", nullable: false),
                    TimeToReachGoalInDays = table.Column<int>(type: "int", nullable: false),
                    NeededCaloriesToReachGoal = table.Column<double>(type: "float", nullable: false),
                    ClientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedByName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedByName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedByName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientGoals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientGoals_Clients_ClientId",
                        column: x => x.ClientId,
                        principalSchema: "Client",
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClientLocations",
                schema: "Client",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: true),
                    Longitude = table.Column<double>(type: "float", nullable: true),
                    Latitude = table.Column<double>(type: "float", nullable: true),
                    FullAddressEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FullAddressAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Area = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Building = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Office = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Street = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PostalCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NotesFromClient = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NotesFromZeroFat = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedByName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedByName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedByName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientLocations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientLocations_Clients_ClientId",
                        column: x => x.ClientId,
                        principalSchema: "Client",
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClientLoyaltyPoints",
                schema: "Client",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Points = table.Column<int>(type: "int", nullable: false),
                    DateEarned = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    ClientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TransactionId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Source = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedByName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedByName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedByName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientLoyaltyPoints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientLoyaltyPoints_Clients_ClientId",
                        column: x => x.ClientId,
                        principalSchema: "Client",
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClientPaymentMethodS",
                schema: "Client",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    StripeId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClientId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CardBrand = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CardExpMonth = table.Column<int>(type: "int", nullable: true),
                    CardFunding = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CardLast4 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CardName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedByName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedByName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedByName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientPaymentMethodS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientPaymentMethodS_Clients_ClientId",
                        column: x => x.ClientId,
                        principalSchema: "Client",
                        principalTable: "Clients",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ClientSubscriptions",
                schema: "Client",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MealPlanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClientLocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SubscriptionType = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: false),
                    SelectedDeliveryDays = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PreferredDeliveryTime = table.Column<int>(type: "int", nullable: false),
                    TotalCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AverageCalories = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaymentQuickLink = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaymentStatus = table.Column<int>(type: "int", nullable: false),
                    SubscriptionStatus = table.Column<int>(type: "int", nullable: false),
                    PaymentOrderId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StripeSubscriptionId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastSyncDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CancelAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsAutoRenewalEnabled = table.Column<bool>(type: "bit", nullable: false),
                    RenewalCount = table.Column<int>(type: "int", nullable: false),
                    NextRenewalDate = table.Column<DateOnly>(type: "date", nullable: true),
                    LastStatusUpdate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedByName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedByName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedByName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientSubscriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientSubscriptions_Clients_ClientId",
                        column: x => x.ClientId,
                        principalSchema: "Client",
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClientSubscriptions_MealPlans_MealPlanId",
                        column: x => x.MealPlanId,
                        principalSchema: "NutriPlan",
                        principalTable: "MealPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DailyHealthLogs",
                schema: "Client",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LogDate = table.Column<DateOnly>(type: "date", nullable: false),
                    WeightValue = table.Column<double>(type: "float", nullable: true),
                    WeightUnit = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    WaterIntakeLiters = table.Column<double>(type: "float", nullable: false),
                    TotalCaloriesConsumed = table.Column<double>(type: "float", nullable: false),
                    TotalCaloriesBurned = table.Column<double>(type: "float", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedByName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedByName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedByName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyHealthLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DailyHealthLogs_Clients_ClientId",
                        column: x => x.ClientId,
                        principalSchema: "Client",
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DiscountRedemptions",
                schema: "Client",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DiscountRuleId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ClientId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TimesUsed = table.Column<int>(type: "int", nullable: false),
                    LastUsedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscountRedemptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DiscountRedemptions_DiscountRules_DiscountRuleId",
                        column: x => x.DiscountRuleId,
                        principalSchema: "Client",
                        principalTable: "DiscountRules",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DailySelections",
                schema: "Client",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    DeliveryTime = table.Column<int>(type: "int", nullable: false),
                    HasCutlery = table.Column<bool>(type: "bit", nullable: false),
                    TotalCalories = table.Column<double>(type: "float", nullable: false),
                    TotalFats = table.Column<double>(type: "float", nullable: false),
                    TotalCarbohydrates = table.Column<double>(type: "float", nullable: false),
                    TotalProteins = table.Column<double>(type: "float", nullable: false),
                    DailySelectionStatus = table.Column<int>(type: "int", nullable: false),
                    ReplacementDate = table.Column<DateOnly>(type: "date", nullable: true),
                    MealPlanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClientSubscriptionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClientLocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedByName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedByName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedByName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailySelections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DailySelections_ClientLocations_ClientLocationId",
                        column: x => x.ClientLocationId,
                        principalSchema: "Client",
                        principalTable: "ClientLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DailySelections_Clients_ClientId",
                        column: x => x.ClientId,
                        principalSchema: "Client",
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "ClientOrders",
                schema: "Client",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClientSubscriptionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: false),
                    TotalCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaymentQuickLink = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MealDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaymentStatus = table.Column<int>(type: "int", nullable: false),
                    IsRecurring = table.Column<bool>(type: "bit", nullable: false),
                    ClientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedByName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedByName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedByName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ClientOrderItems = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientOrders_ClientSubscriptions_ClientSubscriptionId",
                        column: x => x.ClientSubscriptionId,
                        principalSchema: "Client",
                        principalTable: "ClientSubscriptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClientOrders_Clients_ClientId",
                        column: x => x.ClientId,
                        principalSchema: "Client",
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "MealTypeSelections",
                schema: "Client",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuantityPerDay = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MealTypeNameEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MealTypeNameAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MealTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClientSubscriptionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MealTypeSelections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MealTypeSelections_ClientSubscriptions_ClientSubscriptionId",
                        column: x => x.ClientSubscriptionId,
                        principalSchema: "Client",
                        principalTable: "ClientSubscriptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MealTypeSelections_MealTypes_MealTypeId",
                        column: x => x.MealTypeId,
                        principalSchema: "NutriPlan",
                        principalTable: "MealTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Payment",
                schema: "Client",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaymentMethod = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaymentStatus = table.Column<int>(type: "int", nullable: false),
                    TransactionId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrderId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InvoiceNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaymentGateway = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Currency = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDiscountApplied = table.Column<bool>(type: "bit", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClientSubscriptionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedByName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedByName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedByName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payment_ClientSubscriptions_ClientSubscriptionId",
                        column: x => x.ClientSubscriptionId,
                        principalSchema: "Client",
                        principalTable: "ClientSubscriptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Payment_Clients_ClientId",
                        column: x => x.ClientId,
                        principalSchema: "Client",
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "CalorieRecords",
                schema: "Client",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DailyHealthLogId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CalorieRecord_Calories = table.Column<double>(type: "float", nullable: false),
                    RecordType = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Calories = table.Column<double>(type: "float", nullable: true),
                    ProteinInGrams = table.Column<double>(type: "float", nullable: true),
                    CarbsInGrams = table.Column<double>(type: "float", nullable: true),
                    FatInGrams = table.Column<double>(type: "float", nullable: true),
                    RecordedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    MealTime = table.Column<TimeOnly>(type: "time(0)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalorieRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CalorieRecords_DailyHealthLogs_DailyHealthLogId",
                        column: x => x.DailyHealthLogId,
                        principalSchema: "Client",
                        principalTable: "DailyHealthLogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DailyMealSelections",
                schema: "Client",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    MealSelectionType = table.Column<int>(type: "int", nullable: false),
                    Qty = table.Column<int>(type: "int", nullable: false),
                    MealId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CustomMealId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    BasePrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AdjustedPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PriceAdjustmentReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TotalCalories = table.Column<double>(type: "float", nullable: false),
                    TotalFats = table.Column<double>(type: "float", nullable: false),
                    TotalCarbohydrates = table.Column<double>(type: "float", nullable: false),
                    TotalProteins = table.Column<double>(type: "float", nullable: false),
                    CustomeMealName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SpecialInstructions = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsConsumed = table.Column<bool>(type: "bit", nullable: false),
                    IsPaid = table.Column<bool>(type: "bit", nullable: false),
                    MealTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    MealPlanId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DailySelectionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClientSubscriptionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedByName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedByName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedByName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyMealSelections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DailyMealSelections_DailySelections_DailySelectionId",
                        column: x => x.DailySelectionId,
                        principalSchema: "Client",
                        principalTable: "DailySelections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DailyMealSelections_Meals_MealId",
                        column: x => x.MealId,
                        principalSchema: "NutriPlan",
                        principalTable: "Meals",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CalorieRecords_DailyHealthLogId",
                schema: "Client",
                table: "CalorieRecords",
                column: "DailyHealthLogId");

            migrationBuilder.CreateIndex(
                name: "IX_CalorieRecords_RecordType",
                schema: "Client",
                table: "CalorieRecords",
                column: "RecordType");

            migrationBuilder.CreateIndex(
                name: "IX_ClientGoals_ClientId",
                schema: "Client",
                table: "ClientGoals",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientLocations_ClientId",
                schema: "Client",
                table: "ClientLocations",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientLoyaltyPoints_ClientId",
                schema: "Client",
                table: "ClientLoyaltyPoints",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientOrders_ClientId",
                schema: "Client",
                table: "ClientOrders",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientOrders_ClientSubscriptionId",
                schema: "Client",
                table: "ClientOrders",
                column: "ClientSubscriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientPaymentMethodS_ClientId",
                schema: "Client",
                table: "ClientPaymentMethodS",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientSubscriptions_ClientId",
                schema: "Client",
                table: "ClientSubscriptions",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientSubscriptions_MealPlanId",
                schema: "Client",
                table: "ClientSubscriptions",
                column: "MealPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_DailyHealthLog_ClientDate",
                schema: "Client",
                table: "DailyHealthLogs",
                columns: new[] { "ClientId", "LogDate" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DailyHealthLog_LogDate",
                schema: "Client",
                table: "DailyHealthLogs",
                column: "LogDate");

            migrationBuilder.CreateIndex(
                name: "IX_DailyMealSelections_DailySelectionId",
                schema: "Client",
                table: "DailyMealSelections",
                column: "DailySelectionId");

            migrationBuilder.CreateIndex(
                name: "IX_DailyMealSelections_Date",
                schema: "Client",
                table: "DailyMealSelections",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_DailyMealSelections_MealId",
                schema: "Client",
                table: "DailyMealSelections",
                column: "MealId");

            migrationBuilder.CreateIndex(
                name: "IX_DailySelections_ClientId",
                schema: "Client",
                table: "DailySelections",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_DailySelections_ClientLocationId",
                schema: "Client",
                table: "DailySelections",
                column: "ClientLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_DailySelections_Date",
                schema: "Client",
                table: "DailySelections",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_DiscountRedemptions_DiscountRuleId",
                schema: "Client",
                table: "DiscountRedemptions",
                column: "DiscountRuleId");

            migrationBuilder.CreateIndex(
                name: "IX_MealTypeSelections_ClientSubscriptionId",
                schema: "Client",
                table: "MealTypeSelections",
                column: "ClientSubscriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_MealTypeSelections_MealTypeId",
                schema: "Client",
                table: "MealTypeSelections",
                column: "MealTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_ClientId",
                schema: "Client",
                table: "Payment",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_ClientSubscriptionId",
                schema: "Client",
                table: "Payment",
                column: "ClientSubscriptionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CalorieRecords",
                schema: "Client");

            migrationBuilder.DropTable(
                name: "ClientChats",
                schema: "Client");

            migrationBuilder.DropTable(
                name: "ClientGoals",
                schema: "Client");

            migrationBuilder.DropTable(
                name: "ClientLoyaltyPoints",
                schema: "Client");

            migrationBuilder.DropTable(
                name: "ClientOrders",
                schema: "Client");

            migrationBuilder.DropTable(
                name: "ClientPaymentMethodS",
                schema: "Client");

            migrationBuilder.DropTable(
                name: "DailyMealSelections",
                schema: "Client");

            migrationBuilder.DropTable(
                name: "DiscountRedemptions",
                schema: "Client");

            migrationBuilder.DropTable(
                name: "MealTypeSelections",
                schema: "Client");

            migrationBuilder.DropTable(
                name: "Payment",
                schema: "Client");

            migrationBuilder.DropTable(
                name: "DailyHealthLogs",
                schema: "Client");

            migrationBuilder.DropTable(
                name: "DailySelections",
                schema: "Client");

            migrationBuilder.DropTable(
                name: "DiscountRules",
                schema: "Client");

            migrationBuilder.DropTable(
                name: "ClientSubscriptions",
                schema: "Client");

            migrationBuilder.DropTable(
                name: "ClientLocations",
                schema: "Client");

            migrationBuilder.DropTable(
                name: "Clients",
                schema: "Client");
        }
    }
}
