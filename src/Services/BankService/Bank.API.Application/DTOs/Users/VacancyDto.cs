using Bank.API.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.API.Application.DTOs.Users
{
    public class VacancyDto
    {
        public Guid? Id { get; set; }

        public Guid? BankId { get; set; }

        [Required(ErrorMessage = "{0} has to be provided")]
        [Display(Name = "Job Title")]
        public string JobTitle { get; set; }

        [Required(ErrorMessage = "{0} has to be provided")]
        [Display(Name = "Category")]
        public string Category { get; set; }


        [Range(1, double.MaxValue, ErrorMessage = "{0} must be more than 0}")]
        [Display(Name = "Category")]
        public double Salary { get; set; }

        public DateOnly? PublicationDate { get; set; }
    }
}
