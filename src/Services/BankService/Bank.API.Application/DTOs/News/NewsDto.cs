using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.API.Application.DTOs.News
{
    public class NewsDto
    {
        public Guid? Id { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "{0} has to be provided")]
        [Display(Name = "Tariffs name")]
        public string Title { get; set; }

        [Required(ErrorMessage = "{0} has to be provided")]
        [Display(Name = "Tariffs name")]
        public string Topic { get; set; }

        [Required(ErrorMessage = "{0} has to be provided")]
        [Display(Name = "Tariffs name")]
        public string Author { get; set; }

        [Required(ErrorMessage = "{0} has to be provided")]
        [Display(Name = "Tariffs name")]
        public string Content { get; set; }

        public DateOnly? CreatedAt { get; set; } 

    }
}
