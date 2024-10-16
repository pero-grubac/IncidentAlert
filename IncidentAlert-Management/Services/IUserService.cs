﻿using IncidentAlert_Management.Models;
using IncidentAlert_Management.Models.Dto;

namespace IncidentAlert_Management.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAll();
        Task Add(CreateUserDto user);
        Task<string> Login(LoginDto loginUser);
        Task<(OAuthResult result, string token)> OAuth(OAuth oauth);
    }
}
