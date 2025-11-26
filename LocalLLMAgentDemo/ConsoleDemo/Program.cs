using AutoGen.Core;
using LLMStudioAgent;
using OpenAI.Chat;

var localAgent = CreateAgentHelper.CreateLMStudioAgent("demo-local-llm-agent", modelName: "openai/gpt-oss-20b");

// Need to convert the message text into a UserChatMessage (OpenAI Type object)
// Then wrap in a MessageEnvelope, otherwise, OpenAIChatClient will reject it as "System.ArgumentException: Invalid message type."
IMessage<ChatMessage> requestMessage =
    new MessageEnvelope<ChatMessage>(ChatMessage.CreateUserMessage("Hello, what can you do?"));

var received = await localAgent.SendAsync(requestMessage);

if (received is MessageEnvelope<ChatCompletion> envelope)
{
    var messageContentPart = envelope.Content.Content.FirstOrDefault();
    if (messageContentPart != null)
    {
        Console.WriteLine("Received response:\r\n");
        Console.WriteLine(messageContentPart.Text);
    }
}