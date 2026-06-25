using Npgsql;

const string connectionString =
    "Host=localhost;Port=5432;Database=zerofatdb;Username=postgres;Password=admin@123;";

const string sql = """
    INSERT INTO "Client"."Clients" (
        "Id", "StripeId", "FullName", "Email", "Mobile", "Gender", "BirthDate",
        "ClientAllergicIds", "HeightInCM", "WeightInKG", "CurrentWeightInKG", "TargetWeightInKG",
        "DietitianGoal", "ActivityValue", "SubscriptionStatus", "BMI", "BMR", "BodyFat", "TDEE",
        "TimeToReachGoalInDays", "NeededCaloriesToReachGoal", "AccountIsDeleted",
        "CreatedBy", "CreatedOn", "LastModifiedBy", "IsActive"
    )
    VALUES (
        '11111111-2222-3333-4444-555555555501',
        'cus_test_wizard_client',
        'Test Wizard Client',
        'testclient@zerofat.ai',
        '+971501234567',
        0,
        '1990-01-15 00:00:00+00',
        '{}',
        175, 80, 80, 75,
        1, 1.7, 4,
        26.1, 1800, 22, 2200,
        56, 2000, false,
        '00000000-0000-0000-0000-000000000000',
        NOW(),
        '00000000-0000-0000-0000-000000000000',
        true
    )
    ON CONFLICT ("Id") DO NOTHING;
    """;

await using var conn = new NpgsqlConnection(connectionString);
await conn.OpenAsync();
await using var cmd = new NpgsqlCommand(sql, conn);
var rows = await cmd.ExecuteNonQueryAsync();

Console.WriteLine(rows > 0
    ? "Test client created. Use clientId: 11111111-2222-3333-4444-555555555501"
    : "Test client already exists. Use clientId: 11111111-2222-3333-4444-555555555501");
