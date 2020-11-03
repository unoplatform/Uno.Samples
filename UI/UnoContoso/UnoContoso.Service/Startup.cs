using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using UnoContoso.Repository;
using UnoContoso.Repository.Sql;

namespace UnoContoso.Service
{
    public class Startup
    {
        readonly string MyPolicy = "_myPolicy";
        private IWebHostEnvironment _appHost;

        public Startup(IConfiguration configuration, IWebHostEnvironment hostEnvironment)
        {
            Configuration = configuration;
            _appHost = hostEnvironment;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers()
                .AddNewtonsoftJson(options => 
                    options.SerializerSettings.ReferenceLoopHandling 
                    = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
                
            services.AddCors(options => 
            {
                options.AddPolicy(name: MyPolicy,
                    builder => 
                    {
                        builder
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowCredentials()
                            .SetIsOriginAllowed((host) => true);
                    });
            });
            var db = new ContosoContext(new DbContextOptionsBuilder<ContosoContext>()
                .UseSqlite($"Data Source={_appHost.ContentRootPath}/Contoso.db").Options);
            services.AddScoped<ICustomerRepository, SqlCustomerRepository>(_ => new SqlCustomerRepository(db));
            services.AddScoped<IOrderRepository, SqlOrderRepository>(_ => new SqlOrderRepository(db));
            services.AddScoped<IProductRepository, SqlProductRepository>(_ => new SqlProductRepository(db));
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseCors(MyPolicy);

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
