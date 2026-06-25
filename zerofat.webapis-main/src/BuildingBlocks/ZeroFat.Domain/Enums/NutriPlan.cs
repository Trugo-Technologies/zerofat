using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace ZeroFat.Domain.Enums;
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum CategoryType
{
    Ingredient,
    Recipe
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum BasicUnitType
{
    Solid, 
    Liquid 
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum BasicUnit
{
    g,
    kg,
    ml,
    l
}


[JsonConverter(typeof(JsonStringEnumConverter))]
/// <summary>
/// Enum for dietary preferences.
/// </summary>
public enum DietaryPreference
{
    Vegan,
    Vegetarian,
    NonVegetarian,
    Paleo,
    Keto,
    None
}

[JsonConverter(typeof(JsonStringEnumConverter))]
/// <summary>
/// Enum for ingredient types.
/// </summary>
public enum IngredientType
{
    Vegetable,
    Fruit,
    Meat,
    Dairy,
    Grain,
    Nut,
    Spice,
    Other
}

[JsonConverter(typeof(JsonStringEnumConverter))]
/// <summary>
/// Enum for ingredient status.
/// </summary>
public enum IngredientStatus
{
    Available,
    OutOfStock,
    Discontinued
}

[JsonConverter(typeof(JsonStringEnumConverter))]
/// <summary>
/// Enum for ingredient status.
/// </summary>
public enum NutriPlanStartegy
{
    BasedOnTime,
    BasedOnCalories
}

[JsonConverter(typeof(JsonStringEnumConverter))]
/// <summary>
/// Enum for dietary categories.
/// </summary>
public enum DietaryCategory
{
    Vegan,
    Vegetarian,
    Pescatarian,
    Omnivore,
    None
}

[JsonConverter(typeof(JsonStringEnumConverter))]
/// <summary>
/// Enum for recipe difficulty.
/// </summary>
public enum RecipeDifficulty
{
    Easy,
    Medium,
    Hard
}

[JsonConverter(typeof(JsonStringEnumConverter))]
/// <summary>
/// Enum for cuisine types.
/// </summary>
public enum CuisineType
{
    Italian,
    Chinese,
    Indian,
    Mexican,
    Japanese,
    Thai,
    French,
    Mediterranean,
    American,
    MiddleEastern,
    Spanish,
    Greek,
    Korean,
    Vietnamese,
    Caribbean,
    African,
    German,
    British,
    Brazilian,
    Other
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum SubscriptionType
{
    OneWeek = 7,
    TwoWeeks = 14,   // Manage Subscription wizard
    ThreeWeeks = 21, // Manage Subscription wizard
    OneMonth = 30,
    TwoMonths = 60,  // Manage Subscription wizard
    ThreeMonths = 90
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PreferredMealTime
{
    EarlyMorning,   // 6:00 AM - 8:00 AM
    Morning,        // 8:00 AM - 10:00 AM
    // LateMorning,    // 10:00 AM - 12:00 PM
    // Noon,           // 12:00 PM - 2:00 PM
    // EarlyAfternoon, // 2:00 PM - 4:00 PM
    // LateAfternoon,  // 4:00 PM - 6:00 PM
    Evening,        // 6:00 PM - 8:00 PM
    // LateEvening,    // 8:00 PM - 10:00 PM
    // Night           // 10:00 PM - 12:00 AM
}

[JsonConverter(typeof(JsonStringEnumConverter))]
/// <summary>
/// Enum to represent subscription statuses.
/// </summary>
public enum SubscriptionStatus
{
    Active,      // Subscription is active
    PastDue,      // Subscription is active
    Canceled,   // Subscription has been cancelled
    Expired,     // Subscription has expired
    Pending,      // Subscription is pending activation

    Unknown
}

[JsonConverter(typeof(JsonStringEnumConverter))]
/// <summary>
/// Enum to represent the various statuses of a payment.
/// </summary>
public enum PaymentStatus
{
    Pending,      // Payment is pending and yet to be processed
    Paid,         // Payment has been successfully processed and completed
    Failed,       // Payment failed during the processing attempt
    Refunded,     // Payment has been refunded to the client
    Cancelled,    // Payment was cancelled before processing
    PartiallyPaid, // Partial payment has been made, but the full amount is not settled
}


[JsonConverter(typeof(JsonStringEnumConverter))]
/// <summary>
/// Enum to represent the various methods of payment available.
/// </summary>
public enum PaymentMethod
{
    CreditCard,    // Payment via credit card (e.g., Visa, MasterCard, Amex)
    // DebitCard,     // Payment via debit card
    PayPal,        // Payment via PayPal account
    // BankTransfer,  // Payment via direct bank transfer
    // CashOnDelivery, // Payment in cash upon delivery
    // MobilePayment, // Payment via mobile payment platforms (e.g., Apple Pay, Google Pay)
    // Cryptocurrency, // Payment via cryptocurrencies (e.g., Bitcoin, Ethereum)
    // Check,         // Payment via paper or electronic check
    // GiftCard,      // Payment via a gift card or voucher
    // DigitalWallet, // Payment via digital wallet (e.g., Samsung Pay, Venmo)
    // DirectDebit    // Payment through automatic debit from bank account
}


[JsonConverter(typeof(JsonStringEnumConverter))]
/// <summary>
/// Enum for ingredient status.
/// </summary>
public enum IngredientSource
{
    Local,
    USDA,
    ZeroFat
}

[JsonConverter(typeof(JsonStringEnumConverter))]
/// <summary>
/// Enum for ingredient status.
/// </summary>
public enum NutrientType
{
    Minerals,
    Lipids,
}

[JsonConverter(typeof(JsonStringEnumConverter))]
/// <summary>
/// Enum for DailySelection Status.
/// </summary>
public enum DailySelectionStatus
{
    Pending,         // The client hasn't selected meals yet.
    Initialized,     // Meals have been selected and the day is set.
    Confirmed,       // The selection has been locked in (after cutoff time).
    Preparing,       // Meals are being prepared in the kitchen.
    OutForDelivery,  // Meals have left the kitchen for delivery.
    Delivered,       // Meals have been successfully delivered.
    Missed,          // Delivery failed or client missed the delivery.
    Paused,          // The day was skipped (client paused the subscription).
    Cancelled        // The selection was cancelled entirely.
}


[JsonConverter(typeof(JsonStringEnumConverter))]
public enum MealSelectionType
{
    Default,    // Original subscribed meal
    AddOn,      // Upgraded meal from catalog
    Custom      // Fully custom meal request
}
