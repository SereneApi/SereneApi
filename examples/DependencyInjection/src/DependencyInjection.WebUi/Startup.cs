using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DependencyInjection.API;
using DependencyInjection.WebUi.Handlers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace DependencyInjection.WebUi
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
            services.AddControllers();


            // Add an ApiHandler to the services collection, this enables dependency injection.
            // You are required to supply an interface for the definition and a class inheriting form ApiHandler and the interface as the definition.
            services.AddApiHandler<IStudentApi, StudentApiHandler>(builder =>
            {
                // Under appsettings.conf, there is an array called ApiConfig.
                // Inside that array is another array called "Student" as you can see below we are getting that.
                builder.UseConfiguration(Configuration.GetApiConfig("Student"));
            });

            // Here a provider is also being used, this allows you to get services that have been registered with dependency injection
            services.AddApiHandler<IClassApi, ClassApiHandler>((builder, provider) =>
            {
                // Instead of using appsettings. You can also manually specify the source information.
                builder.UseSource("http://localhost:52279", "Class");
                
                // Using the provider you can inject a logger factory.
                builder.AddLoggerFactory(provider.GetRequiredService<ILoggerFactory>());
            });

            services.AddSwaggerGen(c => c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebUi", Version = "v1" }));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("../swagger/v1/swagger.json", "WebUi"); });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
