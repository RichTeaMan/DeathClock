﻿using System;
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

namespace DeathClock
{
    class Program
    {
        private static IContainer Container { get; set; }

        static async Task Main(string[] args)
        {
            using (Container = BuildDiContainer())
            {

                Console.WriteLine("Beginning the Deathclock.");

                string resultDirectory;
                if (args.Length >= 1 && !string.IsNullOrWhiteSpace(args[0]))
                {
                    resultDirectory = args[0];
                }
                else
                {
                    resultDirectory = "Results";
                }

                Console.WriteLine($"Results will be written to '{resultDirectory}'.");

                var deathClock = Container.Resolve<DeathClock>();
                deathClock.ResultDirectory = resultDirectory;
                await deathClock.Start();

                Console.WriteLine("The Deathclock has finished.");
            }

        }

        /// <summary>
        /// Builds the dependency injection container.
        /// </summary>
        private static IContainer BuildDiContainer()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging();

            var containerBuilder = new ContainerBuilder();
            containerBuilder.Populate(serviceCollection);
            containerBuilder.RegisterType<DeathClock>().As<DeathClock>();
            return containerBuilder.Build();
        }
    }
}