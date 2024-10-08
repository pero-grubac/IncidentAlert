using IncidentAlert_Management.Models.Dto;

namespace IncidentAlert_Management.Services
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDto>> GetAll();
        Task<CategoryDto?> GetById(int id);
        Task<CategoryDto> Add(string name);
        Task<CategoryDto> Update(CategoryDto categoryDto);
        Task Delete(int id);

    }
}
