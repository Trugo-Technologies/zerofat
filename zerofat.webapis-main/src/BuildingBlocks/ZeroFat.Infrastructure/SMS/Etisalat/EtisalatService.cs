using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using ZeroFat.Application.Common.Interfaces;

namespace ZeroFat.Infrastructure.SMS.Etisalat;

public class EtisalatService : ISMSService
{
    private readonly EtisalatOptions _settings;
    private readonly ILogger<EtisalatService> _logger;
    private readonly HttpClient _httpClient;
    private string? _token;
    private DateTime _tokenExpiry;
    private const int TokenRefreshHours = 8;

    public EtisalatService(EtisalatOptions settings, ILogger<EtisalatService> logger, HttpClient httpClient)
    {
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        _logger = logger;
        _httpClient = httpClient;
    }

    public async Task SendAsync(string phoneNumber, string message, bool isTransactional = true, CancellationToken ct = default)
    {

        // Validate and format phone number
        if (!TryFormatUaeNumber(phoneNumber, out var formattedNumber))
        {
            _logger.LogWarning("Skipping SMS to {PhoneNumber} - only UAE numbers (+971) are supported", phoneNumber);
            return;
        }

        try
        {
            if (string.IsNullOrEmpty(_token) || _tokenExpiry <= DateTime.UtcNow)
            {
                await RefreshTokenAsync(ct);
            }

            var requestBody = new
            {
                msgCategory = isTransactional ? _settings.TransactionCategoryId : _settings.PromotionalCategoryId,
                contentType = "3.1", // Default content type
                senderAddr = isTransactional ? _settings.TransactionSenderId : _settings.PromotionalSenderId,
                priority = isTransactional ? 1 : 2, // Higher priority for transactional
                recipient = formattedNumber,
                msg = message,
                dr = "1" // Delivery report requested
            };

            var request = new HttpRequestMessage(HttpMethod.Post, _settings.SendSmsEndpoint);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);
            request.Content = new StringContent(
                JsonSerializer.Serialize(requestBody),
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.SendAsync(request, ct);
            response.EnsureSuccessStatusCode();

            _logger.LogInformation("SMS ({Type}) sent successfully to {PhoneNumber}",
                isTransactional ? "Transactional" : "Promotional",
                phoneNumber);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to send {Type} SMS to {PhoneNumber}",
                isTransactional ? "Transactional" : "Promotional",
                phoneNumber);
            // throw new SmsServiceException($"Failed to send SMS to {phoneNumber}", e);
        }
    }

    public async Task SendTransactionalAsync(string phoneNumber, string message, CancellationToken ct = default)
    {
        await SendAsync(phoneNumber, message, true, ct);
    }

    public async Task SendPromotionalAsync(string phoneNumber, string message, CancellationToken ct = default)
    {
        await SendAsync(phoneNumber, message, false, ct);
    }

    private async Task RefreshTokenAsync(CancellationToken ct)
    {
        if (string.IsNullOrEmpty(_settings.LoginEndpoint) ||
            string.IsNullOrEmpty(_settings.Username) ||
            string.IsNullOrEmpty(_settings.Password))
        {
            throw new InvalidOperationException("Etisalat authentication credentials are not configured");
        }

        try
        {
            var loginRequest = new
            {
                username = _settings.Username,
                password = _settings.Password
            };

            var response = await _httpClient.PostAsJsonAsync(_settings.LoginEndpoint, loginRequest, ct);
            response.EnsureSuccessStatusCode();

            var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>(cancellationToken: ct);
            _token = authResponse?.Token;
            _tokenExpiry = DateTime.UtcNow.AddHours(TokenRefreshHours);

            _logger.LogInformation("Successfully refreshed Etisalat auth token");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to refresh Etisalat auth token");
            // throw new SmsServiceException("Failed to refresh authentication token", e);
        }
    }

    public async Task<bool> SendMulticast(string message, List<string?>? phoneNumbers, bool isTransactional = true, CancellationToken ct = default)
    {
        if (phoneNumbers == null || phoneNumbers.Count == 0)
            return true;

        var success = true;
        foreach (var phoneNumber in phoneNumbers.Where(pn => !string.IsNullOrEmpty(pn)))
        {
            try
            {
                await SendAsync(phoneNumber!, message, isTransactional, ct);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to send {Type} multicast SMS to {PhoneNumber}",
                    isTransactional ? "Transactional" : "Promotional",
                    phoneNumber);
                success = false;
            }
        }
        return success;
    }

    private bool TryFormatUaeNumber(string phoneNumber, out string formattedNumber)
    {
        formattedNumber = null;

        // Remove all non-digit characters
        var digitsOnly = new string(phoneNumber.Where(char.IsDigit).ToArray());

        // Check if number starts with UAE country code (with or without +)
        if (phoneNumber.StartsWith("+971", StringComparison.OrdinalIgnoreCase) && digitsOnly.StartsWith("971", StringComparison.OrdinalIgnoreCase))
        {
            // +9715... format - ensure it's 12 digits total
            if (digitsOnly.Length == 12)
            {
                formattedNumber = digitsOnly;
                return true;
            }
        }
        // Check for local UAE format (05...)
        else if (phoneNumber.StartsWith("05", StringComparison.OrdinalIgnoreCase) && digitsOnly.StartsWith("05", StringComparison.OrdinalIgnoreCase))
        {
            // Convert 05... to 9715...
            if (digitsOnly.Length == 10)
            {
                formattedNumber = "971" + digitsOnly.Substring(1);
                return true;
            }
        }
        // Check for raw 971... format
        else if (digitsOnly.StartsWith("971", StringComparison.OrdinalIgnoreCase) && digitsOnly.Length == 12)
        {
            formattedNumber = digitsOnly;
            return true;
        }

        return false;
    }
}

// Add this class to handle auth response
public class AuthResponse
{
    [JsonPropertyName("token")]
    public string? Token { get; set; }
}

// Custom exception for SMS service
public class SmsServiceException : Exception
{
    public SmsServiceException(string message) : base(message) { }
    public SmsServiceException(string message, Exception inner) : base(message, inner) { }
}
