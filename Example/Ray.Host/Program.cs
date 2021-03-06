﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using Ray.Core;
using Ray.EventBus.RabbitMQ;
using Ray.Grain;
using Ray.Storage.MongoDB;
using Ray.Storage.PostgreSQL;

namespace Ray.MongoHost
{
    class Program
    {
        static int Main(string[] args)
        {
            return RunMainAsync().Result;
        }
        private static async Task<int> RunMainAsync()
        {
            try
            {
                using (var host = await StartSilo())
                {
                    while (true)
                    {
                        Console.WriteLine("Input any key to stop");
                        Console.ReadLine();
                        await host.StopAsync();
                        Console.WriteLine("Input any key to Start");
                        Console.ReadLine();
                        await host.StartAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return 1;
            }
        }
        private static async Task<ISiloHost> StartSilo()
        {
            var builder = new SiloHostBuilder()
                .UseLocalhostClustering()
                .UseDashboard()
                .AddRay<Configuration>()
                .Configure<EndpointOptions>(options => options.AdvertisedIPAddress = IPAddress.Loopback)
                .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(Account).Assembly).WithReferences())
                .ConfigureServices((context, servicecollection) =>
                {
                    //注册postgresql为事件存储库
                    servicecollection.AddPostgreSQLStorage<PostgreSQLStorageConfig>(config =>
                    {
                        config.ConnectionDict = new Dictionary<string, string>
                        {
                            { "core_event","Server=127.0.0.1;Port=5432;Database=Ray;User Id=postgres;Password=admin;Pooling=true;MaxPoolSize=20;"}
                        };
                    });
                    //注册mongodb为事件存储库
                    //servicecollection.AddMongoDBStorage<MongoDBStorageConfig>(config => { config.Connection = "mongodb://127.0.0.1:27017"; });
                    servicecollection.AddRabbitMQ(config =>
                    {
                        config.UserName = "admin";
                        config.Password = "admin";
                        config.Hosts = new[] { "127.0.0.1:5672" };
                        config.MaxPoolSize = 100;
                        config.VirtualHost = "/";
                    }, async container =>
                    {
                        await container.CreateEventBus<Account>("Account", "account", 5).DefaultConsumer<long>();
                    });
                })
                 .Configure<GrainCollectionOptions>(options =>
                 {
                     options.CollectionAge = TimeSpan.FromMinutes(5);
                 })
                .ConfigureLogging(logging =>
                {
                    logging.SetMinimumLevel(LogLevel.Information);
                    logging.AddConsole(options => options.IncludeScopes = true);
                }).EnableDirectClient();

            var host = builder.Build();
            await host.StartAsync();
            return host;
        }
    }
}
