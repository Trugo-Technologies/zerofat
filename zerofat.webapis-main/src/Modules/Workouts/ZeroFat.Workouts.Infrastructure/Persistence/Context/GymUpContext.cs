using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ZeroFat.Domain.Common.Contracts;
using MediatR;
using ZeroFat.Infrastructure.Persistence.Configurations;

namespace ZeroFat.GymUp.Infrastructure.Persistence.Context;

public class GymUpContext : IPDbContext
{
    public GymUpContext(IPublisher publisher, DbContextOptions<GymUpContext> options, ICurrentUser currentUser, IOptions<DatabaseOptions> settings) : base(publisher, options, currentUser, settings)
    {
    }

    #region Catalog 
    public DbSet<BodyPart> BodyParts => Set<BodyPart>();
    public DbSet<Equipment> Equipments => Set<Equipment>();
    public DbSet<EquipmentCategory> EquipmentCategories => Set<EquipmentCategory>();
    public DbSet<PlanGoal> PlanGoals => Set<PlanGoal>();
    public DbSet<WorkoutType> WorkoutTypes => Set<WorkoutType>();

    #endregion

    #region Creator

    public DbSet<Trainer> Trainers => Set<Trainer>();
    public DbSet<Workout> Workouts => Set<Workout>();
    public DbSet<WorkoutBodyPart> WorkoutBodyParts => Set<WorkoutBodyPart>();
    public DbSet<WorkoutEquipment> WorkoutEquipments => Set<WorkoutEquipment>();
    public DbSet<WorkoutExercise> WorkoutExercises => Set<WorkoutExercise>();
    public DbSet<PlanSchedule> PlanSchedules => Set<PlanSchedule>();
    public DbSet<Exercise> Exercises => Set<Exercise>();
    public DbSet<ExerciseBodyPart> ExerciseBodyParts => Set<ExerciseBodyPart>();
    public DbSet<Plan> Plans => Set<Plan>();
    public DbSet<PlanWishlist> PlanWishlists => Set<PlanWishlist>();
    public DbSet<PlanReview> PlanReviews => Set<PlanReview>();
    public DbSet<ClientWorkout> ClientWorkouts => Set<ClientWorkout>();

    #endregion

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.AppendGlobalQueryFilter<ISoftDelete>(s => s.DeletedOn == null);
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);

        modelBuilder.Entity<Client>().ToTable("Clients", schema: "Client", x => x.ExcludeFromMigrations());

    }
}
