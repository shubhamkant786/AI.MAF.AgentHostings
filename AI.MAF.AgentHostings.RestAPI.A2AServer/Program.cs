using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI;
using System.ClientModel;
using Microsoft.Agents.AI.Hosting;
using Microsoft.Agents.AI.Hosting.A2A;

var builder = WebApplication.CreateBuilder(args);

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
builder.Services.AddSingleton(chatClient);

var developerAgent = builder.AddAIAgent(
    name: "DeveloperAgent",
    instructions:
        """
        You are a developer Agent.
        Your answers must be concise, professional, and limited strictly to development query and provide sample code only.        
        Keep responses concise — 3-5 sentences per turn. Be polite and opinionated.        
        """, chatClient);

developerAgent.AddA2AServer();

var app = builder.Build();


app.MapA2AHttpJson(developerAgent, path: "/a2a/developer");
app.MapA2AJsonRpc(developerAgent, path: "/a2a/rpc/developer");

app.Run();
