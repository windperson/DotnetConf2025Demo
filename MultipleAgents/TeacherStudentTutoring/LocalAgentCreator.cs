using System.ClientModel;
using AutoGen.Core;
using AutoGen.OpenAI;
using OpenAI;
using OpenAI.Chat;

namespace TeacherStudentTutoring;

public static class LocalAgentCreator
{
    public static MiddlewareAgent<OpenAIChatAgent> CreateLocalAgent(string agentName, string modelApiIdentifier = "openai/gpt-oss-20b",
        string systemMessage = "",
        string openAiCompatibleApiUrl = "http://localhost:1234/v1",
        float? temperature = null)
    {
        var openAiClient = new OpenAIClient(new ApiKeyCredential("api-key"), new OpenAIClientOptions
        {
            Endpoint = new Uri(openAiCompatibleApiUrl)
        });

        var chatClient = openAiClient.GetChatClient(modelApiIdentifier);

        var aiAgent = new OpenAIChatAgent(chatClient: chatClient, name: agentName, systemMessage: systemMessage,
                temperature: temperature)
            .RegisterMiddleware(async (messages, option, agent, cancellationToken) =>
                {
                    var reply = await agent.GenerateReplyAsync(messages, option, cancellationToken);

                    if (reply is not MessageEnvelope<ChatCompletion> chatCompletion)
                    {
                        return reply;
                    }

                    var inputTokenCount = chatCompletion.Content.Usage.InputTokenCount;
                    var outputTokenCount = chatCompletion.Content.Usage.OutputTokenCount;
                    var totalTokenCount = chatCompletion.Content.Usage.TotalTokenCount;

                    Console.WriteLine($"======== {agentName} ========");
                    Console.WriteLine($"Input  token count: {inputTokenCount}");
                    Console.WriteLine($"Output token count: {outputTokenCount}");
                    Console.WriteLine($"Total  token count: {totalTokenCount}");
                    Console.WriteLine("=========================\r\n");

                    return reply;
                }
            )
            .RegisterMiddleware(new OpenAIChatRequestMessageConnector())
            .RegisterPrintMessage();


        return aiAgent;
    }
}