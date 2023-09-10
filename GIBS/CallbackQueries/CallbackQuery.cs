using GIBS.CallbackQueries.Serialization;
using NLog;
using Telegram.Bot.Types;
using ILogger = NLog.ILogger;

namespace GIBS.CallbackQueries;

/// <summary>
/// <see cref="CallbackQuery.Data"/> wrapper that contains type-specific <typeparamref name="ECallbackData"/>
/// with optional arguments and can be handled by its corresponding <see cref="CallbackQueryHandler{TCallbackQuery, ECallbackData}"/>.
/// </summary>
/// <remarks>
/// <see cref="CallbackQuerySerializer"/> should be used to serialize/deserialize instances of this class to/from <see cref="string"/>.
/// </remarks>
/// <typeparam name="ECallbackData">
/// <see cref="CallbackQuery.Data"/> based on which <see cref="CallbackQuery{ECallbackData}"/> is matched
/// to its corresponding <see cref="CallbackQueryHandler{TCallbackQuery, ECallbackData}"/>.
/// </typeparam>
public abstract record CallbackQuery<ECallbackData>
    where ECallbackData : struct, Enum
{
    internal const int MaxLength = 64;
    internal static object[] EmptyArguments = Array.Empty<object>();

    public ECallbackData Data { get; private set; }
    public object[] Arguments { get; private set; } = EmptyArguments;

    protected object ArgumentAt(int index)
    {
        if (Arguments.ElementAtOrDefault(index) is object argument)
            return argument;
        else
        {
            var exception = new ArgumentNullException(nameof(argument), "Missing required callback query argument.");
            _logger.Fatal(exception.Message);
            throw exception;
        }
    }

    readonly static ILogger _logger = LogManager.GetCurrentClassLogger();

    public class Builder<TCallbackQuery>
        where TCallbackQuery : CallbackQuery<ECallbackData>, new()
    {
        readonly TCallbackQuery _callbackQuery = new();

        public Builder<TCallbackQuery> Data(ECallbackData _)
        { _callbackQuery.Data = _; return this; }

        public Builder<TCallbackQuery> Arguments(params object[] _)
        { _callbackQuery.Arguments = _; return this; }

        public TCallbackQuery Build() => _callbackQuery;
    }
}
