using CustomerApi.SharedKernel;
using Microsoft.AspNetCore.Mvc;

namespace CustomerApi.Web.Controllers;

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

    if (Equals(response.Value, default(TValue)))
    {
      return NotFound();
    }

    if (response.Value != null && response.Value.GetType().IsGenericType &&
        response.Value.GetType().GetGenericTypeDefinition() == typeof(PagedResponse<>))
    {
      var pagedResponse = (dynamic)response.Value;
      if (IsResultAnEmptyList(pagedResponse))
      {
        return NoContent();
      }
    }


    return Ok(response.Value);
  }

  private static bool IsResultAnEmptyList<TValue>(PagedResponse<TValue> response)
  {
    return response.Data.Count == 0;
  }

}
