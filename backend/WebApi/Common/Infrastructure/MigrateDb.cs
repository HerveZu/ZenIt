using Microsoft.EntityFrameworkCore;

namespace WebApi.Common.Infrastructure;

// ReSharper disable once SuggestBaseTypeForParameterInConstructor
internal sealed class MigrateDb(AppDbContext dbContext, ILogger<MigrateDb> logger)
    : IStartupService
{
    public async Task Run()
    {
        if (EF.IsDesignTime)
        {
            return;
        }

        logger.LogInformation("Applying migrations...");

        await dbContext.Database.MigrateAsync();

        logger.LogInformation("Migrations have been applied");
    }
}