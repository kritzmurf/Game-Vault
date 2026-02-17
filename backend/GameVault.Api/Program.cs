var builder = WebApplication.CreateBuilder(args);

//Services
builder.Services.AddControllers();
//Automatic REST API documentation for development only
// See: https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

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
