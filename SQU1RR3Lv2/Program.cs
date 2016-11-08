using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Rest;
using System.IO;

class Program
{
    // Convert our sync-main to an async main method
    static void Main(string[] args) => new Program().Run().GetAwaiter().GetResult();

    // Define some variables
    public static string botPrefix = "s!";
    public static DateTimeOffset MessageSent;
    public string LastUsedRPS;
    public static int CommandsUsed;
    public static DateTime StartupTime = DateTime.Now;
    static Random rnd = new Random();
    public static bool onlyOpToggle = false;

    // Create a DiscordClient with WebSocket support
    private DiscordSocketClient client;

    public async Task Run()
    {
        Console.WriteLine("Starting up bot...");

        client = new DiscordSocketClient();
        // logging stuff should go here but couldnt get it to work

        // Grab bot token from token.txt
        string token = File.ReadAllText("token.txt");

        // Hook into the MessageReceived event on DiscordSocketClient
        client.MessageReceived += async (message) =>
        {   
            if (message.Content.StartsWith($"{botPrefix}"))
            {
                CommandsUsed++;
                if (message.Content == $"{botPrefix}help")
                {
                    await message.Channel.SendMessageAsync("Current available commands:\n" +
                                                          $"`help` - Lists the available commands.\n" +
                                                          $"`ping` - Shows the bot response time.\n" +
                                                          $"Current bot prefix: `{botPrefix}`");
                }
                else if (message.Content == $"{botPrefix}ping")
                {
                    MessageSent = message.Timestamp;
                    await message.Channel.SendMessageAsync("beepboop, calculating response time...");
                }
                else if (message.Content == "beepboop, calculating response time...")
                {
                    var ping = message.Timestamp - MessageSent;
                    await message.DeleteAsync();
                    await message.Channel.SendMessageAsync($"Pong! The bot responded in {ping.TotalMilliseconds}ms.");
                }
            }
        };

        // Configure the client to use a Bot token, and use our token
        await client.LoginAsync(TokenType.Bot, token);
        // Connect the client to Discord's gateway
        await client.ConnectAsync();

        // Block this task until the program is exited.
        await Task.Delay(-1);
    }

    private Task Client_Log(LogMessage arg)
    {
        throw new NotImplementedException();
    }
}