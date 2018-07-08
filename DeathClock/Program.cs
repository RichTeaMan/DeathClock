using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Threading;

namespace DeathClock
{
    class Program
    {

        static async Task Main(string[] args)
        {
            string resultDirectory;
            if (args.Length >= 1 && !string.IsNullOrWhiteSpace(args[0]))
            {
                resultDirectory = args[0];
            } else
            {
                resultDirectory = "Results";
            }

            var deathClock = new DeathClock() { ResultDirectory = resultDirectory };
            await deathClock.Start();
        }
    }
}
