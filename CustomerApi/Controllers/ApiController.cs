using CustomerApi.SharedKernel;
using Microsoft.AspNetCore.Mvc;

namespace CustomerApi.Web.Controllers
{
    public class ApiController : ControllerBase
    {
        protected ActionResult CustomResponse<TValue, TError>(Result<TValue, TError> response)
        {
            if (response == null)
            {
                return Ok();
            }

            if (!response.IsSuccess)
            {
                return UnprocessableEntity(response.Errors);
            }

            if (object.Equals(response, default(TValue)))
            {
                return Ok();
            }

            if (IsResultAnEmptyList(response))
            {
                return NoContent();
            }

            return Ok(response.Value);
        }

        private static bool IsResultAnEmptyList<TValue, TError>(Result<TValue, TError> response)
        {
            IEnumerable<TValue>? list = response.Value as IEnumerable<TValue>;
            return list != null && !list.Cast<object>().Any();
        }
    }
}
