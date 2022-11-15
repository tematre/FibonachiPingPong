using System.Collections.Concurrent;
using Common;
using Microsoft.Extensions.Configuration;

namespace WebApp.Infrastructure
{
    public class FibonachiRequestWebApiConsumer : IExternalSystemConsumer<FibonachiRequest>,
        IExternalSystemConsumer<FibonachiResponse>
    {
        private readonly BlockingCollection<IConsumerHandler<FibonachiRequest>> _requestHandlers;
        private readonly BlockingCollection<IConsumerHandler<FibonachiResponse>> _responseHandlers;

        public FibonachiRequestWebApiConsumer(IConfiguration config)
        {
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
    }
}