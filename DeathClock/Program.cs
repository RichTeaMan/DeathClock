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
using System.Threading.Tasks;

namespace DeathClock
{
    internal class Program
    {
        private static IContainer Container { get; set; }

        private static int Main(string[] args)
        {
            int result = 0;
            MethodInvoker command = null;
            try
            {
                command = new CommandLineParserInvoker().GetCommand(typeof(Program), args);
            }
            catch (Exception ex)
            {
                result = -1;
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
                    result = 1;
                }
            }
            Console.ReadKey();
            return result;
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

                var tmdbFactory = Container.Resolve<Tmdb.TmdbFactory>();

                tmdbFactory.ApiKey = tmdbApiKey;
                await tmdbFactory.FindNewPersons();

                Console.WriteLine("The Deathclock has finished.");
            }
        }

        [ClCommand("tmdb-update")]
        public static async Task RunDeathDeathclockUpdateTmdbData(
            [ClArgs("outputDirectory", "od")]string outputDirectory = "Results",
            [ClArgs("cacheDirectory", "cd")]string cacheDirectory = null,
            [ClArgs("tmdbApiKey")]string tmdbApiKey = null)
        {
            Console.WriteLine("Beginning the Deathclock.");
            Console.WriteLine("Updating existing TMDB results.");

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

                var tmdbFactory = Container.Resolve<Tmdb.TmdbFactory>();

                tmdbFactory.ApiKey = tmdbApiKey;
                await tmdbFactory.UpdateExistingPersons();

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

                var deathClock = Container.Resolve<WikipediaPersonFactory>();
                deathClock.OutputDirectory = outputDirectory;
                deathClock.ListArticles = listArticles;
                await deathClock.FindNewPersons();

                Console.WriteLine("The Deathclock has finished.");
            }
        }

        /// <summary>
        /// Builds the dependency injection container.
        /// </summary>
        private static IContainer BuildDiContainer(string resultDirectory, string cacheDirectory)
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false);
            var config = builder.Build();
            var connectionString = config.GetConnectionString("DeathClockDatabase");

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging(l =>
            {
                l.AddConfiguration(config.GetSection("Logging")).AddConsole().AddDebug().AddFile(Path.Combine(resultDirectory, "log.txt"), LogLevel.Trace);
            });
            serviceCollection.AddDbContext<DeathClockContext>(options => options.UseSqlServer(connectionString), ServiceLifetime.Transient);

            WebCache.DefaultCacheName = "DeathListCache";
            if (!string.IsNullOrEmpty(cacheDirectory))
            {
                WebCache.DefaultCachePath = cacheDirectory;
            }
            var containerBuilder = new ContainerBuilder();
            containerBuilder.Populate(serviceCollection);
            containerBuilder.RegisterType<WikipediaPersonFactory>().SingleInstance();
            containerBuilder.RegisterType<PersonFactory>().SingleInstance();
            containerBuilder.RegisterType<WikiListFactory>().SingleInstance();
            containerBuilder.RegisterType<WikiUtility>().SingleInstance();
            containerBuilder.RegisterType<DeathClockContext>().SingleInstance();
            containerBuilder.RegisterType<WikipediaPersonMapper>().SingleInstance();
            containerBuilder.RegisterType<Tmdb.TmdbFactory>().SingleInstance();
            containerBuilder.RegisterType<WebCache>().SingleInstance();
            return containerBuilder.Build();
        }
    }
}