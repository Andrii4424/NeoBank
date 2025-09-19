using Bank.API.Application.Helpers.HelperClasses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Bank.API.WebUI.Helpers
{
    public static class ModelStateHelper
    {
        public static ModelStateResult CheckValid(ModelStateDictionary modelState)
        {
            if (modelState.IsValid)
            {
                return ModelStateResult.Ok();
            }
            List<string> errors = new List<string>();

            foreach (var value in modelState.Values)
            {
                foreach (var error in value.Errors)
                {
                    errors.Add(error.ErrorMessage);
                }
            }
            return ModelStateResult.Error(errors);
        }
    }
}
