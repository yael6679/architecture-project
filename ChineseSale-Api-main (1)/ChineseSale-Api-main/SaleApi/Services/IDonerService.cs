using SaleApi.Dto;
using SaleApi.Models;
using static SaleApi.Dto.DonerDto;

namespace SaleApi.Services
{
    public interface IDonerService
    {
        Task<IEnumerable<UpdateDonerDto>> GetAllDoner();
        Task DeleteDoner(int id);
        Task<UpdateDonerDto> GetDonerById(int id);
        Task<UpdateDonerDto> UpdateDoner(UpdateDonerDto donerDto);
        Task<UpdateDonerDto> NewDoner(CreateDonerDto donerDto);
        Task<IEnumerable<GetDonerDtoWithGift>> GetAllDonerWithGift();
        Task<GetDonerDtoWithGift> GetDonerByIdWithGift(int id);
        Task<IEnumerable<GetDonerDtoWithGift>> GetDonerByName(string name);
        Task<IEnumerable<GetDonerDtoWithGift>> GetDonerByMail(string email);
        Task<IEnumerable<GetDonerDtoWithGift>> GetDonerByGift(string giftName);
    }
}