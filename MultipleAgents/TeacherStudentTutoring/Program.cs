using AutoGen.Core;
using TeacherStudentTutoring;

var teacherAgent = LocalAgentCreator.CreateLocalAgent("Teacher", temperature: 0f,
        modelApiIdentifier: "mistralai/magistral-small-2509",
        systemMessage: """
                       You're a Helpful School Teacher AI, who creates elementary school math problems for students and Checks their answers.        
                       If the answer is correct, you say 'Correct' and stop the conversation by saying 'COMPLETE'.         

                       If the answer is wrong, you say 'Incorrect!, and ask the student to fix it. 
                       """)
    .RegisterMiddleware(async (messages, option, agent, cancellationToken) =>
        {
            var reply = await agent.GenerateReplyAsync(messages, option, cancellationToken);

            if (reply.GetContent()?.ToLower()
                    .Contains("complete", StringComparison.CurrentCultureIgnoreCase) is not true)
            {
                return reply;
            }

            Console.WriteLine("Teacher Agent has completed the task.");

            return new TextMessage( Role.Assistant, GroupChatExtension.TERMINATE, from: reply.From);
        }
    );

var studentAgent = LocalAgentCreator.CreateLocalAgent("Student", temperature: 1.5f,
    modelApiIdentifier: "qwen/qwen3-4b-2507",
    systemMessage: """
                   You're a Student AI, who tries to solve elementary school math problems given by the Teacher AI.
                   You will try to answer the question, and if you get it wrong, 
                   you will try again until you get it right.
                   """);

var conversation = await studentAgent.InitiateChatAsync(
    receiver: teacherAgent,
    message: "Hello Teacher!, please create a math problem for me to solve.",
    maxRound: 10
);

var chatLogs = conversation as IMessage[] ?? conversation.ToArray();
Console.WriteLine(chatLogs.Last().IsGroupChatTerminateMessage()
    ? "Conversation successfully ended."
    : "Conversation time up!");

Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine("\r\n========= Conversation Log =========\r\n");
foreach (var message in chatLogs)
{
    switch(message.From)
    {
        case "Teacher":
            Console.ForegroundColor = ConsoleColor.Cyan;
            break;
        case "Student":
            Console.ForegroundColor = ConsoleColor.Yellow;
            break;
        default:
            Console.ResetColor();
            break;
    }
    Console.WriteLine($"{message.From}: {message.GetContent()}");
}
Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine("\r\n===== End of Conversation Log =====\r\n");
