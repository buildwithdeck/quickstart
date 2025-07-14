using System.Text.Json;
using DeckQuickstart;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddSingleton<DeckClient>();
builder.Services.AddSingleton<WebhookHandler>();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Load environment variables from .env file if present
if (File.Exists(Path.Combine(Directory.GetCurrentDirectory(), ".env")))
{
    foreach (var line in File.ReadAllLines(Path.Combine(Directory.GetCurrentDirectory(), ".env")))
    {
        if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
            continue;

        var parts = line.Split('=', 2, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 2)
        {
            Environment.SetEnvironmentVariable(parts[0], parts[1]);
        }
    }
}

// Add environment variables from .env to configuration
builder.Configuration.AddEnvironmentVariables();

var app = builder.Build();

app.UseCors();

// Serve static files from wwwroot
app.UseStaticFiles();

// Default route to index.html
app.MapGet("/", () =>
{
    return Results.Redirect("/index.html");
});

// Quickstart API endpoints
app.MapPost("/api/create_link_token", async (HttpContext context, DeckClient deck, ILogger<Program> logger) =>
{
    try
    {
        var result = await deck.CreateWidgetToken();
        logger.LogInformation("Link token created successfully");
        return Results.Json(result);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error creating link token");
        return Results.Problem("Error creating link token: " + ex.Message);
    }
});

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

app.MapGet("/api/fetch_payment_methods", async (HttpContext context, DeckClient deck, WebhookHandler webhookHandler, ILogger<Program> logger) =>
{
    try
    {
        if (!webhookHandler.Database.TryGetValue("access_token", out var accessTokenObj) || 
            accessTokenObj is not string accessToken || 
            string.IsNullOrEmpty(accessToken))
        {
            return Results.BadRequest("No access token found. Please link an account first.");
        }

        await deck.SubmitJob("FetchPaymentMethods", new { access_token = accessToken });
        return Results.Text("FetchPaymentMethods triggered. Check your webhook logs for payment methods.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error fetching payment methods");
        return Results.Problem("Error fetching payment methods: " + ex.Message);
    }
});

// Create the wwwroot directory if it doesn't exist
if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")))
{
    Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"));
}

app.Run();
