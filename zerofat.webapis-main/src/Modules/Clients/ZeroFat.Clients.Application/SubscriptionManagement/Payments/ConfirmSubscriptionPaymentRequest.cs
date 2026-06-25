//using System.Text.Json;
//using System.Text.Json.Serialization;
//using MediatR;
//using Microsoft.Extensions.Localization;
//using Microsoft.Extensions.Logging;
//using ZeroFat.Application.Common.Interfaces;
//using ZeroFat.Application.Common.Persistence;
//using ZeroFat.Application.Common.Specification;
//using ZeroFat.Application.Common.Validation;
//using ZeroFat.ClientPortal.Application.Contracts;
//using ZeroFat.ClientPortal.Domain.ClientManagement;
//using ZeroFat.ClientPortal.Domain.SubscriptionManagement;
//using ZeroFat.Domain.Common;
//using ZeroFat.Domain.Enums;
//using ZeroFat.NutriPlan.Domain.MealPlanning;

//namespace ZeroFat.ClientPortal.Application.SubscriptionManagement.Payments;

//public class ConfirmSubscriptionPaymentRequest : ICommand<Result<DefaultIdType>>
//{
//    [JsonPropertyName("obj")]
//    public Transaction Obj { get; set; } = default!;

//}

//public class Transaction
//{
//    [JsonPropertyName("id")]
//    public long Id { get; set; } // Transaction ID

//    [JsonPropertyName("pending")]
//    public bool Pending { get; set; } // Transaction pending status

//    [JsonPropertyName("success")]
//    public bool Success { get; set; } // Indicates if the transaction was successful

//    [JsonPropertyName("is_auth")]
//    public bool IsAuth { get; set; } // Indicates if the transaction was authorized

//    [JsonPropertyName("is_capture")]
//    public bool IsCapture { get; set; } // Indicates if the transaction was captured

//    [JsonPropertyName("amount_cents")]
//    public int AmountCents { get; set; } // Amount paid in cents

//    [JsonPropertyName("is_voided")]
//    public bool IsVoided { get; set; } // Indicates if the transaction was voided

//    [JsonPropertyName("is_refunded")]
//    public bool IsRefunded { get; set; } // Indicates if the transaction was refunded

//    [JsonPropertyName("is_3d_secure")]
//    public bool Is3DSecure { get; set; } // Indicates if the transaction was 3D secured

//    [JsonPropertyName("integration_id")]
//    public int IntegrationId { get; set; } // ID of the payment integration

//    [JsonPropertyName("order")]
//    public Order? Order { get; set; } // Related order data

//    [JsonPropertyName("currency")]
//    public string? Currency { get; set; } // Currency of the payment
//}

//public class Order
//{
//    [JsonPropertyName("id")]
//    public long Id { get; set; } // Order ID in Accept's database

//    [JsonPropertyName("merchant_order_id")]
//    public string? MerchantOrderId { get; set; } // Order ID in Accept's database

//    [JsonPropertyName("created_at")]
//    public DateTime CreatedAt { get; set; } // Date and time of order creation

//    [JsonPropertyName("delivery_needed")]
//    public bool DeliveryNeeded { get; set; } // If order needed delivery services

//    [JsonPropertyName("amount_cents")]
//    public int AmountCents { get; set; } // Original price of the order in cents

//    [JsonPropertyName("paid_amount_cents")]
//    public int PaidAmountCents { get; set; } // Amount paid for the order in cents
//}

////2024-10-03 12:37:46.664 -07:00 [INF] {"id":579723,"pending":false,"success":true,"is_auth":false,"is_capture":false,"amount_cents":656600,"is_voided":false,"is_refunded":false,"is_3d_secure":true,"integration_id":50507,"order":{"id":697619,"created_at":"2024-10-03T12:36:57.731814-07:00","delivery_needed":false,"amount_cents":656600,"paid_amount_cents":656600},"currency":"AED"}

//public class ConfirmSubscriptionPaymentRequestValidator : CustomValidator<ConfirmSubscriptionPaymentRequest>
//{
//    public ConfirmSubscriptionPaymentRequestValidator(IStringLocalizer<ConfirmSubscriptionPaymentRequestValidator> localaizer)
//    {
//    }
//}


//public class ConfirmSubscriptionPaymentRequestHandler(
//    IRepositoryWithEvents<Payment> repository,
//    IRepository<Client> clientRepo,
//    IRepository<ClientSubscription> clientSubscriptionRepo,
//    IRepositoryWithEvents<DailyMealSelection> dailyMealSelectionRepo,
//    IRepositoryWithEvents<ClientLoyaltyPoint> clientLoyaltyPointRepo,
//    IRepositoryWithEvents<ClientOrder> clientOrderRepo,
//    IRepositoryWithEvents<Meal> mealRepo,
//    ICurrentUser currentUser,
//    IPaymobService paymobService,
//    IClientPortalSettingservice clientPortalSettingservice,
//    IJobService jobService,
//    IStringLocalizer<ConfirmSubscriptionPaymentRequestHandler> localizer,
//    ILogger<ConfirmSubscriptionPaymentRequestHandler> logger
//    ) : IRequestHandler<ConfirmSubscriptionPaymentRequest, Result<DefaultIdType>>
//{
//    private readonly IRepositoryWithEvents<Payment> _repository = repository;
//    private readonly IRepository<Client> _clientRepo = clientRepo;
//    private readonly IRepository<ClientSubscription> _clientSubscriptionRepo = clientSubscriptionRepo;
//    private readonly IRepositoryWithEvents<ClientLoyaltyPoint> _clientLoyaltyPointRepo = clientLoyaltyPointRepo;
//    private readonly IRepositoryWithEvents<ClientOrder> _clientOrderRepo = clientOrderRepo;
//    private readonly IRepositoryWithEvents<DailyMealSelection> _dailyMealSelectionRepo = dailyMealSelectionRepo;
//    private readonly IRepositoryWithEvents<Meal> _mealRepo = mealRepo;
//    private readonly ICurrentUser _currentUser = currentUser;
//    private readonly IPaymobService _paymobService = paymobService;
//    private readonly IJobService _jobService = jobService;
//    private readonly IClientPortalSettingservice _clientPortalSettingservice = clientPortalSettingservice;
//    private readonly IStringLocalizer _localizer = localizer;
//    private readonly ILogger<ConfirmSubscriptionPaymentRequestHandler> _logger = logger;


//    public async Task<Result<DefaultIdType>> Handle(ConfirmSubscriptionPaymentRequest request, CancellationToken cancellationToken)
//    {
//        var json = JsonSerializer.Serialize(request.Obj);
//        _logger.LogInformation(json);
//        string? orderId = request.Obj?.Order?.MerchantOrderId;


//            if(clientOrder == null)
//            {
//                var dailyMealSelection = await _dailyMealSelectionRepo.FirstOrDefaultAsync(new ExpressionSpecification<DailyMealSelection>(x => x.TransactionId.ToString() == orderId), cancellationToken);
//                if (dailyMealSelection != null) 
//                {
//                    Client? client = await _clientRepo.FirstOrDefaultAsync(new ExpressionSpecification<Client>(x => x.Id == dailyMealSelection.ClientId), cancellationToken);
//                    if (request.Obj.Success)
//                    {
//                        var payment = new Payment()
//                        {
//                            ClientId = client.Id,
//                            ClientSubscriptionId = client.ClientSubscriptionId.Value,
//                            InvoiceNumber = request.Obj.Order.Id.ToString(),
//                            TransactionId = request.Obj.Id.ToString(),
//                            PaymentStatus = PaymentStatus.Paid,
//                            PaymentMethod = request.Obj.IntegrationId.ToString(),
//                            Amount = request.Obj.AmountCents,
//                            PaymentGateway = "Paymob",
//                            Currency = request.Obj.Currency,
//                            PaymentDate = SystemTime.Now(),
//                            Reason = "Customize Meal Payment",

//                            OrderId = request.Obj.Order.Id.ToString(),
//                            Notes = json,
//                        };

//                        await _repository.AddAsync(payment, withSaveChanges: false, cancellationToken: cancellationToken);
//                        await _clientLoyaltyPointRepo.AddAsync(new ClientLoyaltyPoint()
//                        {
//                            ClientId = client.Id,
//                            Reason = "Customize Meal Payment",
//                            Source = "Payment",
//                            TransactionId = payment.Id.ToString(),
//                            Points = 0,
//                            Type = TransactionType.Earn,
//                            DateEarned = SystemTime.Now(),
//                        },

//                        withSaveChanges: false,
//                        cancellationToken: cancellationToken);

//                        dailyMealSelection.ExtraPaidPrice = dailyMealSelection.ExtraPrice;

//                        if (dailyMealSelection.MealId.HasValue)
//                        {
//                            var meal = await _mealRepo.GetByIdAsync(dailyMealSelection.MealId.Value, cancellationToken);
//                            if(meal != null)
//                            {
//                                Meal newMeal = new()
//                                {
//                                    ImageUrl = meal.ImageUrl,
//                                    RecipeId = meal.RecipeId,
//                                    NameAr = dailyMealSelection.CustomeMealName ?? meal.NameAr,
//                                    NameEn = dailyMealSelection.CustomeMealName ?? meal.NameEn,
//                                    Calories = meal.Calories,
//                                    Protein = meal.Protein,
//                                    Water = meal.Water,
//                                    WeightInGrams = meal.WeightInGrams,
//                                    Carbs = meal.Carbs,
//                                    Fat = meal.Fat,
//                                    Cuisine = meal.Cuisine,
//                                    SuitableForFreezing = meal.SuitableForFreezing,
//                                    PriceForCustomer = meal.PriceForCustomer,
//                                    IsActive = meal.IsActive,
//                                    IsCold = meal.IsCold,
//                                    IsWarm = meal.IsWarm,
//                                    IsDairyFree = meal.IsDairyFree,
//                                    DietaryCategories = meal.DietaryCategories,
//                                    IsGlutenFree = meal.IsGlutenFree,
//                                    IsLowGI = meal.IsLowGI,
//                                    OrginalMealId = meal.Id,
//                                    ClientId = client.Id
//                                };

//                                await _mealRepo.AddAsync(newMeal, withSaveChanges: false, cancellationToken: cancellationToken);
//                            }
//                        }

//                        await _repository.SaveChangesAsync(cancellationToken);
//                        await _mealRepo.SaveChangesAsync(cancellationToken);
//                        // _jobService.Enqueue<ISubscriptionsJob>(x => x.CreateSubscriptionDailySelection(clientSubscription.Id));
//                    }
//                }
//            }

//        return await Result<DefaultIdType>.SuccessAsync(clientSubscription.Id);
//    }

//}
