using Azure.Core;
using Microsoft.Agents.AI;
using Microsoft.AspNetCore.Mvc;

namespace AI.MAF.AgentHostings.RestAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TourismAgentController : ControllerBase
    {        
        [HttpGet("api/chat")]
        public async Task<IResult> Get([FromQuery] QueryRequest request,
    [FromKeyedServices("TourismQueryPlanningAgent")] AIAgent querySupportAgent)
        {
            var response = await querySupportAgent.RunAsync(request.Message);
            return Results.Ok(response.Text);
        }

        [HttpGet("api/generatePlan")]
        public async Task<IResult> GeneratePlan([FromQuery] QueryRequest request,
    [FromKeyedServices("TourismQueryPlanningAgent")] AIAgent querySupportAgent)
        {
            AgentResponse<GenerateResponse> response = await querySupportAgent.RunAsync<GenerateResponse>(request.Message);
            return Results.Ok(response.Result);
        }
    }    
}
