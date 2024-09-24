using IncidentAlert.Data;
using IncidentAlert.Models;
using Microsoft.EntityFrameworkCore;

namespace IncidentAlert.Repositories.Implementation
{
    public class ImageRepository(DataContext dataContext) : IImageRepository
    {
        private readonly DataContext _dataContext = dataContext;

        public async Task Add(Image image)
        {
            await _dataContext.Images.AddAsync(image);
            await _dataContext.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var image = await GetById(id);
            if (image != null)
            {
                _dataContext.Images.Remove(image);
                await _dataContext.SaveChangesAsync();
            }
        }

        public async Task<Image?> GetById(int id)
             => await _dataContext.Images.FirstOrDefaultAsync(i => i.Id == id);

        public async Task<IEnumerable<Image>> GetByIncidentId(int incidentId)
            => await _dataContext.Images.Where(i => i.IncidentId == incidentId).ToListAsync();
    }
}
