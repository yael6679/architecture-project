using SaleApi.Models;
using SaleApi.Repositories;
using static SaleApi.Dto.DonerDto;
using static SaleApi.Dto.GiftDto;

namespace SaleApi.Services
{
    public class GiftService : IGiftService
    {
        private readonly IGiftRepository _giftRepository;
        private readonly ILogger<GiftService> _logger;

        public GiftService(IGiftRepository giftRepository, ILogger<GiftService> logger)
        {
            _giftRepository = giftRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<GetGiftDto>> GetAllGift()
        {
            var gifts = await _giftRepository.GetAllGift();

            return gifts.Select(g => new GetGiftDto
            {
                Id = g.Id,
                Name = g.Name,
                Description = g.Description,
                Img = g.Img,
                Price = g.Price,
                IdDoner = g.IdDoner,
                CategoryId = g.CategoryId,
                Doner = new UpdateDonerDto
                {
                    Id = g.Doner.Id,
                    FirstName = g.Doner.FirstName,
                    LastName = g.Doner.LastName,
                    EMail = g.Doner.Email
                }
            }).ToList();
        }

        public async Task<GiftResponseDto> NewGift(CreateGiftDto giftDto)
        {
            if (giftDto.Price < 10)
                throw new ArgumentException($"Price {giftDto.Price} is smaller");

            var gift = new Gift
            {
                Name = giftDto.Name,
                Description = giftDto.Description,
                Price = giftDto.Price,
                IdDoner = giftDto.IdDoner,
                CategoryId = giftDto.CategoryId,
            };

            if (giftDto.Image != null)
            {
                var fileName = Guid.NewGuid() +
                               Path.GetExtension(giftDto.Image.FileName);

                var folderPath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot/images/gifts"
                );

                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                var filePath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await giftDto.Image.CopyToAsync(stream);
                }

                gift.Img = $"images/gifts/{fileName}";
            }

            var created = await _giftRepository.NewGift(gift);
            _logger.LogInformation("Gift created with ID: {GiftId}", created.Id);

            return MapToResponseGiftDto(created);
        }

        public async Task DeletGift(int id)
        {
            await _giftRepository.DeleteGift(id);
        }

        public async Task<GetGiftDto> GetGiftById(int id)
        {
            var g = await _giftRepository.GetGiftById(id);
            if (g == null) return null;

            return new GetGiftDto
            {
                Id = g.Id,
                Name = g.Name,
                Description = g.Description,
                Img = g.Img,
                Price = g.Price,
                IdDoner = g.IdDoner,
                CategoryId = g.CategoryId,
                Doner = new UpdateDonerDto
                {
                    Id = g.Doner.Id,
                    FirstName = g.Doner.FirstName,
                    LastName = g.Doner.LastName,
                    EMail = g.Doner.Email
                }
            };
        }

        public async Task<GiftResponseDto> UpdateGift(UpdateGiftDto giftDto)
        {
            var existing = await _giftRepository.GetGiftById(giftDto.Id);
            if (existing == null)
                throw new KeyNotFoundException($"Gift with id {giftDto.Id} not found");

            if (giftDto.Price < 10)
                throw new ArgumentException($"Price {giftDto.Price} is smaller");

            existing.Price = giftDto.Price;
            existing.Name = giftDto.Name ?? existing.Name;
            existing.Description = giftDto.Description ?? existing.Description;
            existing.CategoryId = giftDto.CategoryId ?? existing.CategoryId;

            if (giftDto.Image != null)
            {
                if (!string.IsNullOrEmpty(existing.Img))
                {
                    var oldPath = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot",
                        existing.Img.TrimStart('/')
                    );

                    if (File.Exists(oldPath))
                        File.Delete(oldPath);
                }

                var extension = Path.GetExtension(giftDto.Image.FileName);
                var fileName = $"{Guid.NewGuid()}{extension}";
                var folderPath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "images",
                    "gifts"
                );

                Directory.CreateDirectory(folderPath);

                var filePath = Path.Combine(folderPath, fileName);
                using var stream = new FileStream(filePath, FileMode.Create);
                await giftDto.Image.CopyToAsync(stream);

                existing.Img = $"images/gifts/{fileName}";
            }

            var updatedGift = await _giftRepository.UpdateGift(existing);
            _logger.LogInformation("Gift updated with ID: {GiftId}", updatedGift.Id);

            return MapToResponseGiftDto(updatedGift);
        }

        public async Task<GiftDonerDto> GetGiftDoner(int id)
        {
            var donerEntity = await _giftRepository.GetGiftDoner(id);
            if (donerEntity == null)
                return null;

            return new GiftDonerDto
            {
                Id = donerEntity.Id,
                FirstName = donerEntity.FirstName,
                LastName = donerEntity.LastName,
                EMail = donerEntity.Email
            };
        }

        public async Task<IEnumerable<GetGiftDto>> GetGiftByDoner(string name)
        {
            var g = await _giftRepository.GetGiftByDoner(name);

            return g.Select(g => new GetGiftDto
            {
                Id = g.Id,
                Name = g.Name,
                Description = g.Description,
                Img = g.Img,
                Price = g.Price,
                IdDoner = g.IdDoner,
                Doner = new UpdateDonerDto
                {
                    Id = g.Doner.Id,
                    FirstName = g.Doner.FirstName,
                    LastName = g.Doner.LastName,
                    EMail = g.Doner.Email
                }
            }).ToList();
        }

        public async Task<IEnumerable<GiftResponseDto>> GetGiftByName(string name)
        {
            var g = await _giftRepository.GetGiftByName(name);

            return g.Select(g => new GiftResponseDto
            {
                Id = g.Id,
                Name = g.Name,
                Description = g.Description,
                Img = g.Img,
                Price = g.Price,
                CategoryId = g.CategoryId,
                IdDoner = g.IdDoner,
            }).ToList();
        }

        private static GiftResponseDto MapToResponseGiftDto(Gift gift)
        {
            return new GiftResponseDto
            {
                Id = gift.Id,
                Name = gift.Name,
                Description = gift.Description,
                Img = gift.Img,
                Price = gift.Price,
                CategoryId = gift.CategoryId,
                IdDoner = gift.IdDoner,
            };
        }
    }
}
