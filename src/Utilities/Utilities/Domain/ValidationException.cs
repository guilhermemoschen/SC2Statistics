using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Practices.EnterpriseLibrary.Validation;

namespace SC2LiquipediaStatistics.Utilities.Domain
{
    public class ValidationException : Exception
    {
        public ValidationResults ValidationResults { get; private set; }

        public ValidationException() : base()
        {
        }

        public ValidationException(ValidationResults validationResults = null) : this(null, null, validationResults)
        {
        }

        public ValidationException(string message, ValidationResults validationResults = null) : this(message, null, validationResults)
        {
        }


        public ValidationException(string message, Exception innerException, ValidationResults validationResults = null) : base(message, innerException)
        {
            ValidationResults = validationResults;
        }

        public string GetFormatedMessage()
        {
            if (ValidationResults == null)
                return Message;

            var messageString = Message ?? "Validation errors include: ";
            int i = 1;
            foreach (var result in ValidationResults)
            {
                messageString += i + " - " + result.Message;
                i++;
            }

            return messageString;
        }
    }
}
