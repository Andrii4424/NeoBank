using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.API.Application.DTOs.Identity
{
    public class AuthenticationResponse
    {
        public string AccessToken { get; set; }
        public DateTime ExpiresOn { get; set; }
    }
}
