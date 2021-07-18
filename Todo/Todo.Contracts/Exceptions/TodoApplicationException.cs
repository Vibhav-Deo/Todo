using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Todo.Contracts.Exceptions
{
    public class TodoApplicationException : Exception
    {
        public TodoApplicationException(string message, Exception innerException) : base(message, innerException)
        {

        }
        public TodoApplicationException(string message, int statusCode, Exception innerException) : base(message, innerException)
        {
            StatusCode = statusCode;
            ValidationErrors = string.Join(",", innerException);
        }
        public int StatusCode { get; }
        public string ValidationErrors { get; set; }
    }
}
