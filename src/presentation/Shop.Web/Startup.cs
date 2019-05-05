using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shop.Domain.Repositories;
using Shop.DataAccess.Repositories;
using Shop.Services;
using Shop.Services.Impl;
using StackExchange.Redis;
using Shop.DataAccess;

namespace Shop.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration,
            IHostingEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public IHostingEnvironment Environment { get; }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddMemoryCache();

            services.AddSingleton<IEventBus, EventBusStub>();
            services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();

            if (Environment.IsDevelopment())
            {
                services.AddTransient<ICatalogItemRepository, MemoryCatalogItemRepository>();
                services.AddTransient<ICartRepository, MemoryCartRepository>();
            }
            else
            {
                services.AddTransient<ICatalogItemRepository, SqlCatalogItemRepository>();

                services.AddSingleton<ConnectionMultiplexer>(sp =>
                {
                    var configuration = ConfigurationOptions.Parse(Configuration.GetConnectionString("RedisConnection"), true);

                    configuration.ResolveDns = true;

                    return ConnectionMultiplexer.Connect(configuration);
                });
                services.AddTransient<ICartRepository, RedisCartRepository>();
            }
            services.AddTransient<ICartManager, CartManager>();

            services.AddMvc(options =>
            {
                options.MaxModelValidationErrors = 50;
            })
            .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}