using Bank.API.Application.ServiceContracts.MessageServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Bank.API.Application.Services.MessageServices
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

            ConnectionFactory factory = new ConnectionFactory()
            {
                UserName = _configuration.GetValue<string>("TransactionRabbitMq:Username"),
                Password = _configuration.GetValue<string>("TransactionRabbitMq:Password"),
                HostName = _configuration.GetValue<string>("TransactionRabbitMq:Host"),
                Port = _configuration.GetValue<int>("TransactionRabbitMq:Port"),
                VirtualHost = _configuration.GetValue<string>("TransactionRabbitMq:VirtualHost")
            };

            _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
            _channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();

            _logger = logger;
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

            _logger.LogInformation("Sending response to transaction service");
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
