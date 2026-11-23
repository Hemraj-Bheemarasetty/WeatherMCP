using ModelContextProtocol.AspNetCore;
using ModelContextProtocol.Server;
var builder = WebApplication.CreateBuilder(args);

// Register MCP services and configure tools
builder.Services
    .AddMcpServer()
    .WithHttpTransport()
    .WithToolsFromAssembly();

builder.Services.AddHttpClient("WeatherApi", client =>
{
    client.BaseAddress = new Uri("https://api.weatherapi.com/v1/");
});

var app = builder.Build();

// Map the MCP endpoint
app.MapMcp("/mcp");

app.Run();