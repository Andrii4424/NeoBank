using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transactions.Application.ServiceContracts
{
    public interface IRabbitMqMessageBusService
    {
        public Task PublishAsync<T>(T message, string exchange, string routingKey);
    }
}
