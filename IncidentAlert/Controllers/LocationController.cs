using IncidentAlert.Services;
using Microsoft.AspNetCore.Mvc;

namespace IncidentAlert.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LocationController(ILocationService locationService) : ControllerBase
    {
        private readonly ILocationService _service = locationService;
    }
}
