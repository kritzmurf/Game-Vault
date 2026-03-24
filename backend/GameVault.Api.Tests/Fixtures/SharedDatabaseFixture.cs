/* SharedDataBaseFixture
 *
 * Class that xunit creates once before any tests run:
 *
 * 1)   Starts PostgreSQL Docker container via Testcontainers
 * 2)   Runs all DbUp migrations against it (schema should match production)
 * 3)   Exposes the connection string for tests
 * 4)   Tears down the container on completion
 */

using Testcontainers.PostgreSql;
using DbUp;

namespace GameVault.Api.Tests.Fixtures;

public class SharedDatabaseFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder("postgres:17")
        .Build();

    public string ConnectionString => _container.GetConnectionString();

    public async Task InitializeAsync()
    {
        await _container.StartAsync();

        var upgrader = DeployChanges.To
            .PostgresqlDatabase(ConnectionString)
            .WithScriptsEmbeddedInAssembly(typeof(Program).Assembly)
            .LogToConsole()
            .Build();

        var result = upgrader.PerformUpgrade();
        if (!result.Successful)
            throw new Exception("Database migration failed", result.Error);
    }

    public async Task DisposeAsync()
    {
        await _container.DisposeAsync();
    }
}

