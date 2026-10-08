using AI.MAF.AgentHostings.RestAPI.AgentHost.Invocations.Agents;
using Azure.AI.AgentServer.Invocations;
using Microsoft.Extensions.Diagnostics.HealthChecks;

//https://github.com/microsoft-foundry/foundry-samples/tree/main/samples/csharp/hosted-agents/agent-framework/invocations-echo-agent
var builder = AgentHost.CreateBuilder(args);

builder.ConfigureHealth(a => a.AddCheck("AgentHost", () => HealthCheckResult.Healthy("AgentHost is healthy.")));
builder.ConfigureTracing(a => a.AddSource("AgentHost"));
builder.ConfigureShutdown(TimeSpan.FromHours(1));

builder.Services.AddSingleton<TranslateAIAgent>();

builder.Services.AddInvocationsServer();
builder.Services.AddScoped<InvocationHandler, TranslateInvocationHandler>();

// Register the Invocations endpoints through AgentHost so the shared port, middleware,
// health checks, platform headers, and telemetry initialization remain active.
builder.RegisterProtocol("invocations", endpoints => endpoints.MapInvocationsServer());


var app = builder.Build();
app.Run();
