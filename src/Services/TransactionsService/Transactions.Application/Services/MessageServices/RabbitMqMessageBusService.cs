using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;
using Transactions.Application.ServiceContracts;

namespace Transactions.Application.Services.MessageServices
{
    public class RabbitMqMessageBusService : IRabbitMqMessageBusService, IAsyncDisposable
    {
        private readonly IConnection _connection;
        private readonly IChannel _channel;
        private readonly IConfiguration _configuration;

        public RabbitMqMessageBusService(IConfiguration configuration)
        {
            ConnectionFactory factory = new ConnectionFactory()
            {
                UserName = configuration.GetValue<string>("RabbitMq:Username"),
                Password = configuration.GetValue<string>("RabbitMq:Password"), 
                HostName = configuration.GetValue<string>("RabbitMq:Host"),
                Port = configuration.GetValue<int>("RabbitMq:Port"),
                VirtualHost = configuration.GetValue<string>("RabbitMq:VirtualHost")
            };

            _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
            _channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();
        }

        public async Task PublishAsync<T>(T message, string exchange, string routingKey)
        {
            await _channel.ExchangeDeclareAsync(exchange, ExchangeType.Direct, durable: true);

            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);

            var props = new BasicProperties
            {
                ContentType = "application/json",
                DeliveryMode = DeliveryModes.Persistent 
            };

            await _channel.BasicPublishAsync(
                exchange: exchange,
                routingKey: routingKey,
                mandatory: false,
                basicProperties: props,
                body: body
            );
        }

        public async ValueTask DisposeAsync()
        {
            if (_channel != null)
            {
                await _channel.CloseAsync();
                await _channel.DisposeAsync();
            }

            if (_connection != null)
            {
                await _connection.CloseAsync();
                await _connection.DisposeAsync();
            }
        }
    }
}
