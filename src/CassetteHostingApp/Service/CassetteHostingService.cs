﻿using System;
using System.ServiceModel;
using System.ServiceProcess;
using System.Xml;
using CassetteHostingEnvironment.DependencyGraphInteration.Service;
using CassetteHostingEnvironment.Hosting;

namespace CassetteHostingEnvironment
{
    /// <summary>
    /// Trivial windows service to host the cassette WCF entry point. 
    /// </summary>
    public class CassetteHostingService : ServiceBase
    {
        public const string CassetteServiceName = "CassetteHostingService";

        public static void Main()
        {
            //Run();\
            var service = new CassetteHostingService();
            service.OnStart(null);
            Console.WriteLine("You have launched the out of process Cassette hosting app.");
            Console.WriteLine("");
            Console.WriteLine("Just let this window hang out and host your cassette for you.");
            Console.WriteLine("");
            Console.WriteLine("When you want to shut it down, just hit enter.");
            Console.WriteLine("");
            Console.ReadLine();
        }

        public ServiceHost ServiceHost = null;

        public CassetteHostingService()
        {
            ServiceName = CassetteServiceName;
        }

        protected override void OnStart(string[] args)
        {
            if (ServiceHost != null)
            {
                ServiceHost.Close();
            }

            var util = new InterationServiceUtility();

            ServiceHost = new ServiceHost(typeof(CassetteHost), new Uri(util.GetServiceUri()));

            ServiceHost.AddServiceEndpoint(typeof(ICassetteHost),
                util.GetBinding(),
                util.GetServiceName());

            ServiceHost.Open();
        }

        protected override void OnStop()
        {
            if (ServiceHost != null)
            {
                ServiceHost.Close();
                ServiceHost = null;
            }
        }
    }
}
