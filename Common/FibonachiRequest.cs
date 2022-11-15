using System;
using MediatR;

namespace Common
{
    public class FibonachiRequest : IRequest<FibonachiResponse>
    {
        public Guid CorrelationId { get; set; }

        public int Number { get; set; }

        public bool InitialRequest { get; set; }
    }
}