using Application.Services;
using Domain.Repositories;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IAuthorRepository, AuthorRepository>();
        services.AddScoped<INewsRepository, NewsRepository>();
        services.AddScoped<IHashingService, HashingService>();
        services.AddScoped<IClaimsService, ClaimsService>();

        services.AddDbContext<ApplicationDbContext>(optionsBuilder =>
        optionsBuilder.UseSqlServer(configuration.GetConnectionString("Database")));
        //optionsBuilder.UseSqlServer("Server=.;Initial Catalog=news;Encrypt=False;Trusted_Connection=True;"));
        
        return services;
    }

    public static IServiceProvider ApplyMigrations(this IServiceProvider services)
    {
        using IServiceScope scope = services.CreateScope();
        using ApplicationDbContext context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        context.Database.Migrate();

        return services;
    }
}
