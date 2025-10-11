using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transactions.Application.DTOs;
using Transactions.Domain.Entities;

namespace Transactions.Application.Helpers
{
    public class MappingProfile :Profile
    {
        public MappingProfile() {

            CreateMap<TransactionDto, TransactionEntity>();
            CreateMap<TransactionEntity, TransactionDto>();

        }
    }
}
