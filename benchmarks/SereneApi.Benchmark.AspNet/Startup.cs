using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SereneApi.Abstractions.Requests;
using SereneApi.Benchmark.AspNet.API;
using SereneApi.Extensions.DependencyInjection;
using SereneApi.Extensions.Mocking;

namespace SereneApi.Benchmark.AspNet
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
            services.RegisterApi<IStudentApi, StudentApiHandler>(o =>
            {
                o.SetSource("http://localhost", "Student");
            }).WithMockResponse(m =>
            {
                m.AddMockResponse(new StudentDto
                {
                    Email = "John.Smith@gmail.com",
                    FirstName = "John",
                    LastName = "Smith",
                    Id = 0
                }).RespondsToRequestsWith(Method.Get);
            });

            services.AddRazorPages();
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
    }
}
