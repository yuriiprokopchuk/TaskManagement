using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Reflection;
using TaskManagement.Bus.Infrastructure.Extensions;
using TaskManagement.Bus.Services;

namespace TaskManagement.Bus.Infrastructure
{
    public class ServiceBusHandler : IDisposable
    {
        private readonly IRabbitConnection _rabbitConnection;
        private readonly IServiceScope _serviceScope;

        public ServiceBusHandler(IRabbitConnection rabbitConnection, IServiceProvider serviceProvider)
        {
            _rabbitConnection = rabbitConnection;
            _serviceScope = serviceProvider.CreateScope();
        }

        public void SendMessage<TCommand>(TCommand command, IBasicProperties basicProperties = null) where TCommand : class
        {
            var commandType = command.GetType();
            var queueName = commandType.CreateName();
            var message = JsonConvert.SerializeObject(command).ToBytes();

            var channel = _rabbitConnection.Connection.CreateModel();

            channel.BasicPublish(exchange: string.Empty, routingKey: queueName, basicProperties: basicProperties, body: message);
        }

        public void ReceiveMessage()
        {
            Type[] supportedCommands = GetSupportCommands();

            foreach (var supportedCommand in supportedCommands)
            {
                var queueName = supportedCommand.CreateName();

                var channel = _rabbitConnection.Connection.CreateModel();

                channel.QueueDeclare(queueName, exclusive: false, autoDelete: false);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (ch, ea) =>
                {
                    var content = ea.Body.ToArray().ToContent();

                    HandleMessage(channel, supportedCommand, ea);

                    channel.BasicAck(ea.DeliveryTag, false);
                };

                channel.BasicConsume(queueName, false, consumer);
            }
        }

        private void HandleMessage(IModel channel, Type supportedCommand, BasicDeliverEventArgs brokeredMessage)
        {
            var content = brokeredMessage.Body.ToArray().ToContent();

            var command = JsonConvert.DeserializeObject(content, supportedCommand);

            var serviceType = GetServiceType(supportedCommand);

            var obj = _serviceScope.ServiceProvider.GetService(serviceType);

            MethodInfo methodInfo = serviceType.GetMethod("Handle", new[] { supportedCommand });

            try
            {
                methodInfo.Invoke(obj, new object[] { command });
            }
            catch (Exception ex)
            {
                SetRetrylogic(brokeredMessage, channel, command);
            }
        }

        private void SetRetrylogic(BasicDeliverEventArgs brokeredMessage, IModel channel, object command)
        {
            var headers = brokeredMessage.BasicProperties.Headers ?? new Dictionary<string, object>();
            var retryCount = headers.ContainsKey("retry-count") ? (int)headers["retry-count"] : 0;

            if (retryCount < 3)
            {
                // Increase retry count and requeue message with delay
                headers["retry-count"] = retryCount + 1;
                var delayMs = 500 * (retryCount + 1);

                if (headers.ContainsKey("x-delay"))
                    headers["x-delay"] = delayMs;
                else
                    headers.Add("x-delay", delayMs);

                var properties = channel.CreateBasicProperties();
                properties.Headers = headers;

                SendMessage(command, properties);
            }
        }

        private Type[] GetSupportCommands()
        {
            Type interfaceType = typeof(IHandleCommand<>);

            Type[] genericTypeArguments = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => p.IsClass && !p.IsAbstract)
                .SelectMany(t => t.GetInterfaces())
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == interfaceType)
                .Select(i => i.GetGenericArguments()[0])
                .Distinct()
                .ToArray();

            return genericTypeArguments;
        }

        private Type GetServiceType(Type commandType)
        {
            var commandHandlerType = typeof(IHandleCommand<>).MakeGenericType(commandType);
            var type = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Single(p => commandHandlerType.IsAssignableFrom(p) && !p.IsInterface && !p.IsAbstract);

            return type;
        }

        public void Dispose()
        {
            _rabbitConnection?.Dispose();
            _serviceScope?.Dispose();
        }
    }
}
