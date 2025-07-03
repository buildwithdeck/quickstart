using System.Text.Json;
using DeckQuickstart;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddSingleton<DeckClient>();
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
app.MapGet("/ensure-connection", async (HttpContext context, DeckClient deck, ILogger<Program> logger) =>
{
    try
    {
        await deck.EnsureConnection();
        logger.LogInformation("EnsureConnection job submitted successfully");
        return Results.Text("EnsureConnection triggered. Check your webhook logs.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error submitting EnsureConnection job");
        return Results.Problem("Error submitting EnsureConnection job: " + ex.Message);
    }
});

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

app.MapPost("/api/webhook", async (HttpContext context, ILogger<Program> logger) =>
{
    try
    {
        // Read and log the request body
        context.Request.EnableBuffering();
        using var reader = new StreamReader(context.Request.Body, leaveOpen: true);
        var body = await reader.ReadToEndAsync();

        // Reset the position to allow the body to be read again if needed
        context.Request.Body.Position = 0;
        
        logger.LogInformation("Webhook received: {Body}", body);
        
        return Results.Ok("Webhook received");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error processing webhook");
        return Results.Problem("Error processing webhook: " + ex.Message);
    }
});

// Create the wwwroot directory if it doesn't exist
if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")))
{
    Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"));
}

app.Run();
