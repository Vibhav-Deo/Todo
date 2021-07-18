using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Security;
using System.Threading.Tasks;
using Todo.Contracts.Exceptions;

namespace Todo.Web.Controllers
{
    public class BaseApiController : ControllerBase
    {
        protected async Task<IActionResult> MakeServiceCall(Func<Task<IActionResult>> callback, string errorMessage = null)
        {
            IActionResult result = null;
            try
            {
                result = await callback();
            }
            catch (ValidationException exception)
            {
                var errorMsg = string.IsNullOrWhiteSpace(exception.Message) || string.IsNullOrEmpty(exception.Message) ? errorMessage : exception.Message;
                var statusCode = exception.StatusCode == 0 ? 400 : exception.StatusCode;

                ModelState.AddModelError("ValidationException", errorMsg);
                ModelState.TryGetValue("ValidationException", out var error);

                result = statusCode == 400 ? BadRequest(error) : NotFound(error);
            }
            catch (TodoApplicationException exception)
            {
                var errorMsg = string.IsNullOrWhiteSpace(exception.Message) || string.IsNullOrEmpty(exception.Message) ? errorMessage : exception.Message;
                var statusCode = exception.StatusCode == 0 ? 500 : exception.StatusCode;

                ModelState.AddModelError("ApplicationException", errorMsg);
                ModelState.TryGetValue("ApplicationException", out var modelStateErrors);

                if (statusCode == 500)
                {
                    result = StatusCode(statusCode, modelStateErrors.Errors);
                }

                if (statusCode == 404)
                {
                    result = NotFound(modelStateErrors.Errors);
                }

                if (statusCode == 409)
                {
                    result = Conflict(modelStateErrors.Errors);
                }
            }
            catch (SecurityException exception)
            {
                var errorMsg = string.IsNullOrWhiteSpace(exception.Message) || string.IsNullOrEmpty(exception.Message) ? errorMessage : exception.Message;

                ModelState.AddModelError("SecurityException", errorMsg);
                ModelState.TryGetValue("SecurityException", out var modelStateErrors);

                result = Unauthorized(modelStateErrors.Errors);

            }
            catch (Exception exception)
            {
                var errorMsg = string.IsNullOrWhiteSpace(exception.Message) || string.IsNullOrEmpty(exception.Message) ? errorMessage : exception.Message;

                ModelState.AddModelError("UnknownException", errorMsg);
                ModelState.TryGetValue("UnknownException", out var modelStateErrors);

                result = StatusCode(500, modelStateErrors.Errors);
            }
            return result;
        }

        protected IActionResult Success<T>(T data)
        {
            return Ok(new Response<T>(data));
        }

        protected IActionResult Created<T>(T data)
        {
            return Created("Created", new Response<T>(data));
        }

        protected internal class Response<T>
        {
            public Response(T data)
            {
                Data = data;

            }
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public T Data { get; set; }
        }
    }
}
