using Microsoft.AspNetCore.Mvc;

namespace FPTJobMatch.Controllers
{
    public class ErrorController : Controller
    {
        [Route("error/404")]
        public IActionResult PageNotFound()
        {
            Response.Clear();
            Response.StatusCode = StatusCodes.Status404NotFound;
            return View();
        }

        [Route("error/{code}")]
        public IActionResult GenericError(int code, string? errorMessage)
        {
            Response.Clear(); // Clear all content of response
            Response.StatusCode = code;
            ViewBag.ErrorCode = code; 
            ViewBag.ErrorMessage = errorMessage;
            return View();
        }
        [Route("error/accessdenied")]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
