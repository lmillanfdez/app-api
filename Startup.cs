using App_api.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using App_api.Contracts;
using App_api.Respositories;

namespace App_api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<ITodoRepository, TodoRepository>();
            services.AddResponseCaching();
            services.AddMemoryCache();
            
            /*var stringLocalConnection =
                "Server=(localdb)\\mssqllocaldb;Database=api-database-20180601;Trusted_Connection=True;MultipleActiveResultSets=true";*/
            
            var stringSQLExpressConnection = "Server=.\\MSSQLEXPRESS;Database=apiDb;Integrated Security=True;";
            
            services.AddEntityFrameworkSqlServer().AddDbContext<ApiDbContext>(options => options.UseSqlServer(stringSQLExpressConnection));
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseResponseCaching();

            app.UseMvc(routes =>
                {
                    routes.MapRoute(name: "default", template: "{controller = Home}/{action = Index}/{id?}");
                });
        }
    }
}