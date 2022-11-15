using System;
using System.Net.Http;
using System.Threading.Tasks;
using Common;
using EasyNetQ;
using Microsoft.Extensions.Configuration;

namespace RabbitApp.Infrastructure
{
    public class FibonachiRequestRabbitMqProducer : IExternalSystemProducer<FibonachiRequest>,
        IExternalSystemProducer<FibonachiResponse>
    {
        private const string ResultSubscription = "Rabbit:ResultSubscription";
        private const string CalculateSubscription = "Rabbit:CalculateSubscription";

        private readonly IBus _bus;

        private readonly string _calculateSubscription;
        private readonly string _resultSubscription;


        public FibonachiRequestRabbitMqProducer(IConfiguration config, IBus bus)
        {
            var handler = new SocketsHttpHandler
            {
                PooledConnectionLifetime = TimeSpan.FromMinutes(15) // Recreate every 15 minutes
            };

            _bus = bus;
            _calculateSubscription = config[CalculateSubscription];
            _resultSubscription = config[ResultSubscription];
        }

        public async Task Produce(FibonachiRequest dto)
        {
            await _bus.PubSub.PublishAsync(dto, _calculateSubscription).ConfigureAwait(false);
        }

        public async Task Produce(FibonachiResponse dto)
        {
            await _bus.PubSub.PublishAsync(dto, _resultSubscription).ConfigureAwait(false);
        }
    }
}