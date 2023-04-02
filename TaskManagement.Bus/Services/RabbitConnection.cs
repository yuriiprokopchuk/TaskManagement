using RabbitMQ.Client;

namespace TaskManagement.Bus.Services
{
    public class RabbitConnection : IRabbitConnection
    {
        private IConnection _connection;

        public IConnection Connection => GetConnection();

        private static readonly object locker = new object();

        private IConnection GetConnection()
        {
            lock (locker)
            {
                if (_connection == null || !_connection.IsOpen)
                {
                    var factory = new ConnectionFactory() { HostName = "localhost" };

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
