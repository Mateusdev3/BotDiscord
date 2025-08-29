using Discord;
using Discord.WebSocket;
using Discord.Interactions;
using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

class Program
{
    private DiscordSocketClient _client;
    private InteractionService _interactionService;
    private IServiceProvider _services;
    private static readonly string Token = "";
    public static readonly ulong ChannelId = 1402103538833686598; 
    public static Program Instance { get; private set; }

    public Program()
    {
        Instance = this;
    }

    static async Task Main(string[] args)
    {
        Program program = new Program();
        await program.RunBotAsync();
    }

    public async Task RunBotAsync()
    {
        _client = new DiscordSocketClient(new DiscordSocketConfig
        {
            GatewayIntents = GatewayIntents.All
        });

        _services = new ServiceCollection()
            .AddSingleton(_client)
            .AddSingleton(new InteractionService(_client.Rest))
            .BuildServiceProvider();
        _interactionService = _services.GetRequiredService<InteractionService>();
        _client.Log += LogAsync;
        _client.Ready += ReadyAsync;
        _client.InteractionCreated += HandleInteraction;
        _client.MessageReceived += StatusUpdateOnMessageAsync;
        await _client.LoginAsync(TokenType.Bot, Token);
        await _client.StartAsync();

        await Task.Delay(-1);
    }

    private Task LogAsync(LogMessage log)
    {
        Console.WriteLine(log.ToString());
        return Task.CompletedTask;
    }

    private async Task ReadyAsync()
    {
        Console.WriteLine($"{_client.CurrentUser} está online!");
        await _interactionService.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        await _interactionService.RegisterCommandsToGuildAsync(1394412758115811491);
        int players = await GetPlayersFromChannel();
        await UpdateBotStatus(players);
    }

    private async Task HandleInteraction(SocketInteraction interaction)
    {
        try
        {
            var ctx = new SocketInteractionContext(_client, interaction);
            await _interactionService.ExecuteCommandAsync(ctx, _services);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro em interação: {ex}");
        }
    }

    private async Task StatusUpdateOnMessageAsync(SocketMessage message)
    {
        if (message.Channel.Id != ChannelId || message.Author.IsBot == false) return;

        int players = ExtractPlayersFromMessage(message);
        if (players >= 0)
        {
            await UpdateBotStatus(players);
        }
    }

    public async Task<int> GetPlayersFromChannel()
    {
        try
        {
            var channel = _client.GetChannel(ChannelId) as IMessageChannel;

            if (channel == null)
            {
                Console.WriteLine("Canal não encontrado.");
                return 0;
            }

            var messages = await channel.GetMessagesAsync(1).FlattenAsync();
            var lastMessage = messages.FirstOrDefault();

            if (lastMessage != null)
                return ExtractPlayersFromMessage(lastMessage);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao buscar players no canal: {ex.Message}");
        }

        return 0;
    }

    public int ExtractPlayersFromMessage(IMessage message)
    {
        if (!string.IsNullOrWhiteSpace(message.Content))
        {
            var match = Regex.Match(message.Content, @"Players:\s*(\d+)/\d+");
            if (match.Success) return int.Parse(match.Groups[1].Value);
        }

        if (message.Embeds.Any())
        {
            var embed = message.Embeds.First();

            if (!string.IsNullOrEmpty(embed.Description))
            {
                var match = Regex.Match(embed.Description, @"Players:\s*(\d+)/\d+");
                if (match.Success) return int.Parse(match.Groups[1].Value);
            }

            foreach (var field in embed.Fields)
            {
                var match = Regex.Match(field.Value, @"Players:\s*(\d+)/\d+");
                if (match.Success) return int.Parse(match.Groups[1].Value);
            }
        }

        return 0;
    }

    private async Task UpdateBotStatus(int players)
    {
        await _client.SetGameAsync($"{players + 2} players online", type: ActivityType.Watching);
        Console.WriteLine($"[Status atualizado] {players} Players Online");
    }
}

// Módulo de Slash Commands
public class CommandsModule : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("ping", "Mostra o ping do bot")]
    public async Task PingAsync()
    {
        await RespondAsync("Pong!", ephemeral: true); 
    }

    [SlashCommand("players", "Mostra jogadores online")]
    public async Task PlayersAsync()
    {
        int players = await Program.Instance.GetPlayersFromChannel();
        await RespondAsync($"Atualmente há **{players + 2}** jogadores online!", ephemeral: true);
    }
}
