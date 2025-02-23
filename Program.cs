using dotnet_llm_integration;
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
