using System;
using System.Threading.Tasks;

namespace Common
{
    public class FibonachiResponseConsumerHandler : IConsumerHandler<FibonachiResponse>
    {
        private readonly Guid _guid;
        private readonly TaskCompletionSource<FibonachiResponse> _taskCompletionSource;

        public FibonachiResponseConsumerHandler(Guid guid, TaskCompletionSource<FibonachiResponse> taskCompletionSource)
        {
            _taskCompletionSource = taskCompletionSource;
            _guid = guid;
        }

        public void Handle(FibonachiResponse model)
        {
            if (_guid == model.CorrelationId) _taskCompletionSource.SetResult(model);
        }
    }
}