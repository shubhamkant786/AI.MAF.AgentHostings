using Microsoft.Agents.AI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AI.MAF.AgentHostings.RestAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceDeskAgentController : ControllerBase
    {
        [HttpGet("api/chat")]
        public async Task<IResult> Get([FromQuery] QueryRequest request,
            [FromKeyedServices("ItServiceDeskAgent")] AIAgent querySupportAgent)
        {
            var response = await querySupportAgent.RunAsync(request.Message);
            return Results.Ok(response.Text);
        }
    }
}
