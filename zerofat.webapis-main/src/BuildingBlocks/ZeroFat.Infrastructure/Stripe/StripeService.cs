using Castle.Core.Resource;
using Mapster;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Extensions;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Models;
using ZeroFat.Domain.Enums;
using ZeroFat.Shared.Paymob;

namespace ZeroFat.Infrastructure.Stripe;

public class StripeService : IStripeService
{
    private readonly StripeSettings _settings;
    private readonly ILogger<StripeService> _logger;
    private readonly StripeClient _client;

    public StripeService(IOptions<StripeSettings> settings, ILogger<StripeService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
        _client = new StripeClient(_settings.SecretKey);
    }

    public async Task<string?> CreateSetupIntentAsync(string customerId)
    {
        var options = new SetupIntentCreateOptions
        {
            Customer = customerId,
        };

        var service = new SetupIntentService(_client);
        try
        {
            SetupIntent setupIntent = await service.CreateAsync(options);
            return setupIntent.ClientSecret; // This goes to the Flutter app
        }
        catch (StripeException e)
        {
            _logger.LogError(e, "SetupIntent creation failed");
            return null;
        }
    }

    public async Task<string> GetPaymentIntentIdFromSession(string Id)
    {
        var sessionService = new SessionService(_client);
        Session session = await sessionService.GetAsync(Id);
        if (session.PaymentStatus != "paid")
        {
            return string.Empty;
        }

        return session.PaymentIntentId;
    }

    public async Task<string?> CreateCustomerOnStripe(string? email, string? name, string? phone, string referenceId)
    {
        var options = new CustomerCreateOptions
        {
            Email = email,
            Name = name,
            Phone = phone,
            Metadata = new Dictionary<string, string> // Add metadata
            {
                { "reference_id", referenceId } // Store your reference ID here
            }
        };

        var service = new CustomerService(_client);
        try
        {
            Customer customer = await service.CreateAsync(options);
            return customer.Id; // Return the Stripe customer ID
        }
        catch (StripeException e)
        {
            _logger.LogError(e, "Customer Creeation Faild");
            return null;
        }
    }

    public async Task<string?> CreateProductOnStripe(string? code, string? name, string referenceId)
    {
        var options = new ProductCreateOptions
        {
            Name = code,
            Description = name,
            Metadata = new Dictionary<string, string> // Add metadata
            {
                { "reference_id", referenceId } // Store your reference ID here
            }
        };

        var service = new ProductService(_client);
        try
        {
            Product product = await service.CreateAsync(options);
            return product.Id; // Return the Stripe customer ID
        }
        catch (StripeException e)
        {
            _logger.LogError(e, "Proudct Publishing Faild.");
            throw new BadRequestException("Proudct Publishing Faild.");
        }
    }

    public async Task<string?> UpdateCustomerOnStripe(string customerId, string? email, string? name, string? phone, string referenceId)
    {
        var options = new CustomerUpdateOptions
        {
            Email = email,
            Name = name,
            Phone = phone,
            Metadata = new Dictionary<string, string> // Update metadata
        {
            { "reference_id", referenceId } // Update your reference ID here
        }
        };

        var service = new CustomerService(_client);
        try
        {
            Customer customer = await service.UpdateAsync(customerId, options);
            return customer.Id; // Return the updated Stripe customer ID
        }
        catch (StripeException e)
        {
            _logger.LogError(e, "Customer Update Failed");
            return null;
        }
    }

    public async Task<StrripeSessionInfo?> GetStripeSeesionInfoAsync(string sessionId)
    {
        var service = new SessionService(_client);
        var session = await service.GetAsync(sessionId);

        string subscriptionId = session.SubscriptionId;
        var sessionInfo = new StrripeSessionInfo
        {
            SessionId = session.Id,
            PaymentStatus = session.PaymentStatus,
            CustomerId = session.CustomerId,
            PaymentIntentId = session.PaymentIntentId,
            AmountTotal = session.AmountTotal.GetValueOrDefault(),
            Currency = session.Currency,
            PaymentMethodTypes = session.PaymentMethodTypes?.ToList() ?? new List<string>(),
            Status = session.Status,
            Metadata = session.Metadata?.ToDictionary(kvp => kvp.Key, kvp => kvp.Value) ?? new Dictionary<string, string>(),
            Created = session.Created,
            ExpiresAt = session.ExpiresAt,
            Url = session.Url,
            SubscriptionId = session.SubscriptionId
        };

        return sessionInfo;
    }

    public async Task<StripeSubscriptionInfo?> GetStripeSubscriptionInfoAsync(string subscriptionId)
    {
        try
        {
            var service = new SubscriptionService(_client);
            var subscription = await service.GetAsync(subscriptionId);

            
            return new StripeSubscriptionInfo
            {
                SubscriptionId = subscription.Id,
                Status = subscription.Status,
                CurrentPeriodStart = subscription.StartDate,
                CurrentPeriodEnd = subscription.EndedAt,
                CancelAt = subscription.CancelAt,
                CancelAtPeriodEnd = subscription.CancelAtPeriodEnd,
                CustomerId = subscription.CustomerId,
                DefaultPaymentMethodId = subscription.DefaultPaymentMethodId,
                LatestInvoiceId = subscription.LatestInvoiceId,
                StartDate = subscription.StartDate,
                Metadata = subscription.Metadata?.ToDictionary(kvp => kvp.Key, kvp => kvp.Value) ?? [],
                LatestInvoice = subscription.LatestInvoice != null ? new StripeInvoiceDto
                {
                    Id = subscription.LatestInvoice.Id,
                    Status = subscription.LatestInvoice.Status,
                    AmountDue = subscription.LatestInvoice.AmountDue,
                    AmountPaid = subscription.LatestInvoice.AmountPaid,
                    AmountRemaining = subscription.LatestInvoice.AmountRemaining,
                    Created = subscription.LatestInvoice.Created,
                    DueDate = subscription.LatestInvoice.DueDate,
                    Currency = subscription.LatestInvoice.Currency,
                    HostedInvoiceUrl = subscription.LatestInvoice.HostedInvoiceUrl,
                    InvoicePdf = subscription.LatestInvoice.InvoicePdf,
                    CustomerId = subscription.LatestInvoice.CustomerId,
                    // PaymentIntentId = subscription.LatestInvoice.Int,
                    PaymentMethodTyped = subscription.LatestInvoice.DefaultPaymentMethod.Type,
                } : null,
                CollectionMethod = subscription.CollectionMethod,
                DaysUntilDue = subscription.DaysUntilDue,
                DefaultTaxRates = subscription.DefaultTaxRates?.Select(t => t.Id).ToList()
            };



        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Failed to get Stripe subscription {SubscriptionId}", subscriptionId);
            return null;
        }
    }

    public async Task<(string? id, string? url)> CreateSubscriptionLink(
        string subscriptionId,
        string customerId,
        string productId,
        decimal amount,
        string mealSelections,
        string productName,
        string description,
        DefaultIdType mealPlanId,
        List<DayOfWeek> days,
        bool isRecurring,
        SubscriptionType subscriptionType,
        DateOnly startDate,
        DateOnly endDate,
        int? offsetSubscription,
        string? couponCode
        )
    {
        // Determine the billing interval based on the subscription type
        string interval = "week"; // Default to monthly
        int intervalCount = 1; // Default to every 1 week

        switch (subscriptionType)
        {
            case SubscriptionType.OneWeek:
                interval = "week";
                intervalCount = 1;
                break;
            case SubscriptionType.OneMonth:
                interval = "week";
                intervalCount = 4;
                break;
            case SubscriptionType.ThreeMonths:
                interval = "week";
                intervalCount = 12;
                break;
            default:
                break;
        }

        var now = DateOnly.FromDateTime(DateTime.Now);
        // Calculate the billing cycle anchor
        var trialEnd = startDate.AddDays(-offsetSubscription.GetValueOrDefault());
        var service = new SessionService(_client);

        var options = new SessionCreateOptions
        {
            PaymentMethodTypes = ["card"],
            Currency = _settings.Currency,
            // ADD THIS BLOCK FOR ONE-TIME PAYMENTS
            PaymentIntentData = new SessionPaymentIntentDataOptions
            {
                SetupFutureUsage = "on_session"
            },

            Metadata = new Dictionary<string, string> // Add metadata
            {
                { "subscriptionId", subscriptionId },
                { "mealPlanId", mealPlanId.ToString() },
                { "mealsStartDate", startDate.ToString() },
                { "mealsEndDate", endDate.ToString() },
                { "DaysPerWeek", string.Join(",", days) }
            },

            SubscriptionData = isRecurring ? new SessionSubscriptionDataOptions()
            {
                Metadata = new Dictionary<string, string> // Add metadata
                {
                    { "subscriptionId", subscriptionId },
                    { "mealPlanId", mealPlanId.ToString() },
                    { "mealsStartDate", startDate.ToString() },
                    { "mealsEndDate", endDate.ToString() },
                    { "DaysPerWeek", string.Join(",", days) }
                },
                Description = "Test Desciption",
                // BillingCycleAnchor = trialEnd.ToDateTime(TimeOnly.MinValue),
                // TrialPeriodDays = startDate.DayNumber - now.DayNumber,

                // ProrationBehavior = "none", // Charge a prorated amount now
                // TrialEnd = trialEnd,
                // TrialEnd = trailEnd,
            } : null,
            // TrialEnd = trialEnd,
            LineItems = [ new()
            {
                Quantity = 1,
                PriceData = new SessionLineItemPriceDataOptions
                {
                    UnitAmount = Convert.ToInt64(amount * 100),
                    Currency = _settings.Currency,
                    // Product = productId,
                    ProductData = new SessionLineItemPriceDataProductDataOptions(){
                        Description = description,
                        Name = productName,
                    },
                    Recurring = isRecurring ? new SessionLineItemPriceDataRecurringOptions
                    {
                        Interval = interval, // Billing interval (e.g., week, month)
                        IntervalCount = intervalCount, // Every X weeks/months
                    } : null,
                },
            }],
            Customer = customerId,
            Mode = isRecurring ? "subscription" : "payment", // Mode for subscription or one-time payment
            Expand = ["payment_intent"],
            SuccessUrl = "https://zerofat.ae/subscription-confirmed",

            // SuccessUrl = $"{_settings.SuccessUrlDomain}/payments/payment-success?sessionId={{CHECKOUT_SESSION_ID}}&subscriptionId={subscriptionId}", // &userId={customerId}
        };

        if (couponCode.HasValue())
        {
            options.Discounts = [
                new SessionDiscountOptions
                {
                    Coupon = couponCode // Add your coupon code here
                }
            ];

        }

        Session session = await service.CreateAsync(options);

        // session.Url
        // var ephemeralKeyOptions = new EphemeralKeyCreateOptions
        // {
        //     Customer = customerId,
        // };
        // var ephemeralKeyService = new EphemeralKeyService(_client);
        // 
        // var ephemeralKey = await ephemeralKeyService.CreateAsync(ephemeralKeyOptions);

        return (session.Id, session.Url);
    }

    public async Task<(string? id, string? url)> CreateSubscriptionAsync(
       string subscriptionId,
       string customerId,
       string productId,
       decimal amount,
       string mealSelections,
       string productName,
       string description,
       DefaultIdType mealPlanId,
       List<DayOfWeek> days,
       bool isRecurring,
       SubscriptionType subscriptionType,
       DateOnly startDate,
       DateOnly endDate,
       int? offsetSubscription,
       string? couponCode
       )
    {
        // Determine the billing interval based on the subscription type
        string interval = "week"; // Default to monthly
        int intervalCount = 1; // Default to every 1 week

        switch (subscriptionType)
        {
            case SubscriptionType.OneWeek:
                interval = "week";
                intervalCount = 1;
                break;
            case SubscriptionType.OneMonth:
                interval = "week";
                intervalCount = 4;
                break;
            case SubscriptionType.ThreeMonths:
                interval = "week";
                intervalCount = 12;
                break;
            default:
                break;
        }

        var now = DateOnly.FromDateTime(DateTime.Now);
        var service = new SubscriptionService(_client);

        var options = new SubscriptionCreateOptions
        {
            Currency = _settings.Currency,
            Items = new List<SubscriptionItemOptions>()
            {
                new SubscriptionItemOptions()
                {
                    Quantity = 1,
                    PriceData = new SubscriptionItemPriceDataOptions
                    {
                        Product = "prod_SD3i3FD3CKO6G7",
                        Currency = _settings.Currency,
                        UnitAmount = Convert.ToInt64(amount * 100),
                        Recurring = new SubscriptionItemPriceDataRecurringOptions
                        {
                            Interval = interval, // Billing interval (e.g., week, month)
                            IntervalCount = intervalCount, // Every X weeks/months
                        }
                    },
                }
            },
            Expand = ["latest_invoice.confirmation_secret"],
            PaymentBehavior = "default_incomplete",
            Customer = customerId,
            BillingCycleAnchor = DateTime.UtcNow.AddMinutes(10),
            // TrialPeriodDays = startDate.DayNumber - now.DayNumber,
        };

        if (couponCode.HasValue())
        {
            options.Discounts = [
                new SubscriptionDiscountOptions
                {
                    Coupon = couponCode // Add your coupon code here
                }
            ];
        }

        var subscription = await service.CreateAsync(options);

        var clientSecret = subscription.LatestInvoice.ConfirmationSecret.ClientSecret;
        // session.Url
        // var ephemeralKeyOptions = new EphemeralKeyCreateOptions
        // {
        //     Customer = customerId,
        // };
        // var ephemeralKeyService = new EphemeralKeyService(_client);
        // 
        // var ephemeralKey = await ephemeralKeyService.CreateAsync(ephemeralKeyOptions);

        return (subscription.Id, subscription.Id);
    }

    public async Task<(string? id, string? url)> CreateSubscriptionInvoice(
       string subscriptionId,
       string customerId,
       string productId,
       decimal amount,
       string mealSelections,
       string productName,
       string description,
       DefaultIdType mealPlanId,
       List<DayOfWeek> days,
       bool isRecurring,
       SubscriptionType subscriptionType,
       DateOnly startDate,
       DateOnly endDate,
       int? offsetSubscription,
       string? couponCode)
    {
        var invoiceItemService = new InvoiceItemService(_client);

        var invoiceItemOptions = new InvoiceItemCreateOptions
        {
            Customer = customerId,
            Amount = Convert.ToInt64(amount * 100),
            Currency = _settings.Currency,
            // Product = productId,
            Description = productName + description,

        };
        var invoiceItem = await invoiceItemService.CreateAsync(invoiceItemOptions);



        var service = new InvoiceService(_client);

        var options = new InvoiceCreateOptions
        {
            AutoAdvance = true,
            Currency = _settings.Currency,
            Metadata = new Dictionary<string, string> // Add metadata
            {
                { "subscriptionId", subscriptionId },
                { "mealPlanId", mealPlanId.ToString() },
                { "mealsStartDate", startDate.ToString() },
                { "mealsEndDate", endDate.ToString() },
                { "DaysPerWeek", string.Join(",", days) }
            },
            Customer = customerId,
            // Expand = ["payment_intent"],
            // SuccessUrl = $"{_settings.SuccessUrlDomain}/payments/payment-success?sessionId={{CHECKOUT_SESSION_ID}}&subscriptionId={subscriptionId}", // &userId={customerId}
        };

        if (couponCode.HasValue())
        {
            options.Discounts = [
                new InvoiceDiscountOptions
                {
                    Coupon = couponCode // Add your coupon code here
                }
            ];

        }

        Invoice session = await service.CreateAsync(options);

        // session.Url
        // var ephemeralKeyOptions = new EphemeralKeyCreateOptions
        // {
        //     Customer = customerId,
        // };
        // var ephemeralKeyService = new EphemeralKeyService(_client);
        // 
        // var ephemeralKey = await ephemeralKeyService.CreateAsync(ephemeralKeyOptions);

        return (session.Id, session.Id);
    }
    public async Task<StripeInvoiceDto> AdvanceFirstPaymentAsync(string subscriptionId, string customerId)
    {
        // In your webhook for `checkout.session.completed`:
        var invoiceService = new InvoiceService(_client);
        var invoice = await invoiceService.CreateAsync(new InvoiceCreateOptions
        {
            Customer = customerId,
            Subscription = subscriptionId,
            AutoAdvance = true, // Auto-finalize
        });

        // Charge immediately
        invoice = await invoiceService.PayAsync(invoice.Id);

        return new StripeInvoiceDto()
        {
            Id = invoice.Id,
            Status = invoice.Status,
            AmountDue = invoice.AmountDue,
            AmountPaid = invoice.AmountPaid,
            AmountRemaining = invoice.AmountRemaining,
            Created = invoice.Created,
            DueDate = invoice.DueDate,
            Currency = invoice.Currency,
            HostedInvoiceUrl = invoice.HostedInvoiceUrl,
            InvoicePdf = invoice.InvoicePdf,
            CustomerId = invoice.CustomerId,
            PaymentMethodTyped = invoice.DefaultPaymentMethod.Type,
        };
    }
    public async Task<PaymentIntentResponseDTO> CreateAddOnPaymentLink(
        string orderId,
        string paymentMethodId,
        string customerId,
        decimal amount,
        string mealDescription)
    {
        var service = new PaymentIntentService(_client);
        var options = new PaymentIntentCreateOptions
        {
            Amount = Convert.ToInt64(amount * 100),
            Currency = _settings.Currency,
            Confirm = true,
            Customer = customerId,
            Description = mealDescription,
            // StatementDescriptorSuffix = "ZEROFAT", // Max 22 chars
            PaymentMethod = paymentMethodId,
            CaptureMethod = "automatic",
            AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
            {
                Enabled = true,
                AllowRedirects = "never"
            },
            Metadata = new Dictionary<string, string>
                {
                    { "CustomerId", customerId },
                    { "PaymentMethodId", paymentMethodId },
                    { "OrderId", orderId },
                    { "Meal_Details", mealDescription }
                }
        };
        var paymentIntent = await service.CreateAsync(options);

        return paymentIntent.Adapt<PaymentIntentResponseDTO>();
    }


    #region Customer PaymentMethod
    public async Task<StripePaymentMethodDto?> AttachPaymentMethodToCustomer(string cardId, string customerId)
    {
        try
        {
            var service = new PaymentMethodService(_client);

            var paymentMethodAttach = new PaymentMethodAttachOptions
            {
                Customer = customerId,
            };

            var paymentMethod = await service.AttachAsync(cardId, paymentMethodAttach);

            return paymentMethod.Adapt<StripePaymentMethodDto>();
        }
        catch (Exception ex)
        {
            // _logger.LogError(ex.Message);
            return null;
        }
    }
    public async Task<bool> MakeDefaultPaymentMethod(string cardId, string customerId)
    {
        try
        {
            var customerService = new CustomerService(_client);
            var updateCustomer = new CustomerUpdateOptions
            {
                InvoiceSettings = new CustomerInvoiceSettingsOptions
                {
                    DefaultPaymentMethod = cardId
                }
            };

            await customerService.UpdateAsync(customerId, updateCustomer);

            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }
    public async Task<bool> DetachPaymentMethodFromCustomer(string id)
    {
        try
        {
            var paymentService = new PaymentMethodService(_client);
            await paymentService.DetachAsync(id);
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }
    public async Task<List<StripePaymentMethodDto>?> GetCustomerPaymentMethods(string customerId)
    {
        try
        {
            var service = new PaymentMethodService(_client);
            var options = new PaymentMethodListOptions
            {
                Customer = customerId,
                Type = "card",
            };

            var paymentMethods = await service.ListAsync(options);

            // The result from Stripe is a StripeList object. 
            // We adapt the 'Data' property which is a List<PaymentMethod>.
            return paymentMethods.Data.Adapt<List<StripePaymentMethodDto>>();
        }
        catch (Exception ex)
        {
            // _logger.LogError(ex.Message);
            return null;
        }
    }
    #endregion

    #region PaymentMethod
    public async Task<(string Last4, string Brand)> RetrievePaymentMethod(string id)
    {
        try
        {
            var paymentService = new PaymentMethodService(_client);
            var paymentMethod = await paymentService.GetAsync(id);

            return (paymentMethod.Card.Last4, paymentMethod.Card.Brand);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return (string.Empty, string.Empty);
        }
    }
    //public async Task<List<StripePaymentMethod>> RetrieveCustomersPaymentMethods(string customerId)
    //{
    //    try
    //    {
    //        var paymentService = new PaymentMethodService(_client);
    //        var paymentMethods = await paymentService.ListAsync(
    //            new PaymentMethodListOptions
    //            {
    //                Customer = customerId
    //            });

    //        return paymentMethods.Data.Select(x => new StripePaymentMethod
    //        {
    //            Id = x.Id,
    //            Last4 = x.Card.Last4,
    //            Brand = x.Card.Brand
    //        }).ToList();
    //    }
    //    catch (Exception ex)
    //    {
    //        _logger.LogError(ex.Message);
    //        return null;
    //    }
    //}
    public async Task<(long Year, long Month)> PaymentMethodExpirationDate(string id)
    {
        try
        {
            var paymentService = new PaymentMethodService(_client);
            var paymentMethod = await paymentService.GetAsync(id);
            return (paymentMethod.Card.ExpYear, paymentMethod.Card.ExpMonth);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return (0, 0);
        }
    }
    #endregion

    #region Coupons
    public async Task<StripeCouponDto?> GetCouponByCodeAsync(string couponCode)
    {
        if (couponCode.IsEmpty())
        {
            return null;
        }

        try
        {
            var couponService = new CouponService(_client);
            var coupon = await couponService.GetAsync(couponCode);

            return coupon?.Adapt<StripeCouponDto>();
        }
        catch (StripeException ex) when (ex.StripeError?.Code == "resource_missing")
        {
            // Coupon not found - return null (consistent with null-check above)
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve coupon information for code: {CouponCode}", couponCode);
            return null;
        }
    }

    public async Task<string?> CreateDiscountRuleOnStripeAsync(StripeCouponDto couponDto)
    {
        try
        {
            var couponService = new CouponService(_client);
            var coupon = await couponService.CreateAsync(new CouponCreateOptions()
            {
                Id = couponDto.Code,
                AmountOff = couponDto.AmountOff,
                Currency = _settings.Currency,
                Duration = couponDto.Duration,
                Name = couponDto.Code,
                PercentOff = couponDto.PercentOff,
                DurationInMonths = couponDto.Duration == "repeating" ? couponDto.DurationInMonths : null,
                MaxRedemptions = couponDto.MaxRedemptions,
                RedeemBy = couponDto.RedeemBy,
            });

            return coupon.Id;
        }
        catch (StripeException ex) when (ex.StripeError?.Code == "resource_missing")
        {
            // Coupon not found - throw domain-specific exception instead of returning null
            throw new BadRequestException("Coupon not found in Stripe");
        }
        catch (StripeException ex)
        {
            // Other Stripe errors
            throw new BadRequestException($"Stripe error: {ex.StripeError?.Message ?? ex.Message}");
        }
        catch (Exception ex)
        {
            // General errors
            throw new BadRequestException($"Unexpected error while creating coupon: {ex.Message}");
        }
    }

    #endregion

    #region 
    public async Task<string> CreatePaymentIntent(long amount, string customerId, string paymentMethodId)
    {
        try
        {
            var service = new PaymentIntentService(_client);
            var options = new PaymentIntentCreateOptions
            {
                Amount = amount * 100,
                Currency = "usd",
                Confirm = true,
                Customer = customerId,
                PaymentMethod = paymentMethodId,
                CaptureMethod = "manual",
                Metadata = new Dictionary<string, string>
                {
                    { "CustomerId", customerId },
                    { "PaymentMethodId", paymentMethodId }
                }
            };
            var paymentIntent = await service.CreateAsync(options);
            return paymentIntent.Status == "requires_capture" ? paymentIntent.Id : string.Empty;
        }
        catch (Exception ex)
        {
            // _logger.LogError(ex.Message);
            return string.Empty;
        }
    }
    #endregion
}

/*
public class StripeService : IStripeService
{
    private readonly StripeSettings _settings;
    private readonly IStripeClient _client;
    private readonly IConfiguration _config;
    private readonly IStringLocalizer<StripeService> _localizer;
    private readonly ILogger<StripeService> _logger;

    public StripeService(IOptions<StripeSettings> settings, IStringLocalizer<StripeService> localizer, ILogger<StripeService> logger, IConfiguration confg)
    {
        _settings = settings.Value;
        _localizer = localizer;
        _logger = logger;
        _config = confg;
        var envSetting = _settings.Dev;
        //var envSetting = _config.GetSection("env").Value == "dev" ? _settings.Dev : _settings.Prod;
        _client = new StripeClient(envSetting.SecretKey);
    }


    #region Customer
    public async Task<string> CreateCustomer(string phone, string name = default!)
    {
        try
        {
            var service = new CustomerService(_client);
            var options = new CustomerCreateOptions
            {
                Name = name,
                Phone = phone
            };
            var customer = await service.CreateAsync(options);
            return customer.Id;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return string.Empty;
        }
    }
    public async Task<string> UpdateCustomer(string customerId, string phone, string name = default!)
    {
        try
        {
            var customerService = new CustomerService(_client);
            var updateCustomer = new CustomerUpdateOptions
            {
                Name = name,
                Phone = phone
            };

            await customerService.UpdateAsync(customerId, updateCustomer);

            return customerId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return string.Empty;
        }
    }

    public async Task<string> AttachPaymentMethodToCustomer(string cardId, string customerId)
    {
        try
        {
            var service = new PaymentMethodService(_client);

            var paymentMethodAttach = new PaymentMethodAttachOptions
            {
                Customer = customerId,
            };

            await service.AttachAsync(cardId, paymentMethodAttach);

            return cardId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return string.Empty;
        }
    }
    public async Task<string> MakeDefaultPaymentMethod(string cardId, string customerId)
    {
        try
        {
            var customerService = new CustomerService(_client);
            var updateCustomer = new CustomerUpdateOptions
            {
                InvoiceSettings = new CustomerInvoiceSettingsOptions
                {
                    DefaultPaymentMethod = cardId
                }
            };

            await customerService.UpdateAsync(customerId, updateCustomer);

            return cardId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return string.Empty;
        }
    }
    public async Task<bool> DetachPaymentMethodFromCustomer(string id)
    {
        try
        {
            var paymentService = new PaymentMethodService(_client);
            var paymentMethod = await paymentService.DetachAsync(id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return false;
        }
    }


    #endregion

    #region PaymentMethod
    public async Task<(string Last4, string Brand)> RetrievePaymentMethod(string id)
    {
        try
        {
            var paymentService = new PaymentMethodService(_client);
            var paymentMethod = await paymentService.GetAsync(id);

            return (paymentMethod.Card.Last4, paymentMethod.Card.Brand);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return (string.Empty, string.Empty);
        }
    }
    public async Task<List<StripePaymentMethod>> RetrieveCustomersPaymentMethods(string customerId)
    {
        try
        {
            var paymentService = new PaymentMethodService(_client);
            var paymentMethods = await paymentService.ListAsync(
                new PaymentMethodListOptions
                {
                    Customer = customerId
                });

            return paymentMethods.Data.Select(x => new StripePaymentMethod
            {
                Id = x.Id,
                Last4 = x.Card.Last4,
                Brand = x.Card.Brand
            }).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return null;
        }
    }

    public async Task<(long Year, long Month)> PaymentMethodExpirationDate(string id)
    {
        try
        {
            var paymentService = new PaymentMethodService(_client);
            var paymentMethod = await paymentService.GetAsync(id);
            return (paymentMethod.Card.ExpYear, paymentMethod.Card.ExpMonth);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return (0, 0);
        }
    }

    #endregion

    #region Invoice

    //public async Task<StripeInvoiceDetailsDto> GetInvoice(string id)
    //{
    //    try
    //    {
    //        var service = new InvoiceService(_client);
    //        var invoice = await service.GetAsync(id);
    //        return new StripeInvoiceDetailsDto
    //        {
    //            InvoiceNumber = invoice.Number,
    //            CustomerName = invoice.CustomerName,
    //            CustomerEmail = invoice.CustomerEmail,
    //            PdfUrl = invoice.InvoicePdf,
    //            Id = invoice.Id,
    //            Amount = invoice.AmountDue / 100,
    //            Date = invoice.Created
    //        };
    //    }
    //    catch (Exception ex)
    //    {
    //        _logger.LogError(ex.Message);
    //        return null;
    //    }
    //}


    #endregion

    #region paymentIntent
    public async Task<string> CreatePaymentIntent(long amount, string customerId, string paymentMethodId)
    {
        try
        {
            var service = new PaymentIntentService(_client);
            var options = new PaymentIntentCreateOptions
            {
                Amount = amount * 100,
                Currency = "usd",
                Confirm = true,
                Customer = customerId,
                PaymentMethod = paymentMethodId,
                CaptureMethod = "manual",
                Metadata = new Dictionary<string, string>
                {
                    { "CustomerId", customerId },
                    { "PaymentMethodId", paymentMethodId }
                }
            };
            var paymentIntent = await service.CreateAsync(options);
            return paymentIntent.Status == "requires_capture" ? paymentIntent.Id : string.Empty;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return string.Empty;
        }
    }

    public async Task<bool> CapturePaymentIntent(string paymentIntentId)
    {
        try
        {
            var service = new PaymentIntentService(_client);
            var res = await service.CaptureAsync(paymentIntentId);
            return res.Status == "succeeded";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return false;
        }
    }

    public async Task<bool> CancelPaymentIntent(string paymentIntentId)
    {
        try
        {
            var service = new PaymentIntentService(_client);
            var res = await service.CancelAsync(paymentIntentId);
            return res.Status == "canceled";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return false;
        }
    }


    #endregion
}

*/
