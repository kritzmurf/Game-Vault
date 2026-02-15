using Microsoft.AspNetCore.Mvc;

namespace GameVault.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HelloController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new { message = "Hello from Game Vault!" });
    }
}
