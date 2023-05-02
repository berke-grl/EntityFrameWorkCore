using ServiceContracts.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Helpers
{
    public class ValidationHelpers
    {
        internal static void ModelValidation(object model)
        {
            //Check if PersonName is not null ( Model Validations)
            ValidationContext validationContext = new ValidationContext(model);
            List<ValidationResult> validationResults = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(model, validationContext, validationResults, true);

            if (!isValid) throw new ArgumentException(validationResults.FirstOrDefault()?.ErrorMessage);


        }
    }
}
