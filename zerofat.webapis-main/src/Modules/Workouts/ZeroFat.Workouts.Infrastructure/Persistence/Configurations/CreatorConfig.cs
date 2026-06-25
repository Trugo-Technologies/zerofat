using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZeroFat.GymUp.Infrastructure.Persistence.Configurations;

namespace ZeroFat.GymUp.Infrastructure.Persistence.Configurations;
public class ExerciseConfig : IEntityTypeConfiguration<Exercise>
{
    public void Configure(EntityTypeBuilder<Exercise> builder)
    {
        builder.ToTable("Exercises", SchemaNames.GymUp);
    }
}

public class ExerciseBodyPartConfig : IEntityTypeConfiguration<ExerciseBodyPart>
{
    public void Configure(EntityTypeBuilder<ExerciseBodyPart> builder)
    {
        builder.ToTable("ExerciseBodyParts", SchemaNames.GymUp);
    }
}

public class PlanConfig : IEntityTypeConfiguration<Plan>
{
    public void Configure(EntityTypeBuilder<Plan> builder)
    {
        builder.ToTable("Plans", SchemaNames.GymUp);
    }
}

public class PlanScheduleConfig : IEntityTypeConfiguration<PlanSchedule>
{
    public void Configure(EntityTypeBuilder<PlanSchedule> builder)
    {
        builder.ToTable("PlanSchedules", SchemaNames.GymUp);
    }
}

public class TrainerConfig : IEntityTypeConfiguration<Trainer>
{
    public void Configure(EntityTypeBuilder<Trainer> builder)
    {
        builder.ToTable("Trainers", SchemaNames.GymUp);
    }
}

public class WorkoutConfig : IEntityTypeConfiguration<Workout>
{
    public void Configure(EntityTypeBuilder<Workout> builder)
    {
        builder.ToTable("Workouts", SchemaNames.GymUp);
    }
}

public class WorkoutBodyPartConfig : IEntityTypeConfiguration<WorkoutBodyPart>
{
    public void Configure(EntityTypeBuilder<WorkoutBodyPart> builder)
    {
        builder.ToTable("WorkoutBodyParts", SchemaNames.GymUp);
    }
}

public class WorkoutEquipmentConfig : IEntityTypeConfiguration<WorkoutEquipment>
{
    public void Configure(EntityTypeBuilder<WorkoutEquipment> builder)
    {
        builder.ToTable("WorkoutEquipments", SchemaNames.GymUp);
    }
}

public class WorkoutExerciseConfig : IEntityTypeConfiguration<WorkoutExercise>
{
    public void Configure(EntityTypeBuilder<WorkoutExercise> builder)
    {
        builder.ToTable("WorkoutExercises", SchemaNames.GymUp);
    }
}

public class PlanWishlistConfig : IEntityTypeConfiguration<PlanWishlist>
{
    public void Configure(EntityTypeBuilder<PlanWishlist> builder)
    {
        builder.ToTable("PlanWishlists", SchemaNames.GymUp);
    }
}
public class PlanReviewConfig : IEntityTypeConfiguration<PlanReview>
{
    public void Configure(EntityTypeBuilder<PlanReview> builder)
    {
        builder.ToTable("PlanReviews", SchemaNames.GymUp);
    }
}

public class ClientWorkoutConfig : IEntityTypeConfiguration<ClientWorkout>
{
    public void Configure(EntityTypeBuilder<ClientWorkout> builder)
    {
        builder.ToTable("ClientWorkouts", SchemaNames.GymUp);
    }
}
