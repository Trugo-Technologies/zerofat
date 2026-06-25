using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrators.PostgreSQL.Migrations.GymUp
{
    /// <inheritdoc />
    public partial class initGymUpTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "GymUp");

            migrationBuilder.CreateTable(
                name: "BodyParts",
                schema: "GymUp",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NameAr = table.Column<string>(type: "text", nullable: false),
                    NameEn = table.Column<string>(type: "text", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedByName = table.Column<string>(type: "text", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedByName = table.Column<string>(type: "text", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedByName = table.Column<string>(type: "text", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    ActivationChangedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ActivationChangedByName = table.Column<string>(type: "text", nullable: true),
                    ActivationChangedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BodyParts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EquipmentCategories",
                schema: "GymUp",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NameAr = table.Column<string>(type: "text", nullable: false),
                    NameEn = table.Column<string>(type: "text", nullable: false),
                    IconUrl = table.Column<string>(type: "text", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedByName = table.Column<string>(type: "text", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedByName = table.Column<string>(type: "text", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedByName = table.Column<string>(type: "text", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    ActivationChangedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ActivationChangedByName = table.Column<string>(type: "text", nullable: true),
                    ActivationChangedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PlanGoals",
                schema: "GymUp",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NameAr = table.Column<string>(type: "text", nullable: false),
                    NameEn = table.Column<string>(type: "text", nullable: false),
                    ImageUrl = table.Column<string>(type: "text", nullable: true),
                    ThumbnailUrl = table.Column<string>(type: "text", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedByName = table.Column<string>(type: "text", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedByName = table.Column<string>(type: "text", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedByName = table.Column<string>(type: "text", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    ActivationChangedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ActivationChangedByName = table.Column<string>(type: "text", nullable: true),
                    ActivationChangedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanGoals", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Trainers",
                schema: "GymUp",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NameAr = table.Column<string>(type: "text", nullable: false),
                    NameEn = table.Column<string>(type: "text", nullable: false),
                    AvatarImageUrl = table.Column<string>(type: "text", nullable: true),
                    ProfileMediaUrl = table.Column<string>(type: "text", nullable: true),
                    BriefAr = table.Column<string>(type: "text", nullable: true),
                    BriefEn = table.Column<string>(type: "text", nullable: true),
                    DescriptionAr = table.Column<string>(type: "text", nullable: true),
                    DescriptionEn = table.Column<string>(type: "text", nullable: true),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    SpecialisesIn = table.Column<List<string>>(type: "text[]", nullable: false),
                    InstagramUrl = table.Column<string>(type: "text", nullable: true),
                    FacebookUrl = table.Column<string>(type: "text", nullable: true),
                    PinterestUrl = table.Column<string>(type: "text", nullable: true),
                    YoutubeUrl = table.Column<string>(type: "text", nullable: true),
                    WebsiteUrl = table.Column<string>(type: "text", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedByName = table.Column<string>(type: "text", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedByName = table.Column<string>(type: "text", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedByName = table.Column<string>(type: "text", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    ActivationChangedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ActivationChangedByName = table.Column<string>(type: "text", nullable: true),
                    ActivationChangedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trainers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WorkoutTypes",
                schema: "GymUp",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NameAr = table.Column<string>(type: "text", nullable: false),
                    NameEn = table.Column<string>(type: "text", nullable: false),
                    IconUrl = table.Column<string>(type: "text", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedByName = table.Column<string>(type: "text", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedByName = table.Column<string>(type: "text", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedByName = table.Column<string>(type: "text", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    ActivationChangedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ActivationChangedByName = table.Column<string>(type: "text", nullable: true),
                    ActivationChangedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkoutTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Equipments",
                schema: "GymUp",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NameAr = table.Column<string>(type: "text", nullable: false),
                    NameEn = table.Column<string>(type: "text", nullable: false),
                    ImageUrl = table.Column<string>(type: "text", nullable: true),
                    CategoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedByName = table.Column<string>(type: "text", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedByName = table.Column<string>(type: "text", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedByName = table.Column<string>(type: "text", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    ActivationChangedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ActivationChangedByName = table.Column<string>(type: "text", nullable: true),
                    ActivationChangedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Equipments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Equipments_EquipmentCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalSchema: "GymUp",
                        principalTable: "EquipmentCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Exercises",
                schema: "GymUp",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NameAr = table.Column<string>(type: "text", nullable: false),
                    NameEn = table.Column<string>(type: "text", nullable: false),
                    TrainerId = table.Column<Guid>(type: "uuid", nullable: true),
                    AvatarImageUrl = table.Column<string>(type: "text", nullable: true),
                    MediaUrl = table.Column<string>(type: "text", nullable: true),
                    GifUrl = table.Column<string>(type: "text", nullable: true),
                    InstructionsAr = table.Column<string>(type: "text", nullable: true),
                    InstructionsEn = table.Column<string>(type: "text", nullable: true),
                    Type = table.Column<int>(type: "integer", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedByName = table.Column<string>(type: "text", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedByName = table.Column<string>(type: "text", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedByName = table.Column<string>(type: "text", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    ActivationChangedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ActivationChangedByName = table.Column<string>(type: "text", nullable: true),
                    ActivationChangedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exercises", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Exercises_Trainers_TrainerId",
                        column: x => x.TrainerId,
                        principalSchema: "GymUp",
                        principalTable: "Trainers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Plans",
                schema: "GymUp",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NameAr = table.Column<string>(type: "text", nullable: true),
                    NameEn = table.Column<string>(type: "text", nullable: true),
                    AvatarImageUrl = table.Column<string>(type: "text", nullable: true),
                    ProfileMediaUrl = table.Column<string>(type: "text", nullable: true),
                    DaysPerWeek = table.Column<int>(type: "integer", nullable: false),
                    WeeklyRepetition = table.Column<int>(type: "integer", nullable: true),
                    TrainerId = table.Column<Guid>(type: "uuid", nullable: true),
                    PlanGoalId = table.Column<Guid>(type: "uuid", nullable: true),
                    EquipmentCategoryId = table.Column<Guid>(type: "uuid", nullable: true),
                    Environment = table.Column<int>(type: "integer", nullable: true),
                    Level = table.Column<int>(type: "integer", nullable: false),
                    OverviewAr = table.Column<string>(type: "text", nullable: true),
                    OverviewEn = table.Column<string>(type: "text", nullable: true),
                    PlanConclusionEn = table.Column<string>(type: "text", nullable: true),
                    PlanConclusionAr = table.Column<string>(type: "text", nullable: true),
                    RestDays = table.Column<List<int>>(type: "integer[]", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedByName = table.Column<string>(type: "text", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedByName = table.Column<string>(type: "text", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedByName = table.Column<string>(type: "text", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    ActivationChangedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ActivationChangedByName = table.Column<string>(type: "text", nullable: true),
                    ActivationChangedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Plans_EquipmentCategories_EquipmentCategoryId",
                        column: x => x.EquipmentCategoryId,
                        principalSchema: "GymUp",
                        principalTable: "EquipmentCategories",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Plans_PlanGoals_PlanGoalId",
                        column: x => x.PlanGoalId,
                        principalSchema: "GymUp",
                        principalTable: "PlanGoals",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Plans_Trainers_TrainerId",
                        column: x => x.TrainerId,
                        principalSchema: "GymUp",
                        principalTable: "Trainers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Workouts",
                schema: "GymUp",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NameAr = table.Column<string>(type: "text", nullable: false),
                    NameEn = table.Column<string>(type: "text", nullable: false),
                    AvatarImageUrl = table.Column<string>(type: "text", nullable: true),
                    ProfileMediaUrl = table.Column<string>(type: "text", nullable: true),
                    OverviewAr = table.Column<string>(type: "text", nullable: true),
                    OverviewEn = table.Column<string>(type: "text", nullable: true),
                    DurationInMins = table.Column<int>(type: "integer", nullable: false),
                    CaloriesBurned = table.Column<int>(type: "integer", nullable: false),
                    Level = table.Column<int>(type: "integer", nullable: false),
                    Format = table.Column<int>(type: "integer", nullable: false),
                    TrainerId = table.Column<Guid>(type: "uuid", nullable: true),
                    WorkoutTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    SetNamesEn = table.Column<List<string>>(type: "text[]", nullable: true),
                    SetNamesAr = table.Column<List<string>>(type: "text[]", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedByName = table.Column<string>(type: "text", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedByName = table.Column<string>(type: "text", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedByName = table.Column<string>(type: "text", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    ActivationChangedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ActivationChangedByName = table.Column<string>(type: "text", nullable: true),
                    ActivationChangedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Workouts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Workouts_Trainers_TrainerId",
                        column: x => x.TrainerId,
                        principalSchema: "GymUp",
                        principalTable: "Trainers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Workouts_WorkoutTypes_WorkoutTypeId",
                        column: x => x.WorkoutTypeId,
                        principalSchema: "GymUp",
                        principalTable: "WorkoutTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExerciseBodyParts",
                schema: "GymUp",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ExerciseId = table.Column<Guid>(type: "uuid", nullable: false),
                    BodyPartId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExerciseBodyParts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExerciseBodyParts_BodyParts_BodyPartId",
                        column: x => x.BodyPartId,
                        principalSchema: "GymUp",
                        principalTable: "BodyParts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExerciseBodyParts_Exercises_ExerciseId",
                        column: x => x.ExerciseId,
                        principalSchema: "GymUp",
                        principalTable: "Exercises",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlanReviews",
                schema: "GymUp",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PlanId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    EffectivenessRate = table.Column<double>(type: "double precision", nullable: false),
                    EasyToUseRate = table.Column<double>(type: "double precision", nullable: false),
                    EnjoyabilityRate = table.Column<double>(type: "double precision", nullable: false),
                    TotalRate = table.Column<double>(type: "double precision", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: true),
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
                    table.PrimaryKey("PK_PlanReviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlanReviews_Clients_UserId",
                        column: x => x.UserId,
                        principalSchema: "Client",
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlanReviews_Plans_PlanId",
                        column: x => x.PlanId,
                        principalSchema: "GymUp",
                        principalTable: "Plans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlanWishlists",
                schema: "GymUp",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PlanId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
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
                    table.PrimaryKey("PK_PlanWishlists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlanWishlists_Plans_PlanId",
                        column: x => x.PlanId,
                        principalSchema: "GymUp",
                        principalTable: "Plans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClientWorkouts",
                schema: "GymUp",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkoutId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    Calories = table.Column<double>(type: "double precision", nullable: false),
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
                    table.PrimaryKey("PK_ClientWorkouts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientWorkouts_Workouts_WorkoutId",
                        column: x => x.WorkoutId,
                        principalSchema: "GymUp",
                        principalTable: "Workouts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlanSchedules",
                schema: "GymUp",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Day = table.Column<int>(type: "integer", nullable: false),
                    Index = table.Column<int>(type: "integer", nullable: false),
                    Daytime = table.Column<int>(type: "integer", nullable: true),
                    PlanId = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkoutId = table.Column<Guid>(type: "uuid", nullable: true),
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
                    table.PrimaryKey("PK_PlanSchedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlanSchedules_Plans_PlanId",
                        column: x => x.PlanId,
                        principalSchema: "GymUp",
                        principalTable: "Plans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlanSchedules_Workouts_WorkoutId",
                        column: x => x.WorkoutId,
                        principalSchema: "GymUp",
                        principalTable: "Workouts",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "WorkoutBodyParts",
                schema: "GymUp",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkoutId = table.Column<Guid>(type: "uuid", nullable: false),
                    BodyPartId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkoutBodyParts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkoutBodyParts_BodyParts_BodyPartId",
                        column: x => x.BodyPartId,
                        principalSchema: "GymUp",
                        principalTable: "BodyParts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkoutBodyParts_Workouts_WorkoutId",
                        column: x => x.WorkoutId,
                        principalSchema: "GymUp",
                        principalTable: "Workouts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkoutEquipments",
                schema: "GymUp",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkoutId = table.Column<Guid>(type: "uuid", nullable: false),
                    EquipmentId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkoutEquipments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkoutEquipments_Equipments_EquipmentId",
                        column: x => x.EquipmentId,
                        principalSchema: "GymUp",
                        principalTable: "Equipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkoutEquipments_Workouts_WorkoutId",
                        column: x => x.WorkoutId,
                        principalSchema: "GymUp",
                        principalTable: "Workouts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkoutExercises",
                schema: "GymUp",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkoutId = table.Column<Guid>(type: "uuid", nullable: false),
                    ExerciseId = table.Column<Guid>(type: "uuid", nullable: false),
                    Index = table.Column<int>(type: "integer", nullable: false),
                    SetNameEn = table.Column<string>(type: "text", nullable: true),
                    SetNameAr = table.Column<string>(type: "text", nullable: true),
                    SetIndex = table.Column<int>(type: "integer", nullable: true),
                    Reps = table.Column<int>(type: "integer", nullable: true),
                    Weight = table.Column<int>(type: "integer", nullable: true),
                    DurationInSec = table.Column<int>(type: "integer", nullable: true),
                    Sets = table.Column<int>(type: "integer", nullable: true),
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
                    table.PrimaryKey("PK_WorkoutExercises", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkoutExercises_Exercises_ExerciseId",
                        column: x => x.ExerciseId,
                        principalSchema: "GymUp",
                        principalTable: "Exercises",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkoutExercises_Workouts_WorkoutId",
                        column: x => x.WorkoutId,
                        principalSchema: "GymUp",
                        principalTable: "Workouts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClientWorkouts_WorkoutId",
                schema: "GymUp",
                table: "ClientWorkouts",
                column: "WorkoutId");

            migrationBuilder.CreateIndex(
                name: "IX_Equipments_CategoryId",
                schema: "GymUp",
                table: "Equipments",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ExerciseBodyParts_BodyPartId",
                schema: "GymUp",
                table: "ExerciseBodyParts",
                column: "BodyPartId");

            migrationBuilder.CreateIndex(
                name: "IX_ExerciseBodyParts_ExerciseId",
                schema: "GymUp",
                table: "ExerciseBodyParts",
                column: "ExerciseId");

            migrationBuilder.CreateIndex(
                name: "IX_Exercises_TrainerId",
                schema: "GymUp",
                table: "Exercises",
                column: "TrainerId");

            migrationBuilder.CreateIndex(
                name: "IX_PlanReviews_PlanId",
                schema: "GymUp",
                table: "PlanReviews",
                column: "PlanId");

            migrationBuilder.CreateIndex(
                name: "IX_PlanReviews_UserId",
                schema: "GymUp",
                table: "PlanReviews",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Plans_EquipmentCategoryId",
                schema: "GymUp",
                table: "Plans",
                column: "EquipmentCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Plans_PlanGoalId",
                schema: "GymUp",
                table: "Plans",
                column: "PlanGoalId");

            migrationBuilder.CreateIndex(
                name: "IX_Plans_TrainerId",
                schema: "GymUp",
                table: "Plans",
                column: "TrainerId");

            migrationBuilder.CreateIndex(
                name: "IX_PlanSchedules_PlanId",
                schema: "GymUp",
                table: "PlanSchedules",
                column: "PlanId");

            migrationBuilder.CreateIndex(
                name: "IX_PlanSchedules_WorkoutId",
                schema: "GymUp",
                table: "PlanSchedules",
                column: "WorkoutId");

            migrationBuilder.CreateIndex(
                name: "IX_PlanWishlists_PlanId",
                schema: "GymUp",
                table: "PlanWishlists",
                column: "PlanId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutBodyParts_BodyPartId",
                schema: "GymUp",
                table: "WorkoutBodyParts",
                column: "BodyPartId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutBodyParts_WorkoutId",
                schema: "GymUp",
                table: "WorkoutBodyParts",
                column: "WorkoutId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutEquipments_EquipmentId",
                schema: "GymUp",
                table: "WorkoutEquipments",
                column: "EquipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutEquipments_WorkoutId",
                schema: "GymUp",
                table: "WorkoutEquipments",
                column: "WorkoutId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutExercises_ExerciseId",
                schema: "GymUp",
                table: "WorkoutExercises",
                column: "ExerciseId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutExercises_WorkoutId",
                schema: "GymUp",
                table: "WorkoutExercises",
                column: "WorkoutId");

            migrationBuilder.CreateIndex(
                name: "IX_Workouts_TrainerId",
                schema: "GymUp",
                table: "Workouts",
                column: "TrainerId");

            migrationBuilder.CreateIndex(
                name: "IX_Workouts_WorkoutTypeId",
                schema: "GymUp",
                table: "Workouts",
                column: "WorkoutTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClientWorkouts",
                schema: "GymUp");

            migrationBuilder.DropTable(
                name: "ExerciseBodyParts",
                schema: "GymUp");

            migrationBuilder.DropTable(
                name: "PlanReviews",
                schema: "GymUp");

            migrationBuilder.DropTable(
                name: "PlanSchedules",
                schema: "GymUp");

            migrationBuilder.DropTable(
                name: "PlanWishlists",
                schema: "GymUp");

            migrationBuilder.DropTable(
                name: "WorkoutBodyParts",
                schema: "GymUp");

            migrationBuilder.DropTable(
                name: "WorkoutEquipments",
                schema: "GymUp");

            migrationBuilder.DropTable(
                name: "WorkoutExercises",
                schema: "GymUp");

            migrationBuilder.DropTable(
                name: "Plans",
                schema: "GymUp");

            migrationBuilder.DropTable(
                name: "BodyParts",
                schema: "GymUp");

            migrationBuilder.DropTable(
                name: "Equipments",
                schema: "GymUp");

            migrationBuilder.DropTable(
                name: "Exercises",
                schema: "GymUp");

            migrationBuilder.DropTable(
                name: "Workouts",
                schema: "GymUp");

            migrationBuilder.DropTable(
                name: "PlanGoals",
                schema: "GymUp");

            migrationBuilder.DropTable(
                name: "EquipmentCategories",
                schema: "GymUp");

            migrationBuilder.DropTable(
                name: "Trainers",
                schema: "GymUp");

            migrationBuilder.DropTable(
                name: "WorkoutTypes",
                schema: "GymUp");
        }
    }
}
