using Microsoft.AspNetCore.Mvc;

namespace AI.MAF.AgentHostings.RestAPI.AGUI.CalledService.Controllers;

[ApiController]
[Route("[controller]")]
public class TempController : ControllerBase
{
    [HttpGet("temp")]
    public string Get()
    {
        return string.Empty;
    }
}
