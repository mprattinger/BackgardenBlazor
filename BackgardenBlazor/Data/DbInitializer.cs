using BackgardenBlazor.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace BackgardenBlazor.Data
{
    public class DbInitializer
    {
        public static void Initialize(IServiceProvider services)
        {
            var logger = services.GetRequiredService<ILogger<DbInitializer>>();
            var ctx = services.GetRequiredService<SprinklerContext>();

            logger.LogInformation("DB Initializer starting....");

            ctx.Sprinklers.Add(new SprinklerModel
            {
                Description = "Sprüher",
                GpioPort = 1
            });
            ctx.Sprinklers.Add(new SprinklerModel
            {
                Description = "Werfer",
                GpioPort = 4
            });
            ctx.Sprinklers.Add(new SprinklerModel
            {
                Description = "Tropfer",
                GpioPort = 5
            });
            ctx.SaveChanges();

            logger.LogInformation("DB Initializer done!");
        }
    }
}
