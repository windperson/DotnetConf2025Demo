using System.ClientModel;
using AutoGen;
using AutoGen.Core;
using AutoGen.OpenAI;
using OpenAI;
using OpenAI.Chat;

var openAiClient = new OpenAIClient(new ApiKeyCredential("api-key"), new OpenAIClientOptions
{
    Endpoint = new Uri("http://localhost:1234/v1")
});

var chatClient = openAiClient.GetChatClient("openai/gpt-oss-20b");

var openAiAgent = new OpenAIChatAgent(chatClient: chatClient, name: "local-llm-agent",
        systemMessage: "你是個總是完全都用繁體中文回覆的 AI Assistant。")
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

            Console.WriteLine("====== Token Usage ======");
            Console.WriteLine($"Input  token count: {inputTokenCount}");
            Console.WriteLine($"Output token count: {outputTokenCount}");
            Console.WriteLine($"Total  token count: {totalTokenCount}");
            Console.WriteLine("=========================\r\n");

            return reply;
        }
    )
    .RegisterMiddleware(new OpenAIChatRequestMessageConnector())
    .RegisterPrintMessage();

var userProxyAgent = new UserProxyAgent(
        name: "user-proxy-agent",
        humanInputMode: HumanInputMode.ALWAYS)
    .RegisterPrintMessage();

await userProxyAgent.InitiateChatAsync(receiver: openAiAgent);