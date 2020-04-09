using BackgardenBlazor.Data;
using BackgardenBlazor.Models;
using BackgardenBlazor.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Device.Gpio;

namespace BackgardenBlazor
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();

            var gpioSettings = new GpioSettingsConfiguration();
            Configuration.Bind("GpioSettings", gpioSettings);
            services.AddSingleton(gpioSettings);

#if Linux
            services.AddSingleton<GpioController>();
#else
            services.AddSingleton<GpioController>(x => new GpioController(PinNumberingScheme.Logical, new GpioDriverMock()));
#endif

            services.AddEntityFrameworkSqlite().AddDbContext<SprinklerContext>();

            services.AddSingleton<AppState>();
            //services.AddSingleton<GpioService>(x => 
            //    new GpioService(x.GetRequiredService<AppState>(), x.GetRequiredService<SprinklerContext>(), x.GetRequiredService<GpioSettingsConfiguration>())
            //);
            services.AddSingleton<GpioService>();

            services.AddScoped<ISprinklerService, SprinklerService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, SprinklerContext context)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            //app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            context.Database.EnsureCreated();

            var gpio = app.ApplicationServices.GetService<GpioService>();
            gpio.SetupGpio();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
