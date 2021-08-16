using System;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
using Core;
using MassTransit;
using MassTransit.RabbitMqTransport;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Warehouse.Components;
using Warehouse.Components.Consumers;
using Warehouse.Components.StateMachines;
using Warehouse.Contracts;

namespace Warehouse.Service
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var isService = !(Debugger.IsAttached || args.Contains("--console"));
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();
            var builder = new HostBuilder()
                .ConfigureAppConfiguration((_, config) =>
                {
                    config.AddJsonFile("appsettings.json", true);
                    config.AddEnvironmentVariables();

                    if (args != null)
                        config.AddCommandLine(args);
                })
                .ConfigureServices((_, services) =>
                {
                    Uri schedulerEndpoint = new Uri("rabbitmq://localhost/quartz");
                    services.AddMassTransit(configurator =>
                    {
                        configurator.SetKebabCaseEndpointNameFormatter();
                        configurator.AddConsumersFromNamespaceContaining<AllocateInventoryConsumer>();
                        configurator.AddMessageScheduler(schedulerEndpoint);
                        configurator
                            .AddSagaStateMachine<AllocationStateMachine, AllocationState>()
                            .MongoDbRepository(r =>
                            {
                                r.Connection = "mongodb://127.0.0.1";
                                r.DatabaseName = "allocations";
                            });
                        configurator.UsingRabbitMq(ConfigureBus);

                        configurator.AddRequestClient<AllocateInventory>();
                    });

                    services.AddHostedService<MassTransitHostedService>();
                })
                .ConfigureLogging((context, loggingBuilder) =>
                {
                    loggingBuilder.AddConfiguration(context.Configuration.GetSection("Logging"));
                    loggingBuilder.AddConsole();
                });
            if (isService)
            {
                await builder.UseWindowsService().Build().StartAsync();
            }
            else
            {
                await builder.RunConsoleAsync();
            }

            Console.WriteLine("RabbitMq connected");
            Log.CloseAndFlush();
        }
        
        public static void ConfigureBus(IBusRegistrationContext context, IRabbitMqBusFactoryConfigurator configurator)
        {
            configurator.ConfigureEndpoints(context);
            configurator.UseMessageScheduler(new Uri("rabbitmq://localhost/quartz"));
        }
    }
}