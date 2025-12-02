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
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Topic { get; set; }
        public string Author { get; set; }
        public string Content { get; set; }
        public DateOnly CreatedAt { get; set; }
        public string ImagePath { get; set; }
    }
}
