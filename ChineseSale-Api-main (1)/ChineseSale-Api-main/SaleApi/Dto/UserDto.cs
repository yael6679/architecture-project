using SaleApi.Models;
using System.ComponentModel.DataAnnotations;

namespace SaleApi.Dto
{
    public class UserDto
    {
        public class GetUserDto
        {
            public int Id { get; set; }

            [Required]
            public string FirstName { get; set; } = null!;

            [Required]
            public string LastName { get; set; } = null!;

            [Required, EmailAddress]
            public string Email { get; set; } = null!;

            public string? PhoneNumber { get; set; }
            public UserRole Role { get; set; } = UserRole.User;

        }


        public class UserCreateDto
        {
            [Required]
            [MaxLength(100)]
            public string FirstName { get; set; } = string.Empty;

            [Required]
            [MaxLength(100)]
            public string LastName { get; set; } = string.Empty;

            [Required]
            [EmailAddress]
            [MaxLength(200)]
            public string Email { get; set; } = string.Empty;

            [Required]
            [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
            public string Password { get; set; } 

            [Phone]
            [MaxLength(20)]
            public string? PhoneNumber { get; set; } = string.Empty;

        }
        public class UserResponseDto
        {
            public int Id { get; set; }
            public string FirstName { get; set; } = string.Empty;
            public string LastName { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string PhoneNumber { get; set; } = string.Empty;
            public UserRole Role { get; set; }

        }

        public class UserUpdateDto
        {
            [MaxLength(100)]
            public string? FirstName { get; set; }

            [MaxLength(100)]
            public string? LastName { get; set; }

            [EmailAddress]
            [MaxLength(200)]
            public string? Email { get; set; }

            [Phone]
            [MaxLength(20)]
            public string? PhoneNumber { get; set; }

        }

        public class LoginResponseDto
        {
            public string Token { get; set; } = string.Empty;
            public string TokenType { get; set; } = "Bearer";
            public int ExpiresIn { get; set; }
            public UserResponseDto User { get; set; } = null!;
        }

        public class LoginRequestDto
        {
            [EmailAddress]
            public string Email { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }
    }
}
                