# .NET Quickstart

This is a minimal app that implements Deck using an ASP.NET Core backend with a basic HTML/vanilla JS frontend.

## TL;DR

```bash
npm install -g tunnelmole
tmole 5000 # Register https://your-tunnel-url.com/api/webhook in Deck dashboard

# Navigate to the project directory
cd src

# Copy the environment file and update with your credentials
cp .env.example .env
# Edit .env with your Deck credentials

# Run the application
dotnet run
```

Then visit http://localhost:5000 in your browser.

# Webhook

To receive data from Deck, set up a public webhook URL in your Deck dashboard. 

1. Create a tunnel to your local server using [tunnelmole](https://tunnelmole.com).
2. Set the webhook URL in [Dashboard](https://dashboard.deck.co/). e.g., `https://your-tunnel-url.com/api/webhook`

```
npm install -g tunnelmole
tmole 5000
```

Events sent to the webhook will be logged in the console. You can use this to test your webhook setup.

## Overview

This quickstart demonstrates how to:

1. Create a link token for the Deck widget
2. Initialize the Deck Link SDK in a browser
3. Handle events from the Deck Link SDK
4. Submit an EnsureConnection job
5. Handle webhook events

## Key Files

- `Program.cs` - ASP.NET Core web application that handles API requests
- `Deck.cs` - .NET client for the Deck API
- `wwwroot/index.html` - HTML frontend
- `wwwroot/index.js` - JavaScript for the frontend

## API Endpoints

- `/` - Serves the frontend
- `/ensure-connection` - Submits an EnsureConnection job to Deck
- `/api/create_link_token` - Creates a link token for the Deck widget
- `/api/webhook` - Receives webhook events from Deck

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

### Handling webhook events

```csharp
// In Program.cs
app.MapPost("/api/webhook", (HttpContext context, ILogger<Program> logger) =>
{
    // Process the webhook event
    using var reader = new StreamReader(context.Request.Body);
    var body = reader.ReadToEndAsync().Result;
    logger.LogInformation("Webhook received: {Body}", body);
    
    return "Webhook received";
});
```

## Development

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

### Configuration

Environment variables can be set in the `.env` file in the root of the project:

```
DECK_CLIENT_ID=your_client_id_here
DECK_SECRET=your_secret_here
PORT=5000
```

### Building

```bash
dotnet build
```

### Running

```bash
dotnet run
```