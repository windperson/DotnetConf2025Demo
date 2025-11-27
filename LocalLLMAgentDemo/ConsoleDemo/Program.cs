using AutoGen.Core;
using LLMStudioAgent;
using OpenAI.Chat;

var localAgent = CreateAgentHelper.CreateLMStudioAgent("demo-local-llm-agent", modelName: "openai/gpt-oss-20b");

// Need to create a UserChatMessage (OpenAI Type object) with the user input
// Then wrap in a MessageEnvelope, otherwise, OpenAIChatClient will reject it as throwing "System.ArgumentException: Invalid message type."
var received = await localAgent.SendAsync(MessageEnvelope.Create(new UserChatMessage("Hello LMStudio AI")));

if (received is MessageEnvelope<ChatCompletion> envelope)
{
    var messageContentPart = envelope.Content.Content.FirstOrDefault();
    if (messageContentPart != null)
    {
        Console.WriteLine("Received response:\r\n");
        Console.WriteLine(messageContentPart.Text);
    }
}

Console.WriteLine("\r\n--------- press any key to start streaming demo -------------\r\n");
Console.ReadKey();
Console.WriteLine("\r\nStreaming response:\r\n");

await foreach (var streamingReply in localAgent.GenerateStreamingReplyAsync([
                   MessageEnvelope.Create(new UserChatMessage("What you can do?"))
               ]))
{
    foreach (var update in ((MessageEnvelope<StreamingChatCompletionUpdate>)streamingReply).Content.ContentUpdate)
    {
        Console.Write(update.Text);
    }
}