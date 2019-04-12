
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RestSharp;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.IO;
using System.Reflection;
using WebService.Models;

namespace WebService
{
    public class Startup
    {
        /// <summary>
        /// Logger of OrderController
        /// Logs Requests, Results, Progress and Errors
        /// </summary>
        private readonly ILogger _logger;
        /// <summary>
        /// Combined configuration json files to one object
        /// </summary>
        public IConfiguration Configuration { get; }
        /// <summary>
        /// Definition of Database for Order Model
        /// </summary>
        public IOrderRepository<Order> Repository { get; set; }

        /// <summary>
        /// Startup of WebHost
        /// </summary>
        /// <param name="configuration">Configuration has reqired domain and tableName of orders of in-memory database</param>
        /// <param name="logger">Configured Logger from Program</param>
        public Startup(IConfiguration configuration, ILogger<Startup> logger)
        {
            _logger = logger;
            Configuration = configuration;

            // Initialisation of Order Database Repository
            Repository = new OrderRepository(configuration);
            
        }



        /// <summary>
        /// This method gets called by the runtime. Adding services to Web Host
        /// </summary>
        /// <param name="services">Dependency injection of services to WebHost</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddSingleton<IConfiguration>(Configuration);
            services.AddSingleton<ILogger>(_logger);
            
            services.AddTransient<IOrder, Order>();
            services.AddTransient<IRestClient, RestClient>();
            services.AddSingleton<IOrderRepository<Order>>(Repository);
            services.AddSwaggerGen(c => {
                c.SwaggerDoc("v1", new Info
                {
                    Title = "Auto Service Web Api",
                    Version = "v1"
                });
                // XML Documentation
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
           
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Auto Service Web Api v1");
            });
            app.UseMvc();
        }
    }
}
