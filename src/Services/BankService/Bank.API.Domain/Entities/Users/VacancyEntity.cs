using Bank.API.Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.API.Domain.Entities.Users
{
    public class VacancyEntity :IHasId
    {
        public Guid BankId { get; private set; }

        public BankEntity Bank { get; private set; }

        public Guid Id { get; set; }

        public string JobTitle { get; set; }
        public string Category { get; set; }

        public double Salary { get; set; }

        public DateOnly PublicationDate { get; set; }
    }
}
