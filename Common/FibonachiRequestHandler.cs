using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Common
{
    public class FibonachiRequestHandler : IRequestHandler<FibonachiRequest, FibonachiResponse>
    {
        private IMediator _mediator;
        private readonly IExternalSystemProducer<FibonachiRequest> _requestProducer;

        private readonly IExternalSystemConsumer<FibonachiResponse> _responseConsumer;

        public FibonachiRequestHandler(IExternalSystemConsumer<FibonachiResponse> responseConsumer,
            IExternalSystemProducer<FibonachiRequest> requestProducer)
        {
            _responseConsumer = responseConsumer;
            _requestProducer = requestProducer;
        }

        public async Task<FibonachiResponse> Handle(FibonachiRequest request, CancellationToken cancellationToken)
        {
            if (request.Number == 0) return new FibonachiResponse {Value = 1};

            if (request.Number == 1) return new FibonachiResponse {Value = 1};

            var left = await CallExternall(request.Number - 1);
            var right = await CallExternall(request.Number - 2);

            return new FibonachiResponse
            {
                Value = left.Value + right.Value,
                CorrelationId = request.CorrelationId
            };
        }

        private Task<FibonachiResponse> CallExternall(int number)
        {
            var request = new FibonachiRequest
                {Number = number, CorrelationId = Guid.NewGuid(), InitialRequest = false};

            var taskComplitionSource = new TaskCompletionSource<FibonachiResponse>();

            _responseConsumer.RegisterHandler(
                new FibonachiResponseConsumerHandler(request.CorrelationId, taskComplitionSource));

            _requestProducer.Produce(request);

            return taskComplitionSource.Task;
        }
    }
}