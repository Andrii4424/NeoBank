using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transactions.Domain.Enums;

namespace Transactions.Application.DTOs
{
    public class TransactionDto
    {
        public Guid? Id { get; set; }

        public Guid? SenderCardId { get; set; }

        public Guid? SenderId { get; set; }

        public Guid? GetterCardId { get; set; }

        public Guid? GetterId { get; set; }

        [Required(ErrorMessage ="{0} has to be provided")]
        [Display(Name = "Amount")]
        public decimal Amount { get; set; }

        public decimal? Commission { get; set; }

        public TransactionStatus? Status { get; set; }

        [Required(ErrorMessage = "{0} has to be provided")]
        [Display(Name = "Operation type")]
        public TransactionType Type { get; set; }

        public DateTime? TransactionTime { get; set; }
    }
}
