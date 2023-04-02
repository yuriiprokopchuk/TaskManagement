using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace TaskManagement.Bus.Services
{
    public class RabbitConnection : IRabbitConnection
    {
        private IConnection _connection;

        private readonly IConfiguration _config;

        public RabbitConnection(IConfiguration config)
        {
            _config = config;
        }

        public IConnection Connection => GetConnection();

        private static readonly object locker = new object();

        private IConnection GetConnection()
        {
            lock (locker)
            {
                if (_connection == null || !_connection.IsOpen)
                {
                    var factory = new ConnectionFactory()
                    {
                        HostName = _config["RabbitMqOptions:HostName"],
                        UserName = _config["RabbitMqOptions:UserName"],
                        Password = _config["RabbitMqOptions:Password"],
                    };

                    _connection = factory.CreateConnection();
                }
            }

            return _connection;
        }

        public void Dispose()
        {
            _connection?.Dispose();
        }
    }
}
