using BackgardenBlazor.Data;
using BackgardenBlazor.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackgardenBlazor.Services
{
    public interface ISprinklerService
    {
        Task<List<SprinklerModel>> LoadSprinklersAsync();
        Task<SprinklerModel> LoadSprinklerAsync(int id);
    }

    public class SprinklerService : ISprinklerService
    {
        private readonly ILogger<SprinklerService> _logger;
        private readonly SprinklerContext _ctx;
        
        public SprinklerService(ILogger<SprinklerService> logger, SprinklerContext ctx)
        {
            _logger = logger;
            _ctx = ctx;
            //_data.Add(new SprinklerModel
            //{
            //    Id = 1,
            //    Description = "Werfer",
            //    GpioPort = 12
            //});
            //_data.Add(new SprinklerModel
            //{
            //    Id = 1,
            //    Description = "Werfer",
            //    GpioPort = 12
            //});
            //_data.Add(new SprinklerModel
            //{
            //    Id = 1,
            //    Description = "Werfer",
            //    GpioPort = 12
            //});
        }

        public async Task<List<SprinklerModel>> LoadSprinklersAsync()
        {
            var ret = new List<SprinklerModel>();
            try
            {
                ret = await _ctx.Sprinklers.ToListAsync();
            }
            catch (Exception ex)
            {
                var emsg = $"Error loading sprinklers: {ex.Message}";
                _logger.LogError(ex, emsg);
                throw new Exception(emsg, ex);
            }
            return ret;
        }

        public async Task<SprinklerModel> LoadSprinklerAsync(int id)
        {
            var ret = new SprinklerModel();
            try
            {
                ret = await _ctx.Sprinklers.Where(x => x.Id == id).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                var emsg = $"Error loading sprinkler with id {id}: {ex.Message}";
                _logger.LogError(ex, emsg);
                throw new Exception(emsg, ex);
            }
            return ret;
        }
    }
}
