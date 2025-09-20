using AutoMapper;
using Bank.API.Application.DTO;
using Bank.API.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.API.Application.Services
{
    public static class SharedMethods
    {
        public static Guid GetBankGuid()
        {
            return Guid.Parse("E2A4A522-8486-46F7-9437-5F5B7E539502");
        }

        public static int GetDefaultPageCount() {
            return 10;
        } 
    }
}
