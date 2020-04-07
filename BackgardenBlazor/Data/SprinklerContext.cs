using BackgardenBlazor.Models;
using Microsoft.EntityFrameworkCore;

namespace BackgardenBlazor.Data
{
    public class SprinklerContext : DbContext
    {
        public DbSet<SprinklerModel> Sprinklers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=sqlitedemo.db");
    }
}
