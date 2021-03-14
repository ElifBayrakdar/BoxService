using System;
using System.Threading.Tasks;
using BoxApi;
using MassTransit;

namespace BoxService.Services
{
    public class BoxPrinterService : IBoxPrinterService
    {
        private readonly IPublishEndpoint _endpoint;

        public BoxPrinterService(IPublishEndpoint endpoint)
        {
            _endpoint = endpoint;
        }

        public async Task PrintBox(BoxCreated boxCreated)
        {
            //Create label template
            await Task.Delay(5000);
            
            //Publish PrintLabelCommand
            int id = new Random().Next(1000);
            PrintLabelCommand msg = new PrintLabelCommand {Id = id, Label = $"Box-{id}"};
            await _endpoint.Publish(msg);
        }
    }
}