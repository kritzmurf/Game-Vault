using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace GameVault.Api.Tests.Fixtures;

public class GameVaultWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _connectionString;

    public GameVaultWebApplicationFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Override the connection string before Program.cs reads it
        builder.UseSetting("ConnectionStrings:DefaultConnection", _connectionString);
    }
}
