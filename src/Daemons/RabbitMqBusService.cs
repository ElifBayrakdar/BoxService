using MassTransit;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace BoxService.Daemons
{
    public class RabbitMqBusService : IHostedService
    {
        private readonly IBusControl _busControl;

        public RabbitMqBusService(IBusControl busControl, ILogger<RabbitMqBusService> logger)
        {
            logger.LogInformation("Application is Started!");
            _busControl = busControl;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _busControl.StartAsync(cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _busControl.StopAsync(cancellationToken);
        }
    }
}
