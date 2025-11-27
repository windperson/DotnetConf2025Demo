using System.ClientModel;
using AutoGen.Core;
using AutoGen.OpenAI;
using AutoGen.OpenAI.Extension;
using OpenAI;

namespace LLMStudioAgent;

public static class CreateAgentHelper
{
    // ReSharper disable once InconsistentNaming
    public static IStreamingAgent CreateLMStudioAgent(string agentName, string modelName,
        string systemPrompt = "",
        string lmStudioEndpoint = "http://localhost:1234/v1")
    {
        var openAiClient = new OpenAIClient(new ApiKeyCredential("api-key"), new OpenAIClientOptions
        {
            Endpoint = new Uri(lmStudioEndpoint)
        });

        var chatClient = openAiClient.GetChatClient(modelName);

        var localAgent = string.IsNullOrEmpty(systemPrompt)
            ? new OpenAIChatAgent(chatClient: chatClient, agentName)
            : new OpenAIChatAgent(chatClient: chatClient, agentName, systemPrompt);

        localAgent
            .RegisterMessageConnector()
            .RegisterPrintMessage();

        return localAgent;
    }
}