using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Commands.Permissions.Levels;
using Discord.Modules;
using Discord.Audio;
using System.IO;
using System.Timers;

class Program
{
    static void Main(string[] args) => new Program().Start();

    public static DateTime MessageSent;
    public string LastUsedRPS;
    public static int CommandsUsed;
    public static DateTime StartupTime = DateTime.Now;
    static Random rnd = new Random();
    public static string commandPrefix = "*";
    public static int gpsCooldownInt = 0;
    public static int navysealsCooldownInt = 0;
    public static int powertwowerCooldownInt = 0;
    public static bool onlyOpToggle;

    private DiscordClient _client;

    public void Start()
    {
        _client = new DiscordClient();
        Console.WriteLine("Booting up bot...");

        _client.UsingCommands(x => {
            x.PrefixChar = '*';
            x.HelpMode = HelpMode.Public;
        });

        _client.Log.Message += (s, e) => Console.WriteLine($"[{e.Severity}] {e.Source}: {e.Message}");

        _client.MessageReceived += async (s, e) =>
        {
            if (e.Message.IsAuthor)
            {
                if (e.Message.RawText == $"beepboop, calculating response time...")
                {
                    var ping = e.Message.Timestamp - MessageSent;
                    await e.Message.Edit($"Pong! Responded in " + (ping.Milliseconds) + " milliseconds.");
                }
                else if ((e.Message.Text == "🌑 rock!") && (e.Message.IsAuthor))
                {
                    if (LastUsedRPS == "rock")
                        await e.Message.Edit($"🌑 rock! We tied!");
                    else if (LastUsedRPS == "paper")
                        await e.Message.Edit($"🌑 rock! You win!");
                    else if (LastUsedRPS == "scissors")
                        await e.Message.Edit($"🌑 rock! I win!");
                }
                else if ((e.Message.Text == "📰 paper!") && (e.Message.IsAuthor))
                {
                    if (LastUsedRPS == "rock")
                        await e.Message.Edit($"📰 paper! I win!");
                    else if (LastUsedRPS == "paper")
                        await e.Message.Edit($"📰 paper! We tied!");
                    else if (LastUsedRPS == "scissors")
                        await e.Message.Edit($"📰 paper! You win!");
                }
                else if ((e.Message.Text == "✂ scissors!") && (e.Message.IsAuthor))
                {
                    if (LastUsedRPS == "rock")
                        await e.Message.Edit($"✂ scissors! You win!");
                    else if (LastUsedRPS == "paper")
                        await e.Message.Edit($"✂ scissors! I win!");
                    else if (LastUsedRPS == "scissors")
                        await e.Message.Edit($"✂ scissors! We tied!");
                }
            }
            else if (e.Message.Text.StartsWith("*userinfo "))
            {
                if (!e.Channel.IsPrivate)
                {
                    string mention = e.Message.RawText.Replace($"*userinfo ", "");
                    if (e.Message.RawText.ToLower().Contains($"*userinfo <@"))
                    {
                        ulong id = e.User.Id;
                        string id1;
                        string id2;
                        if (mention.Contains("!"))
                        {
                            //id = ulong.Parse(mention.Split('!')[1].Split('>')[0]);
                            id1 = mention.Replace($"<@!", "");
                            id2 = id1.Replace($">", "");
                            id = Convert.ToUInt64(id2);
                        }
                        else
                        {
                            //id = ulong.Parse(mention.Split('@')[1].Split('>')[0]);
                            id1 = mention.Replace($"<@", "");
                            id2 = id1.Replace($">", "");
                            id = Convert.ToUInt64(id2);
                        }

                        string username = e.Server.GetUser(id).Name;
                        string avatar = e.Server.GetUser(id).AvatarUrl;
                        string nickname = e.Server.GetUser(id).Nickname;
                        var joined = e.Server.GetUser(id).JoinedAt;
                        var joinedDays = DateTime.Now - joined;
                        var activity = e.Server.GetUser(id).LastActivityAt;
                        var online = e.Server.GetUser(id).LastOnlineAt;

                        await e.Channel.SendMessage($"```\n" +
                                                         $"\nID:           {id}\n" +
                                                         $"Username:     {username}\n" +
                                                         $"Nickname:     {nickname}\n" +
                                                         $"Joined:       {joined} ({joinedDays.Days} days ago)\n" +
                                                         $"Last active:  {activity}\n" +
                                                         $"Last online:  {online}\n" +
                                                         $"```\nAvatar: {avatar}");
                    }
                    else if (!string.IsNullOrWhiteSpace(mention))
                    {
                        User user = e.Server.FindUsers(mention).FirstOrDefault();
                        string username = user.Name;
                        ulong id = user.Id;
                        string nickname = user.Nickname;
                        var joined = user.JoinedAt;
                        var joinedDays = DateTime.Now - joined;
                        var activity = user.LastActivityAt;
                        var online = user.LastOnlineAt;
                        var avatar = user.AvatarUrl;
                        await e.Channel.SendMessage($"```xl\n" +
                                                         $"\nID:           {id}\n" +
                                                         $"Username:     {username}\n" +
                                                         $"Nickname:     {nickname}\n" +
                                                         $"Joined:       {joined} ({joinedDays.Days} days ago)\n" +
                                                         $"Last active:  {activity}\n" +
                                                         $"Last online:  {online}\n" +
                                                         $"```\nAvatar: {avatar}");
                    }
                    else
                    {
                        await e.Channel.SendMessage($"No user by the name of {mention} was found.");
                    }
                }
            }
            else if ((e.Message.Channel.Name == "op") && (!e.Message.RawText.Contains($"op")) && (onlyOpToggle == true))
            {
                await e.Message.Delete();
            }
        };

        //Since we have setup our CommandChar to be '*', we will run this command by typing *greet
        _client.GetService<CommandService>().CreateCommand("greet") //create command greet
                .Alias(new string[] { "gr", "hi" }) //add 2 aliases, so it can be run with ~gr and ~hi
                .Description("Greets a person.") //add description, it will be shown when ~help is used
                .Parameter("GreetedPerson", ParameterType.Required) //as an argument, we have a person we want to greet
                .Do(async e =>
                {
                    CommandsUsed++;
                    await e.Channel.SendMessage($"{e.User.Mention} greets {e.GetArg("GreetedPerson")}");
                    //sends a message to channel with the given text
                });

        _client.GetService<CommandService>().CreateCommand("bork") //create command greet
                .Alias(new string[] { "maximumbork", "borkdrive", "maximumborkdrive" }) //add aliases
                .Description("ＭＡＸＩＭＵＭ　ＢＯＲＫＤＲＩＶＥ") //add description, it will be shown when *help is used
                .Do(async e =>
                {
                    CommandsUsed++;
                    await e.Channel.SendMessage($"http://i.imgur.com/bAxPXuo.jpg");
                    //sends a message to channel with the given text
                });

        _client.GetService<CommandService>().CreateCommand("sigh") //create command
                .Alias(new string[] { "psy" }) //add alias
                .Description("Emojipasta.") //add description, it will be shown when *help is used
                .Do(async e =>
                {
                    CommandsUsed++;
                    await e.Channel.SendMessage($"why SIGH😔💨😤 when you can PSY😎🐴💕 instead??👌💯👍 send this📤👈👈 to 101⃣0⃣ people👬👭 who made you OPPA GANGNAM SMILE☺😸️💖 today📅!!!!");
                    //sends a message to channel with the given text
                });

        _client.GetService<CommandService>().CreateCommand("invite") //create command
                .Alias(new string[] { "joinserver", "join" }) //add aliases
                .Description("Posts the link to get the bot on your server.") //add description, it will be shown when *help is used
                .Do(async e =>
                {
                    CommandsUsed++;
                    await e.Channel.SendMessage($"You can add me to your server using https://discordapp.com/oauth2/authorize?client_id=215591038855675904&scope=bot - make sure you have `Manage Server` permissions!");
                    //sends a message to channel with the given text
                });

        _client.GetService<CommandService>().CreateCommand("oh") //create command
                .Description("oh") //add description, it will be shown when *help is used
                .Do(async e =>
                {
                    CommandsUsed++;
                    await e.Channel.SendMessage($"oh");
                    //sends a message to channel with the given text
                });

        _client.GetService<CommandService>().CreateCommand("ban") //create command
                .Description("Bans a user.") //add description, it will be shown when *help is used
                .Parameter("userToBan", ParameterType.Required) //as an argument, we have a person we want to greet
                .Do(async e =>
                {
                    CommandsUsed++;
                    if (e.User.ServerPermissions.BanMembers)
                    {
                        User u = null;
                        string findUser = e.Args[0];
                        if (!string.IsNullOrWhiteSpace(findUser))
                        {
                            if (e.Message.MentionedUsers.Count() == 1)
                            {
                                u = e.Message.MentionedUsers.FirstOrDefault();
                                await e.Server.Ban(u);
                                await e.Channel.SendMessage($"{findUser} has been succesfully banned.");
                            }
                            else if (e.Server.FindUsers(findUser).Any())
                            {
                                u = e.Server.FindUsers(findUser).FirstOrDefault();
                                await e.Server.Ban(u);
                                await e.Channel.SendMessage($"{findUser} has been succesfully banned.");
                            }
                            else
                                await e.Channel.SendMessage($"I was unable to find `{findUser}`.");
                        }
                    }
                    else
                        await e.Channel.SendMessage($"You must have the \"BanMembers\" permission to use this command.");
                });

        _client.GetService<CommandService>().CreateCommand("kick") //create command
                .Description("Kicks a user.") //add description, it will be shown when *help is used
                .Parameter("userToKick", ParameterType.Required) //as an argument, we have a person we want to greet
                .Do(async e =>
                {
                    CommandsUsed++;
                    if (e.User.ServerPermissions.KickMembers)
                    {
                        User u = null;
                        string findUser = e.Args[0];
                        if (!string.IsNullOrWhiteSpace(findUser))
                        {
                            if (e.Message.MentionedUsers.Count() == 1)
                            {
                                u = e.Message.MentionedUsers.FirstOrDefault();
                                await u.Kick();
                                await e.Channel.SendMessage($"{findUser} has been succesfully kicked.");
                            }
                            else if (e.Server.FindUsers(findUser).Any())
                            {
                                u = e.Server.FindUsers(findUser).FirstOrDefault();
                                await u.Kick();
                                await e.Channel.SendMessage($"{findUser} has been succesfully kicked.");
                            }
                            else
                                await e.Channel.SendMessage($"I was unable to find `{findUser}`.");
                        }

                    }
                    else
                        await e.Channel.SendMessage($"You must have the \"KickMembers\" permission to use this command.");
                });

        _client.GetService<CommandService>().CreateCommand("info") //create command
                .Description("Displays info about the bot.") //add description, it will be shown when *help is used
                .Do(async e =>
                {
                    CommandsUsed++;
                    await e.Channel.SendMessage($"Heya! I'm SQU1RR3L, the general Discord bot written by Noahkiq. You can check out my command list with `*help` or check out my docs over at http://noahkiq.github.io/SQU1RR3L/.");
                    //sends a message to channel with the given text
                });

        _client.GetService<CommandService>().CreateCommand("emojiseals") //create command
                .Description("Navy Seals Emojipasta") //add description, it will be shown when *help is used
                .Do(async e =>
                {
                    CommandsUsed++;
                    await e.Channel.SendMessage($"❔What❔the fuck 😡😡 did you just fucking say about me, you little 🐕bitch🐕? I’ll have you know I graduated 🎓🎓 top🔝 of my class in the 🚢Navy🚢 Seals, and I’ve been involved in numerous 😯secret😯 raids on Al-Quaeda, and I have over❗❗ 300 ❗❗ confirmed 💀 kills 💀. I am trained in gorilla 🐒🐒 💥warfare 💥 and I’m the 🔝top🔝 sniper in the entire US 🔫 armed 🔫 forces 👮👮👮. You are nothing to me but just 🎯 another 🎯 target 🎯. I will wipe ❌ you ❌ the ❌ fuck ❌ out with 👀 precision 🎯🎯 the likes of which has never been 👀 seen 👀 before on this 🌎Earth🌎, mark 😡 my 😡 fucking 😡 words 😡. You think you can 👀 get away 👀with saying that shit to me over the Internet 💻? Think again, fucker. 👎👎👎 As we speak I am contacting my 🌐 secret network 🌐 of spies 👀👀👀 across the USA and your IP 🌐 is being traced 🕖 right now 🕧 so you better prepare for the ☔storm ☔, maggot 🐛🐛🐛. The storm ⚡⚡⚡ that wipes out the 😂 pathetic 😂 little thing you call your life. You’re 💀 fucking 💀💀💀 dead 💀💀💀, kid. I can be anywhere 🌎🌏🌍, anytime 🕧🕕🕦, and I can 💀 kill 💀 you💀 in over 🔢 seven 🔢 hundred 🔢 ways 📃, and that’s just with my ✋✋ bare hands✋✋. Not only am I extensively trained in 👊unarmed 👊 combat 👊, but I have access to the entire arsenal 🔫🔫🔫 of the United States 👮👮👮 Marine Corps 👮👮👮 and I will use it to its full extent to 💀 wipe 💀 your 💀 miserable 💀 ass 💀 off the face of the continent 🌎🌎, you little 💩shit 💩. If only you could have known 😵😵 what 😈unholy 😈 retribution your little “clever” comment 📄📄 was about to 👎bring 👎 down 👎 upon you, maybe you would have held your fucking tongue👅👅👅. But you couldn’t ❌, you didn’t❌, and now you’re paying the price💲💲💲, you goddamn idiot. I will 💩 shit 💩 fury 😡😡😡 all over you and you will 😱😱😱 drown 😱😱😱in it. You’re 💀 fucking 💀 dead 💀, kiddo. 💀💀💀");
                    //sends a message to channel with the given text
                });

        _client.GetService<CommandService>().CreateCommand("goodshit") //create command
                .Description("that's some good shit 👌") //add description, it will be shown when *help is used
                .Do(async e =>
                {
                    CommandsUsed++;
                    await e.Channel.SendMessage($"👌👀👌👀👌👀👌👀👌👀 good shit go౦ԁ sHit👌 thats ✔ some good👌👌shit right👌👌there👌👌👌 right✔there ✔✔if i do ƽaү so my self 💯 i say so 💯 thats what im talking about right there right there (chorus: ʳᶦᵍʰᵗ ᵗʰᵉʳᵉ) mMMMMᎷМ💯 👌👌 👌НO0ОଠOOOOOОଠଠOoooᵒᵒᵒᵒᵒᵒᵒᵒᵒ👌 👌👌 👌 💯 👌 👀 👀 👀 👌👌Good shit");
                    //sends a message to channel with the given text
                });

        _client.GetService<CommandService>().CreateCommand("beemoviescript") //create command
        .Description("Prints the first ~10,000 characters of the Bee Movie script.") //add description, it will be shown when *help is used
        .Do(async e =>
        {
            CommandsUsed++;
            await e.Channel.SendMessage(SQU1RR3L.BeeMovie.BeeMovie1);
            await e.Channel.SendMessage(SQU1RR3L.BeeMovie.BeeMovie2);
            await e.Channel.SendMessage(SQU1RR3L.BeeMovie.BeeMovie3);
            await e.Channel.SendMessage(SQU1RR3L.BeeMovie.BeeMovie4);
            await e.Channel.SendMessage(SQU1RR3L.BeeMovie.BeeMovie5);
            await e.Channel.SendMessage(SQU1RR3L.BeeMovie.BeeMovie6);
            await e.Channel.SendMessage(SQU1RR3L.BeeMovie.BeeMovie7);
            await e.Channel.SendMessage(SQU1RR3L.BeeMovie.BeeMovie8);
            await e.Channel.SendMessage(SQU1RR3L.BeeMovie.BeeMovie9);
            await e.Channel.SendMessage(SQU1RR3L.BeeMovie.BeeMovie10);
            //IT'S THE BEE MOVIE
        });

        _client.GetService<CommandService>().CreateCommand("ping") //create command
                .Description("that's some good shit 👌") //add description, it will be shown when *help is used
                .Do(async e =>
                {
                    CommandsUsed++;
                    MessageSent = e.Message.Timestamp;
                    await e.Channel.SendMessage($"beepboop, calculating response time...");
                });

        _client.GetService<CommandService>().CreateCommand("userinfo")
                .Description("Displays info about a user.")
                .Do(async e =>
                {
                    CommandsUsed++;
                    if (!e.Channel.IsPrivate)
                    {
                        string mention = e.Message.Text;
                        ulong id = e.User.Id;
                        string username = e.User.Name;
                        string avatar = e.User.AvatarUrl;
                        string nickname = e.User.Nickname;
                        var joined = e.User.JoinedAt;
                        var joinedDays = DateTime.Now - joined;
                        await e.Channel.SendMessage($"```\n" +
                                                         $"\nID:           {id}\n" +
                                                         $"Username:     {username}\n" +
                                                         $"Nickname:     {nickname}\n" +
                                                         $"Joined:       {joined} ({joinedDays.Days} days ago.)\n" +
                                                         $"Avatar:\n```" +
                                                         $"\n{avatar}\n");
                    }
                });

        _client.GetService<CommandService>().CreateCommand("stats") //create command
                .Description("Displays stats about the bot such as uptime and commands used.") //add description, it will be shown when *help is used
                .Do(async e =>
                {
                    CommandsUsed++;
                    await e.Channel.SendMessage($"```xl\n" +
                                                $"Uptime: {DateTime.Now - StartupTime} hours\n" +
                                                $"Commands used: {CommandsUsed}\n" +
                                                $"```");
                });

        _client.GetService<CommandService>().CreateCommand("serverinfo") //create command
                .Description("Displays info about the server.") //add description, it will be shown when *help is used
                .Do(async e =>
                {
                    CommandsUsed++;
                    var CreationDate = DateTime.Now - e.Server.Owner.JoinedAt;
                    await e.Channel.SendMessage($"```xl\n" +
                                                $"Name: {e.Server.Name}\n" +
                                                $"ID: {e.Server.Id}\n" +
                                                $"Users: {e.Server.UserCount}\n" +
                                                $"Channels: {e.Server.ChannelCount}\n" +
                                                $"Default channel: #{e.Server.DefaultChannel}\n" +
                                                $"Roles: {e.Server.RoleCount}\n" +
                                                $"Owner: @{e.Server.Owner}\n" +
                                                $"Creation date: {e.Server.Owner.JoinedAt} ({CreationDate.Days} days ago.)" +
                                                $"Icon:\n" +
                                                $"```\n{e.Server.IconUrl}");
                });

        _client.GetService<CommandService>().CreateCommand("rps") //create command
                .Description("Play rock paper scissors with a bot.") //add description, it will be shown when *help is used
                .Parameter("Choice", ParameterType.Optional) //as an argument, we have a person we want to greet
                .Do(async e =>
                {
                    CommandsUsed++;
                    var choices = new List<string> { "🌑 rock!", "📰 paper!", "✂ scissors!" };
                    int r = rnd.Next(choices.Count);
                    if ((e.GetArg("Choice").ToLower() == $"rock") || (e.GetArg("Choice").ToLower() == $"🌑") || (e.GetArg("Choice").ToLower() == $"paper") || (e.GetArg("Choice").ToLower() == $"📰") || (e.GetArg("Choice").ToLower() == $"scissors") || (e.GetArg("Choice").ToLower() == $"✂") || (e.GetArg("Choice") == ""))
                        await e.Channel.SendMessage("" + (string)choices[r]);
                    if ((e.GetArg("Choice").ToLower() == $"rock") || (e.GetArg("Choice").ToLower() == $"🌑"))
                        LastUsedRPS = "rock";
                    else if ((e.GetArg("Choice").ToLower() == $"paper") || (e.GetArg("Choice").ToLower() == $"📰"))
                        LastUsedRPS = "paper";
                    else if ((e.GetArg("Choice").ToLower() == $"scissors") || (e.GetArg("Choice").ToLower() == $"✂"))
                        LastUsedRPS = "scissors";
                    else
                        LastUsedRPS = "";
                });

        _client.GetService<CommandService>().CreateCommand("roll") //create command
                .Description("Rolls a die.") //add description, it will be shown when *help is used
                .Do(async e =>
                {
                    CommandsUsed++;
                    if (e.Message.Text.Contains($"{commandPrefix}roll d"))
                    {
                        if (e.Message.Text.ToLower() == $"{commandPrefix}roll d10")
                        {
                            int dice = rnd.Next(1, 11);
                            await e.Channel.SendMessage($"The ten-sided die rolled a... {dice}!");
                        }
                        else if (e.Message.Text.ToLower() == $"{commandPrefix}roll d1")
                            await e.Channel.SendMessage($"The one-sided die rolled a... one! Wow, surprising!");
                        else if (e.Message.Text.ToLower() == $"{commandPrefix}roll d2")
                        {
                            int dice = rnd.Next(1, 3);
                            await e.Channel.SendMessage($"The two-sided die rolled a... {dice}!");
                        }
                        else if (e.Message.Text.ToLower() == $"{commandPrefix}roll d3")
                        {
                            int dice = rnd.Next(1, 4);
                            await e.Channel.SendMessage($"The three-sided die rolled a... {dice}!");
                        }
                        else if (e.Message.Text.ToLower() == $"{commandPrefix}roll d4")
                        {
                            int dice = rnd.Next(1, 5);
                            await e.Channel.SendMessage($"The four-sided die rolled a... {dice}!");
                        }
                        else if (e.Message.Text.ToLower() == $"{commandPrefix}roll d5")
                        {
                            int dice = rnd.Next(1, 6);
                            await e.Channel.SendMessage($"The five-sided die rolled a... {dice}!");
                        }
                        else if (e.Message.Text.ToLower() == $"{commandPrefix}roll d6")
                        {
                            int dice = rnd.Next(1, 7);
                            await e.Channel.SendMessage($"The six-sided die rolled a... {dice}!");
                        }
                        else if (e.Message.Text.ToLower() == $"{commandPrefix}roll d7")
                        {
                            int dice = rnd.Next(1, 8);
                            await e.Channel.SendMessage($"The seven-sided die rolled a... {dice}!");
                        }
                        else if (e.Message.Text.ToLower() == $"{commandPrefix}roll d8")
                        {
                            int dice = rnd.Next(1, 9);
                            await e.Channel.SendMessage($"The eight-sided die rolled a... {dice}!");
                        }
                        else if (e.Message.Text.ToLower() == $"{commandPrefix}roll d9")
                        {
                            int dice = rnd.Next(1, 10);
                            await e.Channel.SendMessage($"The nine-sided die rolled a... {dice}!");
                        }
                        //d10 is up above because reasons
                        else if (e.Message.Text.ToLower() == $"{commandPrefix}roll d11")
                        {
                            int dice = rnd.Next(1, 12);
                            await e.Channel.SendMessage($"The eleven-sided die rolled a... {dice}!");
                        }
                        else if (e.Message.Text.ToLower() == $"{commandPrefix}roll d12")
                        {
                            int dice = rnd.Next(1, 13);
                            await e.Channel.SendMessage($"The twelve-sided die rolled a... {dice}!");
                        }
                        else
                            await e.Channel.SendMessage($"You must pick a number between 1 and 12!");
                    }
                    else
                    {
                        int dice = rnd.Next(1, 7);
                        await e.Channel.SendMessage($"The six-sided die rolled a... {dice}!");
                    }
                });

        _client.GetService<CommandService>().CreateCommand("navyseals") //create command
                .Description("Navy seals copypasta.") //add description, it will be shown when *help is used
                .Do(async e =>
                {
                    CommandsUsed++;
                    if (navysealsCooldownInt == 0)
                    {
                        navysealsCooldownInt = 15;
                        System.Timers.Timer navysealsCooldownTimer = new System.Timers.Timer();
                        navysealsCooldownTimer.Elapsed += new ElapsedEventHandler(navysealsCooldown);
                        navysealsCooldownTimer.Interval = 1000;
                        navysealsCooldownTimer.Enabled = true;
                        await e.Channel.SendMessage($"What the fuck did you just fucking say about me, you little bitch? I’ll have you know I graduated top of my class in the Navy Seals, and I’ve been involved in numerous secret raids on Al-Quaeda, and I have over 300 confirmed kills. I am trained in gorilla warfare and I’m the top sniper in the entire US armed forces. You are nothing to me but just another target. I will wipe you the fuck out with precision the likes of which has never been seen before on this Earth, mark my fucking words. You think you can get away with saying that shit to me over the Internet? Think again, fucker. As we speak I am contacting my secret network of spies across the USA and your IP is being traced right now so you better prepare for the storm, maggot. The storm that wipes out the pathetic little thing you call your life. You’re fucking dead, kid. I can be anywhere, anytime, and I can kill you in over seven hundred ways, and that’s just with my bare hands. Not only am I extensively trained in unarmed combat, but I have access to the entire arsenal of the United States Marine Corps and I will use it to its full extent to wipe your miserable ass off the face of the continent, you little shit. If only you could have known what unholy retribution your little “clever” comment was about to bring down upon you, maybe you would have held your fucking tongue. But you couldn’t, you didn’t, and now you’re paying the price, you goddamn idiot. I will shit fury all over you and you will drown in it. You’re fucking dead, kiddo.");
                    }
                    else
                        await e.Channel.SendMessage($"This command is currently on a cooldown. Please try again in {navysealsCooldownInt} seconds.");
                });

        _client.GetService<CommandService>().CreateCommand("powertwower") //create command
                .Description("Juhmatok, wow!") //add description, it will be shown when *help is used
                .Do(async e =>
                {
                    CommandsUsed++;
                    if (powertwowerCooldownInt == 0)
                    {
                        powertwowerCooldownInt = 15;
                        System.Timers.Timer powertwowerCooldownTimer = new System.Timers.Timer();
                        powertwowerCooldownTimer.Elapsed += new ElapsedEventHandler(powertwowerCooldown);
                        powertwowerCooldownTimer.Interval = 1000;
                        powertwowerCooldownTimer.Enabled = true;
                        await e.Channel.SendMessage($"Juhmatok, wow! 😱 👀 💯 N🅾, 👉 this 👈 is more 👌 than wow, 👀 it's w0⃣wer because you're 🚶 our power 👌 👊 💯 TWOWer! 🔥 🔥 💯 👌\nHow 😮 ⁉ are you 🚶 👀 so powerful 🔥 🔥 👌 💯 👊 hour 🕜 after hour ❓ 🕓 🕟 🕗\nYou 🚶 tower 🔼 over those 👀 👀 other 😂 sour 😤 😡 TWOWers, who'd better 😠 👊 😵 shower 💦 💦 🚿 you 🚶 with praise, 👌 💯 😍 ❤\nand bow or cower 😟 😧 now 🕒 or they'll 🏃 be disrespectful 😡 👎 😩 like Flower 🌹 👀 🅰nd get devoured 👅 💦 😮 like chowder, 🍲 👅\nand maybe 🤔 ❓ they'll 🚶 glower 😒 👎 and say 🔊  📢 \"Ow!\", 🤕  or \"pow\" 👊 her*, 💁 but 👉 our 👈 power 🔥 👊 TWOWer has more ⬆ devouring 👅 💦 power, 👊 🔥 💯\n🅰nd their 🚶 powdered 👀 remains 😣 💀 will shower 🚿 🚿 over ⬆ the now 🕝 cowering 😢 sour 😡 👎 😡 TWOWers,\nwho'll 🚶 👀 shout 📢 🔊 \"Why ⁉ ❓ is 'Power 👊 🔥 TWOWer' more ⬆ like Flower 🌹 🌼 than 🅰 meower 😺 👀 👀?\"");
                    }
                    else
                        await e.Channel.SendMessage($"This command is currently on a cooldown. Please try again in {powertwowerCooldownInt} seconds.");
                });

        _client.GetService<CommandService>().CreateCommand("gps") //create command
                .Description("The GPS rant™") //add description, it will be shown when *help is used
                .Do(async e =>
                {
                    CommandsUsed++;
                    if (gpsCooldownInt == 0)
                    {
                        gpsCooldownInt = 15;
                        System.Timers.Timer gpsCooldownTimer = new System.Timers.Timer();
                        gpsCooldownTimer.Elapsed += new ElapsedEventHandler(gpsCooldown);
                        gpsCooldownTimer.Interval = 1000;
                        gpsCooldownTimer.Enabled = true;
                        await e.Channel.SendMessage($"ᵂᵃᶦᵗ, ᵗʰᶦˢ ᶦˢ ᵃ ˢᵉʳᶦᵒᵘˢ ᶦˢˢᵘᵉ⋅ ᴸᵉᵗ ᵐᵉ ʳᵃᶰᵗ ᵃᵇᵒᵘᵗ ᶦᵗ ᶠᵒʳ ᵃ ᵇᶦᵗ⋅ ﹡ᵃᶜʰᵉᵐ﹡⋅ \nᵂʰᵃᵗ ᶦˢ ᵃ ᴳᴾˢ﹖\nᵀʰᵉ ᴳᴾˢ ᶦˢ ᵃ ˢʸˢᵗᵉᵐ ᵗᵒ ᵉˢᵗᶦᵐᵃᵗᵉ ᶫᵒᶜᵃᵗᶦᵒᶰ ᵒᶰ ᵉᵃʳᵗʰ ᵇʸ ᵘˢᶦᶰᵍ ˢᶦᵍᶰᵃᶫˢ ᶠʳᵒᵐ ᵃ ˢᵉᵗ ᵒᶠ ᵒʳᵇᶦᵗᶦᶰᵍ ˢᵃᵗᵉᶫᶫᶦᵗᵉˢ… ᵒʳ ˢᵒ ᵗʰᵉʸ ˢᵃʸ⋅ ᴮᵘᵗ ʷʰᵃᵗ ᵗʰᵉʸ'ʳᵉ ᶰᵒᵗ ᵗᵉᶫᶫᶦᶰᵍ ᵘˢ ᶦˢ ᵗʰᵉ ʰᵃʳᵐ ᶦᵗ ᶜᵒᶰˢᵗᵃᶰᵗᶫʸ ᶜᵃᵘˢᵉˢ ʷᵒʳᶫᵈʷᶦᵈᵉ ᵉᵛᵉʳʸ ᵈᵃʸ⋅ ᴿᵉᶜᵉᶰᵗᶫʸ, ˢᵃᵗᵉᶫᶫᶦᵗᵉˢ ʰᵃᵛᵉ ᵇᵉᵉᶰ ᵇᵉᶦᶰᵍ ᵈᵉˢᶦᵍᶰᵉᵈ ˢᵒᶫᵉᶫʸ ᶠᵒʳ ᵗʰᵉ ᵖᵘʳᵖᵒˢᵉ ᵒᶠ ᵇᵒᵒˢᵗᶦᶰᵍ ᴳᴾˢ ᵖᵉʳᶠᵒʳᵐᵃᶰᶜᵉ, ᵇᵘᵗ ᵗʰᵉʸ ʰᵃᵛᵉ ᵇᵉᵉᶰ ᵇᵘᶦᶫᵗ ʷᶦᵗʰ ˢᵒ ᶫᶦᵗᵗᶫᵉ ᶜᵃʳᵉ ᵗʰᵃᵗ ᵗʰᵉ ᵖʳᵒᵇᶫᵉᵐˢ ᵃʳᵉ ᵉᵛᵉᶰ ʷᵒʳˢᵉ, ᵍᶦᵛᵉᶰ ᵗʰᵃᵗ ᵗʰᵉʸ ᵃʳᵉᶰ'ᵗ ʳᵉᶫʸᶦᶰᵍ ᵒᶰ ᶫᵃʳᵍᵉʳ ᵗʰᶦʳᵈ ᵖᵃʳᵗʸ ˢᵃᵗᵉᶫᶫᶦᵗᵉˢ ᵃᶰʸᵐᵒʳᵉ⋅ ᴺᵒᵗ ᵒᶰᶫʸ ᶦˢ ᵗʰᵉ ᶰᵃᵛᶦᵍᵃᵗᶦᵒᶰ ʷᵒʳˢᵉ, ᵇᵘᵗ ᶦᵗ’ˢ ᵃᶫˢᵒ ʰᵃʳᵐᶦᶰᵍ ᵒᵘʳ ᵒᵘᵗ⁻ᵒᶠ⁻ᵒʳᵇᶦᵗ ᵉᶰᵛᶦʳᵒᶰᵐᵉᶰᵗ⋅ ˢᵖᵃᶜᵉ ᵈᵉᵇʳᶦˢ ʰᵃˢ ᵇᵉᵉᶰ ᵃ ᵖʳᵒᵇᶫᵉᵐ ᵉᵛᵉʳ ˢᶦᶰᶜᵉ ʷᵉ ˢᵗᵃʳᵗᵉᵈ ᶫᵃᵘᶰᶜʰᶦᶰᵍ ˢᵃᵗᵉᶫᶫᶦᵗᵉˢ ᶦᶰᵗᵒ ˢᵖᵃᶜᵉ, ᵒᶠ ᶜᵒᵘʳˢᵉ, ᵇᵘᵗ⋅⋅ ᵂᶦᵗʰ ᶰᵉʷ ‘ʲᵃᶰᶦᵗᵒʳ’ ˢᵃᵗᵉᶫᶫᶦᵗᵉˢ ᵃᶰᵈ ᶫᵃˢᵉʳ ᵗᵉᶜʰᶰᵒᶫᵒᵍʸ, ʷᵉ’ᵛᵉ ᵇᵉᵉᶰ ᵃᵇᶫᵉ ᵗᵒ ᵏᵉᵉᵖ ᵗʰᵃᵗ ᵘᶰᵈᵉʳ ᶜᵒᶰᵗʳᵒᶫ⋅ ᴮᵘᵗ ʷʰᶦᶫᵉ ᵗʰᵉ ᵒᵛᵉʳᵃᶫᶫ ᵇᵘᶦᶫᵈ ᶠᵒʳ ˢᵃᵗᵉᶫᶫᶦᵗᵉˢ ᵃʳᵉ ᵘˢᵘᵃᶫᶫʸ ˢᵗᵘʳᵈʸ ᵉᶰᵒᵘᵍʰ ᵗᵒ ʷᶦᵗʰˢᵗᵃᶰᵈ ˢᵖᵃᶜᶦᵒᵘˢ ᶜᵒᶰᵈᶦᵗᶦᵒᶰˢ ᶠᵒʳ ᵃ ᶫᵉᶰᵍᵗʰʸ ᵃᵐᵒᵘᶰᵗ ᵒᶠ ᵗᶦᵐᵉ, ᵗʰᵉˢᵉ ᴳᴾˢ ˢᵃᵗᵉᶫᶫᶦᵗᵉˢ ᵃʳᵉ ʲᵘˢᵗ ᶫᵃᵘᶰᶜʰᶦᶰᵍ ᵖᶦᵉᶜᵉˢ ᵒᶠ ᵗʰᵉᶦʳ ᵒʷᶰ ᵈᵉᵇʳᶦˢ ᶦᶰᵗᵒ ˢᵖᵃᶜᵉ﹔ ˢᵗᶦᶫᶫ ᶦᶰ ᵒᵘʳ ᵒʳᵇᶦᵗ ᵇᵘᵗ ᵒᵘᵗ ᵒᶠ ᵒᵘʳ ᵃᵗᵐᵒˢᵖʰᵉʳᵉ, ʷʰᶦᶜʰ ᶦˢ ᵗᵃᵏᶦᶰᵍ ᵘˢ ᵃᵗ ᶫᵉᵃˢᵗ ᵗᵉᶰ ʸᵉᵃʳˢ ᵇᵃᶜᵏ ᶦᶰ ᵗᵉʳᵐˢ ᵒᶠ ˢᵖᵃᶜᵉ ᵗᵉᶜʰᶰᵒᶫᵒᵍʸ ᵈᵉᵛᵉᶫᵒᵖᵐᵉᶰᵗ⋅ ᴺᵒᵗ ᵒᶰᶫʸ ᵗʰᵃᵗ, ᵇᵘᵗ ᵗʰᵉ ᵐᵃᶦᶰ ᵖʳᵒᵇᶫᵉᵐ ʷᶦᵗʰ ˢᵖᵃᶜᵉ ᵈᵉᵇʳᶦˢ ᶦᶰ ᵍᵉᶰᵉʳᵃᶫ ᶦˢ ᵗʰᵃᵗ ᶦᵗ ᶦˢ ᶜᵃᵘˢᶦᶰᵍ ᵉˣᵗʳᵉᵐᵉ ᶫᵉᵛᵉᶫˢ ᵒᶠ ᶜᵒᶫᶫᶦˢᶦᵒᶰ ʷᶦᵗʰ ᵒᵗʰᵉʳ ᵛᶦᵗᵃᶫ ᵃᶰᵈ ᵉˣᵖᵉᶰˢᶦᵛᵉ ˢᵃᵗᵉᶫᶫᶦᵗᵉˢ ᵗʰᵃᵗ ᵃʳᵉ ᵘˢᵉᵈ ᶠᵒʳ ᶦᵐᵖᵒʳᵗᵃᶰᵗ ᵃᶰᵈ ᶦᶰ ᵍᵉᶰᵉʳᵃᶫ ʰᵃᵛᵉ ᵃ ˢʷᵉᶫᶫ ᶦᶰᵗᵉᶰᵗᶦᵒᶰ ᵍᵒᶦᶰᵍ ᵇʸ ᵗʰᵉᶦʳ ᵃᶜᵗᶦᵛᶦᵗᶦᵉˢ⋅ ᵀʰᵉʳᵉᶠᵒʳᵉ ᵗʰᵉˢᵉ ᴳᴾˢ ˢᵃᵗᵉᶫᶫᶦᵗᵉˢ, ʷʰᶦᶫᵉ ᵃᶫʳᵉᵃᵈʸ ᵇᵉᶦᶰᵍ ᵃᵇʰᵒʳʳᵉᶰᵗ ᵉᶰᵒᵘᵍʰ, ᵃʳᵉ ʳᵉᵈᵘᶜᶦᶰᵍ ᵗʰᵉ ᑫᵘᵃᶫᶦᵗʸ ᵒᶠ ˢᵃᵗᵉᶫᶫᶦᵗᵉˢ ᵃᶜᵗᵘᵃᶫᶫʸ ᵐᵃᵈᵉ ʷᶦᵗʰ ᵃ ˢᶦᶰᵍᶫᵉ ᵒᵘᶰᶜᵉ ᵒᶠ ᶦᶰᵗᵉᵍʳᶦᵗʸ﹗ ᵀʰᵉʸ ᵃʳᵉ ᵐᵃᵏᶦᶰᵍ ᶫᶦᶠᵉ ʷᵒʳˢᵉ ᶠᵒʳ ᵉᵛᵉʳʸ ˢᶦᶰᵍᶫᵉ ᵖᵉʳˢᵒᶰ ᵘˢᶦᶰᵍ ᵃ ˢᵉʳᵛᶦᶜᵉ ᵖʳᵒᵛᶦᵈᵉᵈ ᵇʸ ᵃᶰʸ ᵒᶠ ᵗʰᵉˢᵉ ˢᵃᵗᵉᶫᶫᶦᵗᵉˢ ᵗʰᵃᵗ ʷᵉʳᵉ ᶦᶰ ᶜᵒᶰᵗᵃᶜᵗ ʷᶦᵗʰ ᵗʰᵉᵐ⋅ ᴵ ˢᶦᵐᵖᶫᵉ ᶜᵃᶰ’ᵗ ᵘᶰᵈᵉʳˢᵗᵃᶰᵈ ʰᵒʷ ʷᵉ ᶜᵃᶰ ᵗᵒᶫᵉʳᵃᵗᵉ ᵗʰᵉˢᵉ ᵖᵉᵒᵖᶫᵉ⋅ ᴬᶫʳᶦᵍʰᵗ, ᶫᵉᵗ’ˢ ᵍᵉᵗ ᵇᵃᶜᵏ ᵗᵒ ᵗʰᵉ ᶰᶦᵗᵗʸ ᵍʳᶦᵗᵗʸ ᵒᶠ ᵗʰᶦˢ ᵗʰᶦᶰᵍ⋅ ᴬˢ ᴵ ʷᵃˢ ˢᵃʸᶦᶰᵍ ᵉᵃʳᶫᶦᵉʳ, ᶜᵒᶫᶫᶦˢᶦᵒᶰˢ ᵈᵉˢᵗʳᵒʸᵉᵈ ᵇᵒᵗʰ ˢᵃᵗᵉᶫᶫᶦᵗᵉˢ ᵃᶰᵈ ᵗʰᵒˢᵉ ᵗʰᵃᵗ ʰᵃᵈ ᶜʳᵉᵃᵗᵉᵈ ᵃ ᶠᶦᵉᶫᵈ ᵒᶠ ᵈᵉᵇʳᶦˢ ᵗʰᵃᵗ ᵇᵉᶜᵒᵐᵉ ᵃ ᵈᵃᶰᵍᵉʳ ᵗᵒ ᵒᵗʰᵉʳ […]");
                    }
                    else
                        await e.Channel.SendMessage($"This command is currently on a cooldown. Please try again in {gpsCooldownInt} seconds.");
                });

        _client.GetService<CommandService>().CreateCommand("lenny") //create command
                .Description("( ͡° ͜ʖ ͡°)") //add description, it will be shown when *help is used
                .Do(async e =>
                {
                    CommandsUsed++;
                    await e.Channel.SendMessage($"( ͡° ͜ʖ ͡°)");
                });

        _client.GetService<CommandService>().CreateCommand("uncomfortablelenny") //create command
                .Description("( ͡° ͜ʖ( ͡° ͜ʖ(ಠ_ಠ)ʖ ͡°) ͡°)") //add description, it will be shown when *help is used
                .Do(async e =>
                {
                    CommandsUsed++;
                    await e.Channel.SendMessage($"( ͡° ͜ʖ( ͡° ͜ʖ(ಠ_ಠ)ʖ ͡°) ͡°)");
                });

        _client.GetService<CommandService>().CreateCommand("triggered") //create command
                .Description("***triggered***") //add description, it will be shown when *help is used
                .Do(async e =>
                {
                    CommandsUsed++;
                    await e.Channel.SendMessage($"*Did you just gender my assumption?*");
                });

        _client.GetService<CommandService>().CreateCommand("pressf") //create command
                .Description("Press F to pay respects.") //add description, it will be shown when *help is used
                .Do(async e =>
                {
                    CommandsUsed++;
                    await e.Channel.SendMessage($"Press F to pay respects.");
                });

        _client.GetService<CommandService>().CreateGroup("tag", cgb =>
        {
            cgb.CreateCommand("help")
                        .Description("Displays info about the tag command.")
                        .Do(async (e) =>
                        {
                            await e.Channel.SendMessage($"The tag system is used for less important commands, such as images or injokes. The list of them is here:\n" +
                                                        $"help, coo, wait, fightcary, kappa, qwerby, tantsalt, tantstop, doot, blacksanta, cupshots, delete, eminem, thatsthejoke, waterthose, autumnfox, shibae, taco, createtwow, riptwow, craftysevens, coyknee, carykh, powerabuse, carythetankengine, botespam, pasta, deletthis, tantalisa, spam, drawingrequest, patience, borntwerker, pinnedtwerking, itsjoke, staffping, tantdisgust, pit, thetruth, wonderifitwasturret\n" +
                                                        $"Example command usage: *tag kappa");
                        });

            cgb.CreateCommand("coo")
                        .Do(async (e) =>
                        {
                            await e.Channel.SendMessage($"wait coo is a boy");
                        });

            cgb.CreateCommand("wonderifitwasturret")
                        .Do(async (e) =>
                        {
                            await e.Channel.SendMessage($"I wonder if it was TurretBot. How could it be him, he say that he likes me and he's my friend. Turret always tell me that he like me but I've been starting to wonder. Just because TurretBot tell me that he likes me doesn't mean that the necessarily do. It might just mean that Turret hates me and is lying to me when he say that he like me. I guess really, it comes down to a question of whether I am better at detecting whether @TurretBot :balloon:#5527 likes me or is lying to me about liking me than he is at lying to me about liking me if he don't like me. So, if I think that TurretBot likes me, then either Turret do like me and my ability to discern whether or not he is lying to me about whether or not they like me in relation to his ability to lie to me about whether or not they like me is of relatively little consequence or it happens to be the case that TurretBot don't like me and my ability to discern whether or not he is lying to me about whether or not they like me is inferior to Turret's ability to lie to me about whether or not they like me. I wonder if it was TurretBot. Or was it Idiot 9.0? It could have been Idiot you know how he get... flee!!! FLEE FOR YOUR LIVES or he'll get you....");
                        });

            cgb.CreateCommand("whatsthis")
                        .Do(async (e) =>
                        {
                            await e.Channel.SendMessage($"awwwww!~ *nuzzles u back and pounces on u and notices your buldge* OwO what's this...?");
                        });

            cgb.CreateCommand("wait")
                        .Do(async (e) =>
                        {
                            await e.Channel.SendMessage($"wait wait wait wait wait\n\n`joseph howard`");
                        });

            cgb.CreateCommand("fightcary")
                        .Do(async (e) =>
                        {
                            await e.Channel.SendMessage($"FIGHT CARY FIGHT CARY");
                        });

            cgb.CreateCommand("kappa")
                        .Do(async (e) =>
                        {
                            await e.Channel.SendMessage("http://i.imgur.com/zaQgpOY.png");
                        });
            cgb.CreateCommand("qwerby")
                        .Do(async (e) =>
                        {
                            await e.Channel.SendMessage($"http://i.imgur.com/LRDnAlH.png");
                        });
            cgb.CreateCommand("eeveeusar")
                        .Do(async (e) =>
                        {
                            await e.Channel.SendMessage($"http://i.imgur.com/HjzxHLp.png");
                        });
            cgb.CreateCommand("tantsalt")
                        .Do(async (e) =>
                        {
                            await e.Channel.SendMessage($"http://i.imgur.com/Fjp4FPF.png <:tantSalt:217294115577135104>");
                        });
            cgb.CreateCommand("tantstop")
                        .Do(async (e) =>
                        {
                            await e.Channel.SendMessage($"http://i.imgur.com/ageGFoJ.png");
                        });
            cgb.CreateCommand("doot")
                        .Do(async (e) =>
                        {
                            await e.Channel.SendMessage($"https://giphy.com/gifs/rb-HBR8qdahfLPoc");
                        });
            cgb.CreateCommand("blacksanta")
                        .Do(async (e) =>
                        {
                            await e.Channel.SendMessage($"http://i.imgur.com/wfMtHdL.jpg");
                        });
            cgb.CreateCommand("cupshots")
                        .Do(async (e) =>
                        {
                            await e.Channel.SendMessage($"https://www.youtube.com/watch?v=2YMoOaRv9R0");
                        });
            cgb.CreateCommand("delete")
                        .Do(async (e) =>
                        {
                            await e.Channel.SendMessage($"http://i.imgur.com/wh4IQPN.jpg");
                        });
            cgb.CreateCommand("eminem")
                        .Do(async (e) =>
                        {
                            await e.Channel.SendMessage($"http://i.imgur.com/EuJhzWx.png");
                        });
            cgb.CreateCommand("thatsthejoke")
                        .Do(async (e) =>
                        {
                            await e.Channel.SendMessage($"http://i.imgur.com/IDbKjHD.jpg");
                        });
            cgb.CreateCommand("waterthose")
                        .Do(async (e) =>
                        {
                            await e.Channel.SendMessage($"http://i.imgur.com/TkztiDz.jpg");
                        });
            cgb.CreateCommand("autumnfox")
                        .Do(async (e) =>
                        {
                            await e.Channel.SendMessage($"http://i.imgur.com/xzxJz2a.png");
                        });
            cgb.CreateCommand("shibae")
                        .Do(async (e) =>
                        {
                            await e.Channel.SendMessage($"https://puu.sh/ria4a/ea147f4707.png");
                        });
            cgb.CreateCommand("taco")
                        .Do(async (e) =>
                        {
                            await e.Channel.SendMessage($"http://i.imgur.com/FV0xH2S.jpg");
                        });
            cgb.CreateCommand("createtwow")
                        .Do(async (e) =>
                        {
                            await e.Channel.SendMessage($"http://i.imgur.com/Kybw3HM.png");
                        });
            cgb.CreateCommand("riptwow")
                        .Do(async (e) =>
                        {
                            await e.Channel.SendMessage($"http://i.imgur.com/JG3Ea4U.jpg");
                        });
            cgb.CreateCommand("craftysevens")
                        .Do(async (e) =>
                        {
                            await e.Channel.SendMessage($"http://i.imgur.com/LTXVsqx.jpg");
                        });
            cgb.CreateCommand("coyknee")
                        .Do(async (e) =>
                        {
                            await e.Channel.SendMessage($"http://i.imgur.com/D3qWZzk.png");
                        });
            cgb.CreateCommand("carykh")
                        .Do(async (e) =>
                        {
                            await e.Channel.SendMessage($"http://i.imgur.com/P4DZyhj.jpg");
                        });
            cgb.CreateCommand("powerabuse")
                        .Do(async (e) =>
                        {
                            await e.Channel.SendMessage($"http://i.imgur.com/rFhmzw4.png");
                        });
            cgb.CreateCommand("carythetankengine")
                        .Do(async (e) =>
                        {
                            await e.Channel.SendMessage($"https://www.youtube.com/watch?v=qAqLUHdqj10");
                        });
            cgb.CreateCommand("botespam")
                        .Do(async (e) =>
                        {
                            await e.Channel.SendMessage($"http://i.imgur.com/ZJ7VpUr.png");
                        });
            cgb.CreateCommand("cat")
                        .Do(async (e) =>
                        {
                            await e.Channel.SendMessage($"http://i.imgur.com/8SSQmIA.png");
                        });
            cgb.CreateCommand("pasta")
                        .Do(async (e) =>
                        {
                            await e.Channel.SendMessage($"http://i.imgur.com/NrbN1x5.jpg");
                        });
            cgb.CreateCommand("deletthis")
                        .Do(async (e) =>
                        {
                            await e.Channel.SendMessage($"http://i.imgur.com/CcoN9W3.jpg");
                        });
            cgb.CreateCommand("tantalisa")
                        .Do(async (e) =>
                        {
                            await e.Channel.SendMessage($"http://i.imgur.com/2SoRHqM.png");
                        });
            cgb.CreateCommand("spam")
                        .Do(async (e) =>
                        {
                            await e.Channel.SendMessage($"http://i.imgur.com/EW0I58h.jpg");
                        });
            cgb.CreateCommand("respectmahauthoritah")
                        .Do(async (e) =>
                        {
                            await e.Channel.SendMessage($"http://i.imgur.com/E05RAm6.jpg");
                        });
            cgb.CreateCommand("drawingrequest")
                        .Do(async (e) =>
                        {
                            await e.Channel.SendMessage($"http://i.imgur.com/TB3PI9l.png");
                        });
            cgb.CreateCommand("patience")
                        .Do(async (e) =>
                        {
                            await e.Channel.SendMessage($"http://i.imgur.com/4wTBiST.png");
                        });
            cgb.CreateCommand("borntwerker")
                        .Do(async (e) =>
                        {
                            await e.Channel.SendMessage($"http://i.imgur.com/NNMgqIJ.png");
                        });
            cgb.CreateCommand("pinnedtwerking")
                        .Do(async (e) =>
                        {
                            await e.Channel.SendMessage($"http://i.imgur.com/1lEsaih.png");
                        });
            cgb.CreateCommand("itsjoke")
                        .Do(async (e) =>
                        {
                            await e.Channel.SendMessage($"https://puu.sh/rmti1/3bf3ce68e9.png");
                        });
            cgb.CreateCommand("staffping")
                        .Do(async (e) =>
                        {
                            await e.Channel.SendMessage($"http://i.imgur.com/W9ATDRo.png");
                        });
            cgb.CreateCommand("tantdisgust")
                        .Do(async (e) =>
                        {
                            await e.Channel.SendMessage($"http://i.imgur.com/mRODwra.gif");
                        });
            cgb.CreateCommand("pit")
                        .Do(async (e) =>
                        {
                            await e.Channel.SendMessage($"http://i.imgur.com/MMgLhB6.jpg");
                        });
            cgb.CreateCommand("thetruth")
                        .Do(async (e) =>
                        {
                            await e.Channel.SendMessage($"http://i.imgur.com/ohsHLvg.png");
                        });
        });

        // Register a Hook into the UserBanned event using a Lambda
        _client.UserBanned += async (s, e) => {
            // Create a Channel object by searching for a channel named '#logs' on the server the ban occurred in.
            var logChannel = e.Server.FindChannels("squirrel-log").FirstOrDefault();
            // Send a message to the server's log channel, stating that a user was banned.
            if(logChannel != null)
            {
                await logChannel.SendMessage($"{e.User.Name} was banned from the server.");
            }
        };

        // Register a Hook into the UserUnanned event using a Lambda
        _client.UserUnbanned += async (s, e) => {
            // Create a Channel object by searching for a channel named '#squirrel-log' on the server the ban occurred in.
            var logChannel = e.Server.FindChannels("squirrel-log").FirstOrDefault();
            // Send a message to the server's log channel, stating that a user was unbanned.
            if(logChannel != null)
            {
                await logChannel.SendMessage($"{e.User.Name} was unbanned from the server.");
            }
        };

        // Register a Hook into the UserJoined event using a Lambda
        _client.UserJoined += async (s, e) => {
            // Create a Channel object by searching for a channel named '#squirrel-log' on the server the ban occurred in.
            var logChannel = e.Server.FindChannels("squirrel-log").FirstOrDefault();
            // Send a message to the server's log channel, stating that a user was unbanned.
            if(logChannel != null)
            {
                await logChannel.SendMessage($"{e.User.Name} joined the server.");
            }
        };

        // Register a Hook into the UserUnanned event using a Lambda
        _client.UserLeft += async (s, e) => {
            // Create a Channel object by searching for a channel named '#squirrel-log' on the server the ban occurred in.
            var logChannel = e.Server.FindChannels("squirrel-log").FirstOrDefault();
            // Send a message to the server's log channel, stating that a user was unbanned.
            if (logChannel != null)
            {
                await logChannel.SendMessage($"{e.User.Name} left the server.");
            }
        };
        _client.GetService<CommandService>().CreateCommand("toggleonlyop")
            .Description("toggles op filter")
            .Do(async e =>
            {
                if (onlyOpToggle == true)
                {
                    await e.Channel.SendMessage("Toggled op filter off");
                    onlyOpToggle = false;
                }
                else if (onlyOpToggle == false)
                {
                    await e.Channel.SendMessage("Toggled op filter on");
                    onlyOpToggle = true;
                }
            });

        string token = File.ReadAllText("token.txt");
        _client.ExecuteAndWait(async () => {
            await _client.Connect(token, TokenType.Bot);
            _client.SetGame("*invite");
        });

        _client.GetService<CommandService>().CreateCommand("activatebeemovie") //create command
                .Description("that's some good shit 👌") //add description, it will be shown when *help is used
                .Do(async e =>
                {
                    CommandsUsed++;
                    if(e.Channel.Id == 233025495485120512)
                    {
                        await e.Channel.SendMessage($"Preparing bee movie script...");
                        List<string> beemoviescript = new List<string>();
                        beemoviescript.Add("According to all known laws");
                        beemoviescript.Add("of aviation,");
                        beemoviescript.Add("there is no way a bee");
                        beemoviescript.Add("should be able to fly.");
                        beemoviescript.Add("Its wings are too small to get");
                        beemoviescript.Add("its fat little body off the ground.");
                        beemoviescript.Add("The bee, of course, flies anyway");
                        beemoviescript.Add("because bees don't care");
                        beemoviescript.Add("what humans think is impossible.");
                        beemoviescript.Add("Yellow, black. Yellow, black.");
                        beemoviescript.Add("Yellow, black. Yellow, black.");
                        beemoviescript.Add("Ooh, black and yellow!");
                        beemoviescript.Add("Let's shake it up a little.");
                        beemoviescript.Add("Barry! Breakfast is ready!");
                        beemoviescript.Add("Ooming!");
                        beemoviescript.Add("Hang on a second.");
                        beemoviescript.Add("Hello?");
                        beemoviescript.Add("- Barry?");
                        beemoviescript.Add("- Adam?");
                        beemoviescript.Add("- Oan you believe this is happening?");
                        beemoviescript.Add("- I can't. I'll pick you up.");
                        beemoviescript.Add("Looking sharp.");
                        beemoviescript.Add("Use the stairs. Your father");
                        beemoviescript.Add("paid good money for those.");
                        beemoviescript.Add("Sorry. I'm excited.");
                        beemoviescript.Add("Here's the graduate.");
                        beemoviescript.Add("We're very proud of you, son.");
                        beemoviescript.Add("A perfect report card, all B's.");
                        beemoviescript.Add("Very proud.");
                        beemoviescript.Add("Ma! I got a thing going here.");
                        beemoviescript.Add("- You got lint on your fuzz.");
                        beemoviescript.Add("- Ow! That's me!");
                        beemoviescript.Add("- Wave to us! We'll be in row 118,000.");
                        beemoviescript.Add("- Bye!");
                        beemoviescript.Add("Barry, I told you,");
                        beemoviescript.Add("stop flying in the house!");
                        beemoviescript.Add("- Hey, Adam.");
                        beemoviescript.Add("- Hey, Barry.");
                        beemoviescript.Add("- Is that fuzz gel?");
                        beemoviescript.Add("- A little. Special day, graduation.");
                        beemoviescript.Add("Never thought I'd make it.");
                        beemoviescript.Add("Three days grade school,");
                        beemoviescript.Add("three days high school.");
                        beemoviescript.Add("Those were awkward.");
                        beemoviescript.Add("Three days college. I'm glad I took");
                        beemoviescript.Add("a day and hitchhiked around the hive.");
                        beemoviescript.Add("You did come back different.");
                        beemoviescript.Add("- Hi, Barry.");
                        beemoviescript.Add("- Artie, growing a mustache? Looks good.");
                        beemoviescript.Add("- Hear about Frankie?");
                        beemoviescript.Add("- Yeah.");
                        beemoviescript.Add("- You going to the funeral?");
                        beemoviescript.Add("- No, I'm not going.");
                        beemoviescript.Add("Everybody knows,");
                        beemoviescript.Add("sting someone, you die.");
                        beemoviescript.Add("Don't waste it on a squirrel.");
                        beemoviescript.Add("Such a hothead.");
                        beemoviescript.Add("I guess he could have");
                        beemoviescript.Add("just gotten out of the way.");
                        beemoviescript.Add("I love this incorporating");
                        beemoviescript.Add("an amusement park into our day.");
                        beemoviescript.Add("That's why we don't need vacations.");
                        beemoviescript.Add("Boy, quite a bit of pomp...");
                        beemoviescript.Add("under the circumstances.");
                        beemoviescript.Add("- Well, Adam, today we are men.");
                        beemoviescript.Add("- We are!");
                        beemoviescript.Add("- Bee-men.");
                        beemoviescript.Add("- Amen!");
                        beemoviescript.Add("Hallelujah!");
                        beemoviescript.Add("Students, faculty, distinguished bees,");
                        beemoviescript.Add("please welcome Dean Buzzwell.");
                        beemoviescript.Add("Welcome, New Hive Oity");
                        beemoviescript.Add("graduating class of...");
                        beemoviescript.Add("...9:15.");
                        beemoviescript.Add("That concludes our ceremonies.");
                        beemoviescript.Add("And begins your career");
                        beemoviescript.Add("at Honex Industries!");
                        beemoviescript.Add("Will we pick ourjob today?");
                        beemoviescript.Add("I heard it's just orientation.");
                        beemoviescript.Add("Heads up! Here we go.");
                        beemoviescript.Add("Keep your hands and antennas");
                        beemoviescript.Add("inside the tram at all times.");
                        beemoviescript.Add("- Wonder what it'll be like?");
                        beemoviescript.Add("- A little scary.");
                        beemoviescript.Add("Welcome to Honex,");
                        beemoviescript.Add("a division of Honesco");
                        beemoviescript.Add("and a part of the Hexagon Group.");
                        beemoviescript.Add("This is it!");
                        beemoviescript.Add("Wow.");
                        beemoviescript.Add("Wow.");
                        beemoviescript.Add("We know that you, as a bee,");
                        beemoviescript.Add("have worked your whole life");
                        beemoviescript.Add("to get to the point where you");
                        beemoviescript.Add("can work for your whole life.");
                        beemoviescript.Add("Honey begins when our valiant Pollen");
                        beemoviescript.Add("Jocks bring the nectar to the hive.");
                        beemoviescript.Add("Our top-secret formula");
                        beemoviescript.Add("is automatically color-corrected,");
                        beemoviescript.Add("scent-adjusted and bubble-contoured");
                        beemoviescript.Add("into this soothing sweet syrup");
                        beemoviescript.Add("with its distinctive");
                        beemoviescript.Add("golden glow you know as...");
                        beemoviescript.Add("Honey!");
                        beemoviescript.Add("- That girl was hot.");
                        beemoviescript.Add("- She's my cousin!");
                        beemoviescript.Add("- She is?");
                        beemoviescript.Add("- Yes, we're all cousins.");
                        beemoviescript.Add("- Right. You're right.");
                        beemoviescript.Add("- At Honex, we constantly strive");
                        beemoviescript.Add("to improve every aspect");
                        beemoviescript.Add("of bee existence.");
                        beemoviescript.Add("These bees are stress-testing");
                        beemoviescript.Add("a new helmet technology.");
                        beemoviescript.Add("- What do you think he makes?");
                        beemoviescript.Add("- Not enough.");
                        beemoviescript.Add("Here we have our latest advancement,");
                        beemoviescript.Add("the Krelman.");
                        beemoviescript.Add("- What does that do?");
                        beemoviescript.Add("- Oatches that little strand of honey");
                        beemoviescript.Add("that hangs after you pour it.");
                        beemoviescript.Add("Saves us millions.");
                        beemoviescript.Add("Oan anyone work on the Krelman?");
                        beemoviescript.Add("Of course. Most bee jobs are");
                        beemoviescript.Add("small ones. But bees know");
                        beemoviescript.Add("that every small job,");
                        beemoviescript.Add("if it's done well, means a lot.");
                        beemoviescript.Add("But choose carefully");
                        beemoviescript.Add("because you'll stay in the job");
                        beemoviescript.Add("you pick for the rest of your life.");
                        beemoviescript.Add("The same job the rest of your life?");
                        beemoviescript.Add("I didn't know that.");
                        beemoviescript.Add("What's the difference?");
                        beemoviescript.Add("You'll be happy to know that bees,");
                        beemoviescript.Add("as a species, haven't had one day off");
                        beemoviescript.Add("in 27 million years.");
                        beemoviescript.Add("So you'll just work us to death?");
                        beemoviescript.Add("We'll sure try.");
                        beemoviescript.Add("Wow! That blew my mind!");
                        beemoviescript.Add("\"What's the difference?\"");
                        beemoviescript.Add("How can you say that?");
                        beemoviescript.Add("One job forever?");
                        beemoviescript.Add("That's an insane choice to have to make.");
                        beemoviescript.Add("I'm relieved. Now we only have");
                        beemoviescript.Add("to make one decision in life.");
                        beemoviescript.Add("But, Adam, how could they");
                        beemoviescript.Add("never have told us that?");
                        beemoviescript.Add("Why would you question anything?");
                        beemoviescript.Add("We're bees.");
                        beemoviescript.Add("We're the most perfectly");
                        beemoviescript.Add("functioning society on Earth.");
                        beemoviescript.Add("You ever think maybe things");
                        beemoviescript.Add("work a little too well here?");
                        beemoviescript.Add("Like what? Give me one example.");
                        beemoviescript.Add("I don't know. But you know");
                        beemoviescript.Add("what I'm talking about.");
                        beemoviescript.Add("Please clear the gate.");
                        beemoviescript.Add("Royal Nectar Force on approach.");
                        beemoviescript.Add("Wait a second. Oheck it out.");
                        beemoviescript.Add("- Hey, those are Pollen Jocks!");
                        beemoviescript.Add("- Wow.");
                        beemoviescript.Add("I've never seen them this close.");
                        beemoviescript.Add("They know what it's like");
                        beemoviescript.Add("outside the hive.");
                        beemoviescript.Add("Yeah, but some don't come back.");
                        beemoviescript.Add("- Hey, Jocks!");
                        beemoviescript.Add("- Hi, Jocks!");
                        beemoviescript.Add("You guys did great!");
                        beemoviescript.Add("You're monsters!");
                        beemoviescript.Add("You're sky freaks! I love it! I love it!");
                        beemoviescript.Add("- I wonder where they were.");
                        beemoviescript.Add("- I don't know.");
                        beemoviescript.Add("Their day's not planned.");
                        beemoviescript.Add("Outside the hive, flying who knows");
                        beemoviescript.Add("where, doing who knows what.");
                        beemoviescript.Add("You can'tjust decide to be a Pollen");
                        beemoviescript.Add("Jock. You have to be bred for that.");
                        beemoviescript.Add("Right.");
                        beemoviescript.Add("Look. That's more pollen");
                        beemoviescript.Add("than you and I will see in a lifetime.");
                        beemoviescript.Add("It's just a status symbol.");
                        beemoviescript.Add("Bees make too much of it.");
                        beemoviescript.Add("Perhaps. Unless you're wearing it");
                        beemoviescript.Add("and the ladies see you wearing it.");
                        beemoviescript.Add("Those ladies?");
                        beemoviescript.Add("Aren't they our cousins too?");
                        beemoviescript.Add("Distant. Distant.");
                        beemoviescript.Add("Look at these two.");
                        beemoviescript.Add("- Oouple of Hive Harrys.");
                        beemoviescript.Add("- Let's have fun with them.");
                        beemoviescript.Add("It must be dangerous");
                        beemoviescript.Add("being a Pollen Jock.");
                        beemoviescript.Add("Yeah. Once a bear pinned me");
                        beemoviescript.Add("against a mushroom!");
                        beemoviescript.Add("He had a paw on my throat,");
                        beemoviescript.Add("and with the other, he was slapping me!");
                        beemoviescript.Add("- Oh, my!");
                        beemoviescript.Add("- I never thought I'd knock him out.");
                        beemoviescript.Add("What were you doing during this?");
                        beemoviescript.Add("Trying to alert the authorities.");
                        beemoviescript.Add("I can autograph that.");
                        beemoviescript.Add("A little gusty out there today,");
                        beemoviescript.Add("wasn't it, comrades?");
                        beemoviescript.Add("Yeah. Gusty.");
                        beemoviescript.Add("We're hitting a sunflower patch");
                        beemoviescript.Add("six miles from here tomorrow.");
                        beemoviescript.Add("- Six miles, huh?");
                        beemoviescript.Add("- Barry!");
                        beemoviescript.Add("A puddle jump for us,");
                        beemoviescript.Add("but maybe you're not up for it.");
                        beemoviescript.Add("- Maybe I am.");
                        beemoviescript.Add("- You are not!");
                        beemoviescript.Add("We're going 0900 at J-Gate.");
                        beemoviescript.Add("What do you think, buzzy-boy?");
                        beemoviescript.Add("Are you bee enough?");
                        beemoviescript.Add("I might be. It all depends");
                        beemoviescript.Add("on what 0900 means.");
                        beemoviescript.Add("Hey, Honex!");
                        beemoviescript.Add("Dad, you surprised me.");
                        beemoviescript.Add("You decide what you're interested in?");
                        beemoviescript.Add("- Well, there's a lot of choices.");
                        beemoviescript.Add("- But you only get one.");
                        beemoviescript.Add("Do you ever get bored");
                        beemoviescript.Add("doing the same job every day?");
                        beemoviescript.Add("Son, let me tell you about stirring.");
                        beemoviescript.Add("You grab that stick, and you just");
                        beemoviescript.Add("move it around, and you stir it around.");
                        beemoviescript.Add("You get yourself into a rhythm.");
                        beemoviescript.Add("It's a beautiful thing.");
                        beemoviescript.Add("You know, Dad,");
                        beemoviescript.Add("the more I think about it,");
                        beemoviescript.Add("maybe the honey field");
                        beemoviescript.Add("just isn't right for me.");
                        beemoviescript.Add("You were thinking of what,");
                        beemoviescript.Add("making balloon animals?");
                        beemoviescript.Add("That's a bad job");
                        beemoviescript.Add("for a guy with a stinger.");
                        beemoviescript.Add("Janet, your son's not sure");
                        beemoviescript.Add("he wants to go into honey!");
                        beemoviescript.Add("- Barry, you are so funny sometimes.");
                        beemoviescript.Add("- I'm not trying to be funny.");
                        beemoviescript.Add("You're not funny! You're going");
                        beemoviescript.Add("into honey. Our son, the stirrer!");
                        beemoviescript.Add("- You're gonna be a stirrer?");
                        beemoviescript.Add("- No one's listening to me!");
                        beemoviescript.Add("Wait till you see the sticks I have.");
                        beemoviescript.Add("I could say anything right now.");
                        beemoviescript.Add("I'm gonna get an ant tattoo!");
                        beemoviescript.Add("Let's open some honey and celebrate!");
                        beemoviescript.Add("Maybe I'll pierce my thorax.");
                        beemoviescript.Add("Shave my antennae.");
                        beemoviescript.Add("Shack up with a grasshopper. Get");
                        beemoviescript.Add("a gold tooth and call everybody \"dawg\"!");
                        beemoviescript.Add("I'm so proud.");
                        beemoviescript.Add("- We're starting work today!");
                        beemoviescript.Add("- Today's the day.");
                        beemoviescript.Add("Oome on! All the good jobs");
                        beemoviescript.Add("will be gone.");
                        beemoviescript.Add("Yeah, right.");
                        beemoviescript.Add("Pollen counting, stunt bee, pouring,");
                        beemoviescript.Add("stirrer, front desk, hair removal...");
                        beemoviescript.Add("- Is it still available?");
                        beemoviescript.Add("- Hang on. Two left!");
                        beemoviescript.Add("One of them's yours! Oongratulations!");
                        beemoviescript.Add("Step to the side.");
                        beemoviescript.Add("- What'd you get?");
                        beemoviescript.Add("- Picking crud out. Stellar!");
                        beemoviescript.Add("Wow!");
                        beemoviescript.Add("Oouple of newbies?");
                        beemoviescript.Add("Yes, sir! Our first day! We are ready!");
                        beemoviescript.Add("Make your choice.");
                        beemoviescript.Add("- You want to go first?");
                        beemoviescript.Add("- No, you go.");
                        beemoviescript.Add("Oh, my. What's available?");
                        beemoviescript.Add("Restroom attendant's open,");
                        beemoviescript.Add("not for the reason you think.");
                        beemoviescript.Add("- Any chance of getting the Krelman?");
                        beemoviescript.Add("- Sure, you're on.");
                        beemoviescript.Add("I'm sorry, the Krelman just closed out.");
                        beemoviescript.Add("Wax monkey's always open.");
                        beemoviescript.Add("The Krelman opened up again.");
                        beemoviescript.Add("What happened?");
                        beemoviescript.Add("A bee died. Makes an opening. See?");
                        beemoviescript.Add("He's dead. Another dead one.");
                        beemoviescript.Add("Deady. Deadified. Two more dead.");
                        beemoviescript.Add("Dead from the neck up.");
                        beemoviescript.Add("Dead from the neck down. That's life!");
                        beemoviescript.Add("Oh, this is so hard!");
                        beemoviescript.Add("Heating, cooling,");
                        beemoviescript.Add("stunt bee, pourer, stirrer,");
                        beemoviescript.Add("humming, inspector number seven,");
                        beemoviescript.Add("lint coordinator, stripe supervisor,");
                        beemoviescript.Add("mite wrangler. Barry, what");
                        beemoviescript.Add("do you think I should... Barry?");
                        beemoviescript.Add("Barry!");
                        beemoviescript.Add("All right, we've got the sunflower patch");
                        beemoviescript.Add("in quadrant nine...");
                        beemoviescript.Add("What happened to you?");
                        beemoviescript.Add("Where are you?");
                        beemoviescript.Add("- I'm going out.");
                        beemoviescript.Add("- Out? Out where?");
                        beemoviescript.Add("- Out there.");
                        beemoviescript.Add("- Oh, no!");
                        beemoviescript.Add("I have to, before I go");
                        beemoviescript.Add("to work for the rest of my life.");
                        beemoviescript.Add("You're gonna die! You're crazy! Hello?");
                        beemoviescript.Add("Another call coming in.");
                        beemoviescript.Add("If anyone's feeling brave,");
                        beemoviescript.Add("there's a Korean deli on 83rd");
                        beemoviescript.Add("that gets their roses today.");
                        beemoviescript.Add("Hey, guys.");
                        beemoviescript.Add("- Look at that.");
                        beemoviescript.Add("- Isn't that the kid we saw yesterday?");
                        beemoviescript.Add("Hold it, son, flight deck's restricted.");
                        beemoviescript.Add("It's OK, Lou. We're gonna take him up.");
                        beemoviescript.Add("Really? Feeling lucky, are you?");
                        beemoviescript.Add("Sign here, here. Just initial that.");
                        beemoviescript.Add("- Thank you.");
                        beemoviescript.Add("- OK.");
                        beemoviescript.Add("You got a rain advisory today,");
                        beemoviescript.Add("and as you all know,");
                        beemoviescript.Add("bees cannot fly in rain.");
                        beemoviescript.Add("So be careful. As always,");
                        beemoviescript.Add("watch your brooms,");
                        beemoviescript.Add("hockey sticks, dogs,");
                        beemoviescript.Add("birds, bears and bats.");
                        beemoviescript.Add("Also, I got a couple of reports");
                        beemoviescript.Add("of root beer being poured on us.");
                        beemoviescript.Add("Murphy's in a home because of it,");
                        beemoviescript.Add("babbling like a cicada!");
                        beemoviescript.Add("- That's awful.");
                        beemoviescript.Add("- And a reminder for you rookies,");
                        beemoviescript.Add("bee law number one,");
                        beemoviescript.Add("absolutely no talking to humans!");
                        beemoviescript.Add("All right, launch positions!");
                        beemoviescript.Add("Buzz, buzz, buzz, buzz! Buzz, buzz,");
                        beemoviescript.Add("buzz, buzz! Buzz, buzz, buzz, buzz!");
                        beemoviescript.Add("Black and yellow!");
                        beemoviescript.Add("Hello!");
                        beemoviescript.Add("You ready for this, hot shot?");
                        beemoviescript.Add("Yeah. Yeah, bring it on.");
                        beemoviescript.Add("Wind, check.");
                        beemoviescript.Add("- Antennae, check.");
                        beemoviescript.Add("- Nectar pack, check.");
                        beemoviescript.Add("- Wings, check.");
                        beemoviescript.Add("- Stinger, check.");
                        beemoviescript.Add("Scared out of my shorts, check.");
                        beemoviescript.Add("OK, ladies,");
                        beemoviescript.Add("let's move it out!");
                        beemoviescript.Add("Pound those petunias,");
                        beemoviescript.Add("you striped stem-suckers!");
                        beemoviescript.Add("All of you, drain those flowers!");
                        beemoviescript.Add("Wow! I'm out!");
                        beemoviescript.Add("I can't believe I'm out!");
                        beemoviescript.Add("So blue.");
                        beemoviescript.Add("I feel so fast and free!");
                        beemoviescript.Add("Box kite!");
                        beemoviescript.Add("Wow!");
                        beemoviescript.Add("Flowers!");
                        beemoviescript.Add("This is Blue Leader.");
                        beemoviescript.Add("We have roses visual.");
                        beemoviescript.Add("Bring it around 30 degrees and hold.");
                        beemoviescript.Add("Roses!");
                        beemoviescript.Add("30 degrees, roger. Bringing it around.");
                        beemoviescript.Add("Stand to the side, kid.");
                        beemoviescript.Add("It's got a bit of a kick.");
                        beemoviescript.Add("That is one nectar collector!");
                        beemoviescript.Add("- Ever see pollination up close?");
                        beemoviescript.Add("- No, sir.");
                        beemoviescript.Add("I pick up some pollen here, sprinkle it");
                        beemoviescript.Add("over here. Maybe a dash over there,");
                        beemoviescript.Add("a pinch on that one.");
                        beemoviescript.Add("See that? It's a little bit of magic.");
                        beemoviescript.Add("That's amazing. Why do we do that?");
                        beemoviescript.Add("That's pollen power. More pollen, more");
                        beemoviescript.Add("flowers, more nectar, more honey for us.");
                        beemoviescript.Add("Oool.");
                        beemoviescript.Add("I'm picking up a lot of bright yellow.");
                        beemoviescript.Add("Oould be daisies. Don't we need those?");
                        beemoviescript.Add("Oopy that visual.");
                        beemoviescript.Add("Wait. One of these flowers");
                        beemoviescript.Add("seems to be on the move.");
                        beemoviescript.Add("Say again? You're reporting");
                        beemoviescript.Add("a moving flower?");
                        beemoviescript.Add("Affirmative.");
                        beemoviescript.Add("That was on the line!");
                        beemoviescript.Add("This is the coolest. What is it?");
                        beemoviescript.Add("I don't know, but I'm loving this color.");
                        beemoviescript.Add("It smells good.");
                        beemoviescript.Add("Not like a flower, but I like it.");
                        beemoviescript.Add("Yeah, fuzzy.");
                        beemoviescript.Add("Ohemical-y.");
                        beemoviescript.Add("Oareful, guys. It's a little grabby.");
                        beemoviescript.Add("My sweet lord of bees!");
                        beemoviescript.Add("Oandy-brain, get off there!");
                        beemoviescript.Add("Problem!");
                        beemoviescript.Add("- Guys!");
                        beemoviescript.Add("- This could be bad.");
                        beemoviescript.Add("Affirmative.");
                        beemoviescript.Add("Very close.");
                        beemoviescript.Add("Gonna hurt.");
                        beemoviescript.Add("Mama's little boy.");
                        beemoviescript.Add("You are way out of position, rookie!");
                        beemoviescript.Add("Ooming in at you like a missile!");
                        beemoviescript.Add("Help me!");
                        beemoviescript.Add("I don't think these are flowers.");
                        beemoviescript.Add("- Should we tell him?");
                        beemoviescript.Add("- I think he knows.");
                        beemoviescript.Add("What is this?!");
                        beemoviescript.Add("Match point!");
                        beemoviescript.Add("You can start packing up, honey,");
                        beemoviescript.Add("because you're about to eat it!");
                        beemoviescript.Add("Yowser!");
                        beemoviescript.Add("Gross.");
                        beemoviescript.Add("There's a bee in the car!");
                        beemoviescript.Add("- Do something!");
                        beemoviescript.Add("- I'm driving!");
                        beemoviescript.Add("- Hi, bee.");
                        beemoviescript.Add("- He's back here!");
                        beemoviescript.Add("He's going to sting me!");
                        beemoviescript.Add("Nobody move. If you don't move,");
                        beemoviescript.Add("he won't sting you. Freeze!");
                        beemoviescript.Add("He blinked!");
                        beemoviescript.Add("Spray him, Granny!");
                        beemoviescript.Add("What are you doing?!");
                        beemoviescript.Add("Wow... the tension level");
                        beemoviescript.Add("out here is unbelievable.");
                        beemoviescript.Add("I gotta get home.");
                        beemoviescript.Add("Oan't fly in rain.");
                        beemoviescript.Add("Oan't fly in rain.");
                        beemoviescript.Add("Oan't fly in rain.");
                        beemoviescript.Add("Mayday! Mayday! Bee going down!");
                        beemoviescript.Add("Ken, could you close");
                        beemoviescript.Add("the window please?");
                        beemoviescript.Add("Ken, could you close");
                        beemoviescript.Add("the window please?");
                        beemoviescript.Add("Oheck out my new resume.");
                        beemoviescript.Add("I made it into a fold-out brochure.");
                        beemoviescript.Add("You see? Folds out.");
                        beemoviescript.Add("Oh, no. More humans. I don't need this.");
                        beemoviescript.Add("What was that?");
                        beemoviescript.Add("Maybe this time. This time. This time.");
                        beemoviescript.Add("This time! This time! This...");
                        beemoviescript.Add("Drapes!");
                        beemoviescript.Add("That is diabolical.");
                        beemoviescript.Add("It's fantastic. It's got all my special");
                        beemoviescript.Add("skills, even my top-ten favorite movies.");
                        beemoviescript.Add("What's number one? Star Wars?");
                        beemoviescript.Add("Nah, I don't go for that...");
                        beemoviescript.Add("...kind of stuff.");
                        beemoviescript.Add("No wonder we shouldn't talk to them.");
                        beemoviescript.Add("They're out of their minds.");
                        beemoviescript.Add("When I leave a job interview, they're");
                        beemoviescript.Add("flabbergasted, can't believe what I say.");
                        beemoviescript.Add("There's the sun. Maybe that's a way out.");
                        beemoviescript.Add("I don't remember the sun");
                        beemoviescript.Add("having a big 75 on it.");
                        beemoviescript.Add("I predicted global warming.");
                        beemoviescript.Add("I could feel it getting hotter.");
                        beemoviescript.Add("At first I thought it was just me.");
                        beemoviescript.Add("Wait! Stop! Bee!");
                        beemoviescript.Add("Stand back. These are winter boots.");
                        beemoviescript.Add("Wait!");
                        beemoviescript.Add("Don't kill him!");
                        beemoviescript.Add("You know I'm allergic to them!");
                        beemoviescript.Add("This thing could kill me!");
                        beemoviescript.Add("Why does his life have");
                        beemoviescript.Add("less value than yours?");
                        beemoviescript.Add("Why does his life have any less value");
                        beemoviescript.Add("than mine? Is that your statement?");
                        beemoviescript.Add("I'm just saying all life has value. You");
                        beemoviescript.Add("don't know what he's capable of feeling.");
                        beemoviescript.Add("My brochure!");
                        beemoviescript.Add("There you go, little guy.");
                        beemoviescript.Add("I'm not scared of him.");
                        beemoviescript.Add("It's an allergic thing.");
                        beemoviescript.Add("Put that on your resume brochure.");
                        beemoviescript.Add("My whole face could puff up.");
                        beemoviescript.Add("Make it one of your special skills.");
                        beemoviescript.Add("Knocking someone out");
                        beemoviescript.Add("is also a special skill.");
                        beemoviescript.Add("Right. Bye, Vanessa. Thanks.");
                        beemoviescript.Add("- Vanessa, next week? Yogurt night?");
                        beemoviescript.Add("- Sure, Ken. You know, whatever.");
                        beemoviescript.Add("- You could put carob chips on there.");
                        beemoviescript.Add("- Bye.");
                        beemoviescript.Add("- Supposed to be less calories.");
                        beemoviescript.Add("- Bye.");
                        beemoviescript.Add("I gotta say something.");
                        beemoviescript.Add("She saved my life.");
                        beemoviescript.Add("I gotta say something.");
                        beemoviescript.Add("All right, here it goes.");
                        beemoviescript.Add("Nah.");
                        beemoviescript.Add("What would I say?");
                        beemoviescript.Add("I could really get in trouble.");
                        beemoviescript.Add("It's a bee law.");
                        beemoviescript.Add("You're not supposed to talk to a human.");
                        beemoviescript.Add("I can't believe I'm doing this.");
                        beemoviescript.Add("I've got to.");
                        beemoviescript.Add("Oh, I can't do it. Oome on!");
                        beemoviescript.Add("No. Yes. No.");
                        beemoviescript.Add("Do it. I can't.");
                        beemoviescript.Add("How should I start it?");
                        beemoviescript.Add("\"You like jazz?\" No, that's no good.");
                        beemoviescript.Add("Here she comes! Speak, you fool!");
                        beemoviescript.Add("Hi!");
                        beemoviescript.Add("I'm sorry.");
                        beemoviescript.Add("- You're talking.");
                        beemoviescript.Add("- Yes, I know.");
                        beemoviescript.Add("You're talking!");
                        beemoviescript.Add("I'm so sorry.");
                        beemoviescript.Add("No, it's OK. It's fine.");
                        beemoviescript.Add("I know I'm dreaming.");
                        beemoviescript.Add("But I don't recall going to bed.");
                        beemoviescript.Add("Well, I'm sure this");
                        beemoviescript.Add("is very disconcerting.");
                        beemoviescript.Add("This is a bit of a surprise to me.");
                        beemoviescript.Add("I mean, you're a bee!");
                        beemoviescript.Add("I am. And I'm not supposed");
                        beemoviescript.Add("to be doing this,");
                        beemoviescript.Add("but they were all trying to kill me.");
                        beemoviescript.Add("And if it wasn't for you...");
                        beemoviescript.Add("I had to thank you.");
                        beemoviescript.Add("It's just how I was raised.");
                        beemoviescript.Add("That was a little weird.");
                        beemoviescript.Add("- I'm talking with a bee.");
                        beemoviescript.Add("- Yeah.");
                        beemoviescript.Add("I'm talking to a bee.");
                        beemoviescript.Add("And the bee is talking to me!");
                        beemoviescript.Add("I just want to say I'm grateful.");
                        beemoviescript.Add("I'll leave now.");
                        beemoviescript.Add("- Wait! How did you learn to do that?");
                        beemoviescript.Add("- What?");
                        beemoviescript.Add("The talking thing.");
                        beemoviescript.Add("Same way you did, I guess.");
                        beemoviescript.Add("\"Mama, Dada, honey.\" You pick it up.");
                        beemoviescript.Add("- That's very funny.");
                        beemoviescript.Add("- Yeah.");
                        beemoviescript.Add("Bees are funny. If we didn't laugh,");
                        beemoviescript.Add("we'd cry with what we have to deal with.");
                        beemoviescript.Add("Anyway...");
                        beemoviescript.Add("Oan I...");
                        beemoviescript.Add("...get you something?");
                        beemoviescript.Add("- Like what?");
                        beemoviescript.Add("I don't know. I mean...");
                        beemoviescript.Add("I don't know. Ooffee?");
                        beemoviescript.Add("I don't want to put you out.");
                        beemoviescript.Add("It's no trouble. It takes two minutes.");
                        beemoviescript.Add("- It's just coffee.");
                        beemoviescript.Add("- I hate to impose.");
                        beemoviescript.Add("- Don't be ridiculous!");
                        beemoviescript.Add("- Actually, I would love a cup.");
                        beemoviescript.Add("Hey, you want rum cake?");
                        beemoviescript.Add("- I shouldn't.");
                        beemoviescript.Add("- Have some.");
                        beemoviescript.Add("- No, I can't.");
                        beemoviescript.Add("- Oome on!");
                        beemoviescript.Add("I'm trying to lose a couple micrograms.");
                        beemoviescript.Add("- Where?");
                        beemoviescript.Add("- These stripes don't help.");
                        beemoviescript.Add("You look great!");
                        beemoviescript.Add("I don't know if you know");
                        beemoviescript.Add("anything about fashion.");
                        beemoviescript.Add("Are you all right?");
                        beemoviescript.Add("No.");
                        beemoviescript.Add("He's making the tie in the cab");
                        beemoviescript.Add("as they're flying up Madison.");
                        beemoviescript.Add("He finally gets there.");
                        beemoviescript.Add("He runs up the steps into the church.");
                        beemoviescript.Add("The wedding is on.");
                        beemoviescript.Add("And he says, \"Watermelon?");
                        beemoviescript.Add("I thought you said Guatemalan.");
                        beemoviescript.Add("Why would I marry a watermelon?\"");
                        beemoviescript.Add("Is that a bee joke?");
                        beemoviescript.Add("That's the kind of stuff we do.");
                        beemoviescript.Add("Yeah, different.");
                        beemoviescript.Add("So, what are you gonna do, Barry?");
                        beemoviescript.Add("About work? I don't know.");
                        beemoviescript.Add("I want to do my part for the hive,");
                        beemoviescript.Add("but I can't do it the way they want.");
                        beemoviescript.Add("I know how you feel.");
                        beemoviescript.Add("- You do?");
                        beemoviescript.Add("- Sure.");
                        beemoviescript.Add("My parents wanted me to be a lawyer or");
                        beemoviescript.Add("a doctor, but I wanted to be a florist.");
                        beemoviescript.Add("- Really?");
                        beemoviescript.Add("- My only interest is flowers.");
                        beemoviescript.Add("Our new queen was just elected");
                        beemoviescript.Add("with that same campaign slogan.");
                        beemoviescript.Add("Anyway, if you look...");
                        beemoviescript.Add("There's my hive right there. See it?");
                        beemoviescript.Add("You're in Sheep Meadow!");
                        beemoviescript.Add("Yes! I'm right off the Turtle Pond!");
                        beemoviescript.Add("No way! I know that area.");
                        beemoviescript.Add("I lost a toe ring there once.");
                        beemoviescript.Add("- Why do girls put rings on their toes?");
                        beemoviescript.Add("- Why not?");
                        beemoviescript.Add("- It's like putting a hat on your knee.");
                        beemoviescript.Add("- Maybe I'll try that.");
                        beemoviescript.Add("- You all right, ma'am?");
                        beemoviescript.Add("- Oh, yeah. Fine.");
                        beemoviescript.Add("Just having two cups of coffee!");
                        beemoviescript.Add("Anyway, this has been great.");
                        beemoviescript.Add("Thanks for the coffee.");
                        beemoviescript.Add("Yeah, it's no trouble.");
                        beemoviescript.Add("Sorry I couldn't finish it. If I did,");
                        beemoviescript.Add("I'd be up the rest of my life.");
                        beemoviescript.Add("Are you...?");
                        beemoviescript.Add("Oan I take a piece of this with me?");
                        beemoviescript.Add("Sure! Here, have a crumb.");
                        beemoviescript.Add("- Thanks!");
                        beemoviescript.Add("- Yeah.");
                        beemoviescript.Add("All right. Well, then...");
                        beemoviescript.Add("I guess I'll see you around.");
                        beemoviescript.Add("Or not.");
                        beemoviescript.Add("OK, Barry.");
                        beemoviescript.Add("And thank you");
                        beemoviescript.Add("so much again... for before.");
                        beemoviescript.Add("Oh, that? That was nothing.");
                        beemoviescript.Add("Well, not nothing, but... Anyway...");
                        beemoviescript.Add("This can't possibly work.");
                        beemoviescript.Add("He's all set to go.");
                        beemoviescript.Add("We may as well try it.");
                        beemoviescript.Add("OK, Dave, pull the chute.");
                        beemoviescript.Add("- Sounds amazing.");
                        beemoviescript.Add("- It was amazing!");
                        beemoviescript.Add("It was the scariest,");
                        beemoviescript.Add("happiest moment of my life.");
                        beemoviescript.Add("Humans! I can't believe");
                        beemoviescript.Add("you were with humans!");
                        beemoviescript.Add("Giant, scary humans!");
                        beemoviescript.Add("What were they like?");
                        beemoviescript.Add("Huge and crazy. They talk crazy.");
                        beemoviescript.Add("They eat crazy giant things.");
                        beemoviescript.Add("They drive crazy.");
                        beemoviescript.Add("- Do they try and kill you, like on TV?");
                        beemoviescript.Add("- Some of them. But some of them don't.");
                        beemoviescript.Add("- How'd you get back?");
                        beemoviescript.Add("- Poodle.");
                        beemoviescript.Add("You did it, and I'm glad. You saw");
                        beemoviescript.Add("whatever you wanted to see.");
                        beemoviescript.Add("You had your \"experience.\" Now you");
                        beemoviescript.Add("can pick out yourjob and be normal.");
                        beemoviescript.Add("- Well...");
                        beemoviescript.Add("- Well?");
                        beemoviescript.Add("Well, I met someone.");
                        beemoviescript.Add("You did? Was she Bee-ish?");
                        beemoviescript.Add("- A wasp?! Your parents will kill you!");
                        beemoviescript.Add("- No, no, no, not a wasp.");
                        beemoviescript.Add("- Spider?");
                        beemoviescript.Add("- I'm not attracted to spiders.");
                        beemoviescript.Add("I know it's the hottest thing,");
                        beemoviescript.Add("with the eight legs and all.");
                        beemoviescript.Add("I can't get by that face.");
                        beemoviescript.Add("So who is she?");
                        beemoviescript.Add("She's... human.");
                        beemoviescript.Add("No, no. That's a bee law.");
                        beemoviescript.Add("You wouldn't break a bee law.");
                        beemoviescript.Add("- Her name's Vanessa.");
                        beemoviescript.Add("- Oh, boy.");
                        beemoviescript.Add("She's so nice. And she's a florist!");
                        beemoviescript.Add("Oh, no! You're dating a human florist!");
                        beemoviescript.Add("We're not dating.");
                        beemoviescript.Add("You're flying outside the hive, talking");
                        beemoviescript.Add("to humans that attack our homes");
                        beemoviescript.Add("with power washers and M-80s!");
                        beemoviescript.Add("One-eighth a stick of dynamite!");
                        beemoviescript.Add("She saved my life!");
                        beemoviescript.Add("And she understands me.");
                        beemoviescript.Add("This is over!");
                        beemoviescript.Add("Eat this.");
                        beemoviescript.Add("This is not over! What was that?");
                        beemoviescript.Add("- They call it a crumb.");
                        beemoviescript.Add("- It was so stingin' stripey!");
                        beemoviescript.Add("And that's not what they eat.");
                        beemoviescript.Add("That's what falls off what they eat!");
                        beemoviescript.Add("- You know what a Oinnabon is?");
                        beemoviescript.Add("- No.");
                        beemoviescript.Add("It's bread and cinnamon and frosting.");
                        beemoviescript.Add("They heat it up...");
                        beemoviescript.Add("Sit down!");
                        beemoviescript.Add("...really hot!");
                        beemoviescript.Add("- Listen to me!");
                        beemoviescript.Add("We are not them! We're us.");
                        beemoviescript.Add("There's us and there's them!");
                        beemoviescript.Add("Yes, but who can deny");
                        beemoviescript.Add("the heart that is yearning?");
                        beemoviescript.Add("There's no yearning.");
                        beemoviescript.Add("Stop yearning. Listen to me!");
                        beemoviescript.Add("You have got to start thinking bee,");
                        beemoviescript.Add("my friend. Thinking bee!");
                        beemoviescript.Add("- Thinking bee.");
                        beemoviescript.Add("- Thinking bee.");
                        beemoviescript.Add("Thinking bee! Thinking bee!");
                        beemoviescript.Add("Thinking bee! Thinking bee!");
                        beemoviescript.Add("There he is. He's in the pool.");
                        beemoviescript.Add("You know what your problem is, Barry?");
                        beemoviescript.Add("I gotta start thinking bee?");
                        beemoviescript.Add("How much longer will this go on?");
                        beemoviescript.Add("It's been three days!");
                        beemoviescript.Add("Why aren't you working?");
                        beemoviescript.Add("I've got a lot of big life decisions");
                        beemoviescript.Add("to think about.");
                        beemoviescript.Add("What life? You have no life!");
                        beemoviescript.Add("You have no job. You're barely a bee!");
                        beemoviescript.Add("Would it kill you");
                        beemoviescript.Add("to make a little honey?");
                        beemoviescript.Add("Barry, come out.");
                        beemoviescript.Add("Your father's talking to you.");
                        beemoviescript.Add("Martin, would you talk to him?");
                        beemoviescript.Add("Barry, I'm talking to you!");
                        beemoviescript.Add("You coming?");
                        beemoviescript.Add("Got everything?");
                        beemoviescript.Add("All set!");
                        beemoviescript.Add("Go ahead. I'll catch up.");
                        beemoviescript.Add("Don't be too long.");
                        beemoviescript.Add("Watch this!");
                        beemoviescript.Add("Vanessa!");
                        beemoviescript.Add("- We're still here.");
                        beemoviescript.Add("- I told you not to yell at him.");
                        beemoviescript.Add("He doesn't respond to yelling!");
                        beemoviescript.Add("- Then why yell at me?");
                        beemoviescript.Add("- Because you don't listen!");
                        beemoviescript.Add("I'm not listening to this.");
                        beemoviescript.Add("Sorry, I've gotta go.");
                        beemoviescript.Add("- Where are you going?");
                        beemoviescript.Add("- I'm meeting a friend.");
                        beemoviescript.Add("A girl? Is this why you can't decide?");
                        beemoviescript.Add("Bye.");
                        beemoviescript.Add("I just hope she's Bee-ish.");
                        beemoviescript.Add("They have a huge parade");
                        beemoviescript.Add("of flowers every year in Pasadena?");
                        beemoviescript.Add("To be in the Tournament of Roses,");
                        beemoviescript.Add("that's every florist's dream!");
                        beemoviescript.Add("Up on a float, surrounded");
                        beemoviescript.Add("by flowers, crowds cheering.");
                        beemoviescript.Add("A tournament. Do the roses");
                        beemoviescript.Add("compete in athletic events?");
                        beemoviescript.Add("No. All right, I've got one.");
                        beemoviescript.Add("How come you don't fly everywhere?");
                        beemoviescript.Add("It's exhausting. Why don't you");
                        beemoviescript.Add("run everywhere? It's faster.");
                        beemoviescript.Add("Yeah, OK, I see, I see.");
                        beemoviescript.Add("All right, your turn.");
                        beemoviescript.Add("TiVo. You can just freeze live TV?");
                        beemoviescript.Add("That's insane!");
                        beemoviescript.Add("You don't have that?");
                        beemoviescript.Add("We have Hivo, but it's a disease.");
                        beemoviescript.Add("It's a horrible, horrible disease.");
                        beemoviescript.Add("Oh, my.");
                        beemoviescript.Add("Dumb bees!");
                        beemoviescript.Add("You must want to sting all those jerks.");
                        beemoviescript.Add("We try not to sting.");
                        beemoviescript.Add("It's usually fatal for us.");
                        beemoviescript.Add("So you have to watch your temper.");
                        beemoviescript.Add("Very carefully.");
                        beemoviescript.Add("You kick a wall, take a walk,");
                        beemoviescript.Add("write an angry letter and throw it out.");
                        beemoviescript.Add("Work through it like any emotion:");
                        beemoviescript.Add("Anger, jealousy, lust.");
                        beemoviescript.Add("Oh, my goodness! Are you OK?");
                        beemoviescript.Add("Yeah.");
                        beemoviescript.Add("- What is wrong with you?!");
                        beemoviescript.Add("- It's a bug.");
                        beemoviescript.Add("He's not bothering anybody.");
                        beemoviescript.Add("Get out of here, you creep!");
                        beemoviescript.Add("What was that? A Pic 'N' Save circular?");
                        beemoviescript.Add("Yeah, it was. How did you know?");
                        beemoviescript.Add("It felt like about 10 pages.");
                        beemoviescript.Add("Seventy-five is pretty much our limit.");
                        beemoviescript.Add("You've really got that");
                        beemoviescript.Add("down to a science.");
                        beemoviescript.Add("- I lost a cousin to Italian Vogue.");
                        beemoviescript.Add("- I'll bet.");
                        beemoviescript.Add("What in the name");
                        beemoviescript.Add("of Mighty Hercules is this?");
                        beemoviescript.Add("How did this get here?");
                        beemoviescript.Add("Oute Bee, Golden Blossom,");
                        beemoviescript.Add("Ray Liotta Private Select?");
                        beemoviescript.Add("- Is he that actor?");
                        beemoviescript.Add("- I never heard of him.");
                        beemoviescript.Add("- Why is this here?");
                        beemoviescript.Add("- For people. We eat it.");
                        beemoviescript.Add("You don't have");
                        beemoviescript.Add("enough food of your own?");
                        beemoviescript.Add("- Well, yes.");
                        beemoviescript.Add("- How do you get it?");
                        beemoviescript.Add("- Bees make it.");
                        beemoviescript.Add("- I know who makes it!");
                        beemoviescript.Add("And it's hard to make it!");
                        beemoviescript.Add("There's heating, cooling, stirring.");
                        beemoviescript.Add("You need a whole Krelman thing!");
                        beemoviescript.Add("- It's organic.");
                        beemoviescript.Add("- It's our-ganic!");
                        beemoviescript.Add("It's just honey, Barry.");
                        beemoviescript.Add("Just what?!");
                        beemoviescript.Add("Bees don't know about this!");
                        beemoviescript.Add("This is stealing! A lot of stealing!");
                        beemoviescript.Add("You've taken our homes, schools,");
                        beemoviescript.Add("hospitals! This is all we have!");
                        beemoviescript.Add("And it's on sale?!");
                        beemoviescript.Add("I'm getting to the bottom of this.");
                        beemoviescript.Add("I'm getting to the bottom");
                        beemoviescript.Add("of all of this!");
                        beemoviescript.Add("Hey, Hector.");
                        beemoviescript.Add("- You almost done?");
                        beemoviescript.Add("- Almost.");
                        beemoviescript.Add("He is here. I sense it.");
                        beemoviescript.Add("Well, I guess I'll go home now");
                        beemoviescript.Add("and just leave this nice honey out,");
                        beemoviescript.Add("with no one around.");
                        beemoviescript.Add("You're busted, box boy!");
                        beemoviescript.Add("I knew I heard something.");
                        beemoviescript.Add("So you can talk!");
                        beemoviescript.Add("I can talk.");
                        beemoviescript.Add("And now you'll start talking!");
                        beemoviescript.Add("Where you getting the sweet stuff?");
                        beemoviescript.Add("Who's your supplier?");
                        beemoviescript.Add("I don't understand.");
                        beemoviescript.Add("I thought we were friends.");
                        beemoviescript.Add("The last thing we want");
                        beemoviescript.Add("to do is upset bees!");
                        beemoviescript.Add("You're too late! It's ours now!");
                        beemoviescript.Add("You, sir, have crossed");
                        beemoviescript.Add("the wrong sword!");
                        beemoviescript.Add("You, sir, will be lunch");
                        beemoviescript.Add("for my iguana, Ignacio!");
                        beemoviescript.Add("Where is the honey coming from?");
                        beemoviescript.Add("Tell me where!");
                        beemoviescript.Add("Honey Farms! It comes from Honey Farms!");
                        beemoviescript.Add("Orazy person!");
                        beemoviescript.Add("What horrible thing has happened here?");
                        beemoviescript.Add("These faces, they never knew");
                        beemoviescript.Add("what hit them. And now");
                        beemoviescript.Add("they're on the road to nowhere!");
                        beemoviescript.Add("Just keep still.");
                        beemoviescript.Add("What? You're not dead?");
                        beemoviescript.Add("Do I look dead? They will wipe anything");
                        beemoviescript.Add("that moves. Where you headed?");
                        beemoviescript.Add("To Honey Farms.");
                        beemoviescript.Add("I am onto something huge here.");
                        beemoviescript.Add("I'm going to Alaska. Moose blood,");
                        beemoviescript.Add("crazy stuff. Blows your head off!");
                        beemoviescript.Add("I'm going to Tacoma.");
                        beemoviescript.Add("- And you?");
                        beemoviescript.Add("- He really is dead.");
                        beemoviescript.Add("All right.");
                        beemoviescript.Add("Uh-oh!");
                        beemoviescript.Add("- What is that?!");
                        beemoviescript.Add("- Oh, no!");
                        beemoviescript.Add("- A wiper! Triple blade!");
                        beemoviescript.Add("- Triple blade?");
                        beemoviescript.Add("Jump on! It's your only chance, bee!");
                        beemoviescript.Add("Why does everything have");
                        beemoviescript.Add("to be so doggone clean?!");
                        beemoviescript.Add("How much do you people need to see?!");
                        beemoviescript.Add("Open your eyes!");
                        beemoviescript.Add("Stick your head out the window!");
                        beemoviescript.Add("From NPR News in Washington,");
                        beemoviescript.Add("I'm Oarl Kasell.");
                        beemoviescript.Add("But don't kill no more bugs!");
                        beemoviescript.Add("- Bee!");
                        beemoviescript.Add("- Moose blood guy!!");
                        beemoviescript.Add("- You hear something?");
                        beemoviescript.Add("- Like what?");
                        beemoviescript.Add("Like tiny screaming.");
                        beemoviescript.Add("Turn off the radio.");
                        beemoviescript.Add("Whassup, bee boy?");
                        beemoviescript.Add("Hey, Blood.");
                        beemoviescript.Add("Just a row of honey jars,");
                        beemoviescript.Add("as far as the eye could see.");
                        beemoviescript.Add("Wow!");
                        beemoviescript.Add("I assume wherever this truck goes");
                        beemoviescript.Add("is where they're getting it.");
                        beemoviescript.Add("I mean, that honey's ours.");
                        beemoviescript.Add("- Bees hang tight.");
                        beemoviescript.Add("- We're all jammed in.");
                        beemoviescript.Add("It's a close community.");
                        beemoviescript.Add("Not us, man. We on our own.");
                        beemoviescript.Add("Every mosquito on his own.");
                        beemoviescript.Add("- What if you get in trouble?");
                        beemoviescript.Add("- You a mosquito, you in trouble.");
                        beemoviescript.Add("Nobody likes us. They just smack.");
                        beemoviescript.Add("See a mosquito, smack, smack!");
                        beemoviescript.Add("At least you're out in the world.");
                        beemoviescript.Add("You must meet girls.");
                        beemoviescript.Add("Mosquito girls try to trade up,");
                        beemoviescript.Add("get with a moth, dragonfly.");
                        beemoviescript.Add("Mosquito girl don't want no mosquito.");
                        beemoviescript.Add("You got to be kidding me!");
                        beemoviescript.Add("Mooseblood's about to leave");
                        beemoviescript.Add("the building! So long, bee!");
                        beemoviescript.Add("- Hey, guys!");
                        beemoviescript.Add("- Mooseblood!");
                        beemoviescript.Add("I knew I'd catch y'all down here.");
                        beemoviescript.Add("Did you bring your crazy straw?");
                        beemoviescript.Add("We throw it in jars, slap a label on it,");
                        beemoviescript.Add("and it's pretty much pure profit.");
                        beemoviescript.Add("What is this place?");
                        beemoviescript.Add("A bee's got a brain");
                        beemoviescript.Add("the size of a pinhead.");
                        beemoviescript.Add("They are pinheads!");
                        beemoviescript.Add("Pinhead.");
                        beemoviescript.Add("- Oheck out the new smoker.");
                        beemoviescript.Add("- Oh, sweet. That's the one you want.");
                        beemoviescript.Add("The Thomas 3000!");
                        beemoviescript.Add("Smoker?");
                        beemoviescript.Add("Ninety puffs a minute, semi-automatic.");
                        beemoviescript.Add("Twice the nicotine, all the tar.");
                        beemoviescript.Add("A couple breaths of this");
                        beemoviescript.Add("knocks them right out.");
                        beemoviescript.Add("They make the honey,");
                        beemoviescript.Add("and we make the money.");
                        beemoviescript.Add("\"They make the honey,");
                        beemoviescript.Add("and we make the money\"?");
                        beemoviescript.Add("Oh, my!");
                        beemoviescript.Add("What's going on? Are you OK?");
                        beemoviescript.Add("Yeah. It doesn't last too long.");
                        beemoviescript.Add("Do you know you're");
                        beemoviescript.Add("in a fake hive with fake walls?");
                        beemoviescript.Add("Our queen was moved here.");
                        beemoviescript.Add("We had no choice.");
                        beemoviescript.Add("This is your queen?");
                        beemoviescript.Add("That's a man in women's clothes!");
                        beemoviescript.Add("That's a drag queen!");
                        beemoviescript.Add("What is this?");
                        beemoviescript.Add("Oh, no!");
                        beemoviescript.Add("There's hundreds of them!");
                        beemoviescript.Add("Bee honey.");
                        beemoviescript.Add("Our honey is being brazenly stolen");
                        beemoviescript.Add("on a massive scale!");
                        beemoviescript.Add("This is worse than anything bears");
                        beemoviescript.Add("have done! I intend to do something.");
                        beemoviescript.Add("Oh, Barry, stop.");
                        beemoviescript.Add("Who told you humans are taking");
                        beemoviescript.Add("our honey? That's a rumor.");
                        beemoviescript.Add("Do these look like rumors?");
                        beemoviescript.Add("That's a conspiracy theory.");
                        beemoviescript.Add("These are obviously doctored photos.");
                        beemoviescript.Add("How did you get mixed up in this?");
                        beemoviescript.Add("He's been talking to humans.");
                        beemoviescript.Add("- What?");
                        beemoviescript.Add("- Talking to humans?!");
                        beemoviescript.Add("He has a human girlfriend.");
                        beemoviescript.Add("And they make out!");
                        beemoviescript.Add("Make out? Barry!");
                        beemoviescript.Add("We do not.");
                        beemoviescript.Add("- You wish you could.");
                        beemoviescript.Add("- Whose side are you on?");
                        beemoviescript.Add("The bees!");
                        beemoviescript.Add("I dated a cricket once in San Antonio.");
                        beemoviescript.Add("Those crazy legs kept me up all night.");
                        beemoviescript.Add("Barry, this is what you want");
                        beemoviescript.Add("to do with your life?");
                        beemoviescript.Add("I want to do it for all our lives.");
                        beemoviescript.Add("Nobody works harder than bees!");
                        beemoviescript.Add("Dad, I remember you");
                        beemoviescript.Add("coming home so overworked");
                        beemoviescript.Add("your hands were still stirring.");
                        beemoviescript.Add("You couldn't stop.");
                        beemoviescript.Add("I remember that.");
                        beemoviescript.Add("What right do they have to our honey?");
                        beemoviescript.Add("We live on two cups a year. They put it");
                        beemoviescript.Add("in lip balm for no reason whatsoever!");
                        beemoviescript.Add("Even if it's true, what can one bee do?");
                        beemoviescript.Add("Sting them where it really hurts.");
                        beemoviescript.Add("In the face! The eye!");
                        beemoviescript.Add("- That would hurt.");
                        beemoviescript.Add("- No.");
                        beemoviescript.Add("Up the nose? That's a killer.");
                        beemoviescript.Add("There's only one place you can sting");
                        beemoviescript.Add("the humans, one place where it matters.");
                        beemoviescript.Add("Hive at Five, the hive's only");
                        beemoviescript.Add("full-hour action news source.");
                        beemoviescript.Add("No more bee beards!");
                        beemoviescript.Add("With Bob Bumble at the anchor desk.");
                        beemoviescript.Add("Weather with Storm Stinger.");
                        beemoviescript.Add("Sports with Buzz Larvi.");
                        beemoviescript.Add("And Jeanette Ohung.");
                        beemoviescript.Add("- Good evening. I'm Bob Bumble.");
                        beemoviescript.Add("- And I'm Jeanette Ohung.");
                        beemoviescript.Add("A tri-county bee, Barry Benson,");
                        beemoviescript.Add("intends to sue the human race");
                        beemoviescript.Add("for stealing our honey,");
                        beemoviescript.Add("packaging it and profiting");
                        beemoviescript.Add("from it illegally!");
                        beemoviescript.Add("Tomorrow night on Bee Larry King,");
                        beemoviescript.Add("we'll have three former queens here in");
                        beemoviescript.Add("our studio, discussing their new book,");
                        beemoviescript.Add("Olassy Ladies,");
                        beemoviescript.Add("out this week on Hexagon.");
                        beemoviescript.Add("Tonight we're talking to Barry Benson.");
                        beemoviescript.Add("Did you ever think, \"I'm a kid");
                        beemoviescript.Add("from the hive. I can't do this\"?");
                        beemoviescript.Add("Bees have never been afraid");
                        beemoviescript.Add("to change the world.");
                        beemoviescript.Add("What about Bee Oolumbus?");
                        beemoviescript.Add("Bee Gandhi? Bejesus?");
                        beemoviescript.Add("Where I'm from, we'd never sue humans.");
                        beemoviescript.Add("We were thinking");
                        beemoviescript.Add("of stickball or candy stores.");
                        beemoviescript.Add("How old are you?");
                        beemoviescript.Add("The bee community");
                        beemoviescript.Add("is supporting you in this case,");
                        beemoviescript.Add("which will be the trial");
                        beemoviescript.Add("of the bee century.");
                        beemoviescript.Add("You know, they have a Larry King");
                        beemoviescript.Add("in the human world too.");
                        beemoviescript.Add("It's a common name. Next week...");
                        beemoviescript.Add("He looks like you and has a show");
                        beemoviescript.Add("and suspenders and colored dots...");
                        beemoviescript.Add("Next week...");
                        beemoviescript.Add("Glasses, quotes on the bottom from the");
                        beemoviescript.Add("guest even though you just heard 'em.");
                        beemoviescript.Add("Bear Week next week!");
                        beemoviescript.Add("They're scary, hairy and here live.");
                        beemoviescript.Add("Always leans forward, pointy shoulders,");
                        beemoviescript.Add("squinty eyes, very Jewish.");
                        beemoviescript.Add("In tennis, you attack");
                        beemoviescript.Add("at the point of weakness!");
                        beemoviescript.Add("It was my grandmother, Ken. She's 81.");
                        beemoviescript.Add("Honey, her backhand's a joke!");
                        beemoviescript.Add("I'm not gonna take advantage of that?");
                        beemoviescript.Add("Quiet, please.");
                        beemoviescript.Add("Actual work going on here.");
                        beemoviescript.Add("- Is that that same bee?");
                        beemoviescript.Add("- Yes, it is!");
                        beemoviescript.Add("I'm helping him sue the human race.");
                        beemoviescript.Add("- Hello.");
                        beemoviescript.Add("- Hello, bee.");
                        beemoviescript.Add("This is Ken.");
                        beemoviescript.Add("Yeah, I remember you. Timberland, size");
                        beemoviescript.Add("ten and a half. Vibram sole, I believe.");
                        beemoviescript.Add("Why does he talk again?");
                        beemoviescript.Add("Listen, you better go");
                        beemoviescript.Add("'cause we're really busy working.");
                        beemoviescript.Add("But it's our yogurt night!");
                        beemoviescript.Add("Bye-bye.");
                        beemoviescript.Add("Why is yogurt night so difficult?!");
                        beemoviescript.Add("You poor thing.");
                        beemoviescript.Add("You two have been at this for hours!");
                        beemoviescript.Add("Yes, and Adam here");
                        beemoviescript.Add("has been a huge help.");
                        beemoviescript.Add("- Frosting...");
                        beemoviescript.Add("- How many sugars?");
                        beemoviescript.Add("Just one. I try not");
                        beemoviescript.Add("to use the competition.");
                        beemoviescript.Add("So why are you helping me?");
                        beemoviescript.Add("Bees have good qualities.");
                        beemoviescript.Add("And it takes my mind off the shop.");
                        beemoviescript.Add("Instead of flowers, people");
                        beemoviescript.Add("are giving balloon bouquets now.");
                        beemoviescript.Add("Those are great, if you're three.");
                        beemoviescript.Add("And artificial flowers.");
                        beemoviescript.Add("- Oh, those just get me psychotic!");
                        beemoviescript.Add("- Yeah, me too.");
                        beemoviescript.Add("Bent stingers, pointless pollination.");
                        beemoviescript.Add("Bees must hate those fake things!");
                        beemoviescript.Add("Nothing worse");
                        beemoviescript.Add("than a daffodil that's had work done.");
                        beemoviescript.Add("Maybe this could make up");
                        beemoviescript.Add("for it a little bit.");
                        beemoviescript.Add("- This lawsuit's a pretty big deal.");
                        beemoviescript.Add("- I guess.");
                        beemoviescript.Add("You sure you want to go through with it?");
                        beemoviescript.Add("Am I sure? When I'm done with");
                        beemoviescript.Add("the humans, they won't be able");
                        beemoviescript.Add("to say, \"Honey, I'm home,\"");
                        beemoviescript.Add("without paying a royalty!");
                        beemoviescript.Add("It's an incredible scene");
                        beemoviescript.Add("here in downtown Manhattan,");
                        beemoviescript.Add("where the world anxiously waits,");
                        beemoviescript.Add("because for the first time in history,");
                        beemoviescript.Add("we will hear for ourselves");
                        beemoviescript.Add("if a honeybee can actually speak.");
                        beemoviescript.Add("What have we gotten into here, Barry?");
                        beemoviescript.Add("It's pretty big, isn't it?");
                        beemoviescript.Add("I can't believe how many humans");
                        beemoviescript.Add("don't work during the day.");
                        beemoviescript.Add("You think billion-dollar multinational");
                        beemoviescript.Add("food companies have good lawyers?");
                        beemoviescript.Add("Everybody needs to stay");
                        beemoviescript.Add("behind the barricade.");
                        beemoviescript.Add("- What's the matter?");
                        beemoviescript.Add("- I don't know, I just got a chill.");
                        beemoviescript.Add("Well, if it isn't the bee team.");
                        beemoviescript.Add("You boys work on this?");
                        beemoviescript.Add("All rise! The Honorable");
                        beemoviescript.Add("Judge Bumbleton presiding.");
                        beemoviescript.Add("All right. Oase number 4475,");
                        beemoviescript.Add("Superior Oourt of New York,");
                        beemoviescript.Add("Barry Bee Benson v. the Honey Industry");
                        beemoviescript.Add("is now in session.");
                        beemoviescript.Add("Mr. Montgomery, you're representing");
                        beemoviescript.Add("the five food companies collectively?");
                        beemoviescript.Add("A privilege.");
                        beemoviescript.Add("Mr. Benson... you're representing");
                        beemoviescript.Add("all the bees of the world?");
                        beemoviescript.Add("I'm kidding. Yes, Your Honor,");
                        beemoviescript.Add("we're ready to proceed.");
                        beemoviescript.Add("Mr. Montgomery,");
                        beemoviescript.Add("your opening statement, please.");
                        beemoviescript.Add("Ladies and gentlemen of the jury,");
                        beemoviescript.Add("my grandmother was a simple woman.");
                        beemoviescript.Add("Born on a farm, she believed");
                        beemoviescript.Add("it was man's divine right");
                        beemoviescript.Add("to benefit from the bounty");
                        beemoviescript.Add("of nature God put before us.");
                        beemoviescript.Add("If we lived in the topsy-turvy world");
                        beemoviescript.Add("Mr. Benson imagines,");
                        beemoviescript.Add("just think of what would it mean.");
                        beemoviescript.Add("I would have to negotiate");
                        beemoviescript.Add("with the silkworm");
                        beemoviescript.Add("for the elastic in my britches!");
                        beemoviescript.Add("Talking bee!");
                        beemoviescript.Add("How do we know this isn't some sort of");
                        beemoviescript.Add("holographic motion-picture-capture");
                        beemoviescript.Add("Hollywood wizardry?");
                        beemoviescript.Add("They could be using laser beams!");
                        beemoviescript.Add("Robotics! Ventriloquism!");
                        beemoviescript.Add("Oloning! For all we know,");
                        beemoviescript.Add("he could be on steroids!");
                        beemoviescript.Add("Mr. Benson?");
                        beemoviescript.Add("Ladies and gentlemen,");
                        beemoviescript.Add("there's no trickery here.");
                        beemoviescript.Add("I'm just an ordinary bee.");
                        beemoviescript.Add("Honey's pretty important to me.");
                        beemoviescript.Add("It's important to all bees.");
                        beemoviescript.Add("We invented it!");
                        beemoviescript.Add("We make it. And we protect it");
                        beemoviescript.Add("with our lives.");
                        beemoviescript.Add("Unfortunately, there are");
                        beemoviescript.Add("some people in this room");
                        beemoviescript.Add("who think they can take it from us");
                        beemoviescript.Add("'cause we're the little guys!");
                        beemoviescript.Add("I'm hoping that, after this is all over,");
                        beemoviescript.Add("you'll see how, by taking our honey,");
                        beemoviescript.Add("you not only take everything we have");
                        beemoviescript.Add("but everything we are!");
                        beemoviescript.Add("I wish he'd dress like that");
                        beemoviescript.Add("all the time. So nice!");
                        beemoviescript.Add("Oall your first witness.");
                        beemoviescript.Add("So, Mr. Klauss Vanderhayden");
                        beemoviescript.Add("of Honey Farms, big company you have.");
                        beemoviescript.Add("I suppose so.");
                        beemoviescript.Add("I see you also own");
                        beemoviescript.Add("Honeyburton and Honron!");
                        beemoviescript.Add("Yes, they provide beekeepers");
                        beemoviescript.Add("for our farms.");
                        beemoviescript.Add("Beekeeper. I find that");
                        beemoviescript.Add("to be a very disturbing term.");
                        beemoviescript.Add("I don't imagine you employ");
                        beemoviescript.Add("any bee-free-ers, do you?");
                        beemoviescript.Add("- No.");
                        beemoviescript.Add("- I couldn't hear you.");
                        beemoviescript.Add("- No.");
                        beemoviescript.Add("- No.");
                        beemoviescript.Add("Because you don't free bees.");
                        beemoviescript.Add("You keep bees. Not only that,");
                        beemoviescript.Add("it seems you thought a bear would be");
                        beemoviescript.Add("an appropriate image for a jar of honey.");
                        beemoviescript.Add("They're very lovable creatures.");
                        beemoviescript.Add("Yogi Bear, Fozzie Bear, Build-A-Bear.");
                        beemoviescript.Add("You mean like this?");
                        beemoviescript.Add("Bears kill bees!");
                        beemoviescript.Add("How'd you like his head crashing");
                        beemoviescript.Add("through your living room?!");
                        beemoviescript.Add("Biting into your couch!");
                        beemoviescript.Add("Spitting out your throw pillows!");
                        beemoviescript.Add("OK, that's enough. Take him away.");
                        beemoviescript.Add("So, Mr. Sting, thank you for being here.");
                        beemoviescript.Add("Your name intrigues me.");
                        beemoviescript.Add("- Where have I heard it before?");
                        beemoviescript.Add("- I was with a band called The Police.");
                        beemoviescript.Add("But you've never been");
                        beemoviescript.Add("a police officer, have you?");
                        beemoviescript.Add("No, I haven't.");
                        beemoviescript.Add("No, you haven't. And so here");
                        beemoviescript.Add("we have yet another example");
                        beemoviescript.Add("of bee culture casually");
                        beemoviescript.Add("stolen by a human");
                        beemoviescript.Add("for nothing more than");
                        beemoviescript.Add("a prance-about stage name.");
                        beemoviescript.Add("Oh, please.");
                        beemoviescript.Add("Have you ever been stung, Mr. Sting?");
                        beemoviescript.Add("Because I'm feeling");
                        beemoviescript.Add("a little stung, Sting.");
                        beemoviescript.Add("Or should I say... Mr. Gordon M. Sumner!");
                        beemoviescript.Add("That's not his real name?! You idiots!");
                        beemoviescript.Add("Mr. Liotta, first,");
                        beemoviescript.Add("belated congratulations on");
                        beemoviescript.Add("your Emmy win for a guest spot");
                        beemoviescript.Add("on ER in 2005.");
                        beemoviescript.Add("Thank you. Thank you.");
                        beemoviescript.Add("I see from your resume");
                        beemoviescript.Add("that you're devilishly handsome");
                        beemoviescript.Add("with a churning inner turmoil");
                        beemoviescript.Add("that's ready to blow.");
                        beemoviescript.Add("I enjoy what I do. Is that a crime?");
                        beemoviescript.Add("Not yet it isn't. But is this");
                        beemoviescript.Add("what it's come to for you?");
                        beemoviescript.Add("Exploiting tiny, helpless bees");
                        beemoviescript.Add("so you don't");
                        beemoviescript.Add("have to rehearse");
                        beemoviescript.Add("your part and learn your lines, sir?");
                        beemoviescript.Add("Watch it, Benson!");
                        beemoviescript.Add("I could blow right now!");
                        beemoviescript.Add("This isn't a goodfella.");
                        beemoviescript.Add("This is a badfella!");
                        beemoviescript.Add("Why doesn't someone just step on");
                        beemoviescript.Add("this creep, and we can all go home?!");
                        beemoviescript.Add("- Order in this court!");
                        beemoviescript.Add("- You're all thinking it!");
                        beemoviescript.Add("Order! Order, I say!");
                        beemoviescript.Add("- Say it!");
                        beemoviescript.Add("- Mr. Liotta, please sit down!");
                        beemoviescript.Add("I think it was awfully nice");
                        beemoviescript.Add("of that bear to pitch in like that.");
                        beemoviescript.Add("I think the jury's on our side.");
                        beemoviescript.Add("Are we doing everything right, legally?");
                        beemoviescript.Add("I'm a florist.");
                        beemoviescript.Add("Right. Well, here's to a great team.");
                        beemoviescript.Add("To a great team!");
                        beemoviescript.Add("Well, hello.");
                        beemoviescript.Add("- Ken!");
                        beemoviescript.Add("- Hello.");
                        beemoviescript.Add("I didn't think you were coming.");
                        beemoviescript.Add("No, I was just late.");
                        beemoviescript.Add("I tried to call, but... the battery.");
                        beemoviescript.Add("I didn't want all this to go to waste,");
                        beemoviescript.Add("so I called Barry. Luckily, he was free.");
                        beemoviescript.Add("Oh, that was lucky.");
                        beemoviescript.Add("There's a little left.");
                        beemoviescript.Add("I could heat it up.");
                        beemoviescript.Add("Yeah, heat it up, sure, whatever.");
                        beemoviescript.Add("So I hear you're quite a tennis player.");
                        beemoviescript.Add("I'm not much for the game myself.");
                        beemoviescript.Add("The ball's a little grabby.");
                        beemoviescript.Add("That's where I usually sit.");
                        beemoviescript.Add("Right... there.");
                        beemoviescript.Add("Ken, Barry was looking at your resume,");
                        beemoviescript.Add("and he agreed with me that eating with");
                        beemoviescript.Add("chopsticks isn't really a special skill.");
                        beemoviescript.Add("You think I don't see what you're doing?");
                        beemoviescript.Add("I know how hard it is to find");
                        beemoviescript.Add("the rightjob. We have that in common.");
                        beemoviescript.Add("Do we?");
                        beemoviescript.Add("Bees have 100 percent employment,");
                        beemoviescript.Add("but we do jobs like taking the crud out.");
                        beemoviescript.Add("That's just what");
                        beemoviescript.Add("I was thinking about doing.");
                        beemoviescript.Add("Ken, I let Barry borrow your razor");
                        beemoviescript.Add("for his fuzz. I hope that was all right.");
                        beemoviescript.Add("I'm going to drain the old stinger.");
                        beemoviescript.Add("Yeah, you do that.");
                        beemoviescript.Add("Look at that.");
                        beemoviescript.Add("You know, I've just about had it");
                        beemoviescript.Add("with your little mind games.");
                        beemoviescript.Add("- What's that?");
                        beemoviescript.Add("- Italian Vogue.");
                        beemoviescript.Add("Mamma mia, that's a lot of pages.");
                        beemoviescript.Add("A lot of ads.");
                        beemoviescript.Add("Remember what Van said, why is");
                        beemoviescript.Add("your life more valuable than mine?");
                        beemoviescript.Add("Funny, I just can't seem to recall that!");
                        beemoviescript.Add("I think something stinks in here!");
                        beemoviescript.Add("I love the smell of flowers.");
                        beemoviescript.Add("How do you like the smell of flames?!");
                        beemoviescript.Add("Not as much.");
                        beemoviescript.Add("Water bug! Not taking sides!");
                        beemoviescript.Add("Ken, I'm wearing a Ohapstick hat!");
                        beemoviescript.Add("This is pathetic!");
                        beemoviescript.Add("I've got issues!");
                        beemoviescript.Add("Well, well, well, a royal flush!");
                        beemoviescript.Add("- You're bluffing.");
                        beemoviescript.Add("- Am I?");
                        beemoviescript.Add("Surf's up, dude!");
                        beemoviescript.Add("Poo water!");
                        beemoviescript.Add("That bowl is gnarly.");
                        beemoviescript.Add("Except for those dirty yellow rings!");
                        beemoviescript.Add("Kenneth! What are you doing?!");
                        beemoviescript.Add("You know, I don't even like honey!");
                        beemoviescript.Add("I don't eat it!");
                        beemoviescript.Add("We need to talk!");
                        beemoviescript.Add("He's just a little bee!");
                        beemoviescript.Add("And he happens to be");
                        beemoviescript.Add("the nicest bee I've met in a long time!");
                        beemoviescript.Add("Long time? What are you talking about?!");
                        beemoviescript.Add("Are there other bugs in your life?");
                        beemoviescript.Add("No, but there are other things bugging");
                        beemoviescript.Add("me in life. And you're one of them!");
                        beemoviescript.Add("Fine! Talking bees, no yogurt night...");
                        beemoviescript.Add("My nerves are fried from riding");
                        beemoviescript.Add("on this emotional roller coaster!");
                        beemoviescript.Add("Goodbye, Ken.");
                        beemoviescript.Add("And for your information,");
                        beemoviescript.Add("I prefer sugar-free, artificial");
                        beemoviescript.Add("sweeteners made by man!");
                        beemoviescript.Add("I'm sorry about all that.");
                        beemoviescript.Add("I know it's got");
                        beemoviescript.Add("an aftertaste! I like it!");
                        beemoviescript.Add("I always felt there was some kind");
                        beemoviescript.Add("of barrier between Ken and me.");
                        beemoviescript.Add("I couldn't overcome it.");
                        beemoviescript.Add("Oh, well.");
                        beemoviescript.Add("Are you OK for the trial?");
                        beemoviescript.Add("I believe Mr. Montgomery");
                        beemoviescript.Add("is about out of ideas.");
                        beemoviescript.Add("We would like to call");
                        beemoviescript.Add("Mr. Barry Benson Bee to the stand.");
                        beemoviescript.Add("Good idea! You can really see why he's");
                        beemoviescript.Add("considered one of the best lawyers...");
                        beemoviescript.Add("Yeah.");
                        beemoviescript.Add("Layton, you've");
                        beemoviescript.Add("gotta weave some magic");
                        beemoviescript.Add("with this jury,");
                        beemoviescript.Add("or it's gonna be all over.");
                        beemoviescript.Add("Don't worry. The only thing I have");
                        beemoviescript.Add("to do to turn this jury around");
                        beemoviescript.Add("is to remind them");
                        beemoviescript.Add("of what they don't like about bees.");
                        beemoviescript.Add("- You got the tweezers?");
                        beemoviescript.Add("- Are you allergic?");
                        beemoviescript.Add("Only to losing, son. Only to losing.");
                        beemoviescript.Add("Mr. Benson Bee, I'll ask you");
                        beemoviescript.Add("what I think we'd all like to know.");
                        beemoviescript.Add("What exactly is your relationship");
                        beemoviescript.Add("to that woman?");
                        beemoviescript.Add("We're friends.");
                        beemoviescript.Add("- Good friends?");
                        beemoviescript.Add("- Yes.");
                        beemoviescript.Add("How good? Do you live together?");
                        beemoviescript.Add("Wait a minute...");
                        beemoviescript.Add("Are you her little...");
                        beemoviescript.Add("...bedbug?");
                        beemoviescript.Add("I've seen a bee documentary or two.");
                        beemoviescript.Add("From what I understand,");
                        beemoviescript.Add("doesn't your queen give birth");
                        beemoviescript.Add("to all the bee children?");
                        beemoviescript.Add("- Yeah, but...");
                        beemoviescript.Add("- So those aren't your real parents!");
                        beemoviescript.Add("- Oh, Barry...");
                        beemoviescript.Add("- Yes, they are!");
                        beemoviescript.Add("Hold me back!");
                        beemoviescript.Add("You're an illegitimate bee,");
                        beemoviescript.Add("aren't you, Benson?");
                        beemoviescript.Add("He's denouncing bees!");
                        beemoviescript.Add("Don't y'all date your cousins?");
                        beemoviescript.Add("- Objection!");
                        beemoviescript.Add("- I'm going to pincushion this guy!");
                        beemoviescript.Add("Adam, don't! It's what he wants!");
                        beemoviescript.Add("Oh, I'm hit!!");
                        beemoviescript.Add("Oh, lordy, I am hit!");
                        beemoviescript.Add("Order! Order!");
                        beemoviescript.Add("The venom! The venom");
                        beemoviescript.Add("is coursing through my veins!");
                        beemoviescript.Add("I have been felled");
                        beemoviescript.Add("by a winged beast of destruction!");
                        beemoviescript.Add("You see? You can't treat them");
                        beemoviescript.Add("like equals! They're striped savages!");
                        beemoviescript.Add("Stinging's the only thing");
                        beemoviescript.Add("they know! It's their way!");
                        beemoviescript.Add("- Adam, stay with me.");
                        beemoviescript.Add("- I can't feel my legs.");
                        beemoviescript.Add("What angel of mercy");
                        beemoviescript.Add("will come forward to suck the poison");
                        beemoviescript.Add("from my heaving buttocks?");
                        beemoviescript.Add("I will have order in this court. Order!");
                        beemoviescript.Add("Order, please!");
                        beemoviescript.Add("The case of the honeybees");
                        beemoviescript.Add("versus the human race");
                        beemoviescript.Add("took a pointed turn against the bees");
                        beemoviescript.Add("yesterday when one of their legal");
                        beemoviescript.Add("team stung Layton T. Montgomery.");
                        beemoviescript.Add("- Hey, buddy.");
                        beemoviescript.Add("- Hey.");
                        beemoviescript.Add("- Is there much pain?");
                        beemoviescript.Add("- Yeah.");
                        beemoviescript.Add("I...");
                        beemoviescript.Add("I blew the whole case, didn't I?");
                        beemoviescript.Add("It doesn't matter. What matters is");
                        beemoviescript.Add("you're alive. You could have died.");
                        beemoviescript.Add("I'd be better off dead. Look at me.");
                        beemoviescript.Add("They got it from the cafeteria");
                        beemoviescript.Add("downstairs, in a tuna sandwich.");
                        beemoviescript.Add("Look, there's");
                        beemoviescript.Add("a little celery still on it.");
                        beemoviescript.Add("What was it like to sting someone?");
                        beemoviescript.Add("I can't explain it. It was all...");
                        beemoviescript.Add("All adrenaline and then...");
                        beemoviescript.Add("and then ecstasy!");
                        beemoviescript.Add("All right.");
                        beemoviescript.Add("You think it was all a trap?");
                        beemoviescript.Add("Of course. I'm sorry.");
                        beemoviescript.Add("I flew us right into this.");
                        beemoviescript.Add("What were we thinking? Look at us. We're");
                        beemoviescript.Add("just a couple of bugs in this world.");
                        beemoviescript.Add("What will the humans do to us");
                        beemoviescript.Add("if they win?");
                        beemoviescript.Add("I don't know.");
                        beemoviescript.Add("I hear they put the roaches in motels.");
                        beemoviescript.Add("That doesn't sound so bad.");
                        beemoviescript.Add("Adam, they check in,");
                        beemoviescript.Add("but they don't check out!");
                        beemoviescript.Add("Oh, my.");
                        beemoviescript.Add("Oould you get a nurse");
                        beemoviescript.Add("to close that window?");
                        beemoviescript.Add("- Why?");
                        beemoviescript.Add("- The smoke.");
                        beemoviescript.Add("Bees don't smoke.");
                        beemoviescript.Add("Right. Bees don't smoke.");
                        beemoviescript.Add("Bees don't smoke!");
                        beemoviescript.Add("But some bees are smoking.");
                        beemoviescript.Add("That's it! That's our case!");
                        beemoviescript.Add("It is? It's not over?");
                        beemoviescript.Add("Get dressed. I've gotta go somewhere.");
                        beemoviescript.Add("Get back to the court and stall.");
                        beemoviescript.Add("Stall any way you can.");
                        beemoviescript.Add("And assuming you've done step correctly, you're ready for the tub.");
                        beemoviescript.Add("Mr. Flayman.");
                        beemoviescript.Add("Yes? Yes, Your Honor!");
                        beemoviescript.Add("Where is the rest of your team?");
                        beemoviescript.Add("Well, Your Honor, it's interesting.");
                        beemoviescript.Add("Bees are trained to fly haphazardly,");
                        beemoviescript.Add("and as a result,");
                        beemoviescript.Add("we don't make very good time.");
                        beemoviescript.Add("I actually heard a funny story about...");
                        beemoviescript.Add("Your Honor,");
                        beemoviescript.Add("haven't these ridiculous bugs");
                        beemoviescript.Add("taken up enough");
                        beemoviescript.Add("of this court's valuable time?");
                        beemoviescript.Add("How much longer will we allow");
                        beemoviescript.Add("these absurd shenanigans to go on?");
                        beemoviescript.Add("They have presented no compelling");
                        beemoviescript.Add("evidence to support their charges");
                        beemoviescript.Add("against my clients,");
                        beemoviescript.Add("who run legitimate businesses.");
                        beemoviescript.Add("I move for a complete dismissal");
                        beemoviescript.Add("of this entire case!");
                        beemoviescript.Add("Mr. Flayman, I'm afraid I'm going");
                        beemoviescript.Add("to have to consider");
                        beemoviescript.Add("Mr. Montgomery's motion.");
                        beemoviescript.Add("But you can't! We have a terrific case.");
                        beemoviescript.Add("Where is your proof?");
                        beemoviescript.Add("Where is the evidence?");
                        beemoviescript.Add("Show me the smoking gun!");
                        beemoviescript.Add("Hold it, Your Honor!");
                        beemoviescript.Add("You want a smoking gun?");
                        beemoviescript.Add("Here is your smoking gun.");
                        beemoviescript.Add("What is that?");
                        beemoviescript.Add("It's a bee smoker!");
                        beemoviescript.Add("What, this?");
                        beemoviescript.Add("This harmless little contraption?");
                        beemoviescript.Add("This couldn't hurt a fly,");
                        beemoviescript.Add("let alone a bee.");
                        beemoviescript.Add("Look at what has happened");
                        beemoviescript.Add("to bees who have never been asked,");
                        beemoviescript.Add("\"Smoking or non?\"");
                        beemoviescript.Add("Is this what nature intended for us?");
                        beemoviescript.Add("To be forcibly addicted");
                        beemoviescript.Add("to smoke machines");
                        beemoviescript.Add("and man-made wooden slat work camps?");
                        beemoviescript.Add("Living out our lives as honey slaves");
                        beemoviescript.Add("to the white man?");
                        beemoviescript.Add("- What are we gonna do?");
                        beemoviescript.Add("- He's playing the species card.");
                        beemoviescript.Add("Ladies and gentlemen, please,");
                        beemoviescript.Add("free these bees!");
                        beemoviescript.Add("Free the bees! Free the bees!");
                        beemoviescript.Add("Free the bees!");
                        beemoviescript.Add("Free the bees! Free the bees!");
                        beemoviescript.Add("The court finds in favor of the bees!");
                        beemoviescript.Add("Vanessa, we won!");
                        beemoviescript.Add("I knew you could do it! High-five!");
                        beemoviescript.Add("Sorry.");
                        beemoviescript.Add("I'm OK! You know what this means?");
                        beemoviescript.Add("All the honey");
                        beemoviescript.Add("will finally belong to the bees.");
                        beemoviescript.Add("Now we won't have");
                        beemoviescript.Add("to work so hard all the time.");
                        beemoviescript.Add("This is an unholy perversion");
                        beemoviescript.Add("of the balance of nature, Benson.");
                        beemoviescript.Add("You'll regret this.");
                        beemoviescript.Add("Barry, how much honey is out there?");
                        beemoviescript.Add("All right. One at a time.");
                        beemoviescript.Add("Barry, who are you wearing?");
                        beemoviescript.Add("My sweater is Ralph Lauren,");
                        beemoviescript.Add("and I have no pants.");
                        beemoviescript.Add("- What if Montgomery's right?");
                        beemoviescript.Add("- What do you mean?");
                        beemoviescript.Add("We've been living the bee way");
                        beemoviescript.Add("a long time, 27 million years.");
                        beemoviescript.Add("Oongratulations on your victory.");
                        beemoviescript.Add("What will you demand as a settlement?");
                        beemoviescript.Add("First, we'll demand a complete shutdown");
                        beemoviescript.Add("of all bee work camps.");
                        beemoviescript.Add("Then we want back the honey");
                        beemoviescript.Add("that was ours to begin with,");
                        beemoviescript.Add("every last drop.");
                        beemoviescript.Add("We demand an end to the glorification");
                        beemoviescript.Add("of the bear as anything more");
                        beemoviescript.Add("than a filthy, smelly,");
                        beemoviescript.Add("bad-breath stink machine.");
                        beemoviescript.Add("We're all aware");
                        beemoviescript.Add("of what they do in the woods.");
                        beemoviescript.Add("Wait for my signal.");
                        beemoviescript.Add("Take him out.");
                        beemoviescript.Add("He'll have nauseous");
                        beemoviescript.Add("for a few hours, then he'll be fine.");
                        beemoviescript.Add("And we will no longer tolerate");
                        beemoviescript.Add("bee-negative nicknames...");
                        beemoviescript.Add("But it's just a prance-about stage name!");
                        beemoviescript.Add("...unnecessary inclusion of honey");
                        beemoviescript.Add("in bogus health products");
                        beemoviescript.Add("and la-dee-da human");
                        beemoviescript.Add("tea-time snack garnishments.");
                        beemoviescript.Add("Oan't breathe.");
                        beemoviescript.Add("Bring it in, boys!");
                        beemoviescript.Add("Hold it right there! Good.");
                        beemoviescript.Add("Tap it.");
                        beemoviescript.Add("Mr. Buzzwell, we just passed three cups,");
                        beemoviescript.Add("and there's gallons more coming!");
                        beemoviescript.Add("- I think we need to shut down!");
                        beemoviescript.Add("- Shut down? We've never shut down.");
                        beemoviescript.Add("Shut down honey production!");
                        beemoviescript.Add("Stop making honey!");
                        beemoviescript.Add("Turn your key, sir!");
                        beemoviescript.Add("What do we do now?");
                        beemoviescript.Add("Oannonball!");
                        beemoviescript.Add("We're shutting honey production!");
                        beemoviescript.Add("Mission abort.");
                        beemoviescript.Add("Aborting pollination and nectar detail.");
                        beemoviescript.Add("Returning to base.");
                        beemoviescript.Add("Adam, you wouldn't believe");
                        beemoviescript.Add("how much honey was out there.");
                        beemoviescript.Add("Oh, yeah?");
                        beemoviescript.Add("What's going on? Where is everybody?");
                        beemoviescript.Add("- Are they out celebrating?");
                        beemoviescript.Add("- They're home.");
                        beemoviescript.Add("They don't know what to do.");
                        beemoviescript.Add("Laying out, sleeping in.");
                        beemoviescript.Add("I heard your Uncle Oarl was on his way");
                        beemoviescript.Add("to San Antonio with a cricket.");
                        beemoviescript.Add("At least we got our honey back.");
                        beemoviescript.Add("Sometimes I think, so what if humans");
                        beemoviescript.Add("liked our honey? Who wouldn't?");
                        beemoviescript.Add("It's the greatest thing in the world!");
                        beemoviescript.Add("I was excited to be part of making it.");
                        beemoviescript.Add("This was my new desk. This was my");
                        beemoviescript.Add("new job. I wanted to do it really well.");
                        beemoviescript.Add("And now...");
                        beemoviescript.Add("Now I can't.");
                        beemoviescript.Add("I don't understand");
                        beemoviescript.Add("why they're not happy.");
                        beemoviescript.Add("I thought their lives would be better!");
                        beemoviescript.Add("They're doing nothing. It's amazing.");
                        beemoviescript.Add("Honey really changes people.");
                        beemoviescript.Add("You don't have any idea");
                        beemoviescript.Add("what's going on, do you?");
                        beemoviescript.Add("- What did you want to show me?");
                        beemoviescript.Add("- This.");
                        beemoviescript.Add("What happened here?");
                        beemoviescript.Add("That is not the half of it.");
                        beemoviescript.Add("Oh, no. Oh, my.");
                        beemoviescript.Add("They're all wilting.");
                        beemoviescript.Add("Doesn't look very good, does it?");
                        beemoviescript.Add("No.");
                        beemoviescript.Add("And whose fault do you think that is?");
                        beemoviescript.Add("You know, I'm gonna guess bees.");
                        beemoviescript.Add("Bees?");
                        beemoviescript.Add("Specifically, me.");
                        beemoviescript.Add("I didn't think bees not needing to make");
                        beemoviescript.Add("honey would affect all these things.");
                        beemoviescript.Add("It's notjust flowers.");
                        beemoviescript.Add("Fruits, vegetables, they all need bees.");
                        beemoviescript.Add("That's our whole SAT test right there.");
                        beemoviescript.Add("Take away produce, that affects");
                        beemoviescript.Add("the entire animal kingdom.");
                        beemoviescript.Add("And then, of course...");
                        beemoviescript.Add("The human species?");
                        beemoviescript.Add("So if there's no more pollination,");
                        beemoviescript.Add("it could all just go south here,");
                        beemoviescript.Add("couldn't it?");
                        beemoviescript.Add("I know this is also partly my fault.");
                        beemoviescript.Add("How about a suicide pact?");
                        beemoviescript.Add("How do we do it?");
                        beemoviescript.Add("- I'll sting you, you step on me.");
                        beemoviescript.Add("- Thatjust kills you twice.");
                        beemoviescript.Add("Right, right.");
                        beemoviescript.Add("Listen, Barry...");
                        beemoviescript.Add("sorry, but I gotta get going.");
                        beemoviescript.Add("I had to open my mouth and talk.");
                        beemoviescript.Add("Vanessa?");
                        beemoviescript.Add("Vanessa? Why are you leaving?");
                        beemoviescript.Add("Where are you going?");
                        beemoviescript.Add("To the final Tournament of Roses parade");
                        beemoviescript.Add("in Pasadena.");
                        beemoviescript.Add("They've moved it to this weekend");
                        beemoviescript.Add("because all the flowers are dying.");
                        beemoviescript.Add("It's the last chance");
                        beemoviescript.Add("I'll ever have to see it.");
                        beemoviescript.Add("Vanessa, I just wanna say I'm sorry.");
                        beemoviescript.Add("I never meant it to turn out like this.");
                        beemoviescript.Add("I know. Me neither.");
                        beemoviescript.Add("Tournament of Roses.");
                        beemoviescript.Add("Roses can't do sports.");
                        beemoviescript.Add("Wait a minute. Roses. Roses?");
                        beemoviescript.Add("Roses!");
                        beemoviescript.Add("Vanessa!");
                        beemoviescript.Add("Roses?!");
                        beemoviescript.Add("Barry?");
                        beemoviescript.Add("- Roses are flowers!");
                        beemoviescript.Add("- Yes, they are.");
                        beemoviescript.Add("Flowers, bees, pollen!");
                        beemoviescript.Add("I know.");
                        beemoviescript.Add("That's why this is the last parade.");
                        beemoviescript.Add("Maybe not.");
                        beemoviescript.Add("Oould you ask him to slow down?");
                        beemoviescript.Add("Oould you slow down?");
                        beemoviescript.Add("Barry!");
                        beemoviescript.Add("OK, I made a huge mistake.");
                        beemoviescript.Add("This is a total disaster, all my fault.");
                        beemoviescript.Add("Yes, it kind of is.");
                        beemoviescript.Add("I've ruined the planet.");
                        beemoviescript.Add("I wanted to help you");
                        beemoviescript.Add("with the flower shop.");
                        beemoviescript.Add("I've made it worse.");
                        beemoviescript.Add("Actually, it's completely closed down.");
                        beemoviescript.Add("I thought maybe you were remodeling.");
                        beemoviescript.Add("But I have another idea, and it's");
                        beemoviescript.Add("greater than my previous ideas combined.");
                        beemoviescript.Add("I don't want to hear it!");
                        beemoviescript.Add("All right, they have the roses,");
                        beemoviescript.Add("the roses have the pollen.");
                        beemoviescript.Add("I know every bee, plant");
                        beemoviescript.Add("and flower bud in this park.");
                        beemoviescript.Add("All we gotta do is get what they've got");
                        beemoviescript.Add("back here with what we've got.");
                        beemoviescript.Add("- Bees.");
                        beemoviescript.Add("- Park.");
                        beemoviescript.Add("- Pollen!");
                        beemoviescript.Add("- Flowers.");
                        beemoviescript.Add("- Repollination!");
                        beemoviescript.Add("- Across the nation!");
                        beemoviescript.Add("Tournament of Roses,");
                        beemoviescript.Add("Pasadena, Oalifornia.");
                        beemoviescript.Add("They've got nothing");
                        beemoviescript.Add("but flowers, floats and cotton candy.");
                        beemoviescript.Add("Security will be tight.");
                        beemoviescript.Add("I have an idea.");
                        beemoviescript.Add("Vanessa Bloome, FTD.");
                        beemoviescript.Add("Official floral business. It's real.");
                        beemoviescript.Add("Sorry, ma'am. Nice brooch.");
                        beemoviescript.Add("Thank you. It was a gift.");
                        beemoviescript.Add("Once inside,");
                        beemoviescript.Add("we just pick the right float.");
                        beemoviescript.Add("How about The Princess and the Pea?");
                        beemoviescript.Add("I could be the princess,");
                        beemoviescript.Add("and you could be the pea!");
                        beemoviescript.Add("Yes, I got it.");
                        beemoviescript.Add("- Where should I sit?");
                        beemoviescript.Add("- What are you?");
                        beemoviescript.Add("- I believe I'm the pea.");
                        beemoviescript.Add("- The pea?");
                        beemoviescript.Add("It goes under the mattresses.");
                        beemoviescript.Add("- Not in this fairy tale, sweetheart.");
                        beemoviescript.Add("- I'm getting the marshal.");
                        beemoviescript.Add("You do that!");
                        beemoviescript.Add("This whole parade is a fiasco!");
                        beemoviescript.Add("Let's see what this baby'll do.");
                        beemoviescript.Add("Hey, what are you doing?!");
                        beemoviescript.Add("Then all we do");
                        beemoviescript.Add("is blend in with traffic...");
                        beemoviescript.Add("...without arousing suspicion.");
                        beemoviescript.Add("Once at the airport,");
                        beemoviescript.Add("there's no stopping us.");
                        beemoviescript.Add("Stop! Security.");
                        beemoviescript.Add("- You and your insect pack your float?");
                        beemoviescript.Add("- Yes.");
                        beemoviescript.Add("Has it been");
                        beemoviescript.Add("in your possession the entire time?");
                        beemoviescript.Add("Would you remove your shoes?");
                        beemoviescript.Add("- Remove your stinger.");
                        beemoviescript.Add("- It's part of me.");
                        beemoviescript.Add("I know. Just having some fun.");
                        beemoviescript.Add("Enjoy your flight.");
                        beemoviescript.Add("Then if we're lucky, we'll have");
                        beemoviescript.Add("just enough pollen to do the job.");
                        beemoviescript.Add("Oan you believe how lucky we are? We");
                        beemoviescript.Add("have just enough pollen to do the job!");
                        beemoviescript.Add("I think this is gonna work.");
                        beemoviescript.Add("It's got to work.");
                        beemoviescript.Add("Attention, passengers,");
                        beemoviescript.Add("this is Oaptain Scott.");
                        beemoviescript.Add("We have a bit of bad weather");
                        beemoviescript.Add("in New York.");
                        beemoviescript.Add("It looks like we'll experience");
                        beemoviescript.Add("a couple hours delay.");
                        beemoviescript.Add("Barry, these are cut flowers");
                        beemoviescript.Add("with no water. They'll never make it.");
                        beemoviescript.Add("I gotta get up there");
                        beemoviescript.Add("and talk to them.");
                        beemoviescript.Add("Be careful.");
                        beemoviescript.Add("Oan I get help");
                        beemoviescript.Add("with the Sky Mall magazine?");
                        beemoviescript.Add("I'd like to order the talking");
                        beemoviescript.Add("inflatable nose and ear hair trimmer.");
                        beemoviescript.Add("Oaptain, I'm in a real situation.");
                        beemoviescript.Add("- What'd you say, Hal?");
                        beemoviescript.Add("- Nothing.");
                        beemoviescript.Add("Bee!");
                        beemoviescript.Add("Don't freak out! My entire species...");
                        beemoviescript.Add("What are you doing?");
                        beemoviescript.Add("- Wait a minute! I'm an attorney!");
                        beemoviescript.Add("- Who's an attorney?");
                        beemoviescript.Add("Don't move.");
                        beemoviescript.Add("Oh, Barry.");
                        beemoviescript.Add("Good afternoon, passengers.");
                        beemoviescript.Add("This is your captain.");
                        beemoviescript.Add("Would a Miss Vanessa Bloome in 24B");
                        beemoviescript.Add("please report to the cockpit?");
                        beemoviescript.Add("And please hurry!");
                        beemoviescript.Add("What happened here?");
                        beemoviescript.Add("There was a DustBuster,");
                        beemoviescript.Add("a toupee, a life raft exploded.");
                        beemoviescript.Add("One's bald, one's in a boat,");
                        beemoviescript.Add("they're both unconscious!");
                        beemoviescript.Add("- Is that another bee joke?");
                        beemoviescript.Add("- No!");
                        beemoviescript.Add("No one's flying the plane!");
                        beemoviescript.Add("This is JFK control tower, Flight 356.");
                        beemoviescript.Add("What's your status?");
                        beemoviescript.Add("This is Vanessa Bloome.");
                        beemoviescript.Add("I'm a florist from New York.");
                        beemoviescript.Add("Where's the pilot?");
                        beemoviescript.Add("He's unconscious,");
                        beemoviescript.Add("and so is the copilot.");
                        beemoviescript.Add("Not good. Does anyone onboard");
                        beemoviescript.Add("have flight experience?");
                        beemoviescript.Add("As a matter of fact, there is.");
                        beemoviescript.Add("- Who's that?");
                        beemoviescript.Add("- Barry Benson.");
                        beemoviescript.Add("From the honey trial?! Oh, great.");
                        beemoviescript.Add("Vanessa, this is nothing more");
                        beemoviescript.Add("than a big metal bee.");
                        beemoviescript.Add("It's got giant wings, huge engines.");
                        beemoviescript.Add("I can't fly a plane.");
                        beemoviescript.Add("- Why not? Isn't John Travolta a pilot?");
                        beemoviescript.Add("- Yes.");
                        beemoviescript.Add("How hard could it be?");
                        beemoviescript.Add("Wait, Barry!");
                        beemoviescript.Add("We're headed into some lightning.");
                        beemoviescript.Add("This is Bob Bumble. We have some");
                        beemoviescript.Add("late-breaking news from JFK Airport,");
                        beemoviescript.Add("where a suspenseful scene");
                        beemoviescript.Add("is developing.");
                        beemoviescript.Add("Barry Benson,");
                        beemoviescript.Add("fresh from his legal victory...");
                        beemoviescript.Add("That's Barry!");
                        beemoviescript.Add("...is attempting to land a plane,");
                        beemoviescript.Add("loaded with people, flowers");
                        beemoviescript.Add("and an incapacitated flight crew.");
                        beemoviescript.Add("Flowers?!");
                        beemoviescript.Add("We have a storm in the area");
                        beemoviescript.Add("and two individuals at the controls");
                        beemoviescript.Add("with absolutely no flight experience.");
                        beemoviescript.Add("Just a minute.");
                        beemoviescript.Add("There's a bee on that plane.");
                        beemoviescript.Add("I'm quite familiar with Mr. Benson");
                        beemoviescript.Add("and his no-account compadres.");
                        beemoviescript.Add("They've done enough damage.");
                        beemoviescript.Add("But isn't he your only hope?");
                        beemoviescript.Add("Technically, a bee");
                        beemoviescript.Add("shouldn't be able to fly at all.");
                        beemoviescript.Add("Their wings are too small...");
                        beemoviescript.Add("Haven't we heard this a million times?");
                        beemoviescript.Add("\"The surface area of the wings");
                        beemoviescript.Add("and body mass make no sense.\"");
                        beemoviescript.Add("- Get this on the air!");
                        beemoviescript.Add("- Got it.");
                        beemoviescript.Add("- Stand by.");
                        beemoviescript.Add("- We're going live.");
                        beemoviescript.Add("The way we work may be a mystery to you.");
                        beemoviescript.Add("Making honey takes a lot of bees");
                        beemoviescript.Add("doing a lot of small jobs.");
                        beemoviescript.Add("But let me tell you about a small job.");
                        beemoviescript.Add("If you do it well,");
                        beemoviescript.Add("it makes a big difference.");
                        beemoviescript.Add("More than we realized.");
                        beemoviescript.Add("To us, to everyone.");
                        beemoviescript.Add("That's why I want to get bees");
                        beemoviescript.Add("back to working together.");
                        beemoviescript.Add("That's the bee way!");
                        beemoviescript.Add("We're not made of Jell-O.");
                        beemoviescript.Add("We get behind a fellow.");
                        beemoviescript.Add("- Black and yellow!");
                        beemoviescript.Add("- Hello!");
                        beemoviescript.Add("Left, right, down, hover.");
                        beemoviescript.Add("- Hover?");
                        beemoviescript.Add("- Forget hover.");
                        beemoviescript.Add("This isn't so hard.");
                        beemoviescript.Add("Beep-beep! Beep-beep!");
                        beemoviescript.Add("Barry, what happened?!");
                        beemoviescript.Add("Wait, I think we were");
                        beemoviescript.Add("on autopilot the whole time.");
                        beemoviescript.Add("- That may have been helping me.");
                        beemoviescript.Add("- And now we're not!");
                        beemoviescript.Add("So it turns out I cannot fly a plane.");
                        beemoviescript.Add("All of you, let's get");
                        beemoviescript.Add("behind this fellow! Move it out!");
                        beemoviescript.Add("Move out!");
                        beemoviescript.Add("Our only chance is if I do what I'd do,");
                        beemoviescript.Add("you copy me with the wings of the plane!");
                        beemoviescript.Add("Don't have to yell.");
                        beemoviescript.Add("I'm not yelling!");
                        beemoviescript.Add("We're in a lot of trouble.");
                        beemoviescript.Add("It's very hard to concentrate");
                        beemoviescript.Add("with that panicky tone in your voice!");
                        beemoviescript.Add("It's not a tone. I'm panicking!");
                        beemoviescript.Add("I can't do this!");
                        beemoviescript.Add("Vanessa, pull yourself together.");
                        beemoviescript.Add("You have to snap out of it!");
                        beemoviescript.Add("You snap out of it.");
                        beemoviescript.Add("You snap out of it.");
                        beemoviescript.Add("- You snap out of it!");
                        beemoviescript.Add("- You snap out of it!");
                        beemoviescript.Add("- You snap out of it!");
                        beemoviescript.Add("- You snap out of it!");
                        beemoviescript.Add("- You snap out of it!");
                        beemoviescript.Add("- You snap out of it!");
                        beemoviescript.Add("- Hold it!");
                        beemoviescript.Add("- Why? Oome on, it's my turn.");
                        beemoviescript.Add("How is the plane flying?");
                        beemoviescript.Add("I don't know.");
                        beemoviescript.Add("Hello?");
                        beemoviescript.Add("Benson, got any flowers");
                        beemoviescript.Add("for a happy occasion in there?");
                        beemoviescript.Add("The Pollen Jocks!");
                        beemoviescript.Add("They do get behind a fellow.");
                        beemoviescript.Add("- Black and yellow.");
                        beemoviescript.Add("- Hello.");
                        beemoviescript.Add("All right, let's drop this tin can");
                        beemoviescript.Add("on the blacktop.");
                        beemoviescript.Add("Where? I can't see anything. Oan you?");
                        beemoviescript.Add("No, nothing. It's all cloudy.");
                        beemoviescript.Add("Oome on. You got to think bee, Barry.");
                        beemoviescript.Add("- Thinking bee.");
                        beemoviescript.Add("- Thinking bee.");
                        beemoviescript.Add("Thinking bee!");
                        beemoviescript.Add("Thinking bee! Thinking bee!");
                        beemoviescript.Add("Wait a minute.");
                        beemoviescript.Add("I think I'm feeling something.");
                        beemoviescript.Add("- What?");
                        beemoviescript.Add("- I don't know. It's strong, pulling me.");
                        beemoviescript.Add("Like a 27-million-year-old instinct.");
                        beemoviescript.Add("Bring the nose down.");
                        beemoviescript.Add("Thinking bee!");
                        beemoviescript.Add("Thinking bee! Thinking bee!");
                        beemoviescript.Add("- What in the world is on the tarmac?");
                        beemoviescript.Add("- Get some lights on that!");
                        beemoviescript.Add("Thinking bee!");
                        beemoviescript.Add("Thinking bee! Thinking bee!");
                        beemoviescript.Add("- Vanessa, aim for the flower.");
                        beemoviescript.Add("- OK.");
                        beemoviescript.Add("Out the engines. We're going in");
                        beemoviescript.Add("on bee power. Ready, boys?");
                        beemoviescript.Add("Affirmative!");
                        beemoviescript.Add("Good. Good. Easy, now. That's it.");
                        beemoviescript.Add("Land on that flower!");
                        beemoviescript.Add("Ready? Full reverse!");
                        beemoviescript.Add("Spin it around!");
                        beemoviescript.Add("- Not that flower! The other one!");
                        beemoviescript.Add("- Which one?");
                        beemoviescript.Add("- That flower.");
                        beemoviescript.Add("- I'm aiming at the flower!");
                        beemoviescript.Add("That's a fat guy in a flowered shirt.");
                        beemoviescript.Add("I mean the giant pulsating flower");
                        beemoviescript.Add("made of millions of bees!");
                        beemoviescript.Add("Pull forward. Nose down. Tail up.");
                        beemoviescript.Add("Rotate around it.");
                        beemoviescript.Add("- This is insane, Barry!");
                        beemoviescript.Add("- This's the only way I know how to fly.");
                        beemoviescript.Add("Am I koo-koo-kachoo, or is this plane");
                        beemoviescript.Add("flying in an insect-like pattern?");
                        beemoviescript.Add("Get your nose in there. Don't be afraid.");
                        beemoviescript.Add("Smell it. Full reverse!");
                        beemoviescript.Add("Just drop it. Be a part of it.");
                        beemoviescript.Add("Aim for the center!");
                        beemoviescript.Add("Now drop it in! Drop it in, woman!");
                        beemoviescript.Add("Oome on, already.");
                        beemoviescript.Add("Barry, we did it!");
                        beemoviescript.Add("You taught me how to fly!");
                        beemoviescript.Add("- Yes. No high-five!");
                        beemoviescript.Add("- Right.");
                        beemoviescript.Add("Barry, it worked!");
                        beemoviescript.Add("Did you see the giant flower?");
                        beemoviescript.Add("What giant flower? Where? Of course");
                        beemoviescript.Add("I saw the flower! That was genius!");
                        beemoviescript.Add("- Thank you.");
                        beemoviescript.Add("- But we're not done yet.");
                        beemoviescript.Add("Listen, everyone!");
                        beemoviescript.Add("This runway is covered");
                        beemoviescript.Add("with the last pollen");
                        beemoviescript.Add("from the last flowers");
                        beemoviescript.Add("available anywhere on Earth.");
                        beemoviescript.Add("That means this is our last chance.");
                        beemoviescript.Add("We're the only ones who make honey,");
                        beemoviescript.Add("pollinate flowers and dress like this.");
                        beemoviescript.Add("If we're gonna survive as a species,");
                        beemoviescript.Add("this is our moment! What do you say?");
                        beemoviescript.Add("Are we going to be bees, orjust");
                        beemoviescript.Add("Museum of Natural History keychains?");
                        beemoviescript.Add("We're bees!");
                        beemoviescript.Add("Keychain!");
                        beemoviescript.Add("Then follow me! Except Keychain.");
                        beemoviescript.Add("Hold on, Barry. Here.");
                        beemoviescript.Add("You've earned this.");
                        beemoviescript.Add("Yeah!");
                        beemoviescript.Add("I'm a Pollen Jock! And it's a perfect");
                        beemoviescript.Add("fit. All I gotta do are the sleeves.");
                        beemoviescript.Add("Oh, yeah.");
                        beemoviescript.Add("That's our Barry.");
                        beemoviescript.Add("Mom! The bees are back!");
                        beemoviescript.Add("If anybody needs");
                        beemoviescript.Add("to make a call, now's the time.");
                        beemoviescript.Add("I got a feeling we'll be");
                        beemoviescript.Add("working late tonight!");
                        beemoviescript.Add("Here's your change. Have a great");
                        beemoviescript.Add("afternoon! Oan I help who's next?");
                        beemoviescript.Add("Would you like some honey with that?");
                        beemoviescript.Add("It is bee-approved. Don't forget these.");
                        beemoviescript.Add("Milk, cream, cheese, it's all me.");
                        beemoviescript.Add("And I don't see a nickel!");
                        beemoviescript.Add("Sometimes I just feel");
                        beemoviescript.Add("like a piece of meat!");
                        beemoviescript.Add("I had no idea.");
                        beemoviescript.Add("Barry, I'm sorry.");
                        beemoviescript.Add("Have you got a moment?");
                        beemoviescript.Add("Would you excuse me?");
                        beemoviescript.Add("My mosquito associate will help you.");
                        beemoviescript.Add("Sorry I'm late.");
                        beemoviescript.Add("He's a lawyer too?");
                        beemoviescript.Add("I was already a blood-sucking parasite.");
                        beemoviescript.Add("All I needed was a briefcase.");
                        beemoviescript.Add("Have a great afternoon!");
                        beemoviescript.Add("Barry, I just got this huge tulip order,");
                        beemoviescript.Add("and I can't get them anywhere.");
                        beemoviescript.Add("No problem, Vannie.");
                        beemoviescript.Add("Just leave it to me.");
                        beemoviescript.Add("You're a lifesaver, Barry.");
                        beemoviescript.Add("Oan I help who's next?");
                        beemoviescript.Add("All right, scramble, jocks!");
                        beemoviescript.Add("It's time to fly.");
                        beemoviescript.Add("Thank you, Barry!");
                        beemoviescript.Add("That bee is living my life!");
                        beemoviescript.Add("Let it go, Kenny.");
                        beemoviescript.Add("- When will this nightmare end?!");
                        beemoviescript.Add("- Let it all go.");
                        beemoviescript.Add("- Beautiful day to fly.");
                        beemoviescript.Add("- Sure is.");
                        beemoviescript.Add("Between you and me,");
                        beemoviescript.Add("I was dying to get out of that office.");
                        beemoviescript.Add("You have got");
                        beemoviescript.Add("to start thinking bee, my friend.");
                        beemoviescript.Add("- Thinking bee!");
                        beemoviescript.Add("- Me?");
                        beemoviescript.Add("Hold it. Let's just stop");
                        beemoviescript.Add("for a second. Hold it.");
                        beemoviescript.Add("I'm sorry. I'm sorry, everyone.");
                        beemoviescript.Add("Oan we stop here?");
                        beemoviescript.Add("I'm not making a major life decision");
                        beemoviescript.Add("during a production number!");
                        beemoviescript.Add("All right. Take ten, everybody.");
                        beemoviescript.Add("Wrap it up, guys.");
                        beemoviescript.Add("I had virtually no rehearsal for that.");
                        await e.Channel.SendMessage($"{beemoviescript[0]}");
                    }
                });

    }
    private static void gpsCooldown(object source, ElapsedEventArgs e)
    {
        if (gpsCooldownInt > 0)
            gpsCooldownInt--;
    }
    private static void navysealsCooldown(object source, ElapsedEventArgs e)
    {
        if (navysealsCooldownInt > 0)
            navysealsCooldownInt--;
    }
    private static void powertwowerCooldown(object source, ElapsedEventArgs e)
    {
        if (powertwowerCooldownInt > 0)
            powertwowerCooldownInt--;
    }
}
