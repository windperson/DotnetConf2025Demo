using System.ClientModel;
using AutoGen.Core;
using AutoGen.OpenAI;
using AutoGen.OpenAI.Extension;
using OpenAI;

namespace ConsoleDemo;

public static class CreateAgentHelper
{
    public static IStreamingAgent CreateLMStudioAgent(string agentName, string modelName,
        string systemMessage = "",
        string lmStudioEndpoint = "http://localhost:1234/v1")
    {
        var openAiClient = new OpenAIClient(new ApiKeyCredential("api-key"), new OpenAIClientOptions
        {
            Endpoint = new Uri(lmStudioEndpoint)
        });

        var chatClient = openAiClient.GetChatClient(modelName);

        var localAgent = string.IsNullOrEmpty(systemMessage)
            ? new OpenAIChatAgent(chatClient: chatClient, agentName)
            : new OpenAIChatAgent(chatClient: chatClient, agentName, systemMessage);

        localAgent
            .RegisterMessageConnector(new OpenAIChatRequestMessageConnector())
            .RegisterPrintMessage();

        return localAgent;
    }
}