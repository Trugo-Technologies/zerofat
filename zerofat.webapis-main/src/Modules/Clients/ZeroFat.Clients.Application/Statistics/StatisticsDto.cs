namespace ZeroFat.ClientPortal.Application.Statistics;

public class StatisticsDto
{
    public int Clients { get; set; }
    public int MaleCount { get; set; }
    public int FemaleCount { get; set; }
    public int Subscriptions  { get; set; }
    public int RecurringSubscriptions  { get; set; }
    public int Payments  { get; set; }
    public int TotalPaymentsLastMonth  { get; set; }
    public int Chats { get; set; }
    public int Locations { get; set; }
    public Dictionary<string, int> ClientsByDietitianGoal { get; set; } = new Dictionary<string, int>();
    public Dictionary<string, int> SubscriptionsByType { get; set; } = new Dictionary<string, int>();
    public Dictionary<string, int> SubscriptionByStatus { get; set; } = new Dictionary<string, int>();
    public Dictionary<string, int> PaymentsByStatus { get; set; } = new Dictionary<string, int>();
}
