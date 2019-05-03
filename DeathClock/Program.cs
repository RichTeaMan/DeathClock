using Autofac;
using Autofac.Extensions.DependencyInjection;
using DeathClock.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RichTea.CommandLineParser;
using RichTea.WebCache;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DeathClock
{
    internal class Program
    {
        private static IContainer Container { get; set; }

        private static void Main(string[] args)
        {
            MethodInvoker command = null;
            try
            {
                command = new CommandLineParserInvoker().GetCommand(typeof(Program), args);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error parsing command:");
                Console.WriteLine(ex);
            }
            if (command != null)
            {
                try
                {
                    command.Invoke();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error running command:");
                    Console.WriteLine(ex);

                    var inner = ex.InnerException;
                    while (inner != null)
                    {
                        Console.WriteLine(inner);
                        Console.WriteLine();
                        inner = inner.InnerException;
                    }

                    Console.WriteLine(ex.StackTrace);
                }
            }
        }

        [ClCommand("tmdb")]
        public static async Task RunDeathDeathclockTmdbData(
            [ClArgs("outputDirectory", "od")]string outputDirectory = "Results",
            [ClArgs("cacheDirectory", "cd")]string cacheDirectory = null,
            [ClArgs("tmdbApiKey")]string tmdbApiKey = null)
        {
            Console.WriteLine("Beginning the Deathclock.");

            var config = new ConfigurationBuilder()
                .AddUserSecrets("9b9374a9-4a72-4657-a398-2e265456aaf2")
                .Build();

            if (string.IsNullOrEmpty(tmdbApiKey))
            {
                tmdbApiKey = config.GetValue<string>("TmdbApiKey");
                Console.WriteLine("Using TMDB API key from stored secrets.");
            }
            else
            {
                Console.WriteLine("Using TMDB API key from command line parameters.");
            }


            using (Container = BuildDiContainer(outputDirectory, cacheDirectory))
            {
                Directory.CreateDirectory(outputDirectory);

                Console.WriteLine($"Results will be written to '{outputDirectory}'.");

                var context = Container.Resolve<DeathClockContext>();
                var tmdbFactory = Container.Resolve<Tmdb.TmdbFactory>();

                var persons = await tmdbFactory.GetMoviePersonList(tmdbApiKey);

                await context.TmdbPersons.AddRangeAsync(persons.ToArray());
                await context.SaveChangesAsync();

                Console.WriteLine("The Deathclock has finished.");
            }
        }

        [DefaultClCommand]
        public static async Task RunDeathDeathclock(
            [ClArgs("list")]string[] listArticles,
            [ClArgs("outputDirectory", "od")]string outputDirectory = "Results",
            [ClArgs("cacheDirectory", "cd")]string cacheDirectory = null)
        {
            Console.WriteLine("Beginning the Deathclock.");

            using (Container = BuildDiContainer(outputDirectory, cacheDirectory))
            {
                Directory.CreateDirectory(outputDirectory);

                Console.WriteLine($"Results will be written to '{outputDirectory}'.");

                var deathClock = Container.Resolve<DeathClock>();
                deathClock.OutputDirectory = outputDirectory;
                await deathClock.Start(listArticles);

                Console.WriteLine("The Deathclock has finished.");
            }
        }

        /// <summary>
        /// Builds the dependency injection container.
        /// </summary>
        private static IContainer BuildDiContainer(string resultDirectory, string cacheDirectory)
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging(l =>
            {
                l.AddConsole().AddDebug().AddFile(Path.Combine(resultDirectory, "log.txt"), LogLevel.Trace);
            });

            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            var config = builder.Build();
            var connectionString = config.GetConnectionString("DeathClockDatabase");

            serviceCollection.AddDbContext<DeathClockContext>(options => options.UseSqlServer(connectionString), ServiceLifetime.Transient);

            var containerBuilder = new ContainerBuilder();
            containerBuilder.Populate(serviceCollection);
            containerBuilder.RegisterType<DeathClock>().SingleInstance();
            containerBuilder.RegisterType<PersonFactory>().SingleInstance();
            containerBuilder.RegisterType<WikiListFactory>().SingleInstance();
            containerBuilder.RegisterType<WikiUtility>().SingleInstance();
            containerBuilder.RegisterType<DeathClockContext>().SingleInstance();
            containerBuilder.RegisterType<PersonMapper>().SingleInstance();
            containerBuilder.RegisterType<Tmdb.TmdbFactory>().SingleInstance();

            WebCache webCache;
            if (string.IsNullOrEmpty(cacheDirectory))
            {
                webCache = new WebCache("DeathListCache");
            }
            else
            {
                webCache = new WebCache("DeathListCache", cacheDirectory);
            }

            containerBuilder.RegisterInstance(webCache);
            return containerBuilder.Build();
        }
    }
}