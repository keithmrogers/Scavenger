using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Orleans;
using Orleans.Runtime.Configuration;
using Scavenger.Server.GrainInterfaces;

namespace Scavenger.Server.OrleansHost
{
    class Program
    {
        private static readonly CancellationTokenSource CancellationTokenSource = new CancellationTokenSource();

        static void Main(string[] args)
        {
            Task.Run(() =>
            {
                AppDomain hostDomain = AppDomain.CreateDomain("OrleansHost", null, new AppDomainSetup
                {
                    AppDomainInitializer = InitSilo,
                    AppDomainInitializerArguments = args,
                });
            }).Wait();

            var endpoint = new IPEndPoint(IPAddress.Any, int.Parse(ConfigurationManager.AppSettings["ServerPort"]));
            var host = new ScavengerHost(endpoint);
            host.Start(CancellationTokenSource.Token).Wait();

            Console.WriteLine("Scavenger Server Started....");
            Console.ReadLine();
        }

        static void InitSilo(string[] args)
        {
            _hostWrapper = new OrleansHostWrapper(args);

            if (!_hostWrapper.Run())
            {
                Console.Error.WriteLine("Failed to initialize Orleans silo");
            }
        }

        static void ShutdownSilo()
        {
            if (_hostWrapper != null)
            {
                _hostWrapper.Dispose();
                GC.SuppressFinalize(_hostWrapper);
            }
        }

        private static OrleansHostWrapper _hostWrapper;
    }
}
