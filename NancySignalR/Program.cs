using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Hosting;
using Nancy;
using Nancy.Hosting.Self;
using Nancy.Owin;
using Owin;

namespace NancySignalR
{
    class Program
    {
        static void Main(string[] args)
        {
            const string webUrl = "http://localhost:31410";
            const string signalrUrl = "http://localhost:31411";

            var config = new HostConfiguration()
            {
                UrlReservations = new UrlReservations() {  CreateAutomatically = true }
            };

            using (var webHost = new Nancy.Hosting.Self.NancyHost(
                config,
                new Uri(webUrl)))
            {
                using (WebApp.Start<Startup>(signalrUrl))
                {
                    webHost.Start();
                    Console.Write("Press any key");
                    Console.ReadKey();
                    webHost.Stop();
                }
            }
        }
    }

    public class Startup
    {
        public void Configuration(Owin.IAppBuilder app)
        {
            app.MapSignalR();//new HubConfiguration() { EnableCrossDomain = true });
            app.UseNancy(new NancyOptions() { Bootstrapper = new ApplicationBootstrapper() });
            app.UseCors(CorsOptions.AllowAll);
        }
    }

    public class ApplicationBootstrapper : DefaultNancyBootstrapper
    {
        protected override void ConfigureConventions(
            Nancy.Conventions.NancyConventions nancyConventions)
        {
            
            nancyConventions.StaticContentsConventions.Add(
               Nancy.Conventions.StaticContentConventionBuilder
                .AddDirectory("Scripts", @"/Scripts")
            );
            base.ConfigureConventions(nancyConventions);
        }
    }

    public class Chat : Hub
    {
        public void Send(string message)
        {
            Clients.All.addMessage(message);
        }
    }
}
