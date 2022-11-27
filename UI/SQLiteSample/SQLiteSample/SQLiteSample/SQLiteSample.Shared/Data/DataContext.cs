using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Net.Sockets;

namespace SQLiteSample.Data
{
    public class DataContext : DbContext
    {
        public DataContext() 
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var databasePath = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "MyData.db");

            optionsBuilder.UseSqlite($"Data Source={databasePath}");
            optionsBuilder.UseLoggerFactory(MyLoggerFactory);
            base.OnConfiguring(optionsBuilder);
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Socket>();
            modelBuilder.Entity<Valuation>();
        }

        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public virtual DbSet<Stock> Stocks { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public virtual DbSet<Valuation> Valuations { get; set; }


        public static readonly ILoggerFactory MyLoggerFactory
    = LoggerFactory.Create(builder => { builder.AddConsole(); });
    }
}