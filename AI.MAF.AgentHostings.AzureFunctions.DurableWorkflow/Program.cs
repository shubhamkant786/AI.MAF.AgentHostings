using Azure.Monitor.OpenTelemetry.Exporter;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Hosting.AzureFunctions;
using Microsoft.Agents.AI.Workflows;
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

var aiSupportAgent = chatClient.AsAIAgent(
            name: "SupportAgent",
            instructions:
                """
                You are a helpful enterprise support agent.
                Your answers must be concise, professional, and limited strictly to enterprise support queries.        
                Keep responses concise — 3-5 sentences per turn. Be polite and opinionated.        
                """);
var aiHRAgent = chatClient.AsAIAgent(
            name: "SupportAgent",
            instructions:
                """
                You are a helpful enterprise hr agent.
                Your answers must be concise, professional, and limited strictly to enterprise hr queries.        
                Keep responses concise — 3-5 sentences per turn. Be polite and opinionated.        
                """);
var workflow = new WorkflowBuilder(aiSupportAgent)
    .AddEdge(aiSupportAgent, aiHRAgent)
    .WithOutputFrom(aiSupportAgent, aiHRAgent)
    .Build();

builder
    .ConfigureFunctionsWebApplication()
    .ConfigureDurableAgents(options => options.AddAIAgent(aiSupportAgent, timeToLive: TimeSpan.FromHours(1)))
    .ConfigureDurableWorkflows(options => options.AddWorkflow(workflow, exposeStatusEndpoint: true, exposeMcpToolTrigger: true));

if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("APPLICATIONINSIGHTS_CONNECTION_STRING")))
{
    builder.Services.AddOpenTelemetry()
        .UseFunctionsWorkerDefaults()
        .UseAzureMonitorExporter();
}


builder.Build().Run();
