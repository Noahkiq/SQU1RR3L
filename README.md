# SQU1RR3L
Discord Bot written in C# using [discord.net](https://github.com/RogueException/Discord.Net). Made for [TrashChat+++](https://discord.gg/fjHhjGs)

You can [use this](https://discordapp.com/oauth2/authorize?client_id=215591038855675904&scope=bot) to add the bot to your server.

##Special Thanks (or something)

Thanks to [Julien12150](https://github.com/Julien12150) for helping me get the token.config working ~~because I'm dumb~~

also thanks visual studio for being annoying

##Running the bot
If you want to run this bot for whatever reason, just put your token in `SQU1RR3L/token.config`, open the project in Visual Studio, and compile it.

##Commands

| Command  | Description | Example | Alias(es) |
| ------------- | ------------- | ------------- | ------------- |
| help  | Lists the commands.  | *help | N/A |
| greet \<user> | Greets a person.  | *greet @Noahkiq | `gr`, `hi` |
| bork | MAXIMUM BORKDRIVE | *bork | `maximumbork`, `borkdrive`, `maximumborkdrive` |
| sigh | SiIvaGunner Emojipasta. | *sigh | `psy` |
| invite | Posts the link to get SQU1RR3L on your server. | *invite | `joinserver`, `join` |
| oh | oh | *oh | N/A |
| ban \<user> | Bans a user. **Currently broken.** | *ban @Noahkiq | N/A |
| emojiseals | Navy seals emojipasta. | *emojiseals | N/A |
| goodshit | That's some good shit right there 👌 | *goodshit | N/A |


###Prefixes
The prefixes by default are mentioning the bot and `*`. Ex. `@SQU1RR3L help` or `*help`

###Special Commands
These special commands have their own prefixes (or have none at all) which is they're kept seperate from the above table.

| Command  | Response |
| ------------- | ------------- |
| lo;  | lol good grammar noob |

###Logging

If you create a channel named `#squirrel-log`, the bot will post when users are (un)banned, join, or leave.
