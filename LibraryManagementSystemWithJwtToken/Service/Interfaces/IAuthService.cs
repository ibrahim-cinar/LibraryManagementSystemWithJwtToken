using LibraryManagementSystemWithJwtToken.Models;
using Microsoft.AspNetCore.Identity;

namespace LibraryManagementSystemWithJwtToken.Services
{
    public interface IAuthService
    {
        Task<IdentityResult> RegisterUserAsync(Register model);
        Task<string?> LoginUserAsync(Login model);
        Task<IdentityResult> AddRoleAsync(string role);
        Task<IdentityResult> AssignRoleAsync(UserRole model);
    }
}

