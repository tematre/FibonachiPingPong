using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Common
{
    public class FibonachiResponseProducePipelineBehavior : IPipelineBehavior<FibonachiRequest, FibonachiResponse>
    {
        private readonly IExternalSystemProducer<FibonachiResponse> _responseProducer;

        public FibonachiResponseProducePipelineBehavior(IExternalSystemProducer<FibonachiResponse> responseProducer)
        {
            _responseProducer = responseProducer;
        }

        public async Task<FibonachiResponse> Handle(FibonachiRequest request,
            RequestHandlerDelegate<FibonachiResponse> next, CancellationToken cancellationToken)
        {
            var response = await next();
            response.CorrelationId = request.CorrelationId;

            Console.WriteLine("Was calculated result: " + response.Value + " for number " + request.Number);

            if (!request.InitialRequest) await _responseProducer.Produce(response);

            return response;
        }
    }
}