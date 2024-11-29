using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystemWithJwtToken.Controllers
{
    [Authorize(Roles ="Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : Controller
    {

        [HttpGet]
        public IActionResult Get()
        {
            return Ok("You have accessed the Admin controller");
        }
    }
}
