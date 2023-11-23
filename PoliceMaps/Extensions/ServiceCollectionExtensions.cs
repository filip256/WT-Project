using Microsoft.EntityFrameworkCore;

namespace PoliceMaps.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void RunMigrations<TContext>(this IServiceProvider provider)
            where TContext : DbContext
        {
            using var scope = provider.CreateScope();
            using var context = scope.ServiceProvider.GetRequiredService<TContext>();

            context.Database.Migrate();
        }
    }
}
