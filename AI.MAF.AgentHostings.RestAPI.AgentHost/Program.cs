using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Foundry.Hosting;
using Microsoft.Extensions.AI;
using OpenAI;
using System.ClientModel;

//https://github.com/microsoft-foundry/foundry-samples/tree/main/samples/csharp/hosted-agents/agent-framework/hello-world

// Add services to the container.
const string deploymentName = "gpt-5.2";
const string endpointOpenAI = "https://shubham-ms-foundry.services.ai.azure.com/openai/v1";
const string apiKey = $"";

var openAIClient = new OpenAIClient(new ApiKeyCredential(apiKey),
         new OpenAIClientOptions() { Endpoint = new Uri(endpointOpenAI) });


IChatClient chatClient = openAIClient.GetChatClient(deploymentName).AsIChatClient()
    .AsBuilder()
    .UseOpenTelemetry(configure: c => c.EnableSensitiveData = true)
    .Build();

AIAgent aiAgent = chatClient.AsAIAgent(
    name: "TourismQueryPlanningAgent",
    instructions:
        """
        You are a tourism query planning Agent.
        Your answers must be concise, professional, and limited strictly to tourism query planning.        
        Keep responses concise — 3-5 sentences per turn. Be polite and opinionated.        
        """);

var builder = AgentHost.CreateBuilder(args);
builder.Services.AddFoundryResponses(aiAgent);
builder.RegisterProtocol("responses", endpoints => endpoints.MapFoundryResponses());

var app = builder.Build();

app.Run();

/**
 * Example usage:
 *
*   # Turn 1 — invoke the agent:
 *curl - sS - X POST http://localhost:8088/responses \
 *-H "Content-Type: application/json" \
 *-d '{"input": "What is Microsoft Foundry?", "stream": false}' | jq.
 *
 *   # Turn 2 — follow up using the id from the previous response:
 *curl - sS - X POST http://localhost:8088/responses \
 *-H "Content-Type: application/json" \
 *-d '{"input": "Can you summarize that?", "previous_response_id": "<id>", "stream": false}' | jq.
 */
