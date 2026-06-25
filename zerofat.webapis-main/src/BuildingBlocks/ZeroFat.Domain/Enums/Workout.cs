using System.ComponentModel;
using System.Text.Json.Serialization;

namespace ZeroFat.Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TrainerType
{
    [Description("رياضي")]
    Athlete,
    [Description("مدرب")]
    Trainer
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Level
{
    [Description("مبتدا")]
    Beginner,
    [Description("متوسط")]
    Intermediate,
    [Description("متقدم")]
    Advanced
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Daytime
{
    [Description("صباحا")]
    AM,
    [Description("مساء")]
    PM
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum WorkoutFormat
{
    SetsReps,
    FollowAlong
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum GymEnvironment
{
    [Description("المنزل")]
    Home,
    [Description("النادي")]
    Gym
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ExerciseType
{
    Reps,
    WeightAndReps,
    Duration
}
