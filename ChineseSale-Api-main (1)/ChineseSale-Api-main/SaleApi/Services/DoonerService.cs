using SaleApi.Models;
using SaleApi.Repositories;
using static SaleApi.Dto.DonerDto;
using static SaleApi.Dto.GiftDto;

namespace SaleApi.Services
{
    public class DonerService : IDonerService
    {
        private readonly IDonerRepository _donerRepository;
        private readonly ILogger<DonerService> _logger;

        public DonerService(IDonerRepository donerRepository, ILogger<DonerService> logger)
        {
            _donerRepository = donerRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<UpdateDonerDto>> GetAllDoner()
        {
            var doners = await _donerRepository.GetAllDoner();

            return doners.Select(d => new UpdateDonerDto
            {
                Id = d.Id,
                FirstName = d.FirstName,
                LastName = d.LastName,
                EMail = d.Email
            }).ToList();
        }

        public async Task<UpdateDonerDto> NewDoner(CreateDonerDto donerDto)
        {
            if (await _donerRepository.EmailExistsAsync(donerDto.EMail))
            {
                throw new ArgumentException($"Email {donerDto.EMail} is already registered.");
            }

            var doner = new Doner
            {
                FirstName = donerDto.FirstName,
                LastName = donerDto.LastName,
                Email = donerDto.EMail
            };

            var created = await _donerRepository.NewDoner(doner);
            _logger.LogInformation("Donor created with ID: {DonorId}", created.Id);

            return new UpdateDonerDto
            {
                Id = created.Id,
                FirstName = created.FirstName,
                LastName = created.LastName,
                EMail = created.Email
            };
        }

        public async Task DeleteDoner(int id)
        {
            await _donerRepository.DeleteDoner(id);
        }

        public async Task<UpdateDonerDto> GetDonerById(int id)
        {
            var d = await _donerRepository.GetDonerById(id);
            if (d == null) return null;

            return new UpdateDonerDto
            {
                Id = d.Id,
                FirstName = d.FirstName,
                LastName = d.LastName,
                EMail = d.Email
            };
        }

        public async Task<UpdateDonerDto> UpdateDoner(UpdateDonerDto donerDto)
        {
            var existing = await _donerRepository.GetDonerById(donerDto.Id);
            if (existing == null) return null;

            if (donerDto.EMail != null && donerDto.EMail != existing.Email)
            {
                if (await _donerRepository.EmailExistsAsync(donerDto.EMail))
                {
                    throw new ArgumentException($"Email {donerDto.EMail} is already registered.");
                }
                existing.Email = donerDto.EMail;
            }

            existing.FirstName = donerDto.FirstName ?? existing.FirstName;
            existing.LastName = donerDto.LastName ?? existing.LastName;
            existing.Email = donerDto.EMail ?? existing.Email;

            var updatedDoner = await _donerRepository.UpdateDoner(existing);
            if (updatedDoner == null) return null;

            _logger.LogInformation("Donor update with ID: {DonorId}", updatedDoner.Id);

            return new UpdateDonerDto
            {
                Id = updatedDoner.Id,
                FirstName = updatedDoner.FirstName,
                LastName = updatedDoner.LastName,
                EMail = updatedDoner.Email
            };
        }

        public async Task<IEnumerable<GetDonerDtoWithGift>> GetAllDonerWithGift()
        {
            var doners = await _donerRepository.GetAllDonerWithGift();

            return doners.Select(d => new GetDonerDtoWithGift
            {
                Id = d.Id,
                FirstName = d.FirstName,
                LastName = d.LastName,
                EMail = d.Email,
                Gifts = d.Gifts.Select(g => new GetShortGiftDto
                {
                    Id = g.Id,
                    Name = g.Name,
                    Description = g.Description,
                    Img = g.Img,
                    Price = g.Price
                }).ToList()
            }).ToList();
        }

        private static GetDonerDtoWithGift MapDonorWithGifts(Doner d) => new()
        {
            Id = d.Id,
            FirstName = d.FirstName,
            LastName = d.LastName,
            EMail = d.Email,
            Gifts = d.Gifts.Select(g => new GetShortGiftDto
            {
                Id = g.Id,
                Name = g.Name,
                Description = g.Description,
                Img = g.Img,
                Price = g.Price
            }).ToList()
        };

        public async Task<IEnumerable<GetDonerDtoWithGift>> GetDonerByName(string name)
        {
            var d = await _donerRepository.GetDonerByName(name);
            return d.Where(donor => donor != null).Select(donor => MapDonorWithGifts(donor!)).ToList();
        }

        public async Task<IEnumerable<GetDonerDtoWithGift>> GetDonerByMail(string email)
        {
            var d = await _donerRepository.GetDonerByMail(email);
            return d.Where(donor => donor != null).Select(donor => MapDonorWithGifts(donor!)).ToList();
        }

        public async Task<IEnumerable<GetDonerDtoWithGift>> GetDonerByGift(string giftName)
        {
            var d = await _donerRepository.GetDonerByGift(giftName);
            var term = giftName.Trim();

            return d.Where(donor => donor != null).Select(donor => new GetDonerDtoWithGift
            {
                Id = donor!.Id,
                FirstName = donor.FirstName,
                LastName = donor.LastName,
                EMail = donor.Email,
                Gifts = donor.Gifts
                    .Where(g => g.Name.Contains(term))
                    .Select(g => new GetShortGiftDto
                    {
                        Id = g.Id,
                        Name = g.Name,
                        Description = g.Description,
                        Img = g.Img,
                        Price = g.Price
                    }).ToList()
            }).ToList();
        }

        public async Task<GetDonerDtoWithGift> GetDonerByIdWithGift(int id)
        {
            var d = await _donerRepository.GetDonerByIdWithGift(id);
            if (d == null) return null;

            return new GetDonerDtoWithGift
            {
                Id = d.Id,
                FirstName = d.FirstName,
                LastName = d.LastName,
                EMail = d.Email,
                Gifts = d.Gifts.Select(g => new GetShortGiftDto
                {
                    Id = g.Id,
                    Name = g.Name,
                    Description = g.Description,
                    Img = g.Img,
                    Price = g.Price
                }).ToList()
            };
        }
    }
}
