namespace ZeroFat.GymUp.Application.Statistics;

public class StatisticsDto
{
    public int WorkoutTypes { get; set; }
    public int BodyParts { get; set; }
    public int EquipmentCategories { get; set; }
    public int Equipments { get; set; }
    public int Trainers { get; set; }
    public int Plans { get; set; }
    public int Workouts { get; set; }
    public int Exercsises { get; set; }
    public int Reviews { get; set; }
    public Dictionary<string, int> PlansByLevel { get; set; } = new Dictionary<string, int>();
    public Dictionary<string, int> PlansByEnvironment { get; set; } = new Dictionary<string, int>();
    public Dictionary<string, int> WorkoutsByFormat { get; set; } = new Dictionary<string, int>();
    public Dictionary<string, int> MostReviewdPlans { get; set; } = new Dictionary<string, int>();
}
