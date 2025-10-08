using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transactions.Domain.Abstractions
{
    public interface IHasId
    {
        public Guid Id { get; }
    }
}
