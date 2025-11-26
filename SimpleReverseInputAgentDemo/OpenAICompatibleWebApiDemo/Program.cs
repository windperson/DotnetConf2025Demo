using AutoGen.Core;
using AutoGen.WebAPI;
using SimpleReverseInputAgent;

var demoReverseAgent = new ReverseInputAgent(name: "reverse-agent")
    .RegisterPrintMessage();

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseAgentAsOpenAIChatCompletionEndpoint(demoReverseAgent);

app.Run();