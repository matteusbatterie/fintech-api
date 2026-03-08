using FinTech.Domain.Interfaces;
using FinTech.Infrastructure.Persistence;
using FinTech.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FinTech.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        ArgumentException.ThrowIfNullOrEmpty(connectionString);

        services.AddDbContext<FinTechDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IAccountRepository, AccountRepository>();

        return services;
    }
}
