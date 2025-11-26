using AutoGen.Core;

namespace SimpleReverseInputAgent;

public class ReverseInputAgent(string name) : IAgent
{
    public string Name { get; } = name;

    public Task<IMessage> GenerateReplyAsync(IEnumerable<IMessage> messages, GenerateReplyOptions? options = null,
        CancellationToken cancellationToken = new())
    {
        if (messages.LastOrDefault() is not TextMessage lastMessage)
        {
            throw new InvalidOperationException("No messages to reply to.");
        }

        // Reverse the content of the last message
        var reversedContent = new string(lastMessage.Content?.ToCharArray().Reverse().ToArray());
        IMessage replyMessage = new TextMessage(Role.Assistant, reversedContent, from: Name);

        return Task.FromResult(replyMessage);
    }
}