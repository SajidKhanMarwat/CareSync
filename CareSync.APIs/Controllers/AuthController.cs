using Microsoft.AspNetCore.Mvc;

namespace CareSync.APIs.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetString()
    {
        return Ok();
    }
}
