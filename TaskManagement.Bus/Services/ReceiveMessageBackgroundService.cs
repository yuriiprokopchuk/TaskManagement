using Microsoft.Extensions.Hosting;

namespace TaskManagement.Bus.Infrastructure.Services
{
    public class ReceiveMessageBackgroundService : BackgroundService
    {
        private readonly ServiceBusHandler _serviceBusHandler;

        public ReceiveMessageBackgroundService(ServiceBusHandler serviceBusHandler)
        {
            _serviceBusHandler = serviceBusHandler;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _serviceBusHandler.ReceiveMessage();
        }
    }
}
