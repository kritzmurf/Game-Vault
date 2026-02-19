using GameVault.Api.Data;

var builder = WebApplication.CreateBuilder(args);

//Services
builder.Services.AddControllers();

//Automatic REST API documentation for development only
// See: https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

//Set up database connection
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");


builder.Services.AddSingleton(new DbConnectionFactory(connectionString));

var app = builder.Build();

// Development helper to read API (disabled in release)
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

//This is intentionally not configured yet.
app.UseAuthorization();

app.MapControllers();

app.Run();
