using System;
using System.Collections.Concurrent;
using Common;
using EasyNetQ;
using Microsoft.Extensions.Configuration;

namespace RabbitApp.Infrastructure
{
    public class FibonachiRequestRabbitConsumer : IExternalSystemConsumer<FibonachiRequest>,
        IExternalSystemConsumer<FibonachiResponse>
    {
        private const string ResultSubscription = "Rabbit:ResultSubscription";
        private const string CalculateSubscription = "Rabbit:CalculateSubscription";
        private readonly IBus _bus;

        private readonly string _calculateSubscription;
        private readonly string _resultSubscription;

        private readonly BlockingCollection<IConsumerHandler<FibonachiRequest>> _requestHandlers;
        private readonly BlockingCollection<IConsumerHandler<FibonachiResponse>> _responseHandlers;

        public FibonachiRequestRabbitConsumer(IConfiguration config, IBus bus)
        {
            _calculateSubscription = config[CalculateSubscription];
            _resultSubscription = config[ResultSubscription];
            _bus = bus;

            _requestHandlers =
                new BlockingCollection<IConsumerHandler<FibonachiRequest>>();
            _responseHandlers =
                new BlockingCollection<IConsumerHandler<FibonachiResponse>>();
        }

        public void Consume(FibonachiRequest param)
        {
            foreach (var handler in _requestHandlers) handler.Handle(param);
        }

        public void RegisterHandler(IConsumerHandler<FibonachiRequest> handler)
        {
            _requestHandlers.Add(handler);
        }

        public void Consume(FibonachiResponse param)
        {
            foreach (var handler in _responseHandlers) handler.Handle(param);
        }

        public void RegisterHandler(IConsumerHandler<FibonachiResponse> handler)
        {
            _responseHandlers.Add(handler);
        }

        public void Init()
        {
            _bus.PubSub.SubscribeAsync<FibonachiRequest>(
                Guid.NewGuid().ToString(),
                Consume,
                config => config.WithTopic(_calculateSubscription));

            _bus.PubSub.SubscribeAsync<FibonachiResponse>(
                Guid.NewGuid().ToString(),
                Consume,
                config => config.WithTopic(_resultSubscription));
        }
    }
}