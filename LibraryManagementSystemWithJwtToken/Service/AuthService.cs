using LibraryManagementSystemWithJwtToken.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LibraryManagementSystemWithJwtToken.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public AuthService(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        public async Task<IdentityResult> RegisterUserAsync(Register model)
        {
            var user = new IdentityUser { UserName = model.Username };
            return await _userManager.CreateAsync(user, model.Password);
        }

        public async Task<string?> LoginUserAsync(Login model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                var authClaims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };
                authClaims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));

                var token = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Issuer"],
                    expires: DateTime.Now.AddMinutes(double.Parse(_configuration["Jwt:ExpiryMinutes"]!)),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(
                        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!)),
                        SecurityAlgorithms.HmacSha256)
                );

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            return null;
        }

        public async Task<IdentityResult> AddRoleAsync(string role)
        {
            if (!await _roleManager.RoleExistsAsync(role))
            {
                return await _roleManager.CreateAsync(new IdentityRole(role));
            }
            throw new InvalidOperationException("Role already exists");
        }

        public async Task<IdentityResult> AssignRoleAsync(UserRole model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user == null)
            {
                throw new InvalidOperationException("User not found");
            }
            return await _userManager.AddToRoleAsync(user, model.Role);
        }
    }
}
