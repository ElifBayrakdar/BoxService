using System;
using System.Threading.Tasks;
using MassTransit;
using BoxApi;
using BoxService.Services;
using Microsoft.Extensions.Logging;

namespace BoxService.Consumers
{
    public class BoxCreatedEventHandler : IConsumer<BoxCreated>
    {
        private readonly IBoxPrinterService _boxPrinterService;
        private readonly ILogger<BoxCreatedEventHandler> _logger;

        public BoxCreatedEventHandler(IBoxPrinterService boxPrinterService, ILogger<BoxCreatedEventHandler> logger)
        {
            _boxPrinterService = boxPrinterService;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<BoxCreated> context)
        {
            var message = context.Message;
            try
            {
                await _boxPrinterService.PrintBox(message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }
    }
}