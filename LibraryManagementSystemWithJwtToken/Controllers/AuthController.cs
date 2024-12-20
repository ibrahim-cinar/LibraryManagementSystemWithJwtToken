﻿using LibraryManagementSystemWithJwtToken.Models;
using LibraryManagementSystemWithJwtToken.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Diagnostics;

namespace LibraryManagementSystemWithJwtToken.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }


        public IActionResult Index()
        {
            return View();
        }



        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Login model)
        {
            var token = await _authService.LoginUserAsync(model);
            if (token != null)
            {
                return Ok(new { Token = token });
            }
            return Unauthorized();
        }
 

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] Register model)
        {
            var result = await _authService.RegisterUserAsync(model);
            if (result.Succeeded)
            {
                return Ok(new { message = "User registered successfully" });
            }
            return BadRequest(result.Errors);
        }



        [HttpPost("add-role")]
        public async Task<IActionResult> AddRole([FromBody] string role)
        {
            try
            {
                var result = await _authService.AddRoleAsync(role);
                if (result.Succeeded)
                {
                    return Ok(new { message = "Role added successfully" });
                }
                return BadRequest(result.Errors);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("assign-role")]
        public async Task<IActionResult> AssignRole([FromBody] UserRole model)
        {
            try
            {
                var result = await _authService.AssignRoleAsync(model);
                if (result.Succeeded)
                {
                    return Ok(new { message = "Role assigned successfully" });
                }
                return BadRequest(result.Errors);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
