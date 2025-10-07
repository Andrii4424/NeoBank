using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.API.Application.Helpers.HelperClasses
{
    public class OperationResult
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }

        public OperationResult (bool success)
        {
            Success = success;
        }

        public OperationResult(bool success, string errorMessage)
        {
            Success = success;
            ErrorMessage = errorMessage;
        }

        public static OperationResult Ok()
        {
            return new OperationResult(true);
        }

        public static OperationResult Error(string errorMessage) {
            return new OperationResult (false, errorMessage);
        }

    }
}
