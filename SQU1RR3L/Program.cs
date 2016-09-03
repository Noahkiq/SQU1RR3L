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

        _client.UsingCommands(x => {
            x.PrefixChar = '*';
            x.HelpMode = HelpMode.Public;
        });

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

        // Register a Hook into the UserUpdated event using a Lambda
        _client.UserUpdated += async (s, e) => {
            // Check that the user is in a Voice channel
            if (e.After.VoiceChannel == null) return;

            // See if they changed Voice channels
            if (e.Before.VoiceChannel == e.After.VoiceChannel) return;
            var logChannel = e.Server.FindChannels("trash-bin").FirstOrDefault();
            await logChannel.SendMessage($"oops i fukd {e.After.Name}'s mum xddd");
        };

        string token = File.ReadAllText("token.config");
        _client.ExecuteAndWait(async () => {
            await _client.Connect(token);
        });
    }
}