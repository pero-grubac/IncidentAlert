using IncidentAlert.Models.Dto;

namespace IncidentAlert.Services
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDto>> GetAllAsync();
        Task<CategoryDto?> GetAsync(int id);
        Task<CategoryDto> AddAsync(CategoryDto entity);
        Task<CategoryDto> UpdateAsync(int id, CategoryDto entity);
        Task DeleteAsync(int id);

    }
}
