using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.API.Domain.Entities.Users
{
    public class Vacancies
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Salary { get; set; }
    }
}
