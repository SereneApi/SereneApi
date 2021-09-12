using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SereneAp.Handlers.Rest.Benchmark.AspNet.API;
using SereneApi.Benchmark.AspNet.API;
using SereneApi.Core;
using SereneApi.Core.Requests;
using SereneApi.Extensions.Mocking.Rest;

namespace SereneAp.Handlers.Rest.Benchmark.AspNet
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
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

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.RegisterApi<IStudentApi, StudentApiHandler>(o =>
            {
                o.SetSource("http://localhost", "Student");
            }).EnableRestMocking(m =>
            {
                m.RegisterMockResponse()
                    .ForMethod(Method.Get)
                    .RespondsWith(new StudentDto
                    {
                        Email = "John.Smith@gmail.com",
                        FirstName = "John",
                        LastName = "Smith",
                        Id = 0
                    });
            });

            services.AddRazorPages();
        }
    }
}