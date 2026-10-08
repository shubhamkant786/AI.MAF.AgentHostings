using Azure.AI.AgentServer.Invocations;

namespace AI.MAF.AgentHostings.RestAPI.AgentHost.Invocations.Agents
{
    public class TranslateInvocationHandler(TranslateAIAgent translateAIAgent) : InvocationHandler
    {
        public async override Task HandleAsync(HttpRequest request, HttpResponse response, InvocationContext context, CancellationToken cancellationToken)
        {
            using var reader = new StreamReader(request.Body);
            var input = await reader.ReadToEndAsync(cancellationToken);

            var agentResponse = await translateAIAgent.RunAsync(input, cancellationToken: cancellationToken);

            response.ContentType = "text/plain";
            await response.WriteAsync(agentResponse.Text, cancellationToken);
        }
    }
}
