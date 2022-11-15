using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Common;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace RabbitApp.Infrastructure
{
    public class FibonachiRequestWebApiProducer : IExternalSystemProducer<FibonachiRequest>,
        IExternalSystemProducer<FibonachiResponse>
    {
        private const string CalculateUrlKey = "WebApp:CalculateUrl";
        private const string SetResultUrlKey = "WebApp:SetResultUrl";

        private readonly string _calculateUrl;
        private readonly HttpClient _client;
        private readonly string _setResultUrl;

        public FibonachiRequestWebApiProducer(IConfiguration config)
        {
            var handler = new SocketsHttpHandler
            {
                PooledConnectionLifetime = TimeSpan.FromMinutes(15) // Recreate every 15 minutes
            };

            _client = new HttpClient(handler);
            _calculateUrl = config[CalculateUrlKey];
            _setResultUrl = config[SetResultUrlKey];
        }

        public async Task Produce(FibonachiRequest dto)
        {
            await _client.PostAsync(_calculateUrl,
                    new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json"))
                .ConfigureAwait(false);
        }

        public async Task Produce(FibonachiResponse dto)
        {
            await _client.PostAsync(_setResultUrl,
                    new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json"))
                .ConfigureAwait(false);
        }
    }
}