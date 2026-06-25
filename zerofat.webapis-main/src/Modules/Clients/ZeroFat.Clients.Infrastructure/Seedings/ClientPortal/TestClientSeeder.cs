using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ZeroFat.ClientPortal.Domain.ClientManagement;
using ZeroFat.ClientPortal.Infrastructure.Persistence.Context;
using ZeroFat.ClientPortal.Infrastructure.Services;
using ZeroFat.Domain.Enums;

namespace ZeroFat.ClientPortal.Infrastructure.Seedings.ClientPortal;

/// <summary>
/// Seeds one demo client for admin Manage Subscription wizard testing (EnableTestingMode).
/// Client id: 11111111-2222-3333-4444-555555555501
/// </summary>
public class TestClientSeeder(ILogger<TestClientSeeder> logger, ClientPortalContext db) : IClientPortalSeeder
{
    public static readonly Guid TestClientId = Guid.Parse("11111111-2222-3333-4444-555555555501");
    public const string TestClientEmail = "testclient@zerofat.ai";

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        if (await db.Clients.AnyAsync(x => x.Id == TestClientId || x.Email == TestClientEmail, cancellationToken))
        {
            logger.LogInformation("Test client already exists ({ClientId}).", TestClientId);
            return;
        }

        var now = DateTime.UtcNow;
        var client = new Client
        {
            Id = TestClientId,
            FullName = "Test Wizard Client",
            Email = TestClientEmail,
            Mobile = "+971501234567",
            Gender = Gender.Male,
            BirthDate = new DateTime(1990, 1, 15, 0, 0, 0, DateTimeKind.Utc),
            HeightInCM = 175,
            WeightInKG = 80,
            CurrentWeightInKG = 80,
            TargetWeightInKG = 75,
            DietitianGoal = DietitianGoal.LoseWeight,
            ActivityValue = 1.7,
            SubscriptionStatus = SubscriptionStatus.Pending,
            BMI = 26.1,
            BMR = 1800,
            BodyFat = 22,
            TDEE = 2200,
            TimeToReachGoalInDays = 56,
            NeededCaloriesToReachGoal = 2000,
            IsActive = true,
            ClientAllergicIds = [],
            StripeId = "cus_test_wizard_client"
        };

        await db.Clients.AddAsync(client, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Seeded test client for Manage Subscription wizard. ClientId={ClientId}, Email={Email}",
            TestClientId,
            TestClientEmail);
    }
}
