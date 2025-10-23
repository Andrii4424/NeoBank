using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.API.Application.ServiceContracts.BankServiceContracts.Auth
{
    public interface ISmtpService
    {
        public Task SendAsync(string to, string subject, string html);
    }
}
