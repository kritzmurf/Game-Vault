var builder = WebApplication.CreateBuilder(args);

//Services
builder.Services.AddControllers();
//Automatic REST API documentation for development only
// See: https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
