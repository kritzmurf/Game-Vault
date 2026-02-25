using GameVault.Api.Data;
using System.Reflection;
using DbUp;

//C# styling and SQL styling dont play nice, and Dapper needs
//to be massaged to handle the differences
Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

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

//Database Migrations
var upgrader = DeployChanges.To
    .PostgresqlDatabase(connectionString)
    .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
    .LogToConsole()
    .Build();

var result = upgrader.PerformUpgrade();
if (!result.Successful)
    throw new Exception("Database migration failed", result.Error);

// Development helper to read API (disabled in release)
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

//This is intentionally not configured yet.
app.UseAuthorization();

app.MapControllers();

app.Run();
