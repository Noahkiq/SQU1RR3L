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

        _client.GetService<CommandService>().CreateCommand("emojiseals") //create command
                .Description("Naxy Seals Emojipasta") //add description, it will be shown when *help is used
                .Do(async e =>
                {
                    await e.Channel.SendMessage($"❔What❔the fuck 😡😡 did you just fucking say about me, you little 🐕bitch🐕? I’ll have you know I graduated 🎓🎓 top🔝 of my class in the 🚢Navy🚢 Seals, and I’ve been involved in numerous 😯secret😯 raids on Al-Quaeda, and I have over❗❗ 300 ❗❗ confirmed 💀 kills 💀. I am trained in gorilla 🐒🐒 💥warfare 💥 and I’m the 🔝top🔝 sniper in the entire US 🔫 armed 🔫 forces 👮👮👮. You are nothing to me but just 🎯 another 🎯 target 🎯. I will wipe ❌ you ❌ the ❌ fuck ❌ out with 👀 precision 🎯🎯 the likes of which has never been 👀 seen 👀 before on this 🌎Earth🌎, mark 😡 my 😡 fucking 😡 words 😡. You think you can 👀 get away 👀with saying that shit to me over the Internet 💻? Think again, fucker. 👎👎👎 As we speak I am contacting my 🌐 secret network 🌐 of spies 👀👀👀 across the USA and your IP 🌐 is being traced 🕖 right now 🕧 so you better prepare for the ☔storm ☔, maggot 🐛🐛🐛. The storm ⚡⚡⚡ that wipes out the 😂 pathetic 😂 little thing you call your life. You’re 💀 fucking 💀💀💀 dead 💀💀💀, kid. I can be anywhere 🌎🌏🌍, anytime 🕧🕕🕦, and I can 💀 kill 💀 you💀 in over 🔢 seven 🔢 hundred 🔢 ways 📃, and that’s just with my ✋✋ bare hands✋✋. Not only am I extensively trained in 👊unarmed 👊 combat 👊, but I have access to the entire arsenal 🔫🔫🔫 of the United States 👮👮👮 Marine Corps 👮👮👮 and I will use it to its full extent to 💀 wipe 💀 your 💀 miserable 💀 ass 💀 off the face of the continent 🌎🌎, you little 💩shit 💩. If only you could have known 😵😵 what 😈unholy 😈 retribution your little “clever” comment 📄📄 was about to 👎bring 👎 down 👎 upon you, maybe you would have held your fucking tongue👅👅👅. But you couldn’t ❌, you didn’t❌, and now you’re paying the price💲💲💲, you goddamn idiot. I will 💩 shit 💩 fury 😡😡😡 all over you and you will 😱😱😱 drown 😱😱😱in it. You’re 💀 fucking 💀 dead 💀, kiddo. 💀💀💀");
                    //sends a message to channel with the given text
                });

        _client.GetService<CommandService>().CreateCommand("goodshit") //create command
                .Description("that's some good shit 👌") //add description, it will be shown when *help is used
                .Do(async e =>
                {
                    await e.Channel.SendMessage($"👌👀👌👀👌👀👌👀👌👀 good shit go౦ԁ sHit👌 thats ✔ some good👌👌shit right👌👌there👌👌👌 right✔there ✔✔if i do ƽaү so my self 💯 i say so 💯 thats what im talking about right there right there (chorus: ʳᶦᵍʰᵗ ᵗʰᵉʳᵉ) mMMMMᎷМ💯 👌👌 👌НO0ОଠOOOOOОଠଠOoooᵒᵒᵒᵒᵒᵒᵒᵒᵒ👌 👌👌 👌 💯 👌 👀 👀 👀 👌👌Good shit");
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