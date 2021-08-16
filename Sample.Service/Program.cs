using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Components.Consumers;
using Components.CourierActivities;
using Components.StateMachines;
using Components.StateMachines.Activities;
using Core;
using MassTransit;
using MassTransit.RabbitMqTransport;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Warehouse.Contracts;

namespace Service
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
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddJsonFile("appsettings.json", true);
                    config.AddEnvironmentVariables();

                    if (args != null)
                        config.AddCommandLine(args);
                })
                .ConfigureServices((context, services) =>
                {
                    services.AddScoped<AcceptOrderActivity>();
                    services.AddMassTransit(configurator =>
                    {
                        configurator.SetKebabCaseEndpointNameFormatter();
                        configurator.AddConsumersFromNamespaceContaining<SubmitOrderConsumer>();
                        configurator.AddActivitiesFromNamespaceContaining<AllocateInventoryActivity>();
                       
                        configurator
                            .AddSagaStateMachine<OrderStateMachine, OrderState>(typeof(OrderStateMachineDefinition))
                            .MongoDbRepository(r =>
                            {
                                r.Connection = "mongodb://127.0.0.1";
                                r.DatabaseName = "orders";
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
        }
    }
}