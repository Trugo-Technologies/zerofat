using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Serilog;
using Stripe;
using Stripe.Checkout;
using ZeroFat.Application.Common.Models;
using ZeroFat.ClientPortal.Api.Controllers;
using ZeroFat.ClientPortal.Application.Contracts;
using ZeroFat.ClientPortal.Application.SubscriptionManagement;
using ZeroFat.ClientPortal.Application.SubscriptionManagement.Payments;
using ZeroFat.Infrastructure.Stripe;

namespace ZeroFat.ClientPortal.Api.SubscriptionManagement;

internal sealed class PaymentsController : BaseController
{
    private readonly IClientPortalModule _clientPortalModule;
    private readonly StripeSettings _settings;

    public PaymentsController(IClientPortalModule clientPortalModule, IOptions<StripeSettings> settings)
    {
        _clientPortalModule = clientPortalModule;
        _settings = settings.Value;
    }

    [HttpPost("search")]
    public async Task<PaginationResponse<PaymentDto>> SearchAsync(SearchPaymentsRequest request)
        => await _clientPortalModule.ExecuteQueryAsync(request);

    [AllowAnonymous]
    [HttpGet("payment-success")] // Match the route in the SuccessUrl
    public async Task<ActionResult> PaymentSuccess(string sessionId, string subscriptionId)
    {
        Log.Information($"{sessionId} paymnet success");
        // Fetch the Checkout Session details from Stripe

        var result = await _clientPortalModule.ExecuteCommandAsync(new ConfirmSubscriptionPaymentStripeRequest(sessionId, subscriptionId));
        if (result.Succeeded)
            return Ok();
        else
            return BadRequest(result.Data);
    }

    [AllowAnonymous]
    [HttpPost("payment-success-webhook")] // Match the route in the SuccessUrl
    public async Task<ActionResult> PaymentWebhookSuccess()
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
        // Remember to use the webhook secret to verify the event is from Stripe
        var stripeEvent = EventUtility.ConstructEvent(json, Request.Headers["Stripe-Signature"], _settings.CheckoutCompletedSecretKey, throwOnApiVersionMismatch: false);

        if (stripeEvent.Type == "checkout.session.completed")
        {
            var session = stripeEvent.Data.Object as Session;

            // ✅ RETRIEVE YOUR ID HERE
            string yourDbSubscriptionId = session.Metadata["subscriptionId"];

            var result = await _clientPortalModule.ExecuteCommandAsync(new ConfirmSubscriptionPaymentStripeRequest(session.Id, yourDbSubscriptionId));
            if (result.Succeeded)
                return Ok();
            else
                return BadRequest(result.Data);

            // Console.WriteLine($"Payment succeeded for internal subscription: {yourDbSubscriptionId}");
        }

        return Ok();
    }

    [HttpGet("{id:guid}")]
    public async Task<Result<PaymentDetailsDto>> GetAsync(DefaultIdType id)
        => await _clientPortalModule.ExecuteQueryAsync(new GetPaymentRequest(id));
}
