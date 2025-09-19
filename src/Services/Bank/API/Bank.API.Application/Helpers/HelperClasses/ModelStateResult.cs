using Microsoft.AspNetCore.Http.HttpResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.API.Application.Helpers.HelperClasses
{
    public class ModelStateResult
    {
        public bool Success { get; set; }

        public List<string>? Errors { get; set; }


        public static ModelStateResult Ok()
        {
            return new ModelStateResult { Success = true, Errors= null };
        }

        public static ModelStateResult Error(List<string> errors)
        {
            return new ModelStateResult { Success = true, Errors = errors };
        }
    }
}
