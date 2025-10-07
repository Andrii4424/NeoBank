using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.API.Application.DTOs.Users.CardOperations
{
    public class ChangeCreditLimitDto
    {
        public Guid CardId {  get; set; }
        public decimal NewCreditLimit { get; set; }
    }
}
