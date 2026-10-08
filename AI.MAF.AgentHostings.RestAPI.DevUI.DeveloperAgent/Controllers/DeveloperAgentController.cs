using Microsoft.Agents.AI;
using Microsoft.AspNetCore.Mvc;

namespace AI.MAF.AgentHostings.RestAPI.DevUI.DeveloperAgent.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DeveloperAgentController : ControllerBase
    {

        [HttpGet("api/chat")]
        public async Task<IResult> Get([FromQuery] QueryRequest request,
            [FromKeyedServices("DeveloperAgent")] AIAgent querySupportAgent)
        {
            var response = await querySupportAgent.RunAsync(request.Message);
            return Results.Ok(response.Text);
        }
    }
}
