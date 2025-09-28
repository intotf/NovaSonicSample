using Microsoft.AspNetCore.Mvc;

namespace NovaSonicApp.Controllers
{
    /// <summary>
    /// Api 控制器
    /// </summary>
    [ApiController]
    [Route("sonic/[controller]")]
    [SwaggerGroup<SwaggerGroup>(SwaggerGroup.OtherApi)]
    public class ApiController : ControllerBase
    {
    }
}