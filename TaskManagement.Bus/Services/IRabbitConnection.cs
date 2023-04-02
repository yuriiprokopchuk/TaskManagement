using RabbitMQ.Client;

namespace TaskManagement.Bus.Services
{
    public interface IRabbitConnection : IDisposable
    {
        IConnection Connection { get; }
    }
}
