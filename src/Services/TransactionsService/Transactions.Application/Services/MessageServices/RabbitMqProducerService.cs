using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;
using Transactions.Application.ServiceContracts;

namespace Transactions.Application.Services.MessageServices
{
    public class RabbitMqProducerService : IRabbitMqProducerService, IAsyncDisposable
    {
        private readonly IConnection _connection;
        private readonly IChannel _channel;
        private readonly IConfiguration _configuration;
        private readonly ILogger<RabbitMqProducerService> _logger;


        public RabbitMqProducerService(IConfiguration configuration, ILogger<RabbitMqProducerService> logger)
        {
            _configuration = configuration;
            _logger = logger;

            ConnectionFactory factory = new ConnectionFactory()
            {
                UserName = _configuration.GetValue<string>("RabbitMq:Username"),
                Password = _configuration.GetValue<string>("RabbitMq:Password"), 
                HostName = _configuration.GetValue<string>("RabbitMq:Host"),
                Port = _configuration.GetValue<int>("RabbitMq:Port"),
                VirtualHost = _configuration.GetValue<string>("RabbitMq:VirtualHost")
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

            _logger.LogInformation("Publishing message to rabbit mq (for bank.api service) to update balance");
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
