using WebApi.Helper.Filters;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Configuration
{
    [ApiController]
    [ApiResultFilter]
    [Route("api/v{version:apiVersion}/[controller]")]// api/v1/[controller]
    public class BaseController : ControllerBase
    {
    }
}
