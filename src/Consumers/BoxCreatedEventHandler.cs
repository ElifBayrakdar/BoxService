using System;
using System.Threading.Tasks;
using MassTransit;
using BoxApi;
using Microsoft.Extensions.Logging;

namespace BoxService.Consumers
{
    public class BoxCreatedEventHandler : IConsumer<BoxCreated>
    {
        private readonly IPublishEndpoint _endpoint;
        private readonly ILogger<BoxCreatedEventHandler> _logger;

        public BoxCreatedEventHandler(IPublishEndpoint endpoint, ILogger<BoxCreatedEventHandler> logger)
        {
            _endpoint = endpoint;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<BoxCreated> context)
        {
            var message = context.Message;
            
            try
            {
                PrintLabelCommand msg = new PrintLabelCommand(){Id = new Random(10).Next(), Label = $"Box {DateTime.Now}"};
                await _endpoint.Publish(msg);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }
    }
}