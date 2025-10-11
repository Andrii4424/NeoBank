using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text.Json;
using Transactions.Application.DTOs;
using Transactions.Application.ServiceContracts;

namespace Transactions.Application.Services.MessageServices
{
    public class RabbitMqConsumerService : BackgroundService
    {
        private IConnection _connection;
        private IChannel _channel;
        private readonly IConfiguration _configuration;
        private readonly IServiceScopeFactory _scopeFactory;

        private readonly string _exchangeName = "bank.transaction";
        private readonly string _routingKey = "status.update";
        private readonly string _queueName = "transaction.status";

        public RabbitMqConsumerService(IConfiguration configuration, IServiceScopeFactory scopeFactory)
        {
            _configuration = configuration;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            IConnectionFactory factory = new ConnectionFactory()
            {
                UserName = _configuration.GetValue<string>("RabbitMq:Username"),
                Password = _configuration.GetValue<string>("RabbitMq:Password"),
                HostName = _configuration.GetValue<string>("RabbitMq:Host"),
                Port = _configuration.GetValue<int>("RabbitMq:Port"),
                VirtualHost = _configuration.GetValue<string>("RabbitMq:VirtualHost")
            };

            _connection = await factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();


            await _channel.ExchangeDeclareAsync(_exchangeName, ExchangeType.Direct, true);
            await _channel.QueueDeclareAsync(_queueName, true, false, false);
            await _channel.QueueBindAsync(_queueName, _exchangeName, _routingKey);

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (sender, args) =>
            {
                try
                {
                    UpdateBalanceDto? operationInfo = JsonSerializer.Deserialize<UpdateBalanceDto>(args.Body.Span);

                    await using (var scope = _scopeFactory.CreateAsyncScope())
                    {
                        var _transactionService = scope.ServiceProvider.GetRequiredService<ITransactionService>();

                        await _transactionService.UpdateTransactionStatus(operationInfo);

                    }

                    await _channel.BasicAckAsync(args.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    //TO DO: Error Log
                    Console.WriteLine(ex.Message);
                    await _channel.BasicNackAsync(args.DeliveryTag, false, requeue: false);
                }

            };

            await _channel.BasicConsumeAsync(_queueName, autoAck: false, consumer: consumer);

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_channel != null)
            {
                await _channel.CloseAsync();
            }
            if (_connection != null)
            {
                await _connection.CloseAsync();
            }
            await base.StopAsync(cancellationToken);
        }
    }
}
