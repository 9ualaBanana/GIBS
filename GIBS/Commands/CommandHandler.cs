using GIBS.Bot;
using GIBS.Messages;
using Telegram.Bot.Types;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace GIBS.Commands;

/// <summary>
/// Base class for <see cref="CommandHandler"/>s that should be used to handle <see cref="Target"/> and
/// also provides access to received <see cref="Command"/> to its children by calling abstract
/// <see cref="HandleAsync(Command)"/>
/// via publicly available <see cref="HandleAsync()"/>.
/// </summary>
public abstract class CommandHandler : MessageHandler_, ISwitchableService<CommandHandler, Command>
{
    protected readonly Command.Factory CommandFactory;

    readonly Command.Received _receivedCommand;

    /// <summary>
    /// The <see cref="Bot.Types.Message"/> which contains the command being handled.
    /// </summary>
    protected override Message Message => Update.Message!;

    protected CommandHandler(
        Command.Factory commandFactory,
        Command.Received receivedCommand,
        TelegramBot bot,
        IHttpContextAccessor httpContextAccessor,
        ILogger logger)
        : base(bot, httpContextAccessor, logger)
    {
        _receivedCommand = receivedCommand;
        CommandFactory = commandFactory;
    }

    public abstract Command Target { get; }

    /// <summary>
    /// Determines whether this <see cref="CommandHandler"/> is the one that should be used to handle the <paramref name="command"/>.
    /// </summary>
    /// <param name="command">The command that this <see cref="CommandHandler"/> should be able to handle.</param>
    /// <returns>
    /// <see langword="true"/> if this <see cref="CommandHandler"/> is the one that should be used
    /// to handle the <paramref name="command"/>; <see langword="false"/> otherwise.
    /// </returns>
    public bool Matches(Command command)
    {
        var receivedCommand = _receivedCommand.Get();

        if (receivedCommand == Target)
        { Context.Items[MatchedCommandIndex] = receivedCommand; return true; }
        else return false;
    }

    public override async Task HandleAsync()
    {
        if (Context.Items[MatchedCommandIndex] is Command matchedCommand)
            await HandleAsync(matchedCommand);
        else
        {
            var exception = new InvalidOperationException(
                $"{nameof(HandleAsync)} can be called only after this {nameof(CommandHandler)} matched the received command."
                );
            Logger.LogCritical(exception, message: default);
            throw exception;
        }
    }

    /// <summary>
    /// Key which maps to the <see cref="Command"/> that matched this <see cref="CommandHandler"/>.
    /// </summary>
    string MatchedCommandIndex => GetType().FullName!;

    protected abstract Task HandleAsync(Command receivedCommand);
}
