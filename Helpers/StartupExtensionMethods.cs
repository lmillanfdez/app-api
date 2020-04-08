using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Swagger;

static class StartupExtensionMethods
{
    public static void InjectDependencies(this IServiceCollection services)
    {
        services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
    }

    public static void SetDbContexts(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionStrings = configuration.GetSection("ConnectionStrings")
                                            .Get<ConnectionStringsDTO>();
        
        services.AddEntityFrameworkSqlServer()
                .AddDbContext<IdentityServerDbContext>(options => options.UseSqlServer(connectionStrings.IdentityDbConnection))
                .AddDbContext<ApiDbContext>(options => options.UseSqlServer(connectionStrings.AppDbConnection));

        services.AddIdentity<IdentityUser, IdentityRole>()
        .AddEntityFrameworkStores<IdentityServerDbContext>()
        .AddDefaultTokenProviders(); 
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

                    /* options.Events = new JwtBearerEvents
                    {
                        OnChallenge = context => 
                        {
                            var headers = context.HttpContext.Response.Headers;

                            headers.Add("x-auth-failed-reason", "token-expired");
                            return Task.CompletedTask;
                        }
                    }; */
                                        
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = false,
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
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IAccountsService, AccountsService>();
    }

    public static void SetConfigurations(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ConfigurationsDTO>(configuration);
    }

    public static void AddTestingTools(this IServiceCollection services)
    {
        services.AddSwaggerGen(options => 
        {
            options.SwaggerDoc("v1", new Info
            {
                Version = "v1",
                Title = "App API",
                Description = "Generic Wep API",
                TermsOfService = "None",
                Contact = new Contact() 
                { 
                    Name = "Liander Millan", 
                    Email = "lmficr@gmail.com"
                }
            });

            options.AddSecurityDefinition("Bearer", new ApiKeyScheme
            {
                In = "header",
                Description = "Please insert JWT with Bearer into field",
                Name = "Authorization",
                Type = "apiKey"
            });

            options.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>>
            {
                { "Bearer", new string[] { } }
            });
        });
    }
}