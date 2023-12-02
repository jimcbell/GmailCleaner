using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;


namespace GmailCleaner.Common;

public static class GmailCleanerContextExtensions
{
    public static IServiceCollection AddGmailCleanerContext(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<GmailCleanerContext>(options =>
        {
            options.UseSqlServer(connectionString);
        });

        return services;
    }
}
