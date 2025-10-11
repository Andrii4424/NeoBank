using Bank.API.Application.DTOs.Users.CardOperations;
using Bank.API.Application.ServiceContracts.BankServiceContracts.Users;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Bank.API.Application.Services.MessageServices
{
    public class RabbitMqConsumerService :BackgroundService
    {
        private IConnection _connection;
        private IChannel _channel;
        private readonly IConfiguration _configuration;
        private readonly IServiceScopeFactory _scopeFactory;

        private readonly string _exchangeName = "bank.transaction";
        private readonly string _routingKey = "balance.update";
        private readonly string _queueName = "transaction";

        public RabbitMqConsumerService(IConfiguration configuration, IServiceScopeFactory scopeFactory)
        {
            _configuration = configuration;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            IConnectionFactory factory = new ConnectionFactory()
            {
                UserName = _configuration.GetValue<string>("TransactionRabbitMq:Username"),
                Password = _configuration.GetValue<string>("TransactionRabbitMq:Password"),
                HostName = _configuration.GetValue<string>("TransactionRabbitMq:Host"),
                Port = _configuration.GetValue<int>("TransactionRabbitMq:Port"),
                VirtualHost = _configuration.GetValue<string>("TransactionRabbitMq:VirtualHost")
            };

            _connection = await  factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();


            await _channel.ExchangeDeclareAsync(_exchangeName, ExchangeType.Direct, true);
            await _channel.QueueDeclareAsync(_queueName, true, false, false);
            await _channel.QueueBindAsync(_queueName, _exchangeName, _routingKey);

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (sender, args) =>
            {
                try
                {
                    TransactionDto? operationInfo = JsonSerializer.Deserialize<TransactionDto>(args.Body.Span);

                    await using (var scope = _scopeFactory.CreateAsyncScope()) { 
                        var _userCardService = scope.ServiceProvider.GetService<IUserCardService>();

                        operationInfo = await _userCardService.UpdateBalanceAfterTransactionAsync(operationInfo);

                    }

                    await _channel.BasicAckAsync(args.DeliveryTag, false);
                }
                catch (Exception ex) {
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

