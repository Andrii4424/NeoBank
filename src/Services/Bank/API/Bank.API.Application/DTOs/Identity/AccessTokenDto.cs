using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.API.Application.DTOs.Identity
{
    public class AccessTokenDto
    {
        public string AccessToken { get; set; }
        public DateTime ExpirationTime { get; set; }

        public AccessTokenDto(string accessToken, DateTime expirationTime)
        {
            AccessToken = accessToken;
            ExpirationTime = expirationTime;
        }
    }
}
