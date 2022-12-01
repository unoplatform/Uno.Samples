using System;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using System.Threading;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace EFCoreSQLiteSample
{
    public class DataProvider
    {
        static DataProvider()
        {
#if __WASM__
            SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_sqlite3());
#endif
        }

        public static async Task<string[]> Run(string entry)
        {
            using (var db = new BloggingContext())
            {
                db.Database.EnsureCreated();

                Console.WriteLine("Database created");

                db.Blogs.Add(new Blog { Url = entry });
                var count = await db.SaveChangesAsync(CancellationToken.None);

                Console.WriteLine("{0} records saved to database", count);

                Console.WriteLine();
                Console.WriteLine("All blogs in database:");
                foreach (var blog in db.Blogs)
                {
                    Console.WriteLine(" - {0}", blog.Url);
                }

                return db.Blogs.Select(b => b.Url).ToArray();
            }
        }
    }
}
