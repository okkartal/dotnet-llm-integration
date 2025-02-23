using System.Text;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

class Program
{
    private const string OllamaDockerContainerName = "http://localhost:11434";
    private const string OllamaModelName = "llama3.2:1b";

    static async Task Main()
    {
        var chatService = InitializeChatService();
        Console.Clear();
        Console.WriteLine("---CHAT CLIENT---");

        await chatService.StartChatAsync();
    }

    private static ChatService InitializeChatService()
    {
        var appBuilder = Host.CreateApplicationBuilder();
        appBuilder.Services.AddChatClient(new OllamaChatClient(new Uri(OllamaDockerContainerName), OllamaModelName));
        var app = appBuilder.Build();
        var chatClient = app.Services.GetRequiredService<IChatClient>();

        return new ChatService(chatClient);
    }
}

class ChatService
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
