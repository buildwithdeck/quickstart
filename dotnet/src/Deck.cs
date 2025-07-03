using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DeckQuickstart
{
    public class DeckClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl = "https://sandbox.deck.co/api/v1";
        private readonly string? _clientId;
        private readonly string? _secret;
        private readonly ILogger<DeckClient> _logger;

        public DeckClient(IConfiguration configuration, ILogger<DeckClient> logger)
        {
            _httpClient = new HttpClient();
            _clientId = configuration["DECK_CLIENT_ID"];
            _secret = configuration["DECK_SECRET"];
            _logger = logger;

            if (string.IsNullOrEmpty(_clientId) || string.IsNullOrEmpty(_secret))
            {
                _logger.LogWarning("Warning: DECK_CLIENT_ID or DECK_SECRET environment variables are not set");
            }
        }

        public async Task<T> PostAsync<T>(string endpoint, object? bodyJson = null)
        {
            try
            {
                if (string.IsNullOrEmpty(_clientId) || string.IsNullOrEmpty(_secret))
                {
                    throw new InvalidOperationException("DECK_CLIENT_ID and DECK_SECRET environment variables must be set");
                }

                var request = new HttpRequestMessage(HttpMethod.Post, $"{_apiUrl}/{endpoint}");
                request.Headers.Add("x-deck-client-id", _clientId);
                request.Headers.Add("x-deck-secret", _secret);

                if (bodyJson != null)
                {
                    string json = JsonSerializer.Serialize(bodyJson, new JsonSerializerOptions { WriteIndented = true });
                    _logger.LogDebug("Request payload: {Json}", json);
                    request.Content = new StringContent(json, Encoding.UTF8, "application/json");
                }
                else
                {
                    // Even with no body, ensure content type is set
                    request.Content = new StringContent("{}", Encoding.UTF8, "application/json");
                }

                _logger.LogInformation("Sending request to Deck API: {Endpoint}", endpoint);
                var response = await _httpClient.SendAsync(request);
                _logger.LogInformation("Deck API response: {StatusCode} {ReasonPhrase}", (int)response.StatusCode, response.ReasonPhrase);

                var content = await response.Content.ReadAsStringAsync();
                
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Deck API error: {StatusCode} {Content}", (int)response.StatusCode, content);
                    response.EnsureSuccessStatusCode(); // Will throw with appropriate status code
                }
                
                _logger.LogDebug("Response content: {Content}", content);
                return JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                    ?? throw new InvalidOperationException("Failed to deserialize response");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Deck API HTTP error: {Message}", ex.Message);
                throw;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Error deserializing Deck API response: {Message}", ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error communicating with Deck API: {Message}", ex.Message);
                throw;
            }
        }

        public Task<JsonElement> CreateWidgetToken()
        {
            return PostAsync<JsonElement>("link/token/create");
        }

        public Task<JsonElement> EnsureConnection()
        {
            var payload = new
            {
                job_code = "EnsureConnection",
                input = new
                {
                    password = "password",
                    username = "username",
                    source_guid = "a47c74e8-ae4a-425e-8583-922d5515654a" // Youtube
                }
            };

            return PostAsync<JsonElement>("jobs/submit", payload);
        }
    }
}
