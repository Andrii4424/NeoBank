using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.API.Application.DTOs.Users.CardOperations
{
    public class AddFundsDto
    {
        public Guid cardId {  get; set; }
        public decimal amount { get; set; }
    }
}
