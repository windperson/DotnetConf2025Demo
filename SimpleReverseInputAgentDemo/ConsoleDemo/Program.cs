using AutoGen;
using AutoGen.Core;
using SimpleReverseInputAgent;

var userProxyAgent = new UserProxyAgent(
        name: "user-proxy-agent",
        humanInputMode: HumanInputMode.ALWAYS)
    .RegisterPrintMessage();

var demoReverseAgent = new ReverseInputAgent(name: "reverse-agent")
    .RegisterPrintMessage();

await userProxyAgent.InitiateChatAsync(receiver: demoReverseAgent);