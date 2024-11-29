using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystemWithJwtToken.Controllers
{
    [Authorize(Roles = "User")]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {

        [HttpGet]
        public IActionResult Get()
        {
            return Ok("You have accessed the User controller");
        }
    }
}
