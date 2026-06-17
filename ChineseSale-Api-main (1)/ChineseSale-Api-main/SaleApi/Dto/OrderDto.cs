using SaleApi.Models;
using System.ComponentModel.DataAnnotations;

namespace SaleApi.Dto
{
    public class OrderDto
    {
        public class GetOrderDto
        {
            public int Id { get; set; }

            [Required]
            public int IdUser { get; set; }
            public UserShortDto? User { get; set; }
            public OrderShortDto? Gift { get; set; }
            public bool Win { get; set; } = false;
        }

        public class OrderShortDto
        {
            public int Id { get; set; }
            public string Name { get; set; } = null!;
            public int Price { get; set; }
        }

        public class AddOrderDto
        {
            [Required]
            public int IdUser { get; set; }

            [Required]
            public int IdGift { get; set; }
        }

        public class UserShortDto
        {
            [Required]
            public string FirstName { get; set; } = null!;

            [Required]
            public string LastName { get; set; } = null!;

            [Required, EmailAddress]
            public string Email { get; set; } = null!;

            public string? PhoneNumber { get; set; }
        }

        public class GetPurchaserByOrderIdDto
        {
            public int OrderId { get; set; }
            public string FirstName { get; set; } = null!;
            public string LastName { get; set; } = null!;
            public string Email { get; set; } = null!;
            public string GiftName { get; set; } = null!;
        }
    }
}
