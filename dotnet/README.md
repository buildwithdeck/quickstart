# .NET Quickstart

This is a minimal app that implements Deck using an ASP.NET Core backend with a basic HTML/vanilla JS frontend.

## TL;DR

```bash
# Navigate to the project directory
cd src

# Copy the environment file and update with your credentials
cp .env.example .env
# Edit .env with your Deck credentials

# Run the application
dotnet run

npm install -g tunnelmole
tmole 8080 # Register https://your-tunnel-url.com/api/webhook in Deck dashboard
```

Then visit http://localhost:8080 in your browser.

# Webhook

To receive data from Deck, set up a public webhook URL in your Deck dashboard. 

1. Create a tunnel to your local server using [tunnelmole](https://tunnelmole.com).
2. Set the webhook URL in [Dashboard](https://dashboard.deck.co/). e.g., `https://your-tunnel-url.com/api/webhook`

```
npm install -g tunnelmole
tmole 8080
```

Events sent to the webhook will be logged in the console. You can use this to test your webhook setup.

## Overview

This quickstart demonstrates how to:

1. Create a link token for the Deck widget
2. Initialize the Deck Link SDK in a browser
3. Handle events from the Deck Link SDK
4. Handle webhook events and persist access tokens
5. Use access tokens to fetch payment methods

## Key Files

- `Program.cs` - ASP.NET Core web application that handles API requests
- `Deck.cs` - .NET client for the Deck API
- `WebhookHandler.cs` - Webhook handler for processing Deck events
- `wwwroot/index.html` - HTML frontend
- `wwwroot/index.js` - JavaScript for the frontend

## API Endpoints

- `/` - Serves the frontend
- `/api/create_link_token` - Creates a link token for the Deck widget
- `/api/webhook` - Receives webhook events from Deck
- `/api/fetch_payment_methods` - Use access token to get payment methods

## Example Usage

### Initialize the Deck client

```csharp
// The DeckClient is registered as a singleton service in the Program.cs file
// and can be injected into controllers or other services
public class MyController : ControllerBase
{
    private readonly DeckClient _deckClient;

    public MyController(DeckClient deckClient)
    {
        _deckClient = deckClient;
    }
}
```

### Create a widget token

```csharp
// Create a link token for the Deck widget
var token = await _deckClient.CreateWidgetToken();
```

### Submit a job to Deck

```csharp
// Submit any job to Deck
var response = await _deckClient.SubmitJob("FetchPaymentMethods", new { access_token = "your_access_token" });
```

### Handling webhook events

```csharp
// In Program.cs
app.MapPost("/api/webhook", async (HttpContext context, WebhookHandler webhookHandler, ILogger<Program> logger) =>
{
    try
    {
        // Read and parse the request body
        context.Request.EnableBuffering();
        using var reader = new StreamReader(context.Request.Body, leaveOpen: true);
        var body = await reader.ReadToEndAsync();
        context.Request.Body.Position = 0;
        
        var requestData = JsonSerializer.Deserialize<JsonElement>(body);
        var response = webhookHandler.HandleWebhook(requestData);
        
        return Results.Ok(response);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error processing webhook");
        return Results.Problem("Error processing webhook: " + ex.Message);
    }
});
```

### Accessing stored data

```csharp
// Access stored data from webhooks
if (webhookHandler.Database.TryGetValue("access_token", out var accessTokenObj) && 
    accessTokenObj is string accessToken)
{
    // Use the access token
}

if (webhookHandler.Database.TryGetValue("payment_methods", out var paymentMethodsObj))
{
    // Access payment methods
}
```

## Development

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

### Configuration

Environment variables can be set in the `.env` file in the root of the project:

```
DECK_CLIENT_ID=your_client_id_here
DECK_SECRET=your_secret_here
PORT=8080
```

### Building

```bash
dotnet build
```

### Running

```bash
dotnet run
```