using LibraryManagementSystemWithJwtToken.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LibraryManagementSystemWithJwtToken.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public AccountController(UserManager<IdentityUser> userManager,RoleManager<IdentityRole> roleManager,IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] Register model)
        {
            var user = new IdentityUser { UserName = model.Username};
            var result = await _userManager.CreateAsync(user,model.Password);
            if (result.Succeeded) { return Ok(new { message = "User registered Succesfully" }); }
            return BadRequest(result);
         }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Login model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);
            if (user != null &&  await _userManager.CheckPasswordAsync(user,model.Password)) {
            var userRoles = await _userManager.GetRolesAsync(user);
                var authClaims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
            };
                authClaims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));
                var token = new JwtSecurityToken(issuer: _configuration["Jwt:Issuer"],
                    expires: DateTime.Now.AddMinutes(double.Parse(_configuration["Jwt:ExpiryMinutes"]!)),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!)),
                    SecurityAlgorithms.HmacSha256
                    )
                    );
                return Ok(new {Token = new JwtSecurityTokenHandler().WriteToken(token)});
            }
            return Unauthorized();
        }

        [HttpPost("add-role")]
        public async Task<IActionResult> AddRole([FromBody]string role)
        {
            if(!await _roleManager.RoleExistsAsync(role))
            {
                var result = await _roleManager.CreateAsync(new IdentityRole(role));
                if(result.Succeeded)
                {
                    return Ok(new {message ="Role Added Succesfully"});
                }
                return BadRequest(result.Errors);
            }
           return  BadRequest("Role already exist");
        }
        [HttpPost("assing-role")]
        public async Task<IActionResult> AssingRole([FromBody] UserRole model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);
            if(user == null)
            {
                return BadRequest("User not found");
            }
            var result = await _userManager.AddToRoleAsync(user, model.Role);
            if (result.Succeeded)
            {
                return Ok(new { message = "Role assigned succesfully" });
            }
            return BadRequest(result.Errors);
        }
    }
}
