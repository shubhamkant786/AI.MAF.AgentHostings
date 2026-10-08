using Azure.Monitor.OpenTelemetry.Exporter;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Hosting.AzureFunctions;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Azure.Functions.Worker.OpenTelemetry;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenAI;
using OpenTelemetry;
using System.ClientModel;

var builder = FunctionsApplication.CreateBuilder(args);

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

var aiAgent = chatClient.AsAIAgent(
            name: "SupportAgent",
            instructions:
                """
                You are a helpful enterprise support agent.
                Your answers must be concise, professional, and limited strictly to enterprise support queries.        
                Keep responses concise — 3-5 sentences per turn. Be polite and opinionated.        
                """);

builder
    .ConfigureFunctionsWebApplication()
    .ConfigureDurableAgents(options => options.AddAIAgent(aiAgent, timeToLive: TimeSpan.FromHours(1)));

if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("APPLICATIONINSIGHTS_CONNECTION_STRING")))
{
    builder.Services.AddOpenTelemetry()
        .UseFunctionsWorkerDefaults()
        .UseAzureMonitorExporter();
}

builder.Build().Run();

//To call agent:
//http://localhost:7071/api/agents/SupportAgent/run
//Header:"Content-Type: application/json"
//Body: { "input": "hi" }

//To continue your chat:
//http://localhost:7071/api/agents/SupportAgent/run?session_id=your-session-id
//Header:"Content-Type: application/json"
//Body: { "input": "I am good, tell me if you can fix my issue" }
//you will get a session id in first response, use that to continue your chat with the same context.
