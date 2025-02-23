using System.Text;
using Microsoft.Extensions.AI;

namespace dotnet_llm_integration;

public class ChatService
{
    private readonly IChatClient _chatClient;
    private readonly List<ChatMessage> _messages = new();

    public ChatService(IChatClient chatClient)
    {
        _chatClient = chatClient;
    }

    public async Task StartChatAsync()
    {
        while (true)
        {
            string userInput = GetUserInput();
            if (userInput.Equals("exit", StringComparison.OrdinalIgnoreCase))
                break;

            _messages.Add(new ChatMessage(ChatRole.User, userInput));
            string response = await GetResponseAsync();

            _messages.Add(new ChatMessage(ChatRole.Assistant, response));
            Console.WriteLine("\n");
        }
    }

    private static string GetUserInput()
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write("> Enter your question: ");
        Console.ResetColor();
        return Console.ReadLine()?.Trim() ?? string.Empty;
    }

    private async Task<string> GetResponseAsync()
    {
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.Write("> Answer: ");
        Console.ResetColor();

        var stringBuilder = new StringBuilder();
        await foreach (var streamData in _chatClient.GetStreamingResponseAsync(_messages))
        {
            Console.Write(streamData.Text);
            stringBuilder.Append(streamData.Text);
        }
        return stringBuilder.ToString();
    }
}