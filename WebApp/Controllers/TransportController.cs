using Common;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TransportController : ControllerBase
    {
        private readonly IExternalSystemConsumer<FibonachiRequest> _requestConsumer;
        private readonly IExternalSystemConsumer<FibonachiResponse> _responseConsumer;

        public TransportController(IExternalSystemConsumer<FibonachiRequest> requestConsumer,
            IExternalSystemConsumer<FibonachiResponse> responseConsumer)
        {
            _requestConsumer = requestConsumer;
            _responseConsumer = responseConsumer;
        }


        [HttpPost]
        [Route("calculate")]
        public void Calculate(FibonachiRequest dto)
        {
            _requestConsumer.Consume(dto);
        }

        [HttpPost]
        [Route("setResult")]
        public void SetResult(FibonachiResponse dto)
        {
            _responseConsumer.Consume(dto);
        }
    }
}