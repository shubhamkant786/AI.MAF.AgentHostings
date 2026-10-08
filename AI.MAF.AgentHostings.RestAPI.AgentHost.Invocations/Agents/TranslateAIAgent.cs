using Microsoft.Agents.AI;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.AI;
using System.Text.Json;

namespace AI.MAF.AgentHostings.RestAPI.AgentHost.Invocations.Agents
{
    public class TranslateAIAgent : AIAgent
    {
        protected override ValueTask<AgentSession> CreateSessionCoreAsync(CancellationToken cancellationToken = default)
        {
            return new ValueTask<AgentSession>(new TranslateAgentSession());
        }
        protected override ValueTask<JsonElement> SerializeSessionCoreAsync(AgentSession session, JsonSerializerOptions? jsonSerializerOptions = null, CancellationToken cancellationToken = default)
        {
            return new ValueTask<JsonElement>(JsonSerializer.SerializeToElement(new { session }, jsonSerializerOptions));
        }

        protected override ValueTask<AgentSession> DeserializeSessionCoreAsync(JsonElement serializedState, JsonSerializerOptions? jsonSerializerOptions = null, CancellationToken cancellationToken = default)
        {
            return new ValueTask<AgentSession>(JsonSerializer.Deserialize<TranslateAgentSession>(serializedState.GetRawText(), jsonSerializerOptions)!);
        }



        protected override Task<AgentResponse> RunCoreAsync(IEnumerable<ChatMessage> messages, AgentSession? session = null, AgentRunOptions? options = null, CancellationToken cancellationToken = default)
        {
            var inputText = messages.Where(m => m.Role == ChatRole.User)
                .Select(m => m.WithAgentRequestMessageSource(AgentRequestMessageSourceType.ChatHistory))
                .Select(m => m.Text).LastOrDefault() ?? string.Empty;
            var translatedText = $"Translated: {inputText}";
            var response = new AgentResponse(new ChatMessage(ChatRole.Assistant, $"Agent: {translatedText}"));
            return Task.FromResult(response);
        }

        protected async override IAsyncEnumerable<AgentResponseUpdate> RunCoreStreamingAsync(IEnumerable<ChatMessage> messages, AgentSession? session = null, AgentRunOptions? options = null, CancellationToken cancellationToken = default)
        {
            var inputText = messages.Where(m => m.Role == ChatRole.User)
                .Select(m => m.WithAgentRequestMessageSource(AgentRequestMessageSourceType.ChatHistory))
                .Select(m => m.Text).LastOrDefault() ?? string.Empty;
            var translatedText = $"Translated: {inputText}"; // Placeholder for actual translation logic
            yield return new AgentResponseUpdate
            {
                Role = ChatRole.Assistant,
                Contents = new[] { new TextContent($"Agent: {translatedText}") },
            };

            await Task.CompletedTask;
        }
    }
}
