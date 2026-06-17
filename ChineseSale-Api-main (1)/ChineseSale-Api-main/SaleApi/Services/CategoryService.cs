using SaleApi.Models;
using SaleApi.Repositories;
using static SaleApi.Dto.CategoryDto;
using static SaleApi.Dto.GiftDto;

namespace SaleApi.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryrRepository;
        private readonly ILogger<CategoryService> _logger;

        public CategoryService(ICategoryRepository categoryrRepository, ILogger<CategoryService> logger)
        {
            _categoryrRepository = categoryrRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<Category>> GetAllCategory()
        {
            var category = await _categoryrRepository.GetAllCategory();
            return category ?? Enumerable.Empty<Category>();
        }

        public async Task<CreateCategoryDto> NewCategory(CreateCategoryDto categoryDto)
        {
            var category = new Category
            {
                Name = categoryDto.Name,
                Color = categoryDto.Color,
            };

            var cerated = await _categoryrRepository.NewCategory(category);
            _logger.LogInformation("Category created with ID: {CategoryId}", cerated.Id);

            return new CreateCategoryDto
            {
                Name = cerated.Name,
                Color = cerated.Color,
            };
        }

        public async Task DeleteCategory(int id)
        {
            await _categoryrRepository.DeleteCategory(id);
        }

        public async Task<Category> GetCategoryById(int id)
        {
            var c = await _categoryrRepository.GetCategoryById(id);
            if (c == null) return null;
            return c;
        }

        public async Task<GetCategoryDto> UpdateCategory(GetCategoryDto CategoryDto)
        {
            var existing = await _categoryrRepository.GetCategoryById(CategoryDto.Id);
            if (existing == null) return null;

            existing.Name = CategoryDto.Name ?? existing.Name;
            existing.Color = CategoryDto.Color ?? existing.Color;

            var updatedCategory = await _categoryrRepository.UpdateCategory(existing);
            if (updatedCategory == null) return null;

            _logger.LogInformation("Category update with ID: {CategoryId}", updatedCategory.Id);

            return new GetCategoryDto
            {
                Name = updatedCategory.Name,
                Color = updatedCategory.Color
            };
        }

        public async Task<List<GiftResponseDto>> GetGiftByCategoryId(int categoryId)
        {
            var gifts = await _categoryrRepository.GetGiftByCategoryId(categoryId);
            return gifts.Select(g => MapToResponseGiftDto(g)).ToList();
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
