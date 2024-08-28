using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.Errors;
using Talabat.Repository.Data;

namespace Talabat.APIs.Controllers
{    
    public class BuggyController : BaseApiController
    {
        private readonly StoreDbContext _context;

        public BuggyController(StoreDbContext context)
        {
            _context = context;
        }

        [HttpGet("notfound")]  // GET : /api/Buggy/notfound
        public ActionResult GetNotFoundRequest()
        {
            var product = _context.Products.Find(1000);
            if (product is null) 
                return NotFound(new ApiResponse(404));
                //return NotFound(new {Message="Not Found" , Code = 404});


            return Ok(product);
        }


        [HttpGet("servererror")]  // GET : /api/Buggy/servererror
        public ActionResult GetServerError()
        {
            var product = _context.Products.Find(1000);
            var result = product.ToString(); // will throw Exception [NullReferenceException]

            return Ok(result);
        }


        [HttpGet("badrequest")]  // GET : /api/Buggy/badrequest
        public ActionResult GetBadRequest()
        {            
            return BadRequest(new ApiResponse(400));
        }


        [HttpGet("badrequest/{id}")]  // GET : /api/Buggy/badrequest/5
        public ActionResult GetBadRequest(int? id)  // Validation Error
        {
            return BadRequest();
        }


        [HttpGet("unauthorized")]  // GET : /api/Buggy/unauthorized
        public ActionResult GetUnAuthorizedError()  
        {
            return Unauthorized(new ApiResponse(401));
        }


    }
}
