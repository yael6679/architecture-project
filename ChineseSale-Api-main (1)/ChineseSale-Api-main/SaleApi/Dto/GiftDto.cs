using SaleApi.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using static SaleApi.Dto.DonerDto;

namespace SaleApi.Dto
{
    public class GiftDto
    {
        public class GetGiftDto
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string? Description { get; set; }
            public string? Img { get; set; }
            public int Price { get; set; }
            public int IdDoner { get; set; }
            public int? CategoryId { get; set; }
            public UpdateDonerDto Doner { get; set; }
        }
        public class CreateGiftDto
        {
            [Required]
            [MaxLength(100)]
            public string Name { get; set; }
            [MaxLength(500)]
            public string? Description { get; set; }

            public IFormFile? Image { get; set; }

            [Required]
            [JsonPropertyName("price")]
            public int Price { get; set; } = 10;
            [Required]
            public int IdDoner { get; set; }
            [Required]
            public int CategoryId { get; set; }

        }

        public class UpdateGiftDto
        {
            [Required]
            public int Id { get; set; }
            
            [MaxLength(100)]
            public string? Name { get; set; }
            [MaxLength(500)]
            public string? Description { get; set; }

            [MaxLength(1000)]
            public string? Img { get; set; }
            public IFormFile? Image { get; set; }

            public int Price { get; set; }
             public int IdDoner { get; set; }
             public int? CategoryId { get; set; }
        }

        public class GiftDonerDto
        {
            [Required]
            public int Id { get; set; }

            [Required]
            [MaxLength(100)]
            public string FirstName { get; set; }
            [Required]
            [MaxLength(100)]
            public string LastName { get; set; }
            [Required]
            [EmailAddress]
            [MaxLength(200)]
            public string EMail { get; set; }
        }
        public class GetShortGiftDto
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string? Description { get; set; }

            public string? Img { get; set; }

            public int Price { get; set; }
            public int? CategoryId { get; set; }
        }


        public class GiftResponseDto
        {
            public int Id { get; set; }
            public string? Name { get; set; }
            public string? Description { get; set; }
            public string? Img { get; set; }
            public int Price { get; set; }
            public int IdDoner { get; set; }
            public int? CategoryId { get; set; }
        }

    }
}
