using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using Testcontainers.PostgreSql;
using WebApi.Common.Options;

namespace IntegrationTests;

[TestFixture]
[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[Parallelizable(ParallelScope.All)]
internal abstract class IntegrationTestsBase
{
    [SetUp]
    public async Task SetupEnv()
    {
        _pgContainer = new PostgreSqlBuilder()
            .WithImage("postgres:15-alpine")
            .Build();

        await _pgContainer.StartAsync();

        ApplicationFactory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(
                builder =>
                {
                    builder.ConfigureServices(
                        services =>
                        {
                            services.AddSingleton(
                                Options.Create(
                                    new PostgresOptions
                                    {
                                        ConnectionString = _pgContainer.GetConnectionString()
                                    }));
                        });
                });
    }

    [TearDown]
    public async Task TearDownEnv()
    {
        await _pgContainer.DisposeAsync();
        await ApplicationFactory.DisposeAsync();
    }

    protected WebApplicationFactory<Program> ApplicationFactory { get; private set; }
    private PostgreSqlContainer _pgContainer;
}