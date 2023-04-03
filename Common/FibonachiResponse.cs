using System;
using MediatR;

namespace Common
{
    public class FibonachiResponse : INotification
    {
        public Guid CorrelationId { get; set; }

        public int Value { get; set; }
    }
}