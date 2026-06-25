using Flurl.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ZeroFat.Application.Common.Extensions;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Shared.Paymob;

namespace ZeroFat.Infrastructure.Paymob;


public class PaymobService : IPaymobService
{
    private readonly PaymobSettings _paymobSettings;
    private readonly ILogger<IPaymobService> _logger;

    public PaymobService(IOptions<PaymobSettings> paymobSettings, ILogger<IPaymobService> logger)
    {
        _paymobSettings = paymobSettings.Value;
        _logger = logger;
    }

    public async Task<string?> GetToken()
    {
        var result = await (_paymobSettings.Url + _paymobSettings.QuicklinksLoginUrl)
                .PostJsonAsync(new
                {
                    username = _paymobSettings.Username,
                    password = _paymobSettings.Password,
                }).ReceiveJson<QuickLinkLoginResponse>();

        return result.Token;
    }
    public async Task<(string? id, string? url, string? order)> GetPaymentQuicklink(string email, string phone, string fullName, decimal amount, List<PaymobItem>? items = null, string? description = default)
    {
        try
        {
            var token = await GetToken();
            if (!token.HasValue())
                return (string.Empty, string.Empty, string.Empty);
            var url = (_paymobSettings.Url + _paymobSettings.CreatePaymentLinkUrl);


            var result = await url.WithOAuthBearerToken(token)
                         .PostJsonAsync(new
                         {
                             amount_cents = amount * 100,
                             full_name = fullName,
                             email = email,
                             payment_methods = _paymobSettings.PaymentMethods,
                             is_live = _paymobSettings.IsLive,
                             // items = items,
                             // description = description
                         })
                         .ReceiveJson<CreateQuickLinkResponse>();

            return (result.Id.ToString(), result.Client_url, result.Order.ToString());
        }
        catch (FlurlHttpException ex)
        {
            var c = await ex.GetResponseStringAsync();
            _logger.LogError(ex.InnerException, c);
            _logger.LogError(c);
            return (string.Empty, string.Empty, string.Empty);
        }
    }

    #region Plan
    public async Task<string> CreateSubsciptionPlan(int frequency, string type)
    {
        try
        {
            var token = await GetToken();
            string url = _paymobSettings.Url + _paymobSettings.CreateSubscriptionPlanLinkUrl;
            Plan result = await url.WithOAuthBearerToken(token)
                     .PostJsonAsync(new
                     {
                         frequency,
                         name = type + " Plan",
                         integration = _paymobSettings.PaymentMethods.FirstOrDefault().ToString(),
                         use_transaction_amount = true,
                         is_active = true,
                     })
                     .ReceiveJson<Plan>();
            return result.Id.ToString();
        }
        catch (FlurlHttpException ex)
        {
            var c = await ex.GetResponseStringAsync();
            return string.Empty;
        }
    }

    public async Task<string> UpdateSubsciptionPlan(int id, int numberOfDeduction, int amountInCents, int integration)
    {
        try
        {
            var token = await GetToken();
            var url = (_paymobSettings.Url + _paymobSettings.UpdateSubscriptionPlanLinkUrl);
            url = url.Replace("{id}", id.ToString());
            var result = await url.WithOAuthBearerToken(token)
                     .PutJsonAsync(new
                     {
                         number_of_deductions = numberOfDeduction,
                         amount_cents = amountInCents,
                         integration = integration,
                     })
                     .ReceiveJson<Plan>();
            return result.Id.ToString();
        }
        catch (FlurlHttpException ex)
        {
            var c = await ex.GetResponseStringAsync();
            return string.Empty;
        }
    }

    public async Task ListSubsciptionPlan()
    {
        try
        {
            var token = await GetToken();
            var url = (_paymobSettings.Url + _paymobSettings.ListSubscriptionPlanLinkUrl);
            var result = await url.WithOAuthBearerToken(token)
                     .GetAsync()
                     .ReceiveString();
        }
        catch (FlurlHttpException ex)
        {
            var c = await ex.GetResponseStringAsync();
        }
    }

    public async Task<bool> SuspendSubsciptionPlan(int id)
    {
        try
        {
            var token = await GetToken();
            var url = (_paymobSettings.Url + _paymobSettings.SuspendSubscriptionPlanLinkUrl);
            url = url.Replace("{id}", id.ToString());
            var result = await url.WithOAuthBearerToken(token)
                     .PostAsync()
                     .ReceiveJson<Plan>();
            return !result.is_active;
        }
        catch (FlurlHttpException ex)
        {
            var c = await ex.GetResponseStringAsync();
            return false;
        }
    }

    public async Task<bool> ResumeSubsciptionPlan(int id)
    {
        try
        {
            var token = await GetToken();
            var url = (_paymobSettings.Url + _paymobSettings.ResumeSubscriptionPlanLinkUrl);
            url = url.Replace("{id}", id.ToString());
            var result = await url.WithOAuthBearerToken(token)
                     .PostAsync()
                     .ReceiveJson<Plan>();
            return result.is_active;
        }
        catch (FlurlHttpException ex)
        {
            var c = await ex.GetResponseStringAsync();
            return false;
        }
    }
    #endregion

    #region Subscription
    public async Task<(string? id, string? url)> CreateSubscriptionLink(string? id, string email, string phone, string fullName, decimal amount, int? subscriptionPlanId, string planName, string startDate, List<PaymobItem> items)
    {
        try
        {
            _logger.LogWarning(JsonSerializer.Serialize(items));

            var url = (_paymobSettings.Url + _paymobSettings.CreateIntentionLinkUrl);
            if(subscriptionPlanId != null)
            {
                _logger.LogWarning(subscriptionPlanId?.ToString());
                _logger.LogWarning(planName);
                _logger.LogWarning(startDate);

                var result = await url.WithOAuthBearerToken(_paymobSettings.SecretKey)
                         .PostJsonAsync(new
                         {
                             amount = (int)amount,
                             currency = "AED",
                             subscription_plan_id = subscriptionPlanId,
                             items = items, /// you have to multiple each item amount by 100
                             subscription_start_date = startDate, // must be yyyy-mm-dd and in the future

                             customer = new
                             {
                                 first_name = fullName,
                                 last_name = fullName,
                                 email
                             },
                             billing_data = new
                             {
                                 first_name = fullName,
                                 last_name = fullName,
                                 email,
                                 phone_number = phone,
                                 country = "uae",
                             },
                             payment_methods = _paymobSettings.PaymentMethods,
                             special_reference = id
                         })
                         .ReceiveJson<CreateIntentionLinkResponse>();

                if (!result.Id.HasValue() || !result.Client_secret.HasValue())
                {
                    return (string.Empty, string.Empty);
                }

                string checkoutUrl = $"https://uae.paymob.com/unifiedcheckout/?publicKey={_paymobSettings.PublicKey}&clientSecret={result.Client_secret}";

                return (result.Id, checkoutUrl);
            }
            else
            {
                var result = await url.WithOAuthBearerToken(_paymobSettings.SecretKey)
                         .PostJsonAsync(new
                         {
                             amount = (int)amount,
                             currency = "AED",
                             items = items, /// you have to multiple each item amount by 100

                             customer = new
                             {
                                 first_name = fullName,
                                 last_name = fullName,
                                 email
                             },
                             billing_data = new
                             {
                                 first_name = fullName,
                                 last_name = fullName,
                                 email,
                                 phone_number = phone,
                                 country = "uae",
                             },
                             payment_methods = _paymobSettings.PaymentMethods,
                             special_reference = id
                         })
                         .ReceiveJson<CreateIntentionLinkResponse>();

                if (!result.Id.HasValue() || !result.Client_secret.HasValue())
                {
                    return (string.Empty, string.Empty);
                }

                string checkoutUrl = $"https://uae.paymob.com/unifiedcheckout/?publicKey={_paymobSettings.PublicKey}&clientSecret={result.Client_secret}";

                return (result.Id, checkoutUrl);
            }
            
        }
        catch (FlurlHttpException ex)
        {
            var c = await ex.GetResponseStringAsync();
            _logger.LogError(ex.InnerException, c);
            _logger.LogError(c);
            return (string.Empty, string.Empty);
        }
    }
    public async Task<bool> SuspendSubsciption(string id)
    {
        try
        {
            var token = await GetToken();
            var url = (_paymobSettings.Url + _paymobSettings.SuspendSubscriptionUrl);
            url = url.Replace("{id}", id.ToString());
            var result = await url.WithOAuthBearerToken(token)
                     .PostAsync()
                     .ReceiveJson<Subscription>();
            return result.State == "suspended";
        }
        catch (FlurlHttpException ex)
        {
            var c = await ex.GetResponseStringAsync();
            return false;
        }
    }

    public async Task<bool> ResumeSubsciption(string id)
    {
        try
        {
            var token = await GetToken();
            var url = (_paymobSettings.Url + _paymobSettings.ResumeSubscriptionUrl);
            url = url.Replace("{id}", id.ToString());
            var result = await url.WithOAuthBearerToken(token)
                     .PostAsync()
                     .ReceiveJson<Subscription>();
            return result.State == "active";
        }
        catch (FlurlHttpException ex)
        {
            var c = await ex.GetResponseStringAsync();
            return false;
        }
    }

    public async Task<bool> CancelSubsciption(string id)
    {
        try
        {
            var token = await GetToken();
            var url = (_paymobSettings.Url + _paymobSettings.CancelSubscriptionUrl);
            url = url.Replace("{id}", id.ToString());
            var result = await url.WithOAuthBearerToken(token)
                     .PostAsync()
                     .ReceiveJson<Subscription>();
            return result.State == "canceled";
        }
        catch (FlurlHttpException ex)
        {
            var c = await ex.GetResponseStringAsync();
            return false;
        }
    }

    public async Task<bool> UpdateSubsciption(string id, int amountCents, string endsAt)
    {
        try
        {
            var token = await GetToken();
            var url = (_paymobSettings.Url + _paymobSettings.UpdateSubscriptionUrl);
            url = url.Replace("{id}", id.ToString());
            var result = await url.WithOAuthBearerToken(token)
                     .PutJsonAsync(new
                     {
                         amount_cents = amountCents,
                         ends_at = endsAt
                     })
                     .ReceiveJson<Subscription>();
            return true;
        }
        catch (FlurlHttpException ex)
        {
            var c = await ex.GetResponseStringAsync();
            return false;
        }
    }

    public async Task GetSubsciption(string id)
    {
        try
        {
            var token = await GetToken();
            var url = (_paymobSettings.Url + _paymobSettings.GetSubscriptionUrl);
            url = url.Replace("{id}", id.ToString());
            var result = await url.WithOAuthBearerToken(token)
                     .GetAsync()
                     .ReceiveJson<Subscription>();
        }
        catch (FlurlHttpException ex)
        {
            var c = await ex.GetResponseStringAsync();
        }
    }

    public async Task test(string id)
    {
        try
        {
            var token = await GetToken();
            // var url = ("https://uae.paymob.com/api/acceptance/subscriptions?transaction=665883");
            var url = ("https://uae.paymob.com/api/acceptance/subscriptions?transaction=665951");
            url = url.Replace("{id}", id.ToString());
            var result = await url.WithOAuthBearerToken(token)
                     .GetAsync()
                     .ReceiveJson<object>();
        }
        catch (FlurlHttpException ex)
        {
            var c = await ex.GetResponseStringAsync();
        }
    }
    #endregion
}



