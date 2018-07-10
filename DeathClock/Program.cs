using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Logging;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using RichTea.CommandLineParser;

namespace DeathClock
{
    class Program
    {
        private static IContainer Container { get; set; }

        static void Main(string[] args)
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

        static async Task xMain(string[] args)
        {
            await RunDeathDeathclock(new[] { "List_of_Scots" }, "testrun");
        }

        [DefaultClCommand]
        public static async Task RunDeathDeathclock(
            [ClArgs("list")]string[] listArticles,
            [ClArgs("outputDirectory", "od")]string outputDirectory = "Results")
        {
            Console.WriteLine("Beginning the Deathclock.");

            using (Container = BuildDiContainer(outputDirectory))
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
        private static IContainer BuildDiContainer(string resultDirectory)
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging(l =>
            {
                l.AddConsole().AddDebug().AddFile(Path.Combine(resultDirectory, "log.txt"), LogLevel.Trace);
            });

            var containerBuilder = new ContainerBuilder();
            containerBuilder.Populate(serviceCollection);
            containerBuilder.RegisterType<DeathClock>().As<DeathClock>();
            containerBuilder.RegisterType<PersonFactory>().As<PersonFactory>();
            return containerBuilder.Build();
        }
    }
}