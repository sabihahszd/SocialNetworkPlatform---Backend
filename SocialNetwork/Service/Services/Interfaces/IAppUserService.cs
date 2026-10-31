using Service.Helpers.DTOs.AppUser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services.Interfaces
{
    public interface IAppUserService
    {
        Task<AuthResponse> RegisterAsync(RegisterRequest request);
        Task<AuthResponse> LoginAsync(LoginRequest request);
        Task<UserDto> GetAsync(int userId);
        Task<List<UserDto>> SearchAsync(string? query);
        Task<int> CountAsync();
        Task<UserDto> UpdateProfileAsync(int userId, UpdateProfileRequest request);
        Task DeleteAccountAsync(int userId);
        Task<UserDto> ToggleStatusAsync(int actorId, string userPublicId);
        Task<UserDto> SetRoleAsync(int actorId, string userPublicId, string role);
        Task DeleteMemberAsync(int actorId, string userPublicId);
    }
}
