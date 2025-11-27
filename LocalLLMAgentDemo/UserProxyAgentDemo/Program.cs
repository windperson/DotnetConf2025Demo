using System.ClientModel;
using AutoGen;
using AutoGen.Core;
using AutoGen.OpenAI;
using AutoGen.OpenAI.Extension;
using OpenAI;

var openAiClient = new OpenAIClient(new ApiKeyCredential("api-key"), new OpenAIClientOptions
{
    Endpoint = new Uri("http://localhost:1234/v1")
});

var chatClient = openAiClient.GetChatClient("openai/gpt-oss-20b");

var openAiAgent = new OpenAIChatAgent(chatClient: chatClient, name: "local-llm-agent",
        systemMessage: "你是個總是完全都用繁體中文回覆的 AI Assistant。")
    .RegisterMessageConnector()
    .RegisterPrintMessage();

var userProxyAgent = new UserProxyAgent(
        name: "user-proxy-agent",
        humanInputMode: HumanInputMode.ALWAYS)
    .RegisterPrintMessage();

await userProxyAgent.InitiateChatAsync(receiver: openAiAgent);