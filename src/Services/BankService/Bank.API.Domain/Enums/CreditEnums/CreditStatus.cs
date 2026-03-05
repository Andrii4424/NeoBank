using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.API.Domain.Enums.CreditEnums
{
    public enum CreditStatus
    {
        Pending,   
        Active,
        Rejected,
        Closed,
        Overdue
    }
}
