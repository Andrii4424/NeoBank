using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.API.Application.DTOs.Users.CardOperations
{
    public class ChangePinDto
    {
        public Guid cardId { get; set; }
        public string newPin { get; set; }
    }
}
