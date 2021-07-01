using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Configuration;
using WebApi.Configuration.JWT;
using WebApi.DataLayer;
using WebApi.RabbitMQ;
using WebApi.RabbitMQ.Producer;
using WebApi.Swagger;
using WebApi.Utilities;

namespace WebApi
{
    public class Startup
    {
        private readonly SiteSettings _siteSetting;
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            _siteSetting = configuration.GetSection(nameof(SiteSettings)).Get<SiteSettings>();
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();
            services.AddSwagger();

            services.Configure<SiteSettings>(Configuration.GetSection(nameof(SiteSettings)));
            services.AddScoped<IJwtService, JwtService>();
            services.AddHttpClient();

            services.AddDbContext<RaimunDbContext>(options =>
            {
                options
                    .UseSqlServer(Configuration.GetConnectionString("SqlServer"));
            });
            services.AddMinimalMvc();

           services.AddJwtAuthentication(_siteSetting);

            services.AddCustomApiVersioning();


            #region RabbitMQ Dependencies

            services.AddSingleton<IRabbitMQConnection>(sp =>
            {
                var factory = new ConnectionFactory()
                {
                    HostName = Configuration["EventBus:HostName"]
                };

                if (!string.IsNullOrEmpty(Configuration["EventBus:UserName"]))
                {
                    factory.UserName = Configuration["EventBus:UserName"];
                }

                if (!string.IsNullOrEmpty(Configuration["EventBus:Password"]))
                {
                    factory.Password = Configuration["EventBus:Password"];
                }

                return new RabbitMQConnection(factory);
            });

            services.AddSingleton<EventBusRabbitMQConsumer>();
            services.AddSingleton<EventBusRabbitMQProducer>();
            #endregion

            // Add Hangfire services.
            services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(Configuration.GetConnectionString("SqlServer"), new SqlServerStorageOptions
                {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.Zero,
                    UseRecommendedIsolationLevel = true,
                    DisableGlobalLocks = true
                }));

            // Add the processing server as IHostedService
            services.AddHangfireServer();

            // Add framework services.
            services.AddMvc();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwaggerAndUI();

            }
            // HangFire Dashboard
            app.UseHangfireDashboard();

            app.IntializeDatabase();
            app.UseHttpsRedirection();
            app.UseHsts(env);
            app.UseRouting();

            //Initilize Rabbit Listener in ApplicationBuilderExtentions
            app.UseRabbitListener();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                // HangFire Dashboard endpoint
                endpoints.MapHangfireDashboard();
            });
        }
    }
}
