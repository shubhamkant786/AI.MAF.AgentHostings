using Microsoft.Agents.AI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Net;

namespace AI.MAF.AgentHostings.AzureFunctions;

public class HttpTriggerFunction
{
    private readonly ILogger<HttpTriggerFunction> _logger;

    private readonly AIAgent _aiAgent;

    public HttpTriggerFunction(ILogger<HttpTriggerFunction> logger, IChatClient chatClient)
    {
        _logger = logger;
        _aiAgent = chatClient.AsAIAgent(
            name: "SupportAgent",
            instructions:
                """
                You are a helpful enterprise support agent.
                Your answers must be concise, professional, and limited strictly to enterprise support queries.        
                Keep responses concise — 3-5 sentences per turn. Be polite and opinionated.        
                """);

    }
    //Sample call
    //http://localhost:7031/api/supportagent?message=hi

    [Function("HttpTrigger")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "supportagent")] HttpRequestData req)
    {

        _logger.LogInformation("C# HTTP trigger function processed a request.");
        var queryString = req.Url.Query ?? string.Empty;
        if (queryString.Contains("message"))
        {
            var messageValue = req.Query["message"];

            var responseMessage = await _aiAgent.RunAsync(messageValue.ToString());

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
            await response.WriteStringAsync(responseMessage.Text);
            return new OkObjectResult(response);
        }
        return new BadRequestObjectResult("Welcome to Azure Functions!");
    }
}