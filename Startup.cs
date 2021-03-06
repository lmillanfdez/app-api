﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;

class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        services.InjectDependencies();
        services.InjectServices();

        services.SetAuthenticationSchema(Configuration);
        services.SetCorsPolicies();
        services.SetDbContexts(Configuration);

        services.AddResponseCaching();
        services.AddMemoryCache();

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
        });
        
        services.AddMvc();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
        if(env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(options => 
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Generic Web API");
            });
        }

        app.UseCors();
        app.UseStaticFiles();
        app.UseResponseCaching();
        app.UseAuthentication();

        app.UseMvc(routes =>
        {
            routes.MapRoute(name: "default", template: "{controller}/{action}/{id?}");
        });
    }
}