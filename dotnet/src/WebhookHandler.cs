using System.Collections.Concurrent;
using System.Text.Json;

namespace DeckQuickstart
{
    public class WebhookHandler
    {
        private readonly ConcurrentDictionary<string, object> _database = new();
        private readonly ILogger<WebhookHandler> _logger;

        public WebhookHandler(ILogger<WebhookHandler> logger)
        {
            _logger = logger;
        }

        public IReadOnlyDictionary<string, object> Database => _database;

        private void Persist(Dictionary<string, object> data)
        {
            foreach (var kvp in data)
            {
                _database.AddOrUpdate(kvp.Key, kvp.Value, (key, oldValue) => kvp.Value);
            }
            
            _logger.LogInformation("✓ Updated database: {Database}", 
                JsonSerializer.Serialize(_database, new JsonSerializerOptions { WriteIndented = true }));
        }

        public string HandleWebhook(JsonElement requestData)
        {
            _logger.LogInformation("Webhook received: {Data}", requestData.ToString());

            if (requestData.TryGetProperty("output", out var output) && 
                requestData.TryGetProperty("webhook_code", out var webhookCodeElement))
            {
                var webhookCode = webhookCodeElement.GetString();

                switch (webhookCode)
                {
                    case "EnsureConnection":
                        if (output.TryGetProperty("access_token", out var accessTokenElement))
                        {
                            var accessToken = accessTokenElement.GetString();
                            if (!string.IsNullOrEmpty(accessToken))
                            {
                                Persist(new Dictionary<string, object> { ["access_token"] = accessToken });
                            }
                        }
                        break;

                    case "FetchPaymentMethods":
                        if (output.TryGetProperty("payment_methods", out var paymentMethodsElement))
                        {
                            var paymentMethods = paymentMethodsElement.Clone();
                            Persist(new Dictionary<string, object> { ["payment_methods"] = paymentMethods });
                        }
                        break;
                }
            }

            return "Webhook received";
        }
    }
}
