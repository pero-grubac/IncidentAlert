using IncidentAlert.Models.Dto;

namespace IncidentAlert.Services
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDto>> GetAll();
        Task<CategoryDto?> GetById(int id);
        Task<CategoryDto> Add(CategoryDto categoryDto);
        Task<CategoryDto> Update(CategoryDto categoryDto);
        Task Delete(int id);

    }
}
