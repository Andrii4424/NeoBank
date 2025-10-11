using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.API.Application.ServiceContracts.MessageServices
{
    public interface IRabbitMqProducerService
    {
        public Task PublishAsync<T>(T message, string exchange, string routingKey);
    }
}
