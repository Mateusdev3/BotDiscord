
# Discord Players Bot

Bot em **C#** utilizando **Discord.Net** que monitora o número de jogadores online em um servidor de DayZ através de mensagens em um canal específico e atualiza o status do bot. Também possui comandos slash para interagir com usuários.

---

## Funcionalidades

- Atualiza automaticamente o **status do bot** com o número de jogadores online.  
- Extrai o número de jogadores de mensagens normais ou embeds do canal especificado.  
- Comandos slash:
  - `/ping` → Responde com "Pong!".
  - `/players` → Mostra o número atual de jogadores online.  
- Suporte a múltiplas interações simultâneas.  

---

## Configuração

1. Clone o repositório ou adicione o código em um projeto **Console App (.NET 6 ou superior)**.
2. Instale os pacotes NuGet necessários:
   - `Discord.Net`
   - `Microsoft.Extensions.DependencyInjection`
3. No código, configure seu **token** do bot e o **ID do canal** que será monitorado:

```csharp
private static readonly string Token = "SEU_TOKEN_AQUI";
public static readonly ulong ChannelId = 1402103538833686598; 
````

4. Configure também o **ID do servidor/guild** para registrar os comandos:

```csharp
await _interactionService.RegisterCommandsToGuildAsync(1394412758115811491);
```

---

## Estrutura do Código

* `Program.cs` → Classe principal do bot:

  * Inicializa o `DiscordSocketClient` e o `InteractionService`.
  * Registra eventos: `Ready`, `InteractionCreated` e `MessageReceived`.
  * Contém métodos para buscar o número de jogadores e atualizar o status do bot.

* `CommandsModule.cs` → Módulo de comandos slash:

  * Comandos `/ping` e `/players`.

* Métodos importantes:

  * `GetPlayersFromChannel()` → Retorna o número de jogadores extraído da última mensagem do canal.
  * `ExtractPlayersFromMessage(IMessage message)` → Extrai o número de jogadores do conteúdo da mensagem ou embed.
  * `UpdateBotStatus(int players)` → Atualiza o status do bot com o número de jogadores online.

---

## Observações

* O bot adiciona `+2` ao número de jogadores ao mostrar no status, ajuste conforme necessário.
* Verifique se o bot possui permissão para ler mensagens e ver o canal definido.
* Mensagens monitoradas devem ter o formato: `Players: X/Y`.
* Funciona tanto para mensagens de texto simples quanto para embeds.

---

## Dependências

* [.NET 6+](https://dotnet.microsoft.com/)
* [Discord.Net](https://github.com/discord-net/Discord.Net)
* `Microsoft.Extensions.DependencyInjection`

---

## Uso

1. Compile e execute o bot.
2. Aguarde o console exibir `SEU_BOT está online!`.
3. O status do bot será atualizado automaticamente ao receber mensagens no canal configurado.
4. Use os comandos slash no servidor para testar:

   * `/ping`
   * `/players`


