using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Hosting;
using Microsoft.Agents.AI.Hosting.AGUI.AspNetCore;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.AI;
using OpenAI;
using System.ClientModel;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddHttpClient().AddLogging();
builder.Services.AddAGUI();

const string deploymentName = "gpt-5.2";
const string endpointOpenAI = "https://shubham-ms-foundry.services.ai.azure.com/openai/v1";
const string apiKey = $"";


var openAIClient = new OpenAIClient(new ApiKeyCredential(apiKey),
         new OpenAIClientOptions() { Endpoint = new Uri(endpointOpenAI) });


IChatClient chatClient = openAIClient.GetChatClient(deploymentName).AsIChatClient();

AIAgent tourismQueryPlanningAgent = chatClient.AsAIAgent(
    name: "TourismQueryPlanningAgent",
    instructions:
        """
        You are a tourism query planning Agent.
        Your answers must be concise, professional, and limited strictly to tourism query planning.        
        Keep responses concise — 3-5 sentences per turn. Be polite and opinionated.        
        """);

AIAgent itServiceDeskAgent = chatClient.AsAIAgent(
    name: "ItServiceDeskAgent",
    instructions:
        """
        You are an IT service desk Agent.
        Your answers must be concise, professional, and limited strictly to IT service desk queries.        
        Keep responses concise — 3-5 sentences per turn. Be polite and opinionated.        
        """);

//This will save the session info of all the calls made via agui to the agents.
//Extend AgentSessionStore abstract class.
//AgentSessionStore agentSessionStore = new AgentSessionStore();
//builder.Services.AddSingleton(agentSessionStore);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapAGUI("/agui/tourism/support", tourismQueryPlanningAgent);
//app.MapAGUI("/agui/it/support", itServiceDeskAgent);

app.Run();
