using SaleApi.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using static SaleApi.Dto.GiftDto;

namespace SaleApi.Dto
{
    public class BagDto
    {
        public class GetBagDto
        {
            [Key]
            public int Id { get; set; }

            [ForeignKey("User")]
            public int IdUser { get; set; }

            [ForeignKey("Gift")]
            public int IdGift { get; set; }
            public GiftResponseDto Gift { get; set; } = null!;
        }

        public class CreateBagDto
        {
            [JsonPropertyName("idUser")]
            public int IdUser { get; set; }

            [JsonPropertyName("idGift")]
            public int IdGift { get; set; }

            [JsonPropertyName("quantity")]
            public int Quantity { get; set; } = 1;
        }
    }
}
