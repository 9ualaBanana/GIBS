using System.Text;
using Telegram.Bot.Types;
using Telegram.Callbacks;

namespace GIBS.CallbackQueries.Serialization;

public class CallbackQuerySerializer
{
    readonly CallbackQuerySerializerOptions _options;

    internal CallbackQuerySerializer(CallbackQuerySerializerOptions? options = default)
    {
        _options = options ?? new CallbackQuerySerializerOptions.Builder().BuildDefault();
    }

    public string Serialize<ECallbackData>(CallbackQuery<ECallbackData> callbackQuery)
        where ECallbackData : struct, Enum
    {
        var serializedCallbackQuery = new StringBuilder(
            $"{callbackQuery.Data.ToString().Replace(" ", null)}",
            CallbackQuery<ECallbackData>.MaxLength);

        if (callbackQuery.Arguments.Any())
            serializedCallbackQuery
                .Append(_options.DataAndArgumentsSeparator)
                .Append(string.Join(_options.ArgumentsSeparator, callbackQuery.Arguments));

        return serializedCallbackQuery.ToString();
    }

    /// <summary>
    /// Tries to deserialize <paramref name="callbackQuery"/> that contains <see cref="CallbackQuery.Data"/> to <typeparamref name="TCallbackQuery"/>.
    /// </summary>
    /// <returns>
    /// Deserialized <typeparamref name="TCallbackQuery"/> if <paramref name="callbackQuery"/> contains its serialized representation.
    /// </returns>
    internal TCallbackQuery? TryDeserialize<TCallbackQuery, ECallbackData>(CallbackQuery callbackQuery)
        where TCallbackQuery : CallbackQuery<ECallbackData>, new()
        where ECallbackData : struct, Enum
    {
        try { return Deserialize<TCallbackQuery, ECallbackData>(callbackQuery); }
        catch { return null; }
    }

    /// <summary>
    /// Deserializes <paramref name="callbackQuery"/> that contains <see cref="CallbackQuery.Data"/> to <typeparamref name="TCallbackQuery"/>.
    /// </summary>
    /// <returns>Deserialized <typeparamref name="TCallbackQuery"/>.</returns>
    internal TCallbackQuery Deserialize<TCallbackQuery, ECallbackData>(CallbackQuery callbackQuery)
        where TCallbackQuery : CallbackQuery<ECallbackData>, new()
        where ECallbackData : struct, Enum
    {
        var splitCallbackQuery = callbackQuery.Data!.Split(_options.DataAndArgumentsSeparator, StringSplitOptions.RemoveEmptyEntries);

        var data = Enum.Parse<ECallbackData>(splitCallbackQuery.First());
        var arguments = splitCallbackQuery.Length > 1 ?
            splitCallbackQuery.Last().Split(_options.ArgumentsSeparator, StringSplitOptions.RemoveEmptyEntries) :
            CallbackQuery<ECallbackData>.EmptyArguments;

        return new CallbackQuery<ECallbackData>.Builder<TCallbackQuery>()
            .Data(data)
            .Arguments(arguments)
            .Build();
    }
}
