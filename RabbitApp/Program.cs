using System;
using System.Threading.Tasks;
using Common;
using EasyNetQ;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitApp.Infrastructure;

namespace RabbitApp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var container = InitContainer();
            var consumer = container.GetRequiredService<IExternalSystemConsumer<FibonachiRequest>>();

            Console.WriteLine("Pease provide N:");
            var nString = Console.ReadLine();
            if (!int.TryParse(nString, out var number))
            {
                Console.WriteLine("Incorrect input!");
                return;
            }

            Console.WriteLine("Please provide parallelism degree: ");
            var parllelismString = Console.ReadLine();

            if (!int.TryParse(parllelismString, out var parallelismDegree))
            {
                Console.WriteLine("Incorrect input!");
                return;
            }

            for (var i = 0; i < parallelismDegree; i++)
                Task.Run(() => { consumer.Consume(new FibonachiRequest {Number = number, InitialRequest = true}); });


            //Console.WriteLine(result.Value);
            Console.ReadLine();
        }

        private static ServiceProvider InitContainer()
        {
            var serviceProvider = new ServiceCollection();

            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true);

            var config = builder.Build();

            var bus = RabbitHutch.CreateBus(config["Rabbit:ConnectionString"]);

            var consumer = new FibonachiRequestRabbitConsumer(config, bus);

            var producer = new FibonachiRequestWebApiProducer(config);

            serviceProvider.AddTransient<StartCalculationConsumerHandler>();
            serviceProvider.AddSingleton<IExternalSystemConsumer<FibonachiRequest>>(consumer);
            serviceProvider.AddSingleton<IExternalSystemConsumer<FibonachiResponse>>(consumer);
            serviceProvider.AddSingleton<IExternalSystemProducer<FibonachiRequest>>(producer);
            serviceProvider.AddSingleton<IExternalSystemProducer<FibonachiResponse>>(producer);

            serviceProvider.AddMediatR(AppDomain.CurrentDomain.GetAssemblies());

            serviceProvider.AddTransient(typeof(IPipelineBehavior<FibonachiRequest, FibonachiResponse>),
                typeof(FibonachiResponseProducePipelineBehavior));

            var result = serviceProvider.BuildServiceProvider();

            var handler = result.GetService<StartCalculationConsumerHandler>();

            consumer.RegisterHandler(handler);

            consumer.Init();

            return result;
        }
    }
}