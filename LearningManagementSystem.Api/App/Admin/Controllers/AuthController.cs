﻿using LearningManagementSystem.Application.Dtos.Auth;
using LearningManagementSystem.Application.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LearningManagementSystem.Api.App.Admin.Controllers
{
    [Area("Admin")]
    [Route("api/[area]/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AuthController(IAuthService authService, RoleManager<IdentityRole> roleManager)
        {
            _authService = authService;
            _roleManager = roleManager;
        }
        [HttpPost("RegisterForStudent")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]

        public async Task<IActionResult> RegisterForStudent(RegisterDto registerDto)
        {
            return Ok(await _authService.RegisterForStudent(registerDto));
        }
        [HttpPost("RegisterForTeacher")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]

        public async Task<IActionResult> RegisterForTeacher(RegisterDto registerDto)
        {
            return Ok(await _authService.RegisterForTeacher(registerDto));
        }

    }
}
