﻿using System.Configuration;
using System.Linq;
using System.Threading;
using Ninject;
using Pushbaby.Server.Injection;
using Pushbaby.Server.Logging;
using log4net;

namespace Pushbaby.Server
{
    public class Server
    {
        public static readonly string ListenerPath = "";

        public static void Main()
        {
            Log4NetConfiguration.Configure();

            var settings = (Settings) ConfigurationManager.GetSection("pushbaby");

            if (settings == null)
                throw new ConfigurationErrorsException("No 'pushbaby' config section was found. See documentation.");

            var kernel = new StandardKernel();
            kernel.Load<NinjectBindings>();
            kernel.Bind<Settings>().ToConstant(settings);

            var server = kernel.Get<Server>();
            server.Run();
        }

        readonly ILog log;
        readonly Settings settings;
        readonly IEndpointFactory endpointFactory;
        readonly IThreadManager threadManager;

        public Server(ILog log, Settings settings, IEndpointFactory endpointFactory, IThreadManager threadManager)
        {
            this.log = log;
            this.settings = settings;
            this.endpointFactory = endpointFactory;
            this.threadManager = threadManager;
        }

        public void Run()
        {
            this.log.Info("Server is starting up...");

            foreach (var endpointSettings in this.settings.EndpointSettingsCollection.Cast<EndpointSettings>())
            {
                var endpoint = this.endpointFactory.Create(endpointSettings);
                this.threadManager.Create(endpoint.Listen);
            }

            this.log.InfoFormat("Server has started up with {0} endpoint(s).", this.settings.EndpointSettingsCollection.Count);

        }
    }

    public enum State { Greeting = 0, Greeted, Uploading, Uploaded, Executing, Executed }
}
