using Microsoft.Agents.AI;
using Microsoft.Agents.AI.AGUI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging.Abstractions;

namespace AI.MAF.AgentHostings.RestAPI.AGUI.CallerService.Controllers;

[ApiController]
[Route("[controller]")]
public class JustController : ControllerBase
{

    private readonly HttpClient _httpClient;

    public JustController()
    {
        _httpClient = new HttpClient();
    }

    [HttpGet("api/tourismsupport")]
    public async Task<string> Get([FromQuery] string userMessage)
    {
        var aguiClient = new AGUIChatClient(
            _httpClient,
            "http://localhost:5018/agui/tourism/support",
            NullLoggerFactory.Instance);



        // 2. Wrap the remote connection in a local Agent interface
        AIAgent clientAgent = aguiClient.AsAIAgent(
            name: "SupportClient",
            description: "You are a helpful enterprise support agent.");

        // 3. Send the message and stream the UI-compliant response in real-time
        string responseText = string.Empty;
        await foreach (var update in clientAgent.RunStreamingAsync(userMessage))
        {
            responseText += update.Text;
        }

        return responseText;
    }
}
