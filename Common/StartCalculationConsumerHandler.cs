using MediatR;

namespace Common
{
    public class StartCalculationConsumerHandler : IConsumerHandler<FibonachiRequest>
    {
        private readonly IMediator _mediator;

        public StartCalculationConsumerHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public void Handle(FibonachiRequest model)
        {
            _mediator.Send(model);
        }
    }
}