var builder = DistributedApplication.CreateBuilder(args);

var devAgentBuilder = builder.AddProject<Projects.AI_MAF_AgentHostings_RestAPI_DevUI_DeveloperAgent>("DeveloperAgent")
    .WithUrls(context =>
    {
        var baseUrl = context.Urls.FirstOrDefault();
        if (baseUrl is not null)
        {
            context.Urls.Add(new()
            {
                Url = baseUrl.Url.TrimEnd('/') + "/devui",
                DisplayText = "DevUI Visual App"
            });
        }
    })
    .WithHttpHealthCheck("/health");


builder.AddProject<Projects.AI_MAF_AgentHostings_RestAPI_DevUI_TesterAgent>("TesterAgent")
    .WithUrls(context =>
    {
        var baseUrl = context.Urls.FirstOrDefault();
        if (baseUrl is not null)
        {
            context.Urls.Add(new()
            {
                Url = baseUrl.Url.TrimEnd('/') + "/devui",
                DisplayText = "DevUI Visual App"
            });
        }
    })
    .WithHttpHealthCheck("/health")
    .WithReference(devAgentBuilder)
    .WaitFor(devAgentBuilder);

var calledServiceBuilder = builder.AddProject<Projects.AI_MAF_AgentHostings_RestAPI_AGUI_CalledService>("ai-maf-agenthostings-restapi-agui-calledservice");
var callerServiceBuilder = builder.AddProject<Projects.AI_MAF_AgentHostings_RestAPI_AGUI_CallerService>("ai-maf-agenthostings-restapi-agui-callerservice")
    .WithReference(calledServiceBuilder)
    .WaitFor(calledServiceBuilder); ;


builder.AddAzureFunctionsProject<Projects.AI_MAF_AgentHostings_AzureFunctions>("ai-maf-agenthostings-azurefunctions");

builder.AddAzureFunctionsProject<Projects.AI_MAF_AgentHostings_AzureFunctions_DurableAgent>("ai-maf-agenthostings-azurefunctions-durableagent");

builder.AddAzureFunctionsProject<Projects.AI_MAF_AgentHostings_AzureFunctions_DurableWorkflow>("ai-maf-agenthostings-azurefunctions-durableworkflow");

//builder.AddProject<Projects.AI_MAF_AgentHostings_RestAPI_AgentHost_Responses>("ai-maf-agenthostings-restapi-agenthost");

//builder.AddProject<Projects.AI_MAF_AgentHostings_RestAPI_AgentHost_Invocations>("ai-maf-agenthostings-restapi-agenthost-invocations");

builder.AddProject<Projects.AI_MAF_AgentHostings_RestAPI_A2AServer>("ai-maf-agenthostings-restapi-a2aserver");

//builder.AddProject<Projects.AI_MAF_AgentHostings_RestAPI_AgentHost_Responses>("ai-maf-agenthostings-restapi-agenthost");

//builder.AddProject<Projects.AI_MAF_AgentHostings_RestAPI_AgentHost_Invocations>("ai-maf-agenthostings-restapi-agenthost-invocations");

builder.Build().Run();
