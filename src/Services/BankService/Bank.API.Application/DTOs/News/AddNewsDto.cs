using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.API.Application.DTOs.News
{
    public class AddNewsDto
    {
        public Guid? Id { get; set; }

        [Required(ErrorMessage = "{0} has to be provided")]
        [Display(Name = "Title")]
        public string Title { get; set; }

        [Required(ErrorMessage = "{0} has to be provided")]
        [Display(Name = "Topic")]
        public string Topic { get; set; }

        [Required(ErrorMessage = "{0} has to be provided")]
        [Display(Name = "Author")]
        public string Author { get; set; }

        [Required(ErrorMessage = "{0} has to be provided")]
        [Display(Name = "Content")]
        public string Content { get; set; }

        public DateOnly? CreatedAt { get; set; }

        [Required(ErrorMessage = "{0} has to be provided")]
        [Display(Name = "Image")]
        public IFormFile Image { get; set; }
    }
}
