using Bank.API.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.API.Application.DTOs.Users
{
    public class VacancyDto
    {
        public Guid? Id { get; set; }

        public Guid? BankId { get; private set; }

        public string JobTitle { get; set; }

        public string Category { get; set; }

        public double Salary { get; set; }

        public DateOnly PublicationDate { get; set; }
    }
}
