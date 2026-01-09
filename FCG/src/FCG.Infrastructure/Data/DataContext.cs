using Microsoft.EntityFrameworkCore;
using FCG.Domain.Entities;

namespace FCG.Infrastructure.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
            
        }

        public DbSet<VideoGames> VideoGames { get; set; }
        public DbSet<Library> Library { get; set; }
    }
}