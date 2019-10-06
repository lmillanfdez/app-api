using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

static class StartupExtensionMethods
{
    public static void InjectDependencies(this IServiceCollection services)
    {
        services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
    }

    public static void SetDbContexts(this IServiceCollection services, IConfiguration configuration)
    {
        var _connectionStrings = configuration.GetSection("ConnectionStrings")
                                            .Get<ConnectionStringsDTO>();
        
        services.AddEntityFrameworkSqlServer()
                .AddDbContext<IdentityServerDbContext>(options => options.UseSqlServer(_connectionStrings.DefaultConnection))
                .AddDbContext<ApiDbContext>(options => options.UseSqlServer(_connectionStrings.DefaultConnection));

        services.AddIdentity<User, Role>().AddEntityFrameworkStores<IdentityServerDbContext>();
    }

    public static void SetCorsPolicies(this IServiceCollection services)
    {
        var _defaultCorsPolicy = new CorsPolicyBuilder()
                                        .WithOrigins()
                                        .WithHeaders()
                                        .WithMethods("GET", "POST", "PUT", "DELETE")
                                        .Build();
        services.AddCors(options => 
                            options.AddDefaultPolicy(_defaultCorsPolicy));
    }
}