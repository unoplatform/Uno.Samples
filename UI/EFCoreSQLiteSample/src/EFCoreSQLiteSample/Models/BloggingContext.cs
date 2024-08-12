using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.IO;
using Windows.Storage;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace EFCoreSQLiteSample.Models
{
    public class BloggingContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Post> Posts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Uncomment to see EF Core logs
            // EnableLogging(optionsBuilder);

            // When building in app, use Windows.Storage.ApplicationData.Current.LocalFolder.Path
            // instead of /local to get browser persistence.
            var path = Path.Combine(ApplicationData.Current.LocalFolder.Path, "local.db");

            optionsBuilder.UseSqlite($"data source={path}");
        }

        private void EnableLogging(DbContextOptionsBuilder optionsBuilder)
        {
            var factory = LoggerFactory.Create(builder =>
            {
#if __WASM__
                // The Console logger cannot yet be used until .NET WebAssembly supports threading
                builder.AddProvider(new global::Uno.Extensions.Logging.WebAssembly.WebAssemblyConsoleLoggerProvider());
#else
                builder.AddConsole();
#endif

                // Exclude logs below this level
                builder.SetMinimumLevel(LogLevel.Information);
            });

            // Uncomment those to enable logging
            optionsBuilder.UseLoggerFactory(factory);
            optionsBuilder.EnableSensitiveDataLogging(true);
        }
    }
}
