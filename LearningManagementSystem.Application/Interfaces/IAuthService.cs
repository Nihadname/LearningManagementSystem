﻿using LearningManagementSystem.Application.Dtos.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningManagementSystem.Application.Interfaces
{
    public interface IAuthService
    {
         Task<UserGetDto> RegisterForStudent(RegisterDto registerDto);
        Task<UserGetDto> RegisterForTeacher(RegisterDto registerDto);
        Task<string> Login(LoginDto loginDto);
    }
}