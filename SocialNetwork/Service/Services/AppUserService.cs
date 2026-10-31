using Domain.Entities;
using Domain.Enums;
using Service.Helpers.DTOs.AppUser;
using Service.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace Service.Services
{
    public class AppUserService : IAppUserService
    {
        public Task<int> CountAsync()
        {
            throw new NotImplementedException();
        }

        public Task DeleteAccountAsync(int userId)
        {
            throw new NotImplementedException();
        }

        public Task DeleteMemberAsync(int actorId, string userPublicId)
        {
            throw new NotImplementedException();
        }

        public Task<UserDto> GetAsync(int userId)
        {
            throw new NotImplementedException();
        }

        public Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            throw new NotImplementedException();

        }



        public Task<List<UserDto>> SearchAsync(string? query)
        {
            throw new NotImplementedException();
        }

        public Task<UserDto> SetRoleAsync(int actorId, string userPublicId, string role)
        {
            throw new NotImplementedException();
        }

        public Task<UserDto> ToggleStatusAsync(int actorId, string userPublicId)
        {
            throw new NotImplementedException();
        }

        public Task<UserDto> UpdateProfileAsync(int userId, UpdateProfileRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
