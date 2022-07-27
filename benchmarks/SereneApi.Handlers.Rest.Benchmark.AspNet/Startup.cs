using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SereneApi.Core.Configuration;
using SereneApi.Handlers.Rest.Benchmark.AspNet.API;
using System.Net.Http;

namespace SereneApi.Handlers.Rest.Benchmark.AspNet
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This httpMethod gets called by the runtime. Use this httpMethod to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }

        // This httpMethod gets called by the runtime. Use this httpMethod to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.RegisterApi<IStudentApi, StudentApiHandler>(o =>
            {
                o.SetSource("http://localhost", "Student");
                o.EnableMocking(c =>
                {
                    c.RegisterMockResponse()
                        .ForMethod(HttpMethod.Get)
                        .RespondsWith(new StudentDto
                        {
                            Email = "John.Smith@gmail.com",
                            FirstName = "John",
                            LastName = "Smith",
                            Id = 0
                        });
                });
            });

            services.AddRazorPages();
        }
    }
}