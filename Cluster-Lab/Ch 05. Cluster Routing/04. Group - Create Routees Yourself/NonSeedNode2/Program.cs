﻿using Akka.Actor;
using Akka.Configuration;
using System;
using System.IO;

namespace NonSeedNode2
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = ConfigurationFactory.ParseString(File.ReadAllText("App.Akka.conf"));
            
            //
            // "{app-name} - akka.tcp://{actorysystem-name}@{hostname}:{port}"
            //
            Console.Title = $"{config.GetString("akka.system.app-name")}" +
                $" - akka.tcp://{config.GetString("akka.system.actorsystem-name")}" +
                $"@{config.GetString("akka.remote.dot-netty.tcp.hostname")}" +
                $":{config.GetString("akka.remote.dot-netty.tcp.port")}";

            ActorSystem system = ActorSystem.Create("ClusterLab", config);
            system.ActorOf(FooActor.Props(), nameof(FooActor));

            Console.WriteLine();
            Console.WriteLine("NonSeedNode2 is running...");
            Console.WriteLine();

            Console.ReadLine();
        }
    }
}
