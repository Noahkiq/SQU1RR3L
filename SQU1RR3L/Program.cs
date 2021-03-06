﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Commands.Permissions.Levels;
using System.IO;
using System.Timers;
using DynamicExpresso;
using Argotic.Common;
using Argotic.Syndication;

class Program
{
	static void Main(string[] args) => new Program().Start();

	public static DateTime MessageSent;
	public string LastUsedRPS;
	public static int CommandsUsed;
	public static DateTime StartupTime = DateTime.Now;
	static Random rnd = new Random();
	public static string commandPrefix = "^";
	public static int gpsCooldownInt = 0;
	public static int navysealsCooldownInt = 0;
	public static int powertwowerCooldownInt = 0;
	public static bool onlyOpToggle = false;
	public static List<ulong> voted = new List<ulong>();

	public static int choice1 = 0;
	public static int choice2 = 0;
	public static int choice3 = 0;
	public static int choice4 = 0;

	private static DiscordClient _client = new DiscordClient();

	public void Start()
	{
		System.Timers.Timer siivaCheckTimer = new System.Timers.Timer();
		siivaCheckTimer.Elapsed += new ElapsedEventHandler(siivaRipChecker);
		siivaCheckTimer.Interval = 60000;
		siivaCheckTimer.Enabled = false;

		System.Timers.Timer gilvaCheckTimer = new System.Timers.Timer();
		gilvaCheckTimer.Elapsed += new ElapsedEventHandler(gilvaRipChecker);
		gilvaCheckTimer.Interval = 60000;
		gilvaCheckTimer.Enabled = false;

		System.Timers.Timer ttgdCheckTimer = new System.Timers.Timer();
		ttgdCheckTimer.Elapsed += new ElapsedEventHandler(ttgdRipChecker);
		ttgdCheckTimer.Interval = 60000;
		ttgdCheckTimer.Enabled = false;

		_client = new DiscordClient();
		Console.WriteLine("Booting up bot...");

		_client.UsingCommands(x => {
			x.PrefixChar = '^';
			x.AllowMentionPrefix = true;
			x.HelpMode = HelpMode.Public;
		});

		_client.Log.Message += (s, e) => Console.WriteLine($"[{e.Severity}] {e.Source}: {e.Message}");

		_client.MessageReceived += async (s, e) =>
		{
			if (e.Channel.IsPrivate && e.Message.Text.ToLower() == "^role-test")
			{
				Server server = _client.GetServer(229054940180512768);
				User user = server.GetUser(e.User.Id);
				var userRoles = server.GetUser(e.User.Id).Roles;
				var onTeam = false;

				foreach (Role role in userRoles)
				{
					if (role.Name.StartsWith("bot-test"))
					{
						await user.RemoveRoles(role);
						onTeam = true;
					}
				}

				if (onTeam)
					await e.Channel.SendMessage($"You have been removed from your team.");
				else
					await e.Channel.SendMessage($"You are not on a team!");
			}
			else if (e.Channel.Id == 278621300505837569 && e.Message.Text.ToLower() == "!signup")
			{
				Role s5role = e.Server.FindRoles("Series Finale (S5)").FirstOrDefault();
				Role alive = e.Server.FindRoles("Living Contestant").FirstOrDefault();
				await e.User.AddRoles(s5role, alive);
				await e.Channel.SendMessage("You have successfully been given the season 5 roles.");
			}
			if(e.Channel.Id == 242094350597750784)
			{
				string message = e.Message.Text.ToLower().Replace("e", "");
				if ((!string.IsNullOrWhiteSpace(message)) && (!e.User.IsBot))
				{
					await e.Message.Delete();
					User u = e.User;
					// await u.Kick();
					await e.Channel.SendMessage("eeEeEeeeeeEeEEeeEeEee");
				}
			}
			if (e.Message.IsAuthor)
			{
				if (e.Message.RawText == $"beepboop, calculating response time...")
				{
					var ping = e.Message.Timestamp - MessageSent;
					await e.Message.Edit($"Pong! Responded in " + (ping.TotalMilliseconds) + " milliseconds.");
				}
				else if (e.Message.Text == "🌑 rock!")
				{
					if (LastUsedRPS == "rock")
						await e.Message.Edit($"🌑 rock! We tied!");
					else if (LastUsedRPS == "paper")
						await e.Message.Edit($"🌑 rock! You win!");
					else if (LastUsedRPS == "scissors")
						await e.Message.Edit($"🌑 rock! I win!");
				}
				else if (e.Message.Text == "📰 paper!")
				{
					if (LastUsedRPS == "rock")
						await e.Message.Edit($"📰 paper! I win!");
					else if (LastUsedRPS == "paper")
						await e.Message.Edit($"📰 paper! We tied!");
					else if (LastUsedRPS == "scissors")
						await e.Message.Edit($"📰 paper! You win!");
				}
				else if (e.Message.Text == "✂ scissors!")
				{
					if (LastUsedRPS == "rock")
						await e.Message.Edit($"✂ scissors! You win!");
					else if (LastUsedRPS == "paper")
						await e.Message.Edit($"✂ scissors! I win!");
					else if (LastUsedRPS == "scissors")
						await e.Message.Edit($"✂ scissors! We tied!");
				}
			}
			// else if (e.Message.Text.StartsWith($"{commandPrefix}speak"))
			// {
			//     string cleverbotUser = File.ReadLines("cleverbot.txt").Skip(0).Take(1).First();
			//     string cleverbotKey = File.ReadLines("cleverbot.txt").Skip(1).Take(1).First();
			//     var session = CleverbotSession.NewSession(cleverbotUser, cleverbotKey);
			//     var response = session.Send(e.Message.Text.Replace($"{commandPrefix}speak ", ""));
			//     await e.Channel.SendMessage($"{e.User.Mention}: {response}");
			// }
			if (e.Message.Text.StartsWith($"{commandPrefix}userinfo"))
			{
				if (!e.Channel.IsPrivate)
				{
					string mention = e.Message.RawText.Replace($"{commandPrefix}userinfo", "");
					string input = null;
					User user;       
					if (e.Message.RawText.ToLower().Contains($"{commandPrefix}userinfo <@"))
					{
						string id1;
						string id2;
						ulong finalId;
						if (mention.Contains("!"))
						{
							//id = ulong.Parse(mention.Split('!')[1].Split('>')[0]);
							id1 = mention.Replace($" <@!", "");
							id2 = id1.Replace($">", "");
							finalId = Convert.ToUInt64(id2);
						}
						else
						{
							//id = ulong.Parse(mention.Split('@')[1].Split('>')[0]);
							id1 = mention.Replace($" <@", "");
							id2 = id1.Replace($">", "");
							finalId = Convert.ToUInt64(id2);
						}

						user = e.Server.GetUser(finalId);
					}
					else if (!string.IsNullOrWhiteSpace(mention))
					{
						input = e.Message.RawText.Replace($"{commandPrefix}userinfo ", "");
						user = e.Server.FindUsers(input).FirstOrDefault();
					}
					else
						user = e.User;

					if (user == null)
						await e.Channel.SendMessage($"No user by the name of `{input}` could be found.");
					else
					{
						ulong id = user.Id;
						string username = clearformatting(user.Name);
						string discrim = $"#{user.Discriminator}";

						string nickname = "[none]";
						if (!string.IsNullOrWhiteSpace(user.Nickname))
							nickname = clearformatting(user.Nickname);

						string game;
						string status = user.Status.ToString();

						if (user.CurrentGame == null)
							game = "[none]";
						else
						{
							game = clearformatting(user.CurrentGame.GetValueOrDefault().Name);
							string streamUrl = user.CurrentGame.GetValueOrDefault().Url;
							if (user.CurrentGame.GetValueOrDefault().Url != null)
								status = $"streaming # {streamUrl}";
						}

						DateTime joined = user.JoinedAt;
						var joinedDays = DateTime.Now - joined;
						string avatar = user.AvatarUrl;
						await e.Channel.SendMessage($"```ini\n" +
							$"\n[ID]            {id}\n" +
							$"[Username]      {username}\n" +
							$"[Discriminator] {discrim}\n" +
							$"[Nickname]      {nickname}\n" +
							$"[Current game]  {game}\n" +
							$"[Status]        {status}\n" +
							$"[Joined]        {joined} ({joinedDays.Days} days ago)\n" +
							$"[Avatar] {avatar}\n```");
					}
				}
			}
			else if (e.Message.Text == $"{commandPrefix}serverinfo")
			{
				var Roles = e.Server.Roles;
				List<string> rolesList = new List<string>();
				foreach (Role role in Roles)
					rolesList.Add(role.Name);

				var CreationDate = DateTime.Now - e.Server.Owner.JoinedAt;
				string rolesString = string.Join(", ", rolesList.ToArray());
				string region = e.Server.Region.Name;

				await e.Channel.SendMessage($"```ini\n" +
					$"[Name]            {clearformatting(e.Server.Name)}\n" +
					$"[ID]              {e.Server.Id}\n" +
					$"[User Count]      {e.Server.UserCount}\n" +
					$"[Channel Count]   {e.Server.ChannelCount}\n" +
					$"[Default Channel] #{e.Server.DefaultChannel}\n" +
					$"[Role Count]      {e.Server.RoleCount}\n" +
					$"[Roles]           {clearformatting(rolesString)}\n" +
					$"[Owner]           @{clearformatting(e.Server.Owner.Name) + e.Server.Owner.Discriminator}\n" +
					$"[Creation date]   {e.Server.Owner.JoinedAt} ({CreationDate.Days} days ago)\n" +
					$"[Icon] {e.Server.IconUrl}\n" +
					$"```");
			}
			else if (e.Message.Text == $"{commandPrefix}stats")
			{
				var servers = _client.Servers;
				int serverCount = servers.Count();
				int userCount = 0;
				int channelCount = 0;
				foreach(Server server in servers) {
					userCount = userCount + server.UserCount;
					channelCount = channelCount + server.ChannelCount;
				}

				await e.Channel.SendMessage($"```ini\n" +
					$"[Uptime]        {DateTime.Now - StartupTime}\n" +
					$"[Servers]       {serverCount}\n" +
					$"[Channels]      {channelCount}\n" +
					$"[Users]         {userCount}\n" +
					$"[Commands Used] {CommandsUsed}\n" +
					$"```");
			}
			else if ((e.Channel.Name == "op") && (!e.Message.RawText.ToLower().Contains("op")) && (onlyOpToggle == true))
			{
				await e.Message.Delete();
			}
			else if ((e.User.Id == 140564059417346049) && (e.Message.Text.StartsWith($"{commandPrefix}eval")))
			{
				try
				{
					var interpreter = new Interpreter();
					var parameters = new[] { new Parameter("e", e) };
					var message = e.Message.RawText.Replace($"{commandPrefix}eval ", "");
					var result = interpreter.Eval(message, parameters);
					await e.Channel.SendMessage($"Output: `{result}`");
					// await e.Server.Client.FindServers("HUAT Chill Lounge").FirstOrDefault().Leave();
				}
				catch (Exception error)
				{
					Console.WriteLine($"[Error] Bot ran into an issue while trying to run an eval. {error.ToString()}");
				}
			}
			else if (e.Server != null)
			{
				if ((e.Server.Id == 229662782356848640) && (e.Message.Text.ToLower() == $"{commandPrefix}votes"))
				{
					await e.Channel.SendMessage($"Option 1: **{choice1}** votes.\n" +
						$"Option 2: **{choice2}** votes.\n" +
						$"Option 3: **{choice3}** votes.\n" +
						$"Option 4: **{choice4}** votes.\n");
				}
				else if ((e.Server.Id == 229662782356848640) && (e.Message.Text.ToLower() == $"{commandPrefix}resetvotes"))
				{
					if (e.User.ServerPermissions.ManageMessages)
					{
						choice1 = 0;
						choice2 = 0;
						choice3 = 0;
						choice4 = 0;
						voted.Clear();
						await e.Channel.SendMessage($"Votes have been reset.");
					}
					else
						await e.Channel.SendMessage($"You must be a janitor or above to use this command!");
				}
			}
			if (e.Message.Text.StartsWith($"{commandPrefix}vote"))
			{
				if(e.Message.Text != $"{commandPrefix}votes")
				{
					if (e.Server != null)
					if (e.Server.Id == 229662782356848640)
						await e.Channel.SendMessage($"This command can only be run in DM's!"); // Displays error message if run on City Islands
					else { }
					else
					{
						bool voter = false;
						foreach (var id in voted)
						{
							if (id == e.User.Id)
							{
								voter = true; // If the user's ID was found in the "voted" list, "voter" is set to true
							}
						}
						if (voter == true)
						{
							await e.Channel.SendMessage($"You have already voted!"); // Displays error if user has already voted
						}
						else
						{
							if (e.Message.Text.ToLower() == $"{commandPrefix}vote")
								await e.Channel.SendMessage($"Correct command usage: `{commandPrefix}vote [choice]`"); // Displays error if no input
							else
							{
								voted.Add(e.User.Id); // Add user ID to "voted" list
								string vote = e.Message.Text.ToLower().Replace($"{commandPrefix}vote ", ""); // Grab the user's input/vote
								if ((vote == "1") || (vote == "2") || (vote == "3") || (vote == "4"))
								{
									if (vote == "1")
										choice1++;
									if (vote == "2")
										choice2++;
									if (vote == "3")
										choice3++;
									if (vote == "4")
										choice4++;

									await e.Channel.SendMessage($"Your vote has been recorded.");
								}
								else
									await e.Channel.SendMessage($"Invalid input. Make sure you're using the number of the vote choice!");
							}
						}
					}
				}
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

		_client.GetService<CommandService>().CreateCommand("source") //create command
			.Alias(new string[] { "git", "sourcecode", "github", "docs", "documentation" }) //add aliases
			.Description("Displays the bot's source code.") //add description, it will be shown when *help is used
			.Do(async e =>
				{
					CommandsUsed++;
					await e.Channel.SendMessage($"You can check out the bot's source code at <https://github.com/Noahkiq/SQU1RR3L> or view the bot's [outdated] documentation at <http://noahkiq.github.io/SQU1RR3L/>.");
					//sends a message to channel with the given text
				});

		_client.GetService<CommandService>().CreateCommand("speak") //create command
			.Do(e =>
				{
					CommandsUsed++;
					// parameters are dumb with this so the actual command stuff is up somewhere
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
					await e.Channel.SendMessage($"Heya! I'm SQU1RR3L, the general Discord bot written by Noahkiq. You can check out my command list with `^help` or check out my docs over at http://noahkiq.github.io/SQU1RR3L/. \n" +
						$"The current bot version is **1.3.3.1**");
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
			.Do(e =>
				{
					CommandsUsed++;
					// actual command shit is up abovee
				});

		_client.GetService<CommandService>().CreateCommand("stats") //create command
			.Description("Displays stats about the bot such as uptime and commands used.") //add description, it will be shown when *help is used
			.Do(e =>
				{
					CommandsUsed++;
					// again, no shit here thnx monodevelop
				});

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

		_client.GetService<CommandService>().CreateCommand("serverinfo") //create command
			.Description("Displays info about the server.") //add description, it will be shown when *help is used
			.Do(e =>
				{
					CommandsUsed++;
					// no command shit here because fuck monodevelop
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
			.Parameter("number", ParameterType.Optional) //as an argument, we have a person we want to greet
			.Do(async e =>
				{
					CommandsUsed++;
					if (e.Message.Text.Contains($"{commandPrefix}roll "))
					{
						if (e.Message.Text.ToLower() == $"{commandPrefix}roll 10")
						{
							int dice = rnd.Next(1, 11);
							await e.Channel.SendMessage($"The ten-sided die rolled a... {dice}!");
						}
						else if (e.Message.Text.ToLower() == $"{commandPrefix}roll 1")
							await e.Channel.SendMessage($"The one-sided die rolled a... one! Wow, surprising!");
						else if (e.Message.Text.ToLower() == $"{commandPrefix}roll 2")
						{
							int dice = rnd.Next(1, 3);
							await e.Channel.SendMessage($"The two-sided die rolled a... {dice}!");
						}
						else if (e.Message.Text.ToLower() == $"{commandPrefix}roll 3")
						{
							int dice = rnd.Next(1, 4);
							await e.Channel.SendMessage($"The three-sided die rolled a... {dice}!");
						}
						else if (e.Message.Text.ToLower() == $"{commandPrefix}roll 4")
						{
							int dice = rnd.Next(1, 5);
							await e.Channel.SendMessage($"The four-sided die rolled a... {dice}!");
						}
						else if (e.Message.Text.ToLower() == $"{commandPrefix}roll 5")
						{
							int dice = rnd.Next(1, 6);
							await e.Channel.SendMessage($"The five-sided die rolled a... {dice}!");
						}
						else if (e.Message.Text.ToLower() == $"{commandPrefix}roll 6")
						{
							int dice = rnd.Next(1, 7);
							await e.Channel.SendMessage($"The six-sided die rolled a... {dice}!");
						}
						else if (e.Message.Text.ToLower() == $"{commandPrefix}roll 7")
						{
							int dice = rnd.Next(1, 8);
							await e.Channel.SendMessage($"The seven-sided die rolled a... {dice}!");
						}
						else if (e.Message.Text.ToLower() == $"{commandPrefix}roll 8")
						{
							int dice = rnd.Next(1, 9);
							await e.Channel.SendMessage($"The eight-sided die rolled a... {dice}!");
						}
						else if (e.Message.Text.ToLower() == $"{commandPrefix}roll 9")
						{
							int dice = rnd.Next(1, 10);
							await e.Channel.SendMessage($"The nine-sided die rolled a... {dice}!");
						}
						//d10 is up above because reasons
						else if (e.Message.Text.ToLower() == $"{commandPrefix}roll 11")
						{
							int dice = rnd.Next(1, 12);
							await e.Channel.SendMessage($"The eleven-sided die rolled a... {dice}!");
						}
						else if (e.Message.Text.ToLower() == $"{commandPrefix}roll 12")
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
								$"Example command usage: {commandPrefix}tag kappa");
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

		_client.UserBanned += async (s, e) => {
			var logChannel = e.Server.FindChannels("squirrel-log").FirstOrDefault();
			if(logChannel != null)
			{
				await logChannel.SendMessage($"{e.User.Name} ({e.User.Id}) was banned from the server.");
			}
		};

		_client.UserUnbanned += async (s, e) => {
			var logChannel = e.Server.FindChannels("squirrel-log").FirstOrDefault();
			if(logChannel != null)
			{
				await logChannel.SendMessage($"{e.User.Name} ({e.User.Id}) was unbanned from the server.");
			}
		};

		_client.UserJoined += async (s, e) => {
			var logChannel = e.Server.FindChannels("squirrel-log").FirstOrDefault();
			if(logChannel != null)
			{
				await logChannel.SendMessage($"{e.User.Name} ({e.User.Id}) joined the server.");
			}
		};

		_client.UserLeft += async (s, e) => {
			var logChannel = e.Server.FindChannels("squirrel-log").FirstOrDefault();
			if (logChannel != null)
			{
				await logChannel.SendMessage($"{e.User.Name} ({e.User.Id}) left the server.");
			}
		};

		string token = File.ReadAllText("token.txt");
		_client.ExecuteAndWait(async () => {
			await _client.Connect(token, TokenType.Bot);
			_client.SetGame("^invite");
			_client.SetStatus(UserStatus.DoNotDisturb);
		});

	}

	private static void siivaRipChecker(object source, ElapsedEventArgs e)
	{
		SyndicationResourceLoadSettings settings = new SyndicationResourceLoadSettings();
		settings.RetrievalLimit = 1;

		Uri feedUrl = new Uri("https://www.youtube.com/feeds/videos.xml?channel_id=UC9ecwl3FTG66jIKA9JRDtmg");
		AtomFeed feed = AtomFeed.Create(feedUrl, settings);
		var videos = feed.Entries;

		bool alreadyPosted = false;

		if (videos.Count() == 0)
			Console.WriteLine("[Error] Feed contained no information.");

		foreach (var video in videos)
		{
			try
			{
				string videoUrlsFile = Directory.GetCurrentDirectory() + "/videoUrls.txt"; // String for the video URLS to check if a post is new, uses "videoUrls.txt" in directory the .exe is run from by default
				var logFile = File.ReadAllLines(videoUrlsFile);
				List<string> videoUrls = new List<string>(logFile);

				string newVideoUrl = video.Links.FirstOrDefault().Uri.ToString();
				string videoTitle = video.Title.Content;

				foreach (var videoUrl in videoUrls)
					if (newVideoUrl == videoUrl)
						alreadyPosted = true;

				try
				{
					if (alreadyPosted == false)
					{
						Console.WriteLine($"Found new video URL - {newVideoUrl} (\"{videoTitle}\") - Sending to discord");

						using (StreamWriter text = File.AppendText(videoUrlsFile))
							text.WriteLine(newVideoUrl);

						_client.GetServer(273242895451029505).GetChannel(273243365565399041).SendMessage("New SiIvaGunner rip!\n" +
							$"\"{videoTitle}\" - {newVideoUrl}");
					}
				}
				catch (Exception error)
				{
					Console.WriteLine($"[Error] Bot ran into an issue while trying to post the video to discord. {error.ToString()}");
				}
			}
			catch (Exception error)
			{
				Console.WriteLine("[Error] An error occured during the video check -" + error.ToString());
			}
		}

	}

	private static void gilvaRipChecker(object source, ElapsedEventArgs e)
	{
		SyndicationResourceLoadSettings settings = new SyndicationResourceLoadSettings();
		settings.RetrievalLimit = 1;

		Uri feedUrl = new Uri("https://www.youtube.com/feeds/videos.xml?user=GilvaSunner");
		AtomFeed feed = AtomFeed.Create(feedUrl, settings);
		var videos = feed.Entries;

		bool alreadyPosted = false;

		if (videos.Count() == 0)
			Console.WriteLine("[Error] Feed contained no information.");

		foreach (var video in videos)
		{
			try
			{
				string videoUrlsFile = Directory.GetCurrentDirectory() + "/videoUrls.txt"; // String for the video URLS to check if a post is new, uses "videoUrls.txt" in directory the .exe is run from by default
				var logFile = File.ReadAllLines(videoUrlsFile);
				List<string> videoUrls = new List<string>(logFile);

				string newVideoUrl = video.Links.FirstOrDefault().Uri.ToString();
				string videoTitle = video.Title.Content;

				foreach (var videoUrl in videoUrls)
					if (newVideoUrl == videoUrl)
						alreadyPosted = true;

				try
				{
					if (alreadyPosted == false)
					{
						Console.WriteLine($"Found new video URL - {newVideoUrl} (\"{videoTitle}\") - Sending to discord");

						using (StreamWriter text = File.AppendText(videoUrlsFile))
							text.WriteLine(newVideoUrl);

						_client.GetServer(273242895451029505).GetChannel(273243074912845824).SendMessage("New GilvaGunner rip!\n" +
							$"\"{videoTitle}\" - {newVideoUrl}");
					}
				}
				catch (Exception error)
				{
					Console.WriteLine($"[Error] Bot ran into an issue while trying to post the video to discord. {error.ToString()}");
				}
			}
			catch (Exception error)
			{
				Console.WriteLine("[Error] An error occured during the video check -" + error.ToString());
			}
		}

	}

	private static void ttgdRipChecker(object source, ElapsedEventArgs e)
	{
		SyndicationResourceLoadSettings settings = new SyndicationResourceLoadSettings();
		settings.RetrievalLimit = 1;

		Uri feedUrl = new Uri("https://www.youtube.com/feeds/videos.xml?channel_id=UCIXM2qZRG9o4AFmEsKZUIvQ");
		AtomFeed feed = AtomFeed.Create(feedUrl, settings);
		var videos = feed.Entries;

		bool alreadyPosted = false;

		if (videos.Count() == 0)
			Console.WriteLine("[Error] Feed contained no information.");

		foreach (var video in videos)
		{
			try
			{
				string videoUrlsFile = Directory.GetCurrentDirectory() + "/videoUrls.txt"; // String for the video URLS to check if a post is new, uses "videoUrls.txt" in directory the .exe is run from by default
				var logFile = File.ReadAllLines(videoUrlsFile);
				List<string> videoUrls = new List<string>(logFile);

				string newVideoUrl = video.Links.FirstOrDefault().Uri.ToString();
				string videoTitle = video.Title.Content;

				foreach (var videoUrl in videoUrls)
					if (newVideoUrl == videoUrl)
						alreadyPosted = true;

				try
				{
					if (alreadyPosted == false)
					{
						Console.WriteLine($"Found new video URL - {newVideoUrl} (\"{videoTitle}\") - Sending to discord");

						using (StreamWriter text = File.AppendText(videoUrlsFile))
							text.WriteLine(newVideoUrl);

						_client.GetServer(273242895451029505).GetChannel(273243520855441409).SendMessage("New TTGD rip!\n" +
							$"\"{videoTitle}\" - {newVideoUrl}");
					}
				}
				catch (Exception error)
				{
					Console.WriteLine($"[Error] Bot ran into an issue while trying to post the video to discord. {error.ToString()}");
				}
			}
			catch (Exception error)
			{
				Console.WriteLine("[Error] An error occured during the video check -" + error.ToString());
			}
		}

	}

	public static string clearformatting (string input)
	{
		var output = "[empty string]";
		if(!string.IsNullOrWhiteSpace(input))
		    output = input.Replace("`", "​`").Replace("*", "​*").Replace("_", "​_").Replace("‮", " ");
		return output;
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
