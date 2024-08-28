using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.Errors;

namespace Talabat.APIs.Controllers
{
    [Route("error/{code}")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ErrorsController : ControllerBase
    {

        // errors/404
        public ActionResult Error(int code)
        {
            return NotFound(new ApiResponse(code,"EndPoint is not found"));
        }
    }
}
