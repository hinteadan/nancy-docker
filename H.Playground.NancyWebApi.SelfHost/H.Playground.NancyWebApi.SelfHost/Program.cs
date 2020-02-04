using Microsoft.Owin.Hosting;
using Mono.Unix;
using Mono.Unix.Native;
using System;

namespace H.Playground.NancyWebApi.SelfHost
{
    class Program
    {
        static void Main(string[] args)
        {
#if DEBUG
            const string url = "http://localhost:8899";
#else
            const string url = "http://localhost:8888";
#endif

            using (WebApp.Start<Startup>(url))
            {
                Console.WriteLine($"Starting Nancy on {url}...");
#if DEBUG
                System.Diagnostics.Process.Start(url);
#endif

                if (IsRunningOnMono())
                {
                    var terminationSignals = GetUnixTerminationSignals();
                    UnixSignal.WaitAny(terminationSignals);
                }
                else
                {
                    Console.ReadLine();
                }
            }
        }

        private static bool IsRunningOnMono()
        {
            return Type.GetType("Mono.Runtime") != null;
        }

        private static UnixSignal[] GetUnixTerminationSignals()
        {
            return new[]
            {
                new UnixSignal(Signum.SIGINT),
                new UnixSignal(Signum.SIGTERM),
                new UnixSignal(Signum.SIGQUIT),
                new UnixSignal(Signum.SIGHUP)
            };
        }
    }
}
