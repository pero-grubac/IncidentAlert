using IncidentAlert.Models.Dto;

namespace IncidentAlert.Services
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDto>> GetAll();
        Task<CategoryDto?> GetById(int id);
        Task<CategoryDto> Add(CategoryDto entity);
        Task<CategoryDto> Update(int id, CategoryDto entity);
        Task Delete(int id);

    }
}
