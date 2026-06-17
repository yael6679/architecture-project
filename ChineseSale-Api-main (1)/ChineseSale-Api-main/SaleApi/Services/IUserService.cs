using SaleApi.Models;
using static SaleApi.Dto.UserDto;

namespace SaleApi.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserResponseDto>> GetAllUsersAsync();
        Task<UserResponseDto> CreateUserAsync(UserCreateDto createDto);
        Task<UserResponseDto?> GetUserByIdAsync(int id);
        Task<UserResponseDto?> UpdateUserAsync(int id, UserUpdateDto updateDto);
        Task<bool> DeleteUserAsync(int id);
        Task<LoginResponseDto?> AuthenticateAsync(string email, string password);
    }
}
