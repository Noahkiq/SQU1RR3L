using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Modules;
using Discord.Audio;
using System.IO;

class Program
{
    static void Main(string[] args) => new Program().Start();

    private DiscordClient _client;

    public void Start()
    {
        _client = new DiscordClient();
        Console.WriteLine("Booting up bot...");

        _client.UsingCommands(x => {
            x.PrefixChar = '*';
            x.HelpMode = HelpMode.Public;
        });

        _client.MessageReceived += async (s, e) =>
        {
            if (e.Message.Text == "lo;")
                await e.Channel.SendMessage("lol good grammar noob");
        };

        //Since we have setup our CommandChar to be '*', we will run this command by typing *greet
        _client.GetService<CommandService>().CreateCommand("greet") //create command greet
                .Alias(new string[] { "gr", "hi" }) //add 2 aliases, so it can be run with ~gr and ~hi
                .Description("Greets a person.") //add description, it will be shown when ~help is used
                .Parameter("GreetedPerson", ParameterType.Required) //as an argument, we have a person we want to greet
                .Do(async e =>
                {
                    await e.Channel.SendMessage($"{e.User.Name} greets {e.GetArg("GreetedPerson")}");
                    //sends a message to channel with the given text
                });

        _client.GetService<CommandService>().CreateCommand("bork") //create command greet
                .Alias(new string[] { "maximumbork", "borkdrive", "maximumborkdrive" }) //add aliases
                .Description("ＭＡＸＩＭＵＭ　ＢＯＲＫＤＲＩＶＥ") //add description, it will be shown when *help is used
                .Do(async e =>
                {
                    await e.Channel.SendMessage($"http://i.imgur.com/bAxPXuo.jpg");
                    //sends a message to channel with the given text
                });

        _client.GetService<CommandService>().CreateCommand("sigh") //create command
                .Alias(new string[] { "psy" }) //add alias
                .Description("Emojipasta.") //add description, it will be shown when *help is used
                .Do(async e =>
                {
                    await e.Channel.SendMessage($"why SIGH😔💨😤 when you can PSY😎🐴💕 instead??👌💯👍 send this📤👈👈 to 101⃣0⃣ people👬👭 who made you OPPA GANGNAM SMILE☺😸️💖 today📅!!!!");
                    //sends a message to channel with the given text
                });

        _client.GetService<CommandService>().CreateCommand("invite") //create command
                .Alias(new string[] { "joinserver", "join" }) //add aliases
                .Description("Posts the link to get the bot on your server.") //add description, it will be shown when *help is used
                .Do(async e =>
                {
                    await e.Channel.SendMessage($"You can add me to your server using https://discordapp.com/oauth2/authorize?client_id=215591038855675904&scope=bot - make sure you have `Manage Server` permissions!");
                    //sends a message to channel with the given text
                });

        _client.GetService<CommandService>().CreateCommand("oh") //create command
                .Description("oh") //add description, it will be shown when *help is used
                .Do(async e =>
                {
                    await e.Channel.SendMessage($"oh");
                    //sends a message to channel with the given text
                });

        // Register a Hook into the UserBanned event using a Lambda
        _client.UserBanned += async (s, e) => {
            // Create a Channel object by searching for a channel named '#logs' on the server the ban occurred in.
            var logChannel = e.Server.FindChannels("squirrel-log").FirstOrDefault();
            // Send a message to the server's log channel, stating that a user was banned.
            await logChannel.SendMessage($"{e.User.Name} was banned from the server.");
        };

        // Register a Hook into the UserUnanned event using a Lambda
        _client.UserUnbanned += async (s, e) => {
            // Create a Channel object by searching for a channel named '#squirrel-log' on the server the ban occurred in.
            var logChannel = e.Server.FindChannels("squirrel-log").FirstOrDefault();
            // Send a message to the server's log channel, stating that a user was unbanned.
            await logChannel.SendMessage($"{e.User.Name} was unbanned from the server.");
        };

        // Register a Hook into the UserJoined event using a Lambda
        _client.UserJoined += async (s, e) => {
            // Create a Channel object by searching for a channel named '#squirrel-log' on the server the ban occurred in.
            var logChannel = e.Server.FindChannels("squirrel-log").FirstOrDefault();
            // Send a message to the server's log channel, stating that a user was unbanned.
            await logChannel.SendMessage($"{e.User.Name} joined the server.");
        };

        // Register a Hook into the UserUnanned event using a Lambda
        _client.UserLeft += async (s, e) => {
            // Create a Channel object by searching for a channel named '#squirrel-log' on the server the ban occurred in.
            var logChannel = e.Server.FindChannels("squirrel-log").FirstOrDefault();
            // Send a message to the server's log channel, stating that a user was unbanned.
            await logChannel.SendMessage($"{e.User.Name} left the server.");
        };

        string token = File.ReadAllText("token.config");
        _client.ExecuteAndWait(async () => {
            await _client.Connect(token);
            _client.SetGame("*invite");
        });
    }
}