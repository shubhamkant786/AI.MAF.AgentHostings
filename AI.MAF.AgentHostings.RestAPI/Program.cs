using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI.Hosting;
using Microsoft.Extensions.AI;
using OpenAI;
using Scalar.AspNetCore;
using System.ClientModel;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi(o =>
o.OpenApiVersion = Microsoft.OpenApi.OpenApiSpecVersion.OpenApi3_1);


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

builder.AddAIAgent(
    name: "TourismQueryPlanningAgent",
    instructions:
        """
        You are a tourism query planning Agent.
        Your answers must be concise, professional, and limited strictly to tourism query planning.        
        Keep responses concise — 3-5 sentences per turn. Be polite and opinionated.        
        """,
    chatClient);

builder.AddAIAgent(
    name: "ItServiceDeskAgent",
    instructions:
        """
        You are an IT service desk Agent.
        Your answers must be concise, professional, and limited strictly to IT service desk queries.        
        Keep responses concise — 3-5 sentences per turn. Be polite and opinionated.        
        """,
    chatClient);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();//https://localhost:7246/scalar
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
