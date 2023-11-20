using GIBS.Bot;
using GIBS.CallbackQueries.Serialization;
using GIBS.Messages;
using Telegram.Bot;
using Telegram.Bot.Types;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace GIBS.CallbackQueries;

/// <summary>
/// Service type of <see cref="CallbackQueryHandler{TCallbackQuery, ECallbackData}"/> implementations.
/// </summary>
/// <remarks>
/// <see cref="CallbackQueryHandler{TCallbackQuery, ECallbackData}"/> implementations must be registered with this interface
/// as their service type because closed generic types can't be registered as implementations of an open generic service type.
/// </remarks>
public interface ICallbackQueryHandler : IHandler, ISwitchableService<ICallbackQueryHandler, CallbackQuery>
{
}

public abstract class CallbackQueryHandler<TCallbackQuery, ECallbackData> : MessageHandler_, ICallbackQueryHandler
    where TCallbackQuery : CallbackQuery<ECallbackData>, new()
    where ECallbackData : struct, Enum
{
    /// <summary>
    /// The <see cref="Bot.Types.Message"/> which contains the <see cref="Bot.Types.CallbackQuery"/> being handled.
    /// </summary>
    /// <remarks>
    /// Inline messages are not supported.
    /// </remarks>
    protected override Message Message => CallbackQuery.Message!;

    /// <summary>
    /// The <see cref="Bot.Types.CallbackQuery"/> being handled.
    /// </summary>
    protected CallbackQuery CallbackQuery => Update.CallbackQuery!;

    protected readonly CallbackQuerySerializer Serializer;

    protected CallbackQueryHandler(
        CallbackQuerySerializer serializer,
        TelegramBot bot,
        IHttpContextAccessor httpContextAccessor,
        ILogger logger)
        : base(bot, httpContextAccessor, logger)
    {
        Serializer = serializer;
    }

    /// <summary>
    /// The last <typeparamref name="TCallbackQuery"/> matched via this method will be handled in a call to <see cref="HandleAsync()"/> that may follow.
    /// </summary>
    /// <param name="callbackQuery">
    /// Serialized <typeparamref name="TCallbackQuery"/> that may be handled in a call to <see cref="HandleAsync()"/> if this handler is appropriate for it.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if this handler is appropriate for <paramref name="callbackQuery"/>; <see langword="false"/> otherwise.
    /// </returns>
    public bool Matches(CallbackQuery callbackQuery)
    {
        if (Serializer.TryDeserialize<TCallbackQuery, ECallbackData>(callbackQuery) is TCallbackQuery callbackQuery_)
        { Context.Items[MatchedCallbackQuery] = callbackQuery_; return true; }
        else return false;
    }

    /// <summary>
    /// Handles the last <typeparamref name="TCallbackQuery"/> that matched this handler in a call to <see cref="Matches(CallbackQuery)"/> or throws an exception if none matched.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// <typeparamref name="TCallbackQuery"/> hasn't matched this handler in a call to <see cref="Matches(CallbackQuery)"/> or that call didn't take place at all.
    /// </exception>
    public override async Task HandleAsync()
    {
        if (Context.Items[MatchedCallbackQuery] is TCallbackQuery callbackQuery)
        {
            await Bot.AnswerCallbackQueryAsync(CallbackQuery.Id, cancellationToken: RequestAborted);
            await HandleAsync(callbackQuery);
        }
        else
        {
            var exception = new InvalidOperationException(
                $"{nameof(HandleAsync)} can be called only after this {nameof(CallbackQueryHandler<TCallbackQuery, ECallbackData>)} matched the callback query.",
                new ArgumentNullException(nameof(callbackQuery))
                );
            Logger.LogCritical(exception.Message);
            throw exception;
        }
    }

    /// <summary>
    /// Key which maps to deserialized <typeparamref name="TCallbackQuery"/>.
    /// </summary>
    string MatchedCallbackQuery => GetType().FullName!;

    public abstract Task HandleAsync(TCallbackQuery callbackQuery);

    protected Task HandleUnknownCallbackData()
    {
        var exception = new ArgumentException($"Unknown {nameof(ECallbackData)}.");
        Logger.LogCritical(exception.Message);
        throw exception;
    }
}
