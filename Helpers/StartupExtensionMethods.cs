using System;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

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

        services.AddIdentity<User, Role>()
                .AddEntityFrameworkStores<IdentityServerDbContext>();
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

    public static void SetAuthenticationSchema(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettingsDTO>();

        var encodedKey = Encoding.UTF8.GetBytes(jwtSettings.SecretKey);
        var signinKey = new SymmetricSecurityKey(encodedKey);

        services.AddAuthentication(options => {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; 
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; })
                .AddJwtBearer(options => {
                    options.ClaimsIssuer = jwtSettings.Issuer;
                                        
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateIssuerSigningKey = true,
                        ValidateLifetime = true,

                        ValidIssuer = jwtSettings.Issuer,
                        IssuerSigningKey = signinKey,
                        ClockSkew = TimeSpan.Zero
                    };
                });
    }

    public static void InjectServices(this IServiceCollection services)
    {
        services.AddScoped<IAccountsService, AccountsService>();
    }

    public static void SetConfigurations(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ConfigurationsDTO>(configuration);
    }
}