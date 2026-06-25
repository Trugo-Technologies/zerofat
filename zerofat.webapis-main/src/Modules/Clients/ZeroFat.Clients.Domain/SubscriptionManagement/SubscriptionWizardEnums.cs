using System.Text.Json.Serialization;

namespace ZeroFat.ClientPortal.Domain.SubscriptionManagement;

// Wizard enums for Manage Subscription admin UI (api/clientPortal-module/ManageSubscriptions).

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum SubscriptionWizardMode
{
    New,
    Renew
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum SubscriptionWizardStep
{
    RenewalOptions,
    Configuration,
    Schedule,
    BillingReview
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum SubscriptionRenewalStrategy
{
    ExtendAfterExpiry,
    ScheduleFutureDate
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum SubscriptionDraftStatus
{
    Draft,
    Finalized,
    Cancelled
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum SubscriptionAddOnCategory
{
    Meals,
    Snacks,
    Drinks,
    Supplements
}
