using GIBS.CallbackQueries.Serialization;
using Telegram.Bot.Types.ReplyMarkups;

namespace GIBS.Bot.MessagePagination;

public class MessagePaginatorControlButtons
{
    readonly CallbackQuerySerializer _serializer;

    public MessagePaginatorControlButtons(CallbackQuerySerializer serializer)
    {
        _serializer = serializer;
    }

    /// <inheritdoc cref="For(ChunkedText)"/>
    internal InlineKeyboardMarkup For(ChunkedMessage chunkedMessage)
    {
        var controlButtons = For(chunkedMessage.Content);
        return NonControlButtonsOf(chunkedMessage).Append(controlButtons).ToArray();


        static IEnumerable<IEnumerable<InlineKeyboardButton>> NonControlButtonsOf(ChunkedMessage chunkedMessage)
            => chunkedMessage.Content.IsChunked ?
            chunkedMessage.Message.ReplyMarkup!.InlineKeyboard.SkipLast(1) : // Exclude control buttons.
            chunkedMessage.Message.ReplyMarkup?.InlineKeyboard ?? Enumerable.Empty<IEnumerable<InlineKeyboardButton>>(); // Has no control buttons.
    }

    /// <remarks>
    /// Make sure to call it before <see cref="ChunkedText.NextChunk"/> but after <see cref="ChunkedText.MovePointerToBeginningOfPreviousChunk"/> to avoid logical errors.
    /// </remarks>
    /// <returns>Paginator control buttons that depend on current position of <see cref="ChunkedText._pointer"/>.</returns>
    internal IEnumerable<InlineKeyboardButton> For(ChunkedText chunkedText) =>
        !chunkedText.IsChunked ? Enumerable.Empty<InlineKeyboardButton>() :
        chunkedText.IsAtFirstChunk ? new InlineKeyboardButton[] { Next } :
        chunkedText.IsAtLastChunk ? new InlineKeyboardButton[] { Previous } :
        PreviousAndNext;

    InlineKeyboardButton[] PreviousAndNext => new InlineKeyboardButton[] { Previous, Next };

    internal InlineKeyboardButton Previous
        => InlineKeyboardButton.WithCallbackData("<",
            _serializer.Serialize(new MessagePaginatorCallbackQuery.Builder<MessagePaginatorCallbackQuery>()
                .Data(MessagePaginatorCallbackData.Previous)
                .Build())
            );

    internal InlineKeyboardButton Next
        => InlineKeyboardButton.WithCallbackData(">",
            _serializer.Serialize(new MessagePaginatorCallbackQuery.Builder<MessagePaginatorCallbackQuery>()
                .Data(MessagePaginatorCallbackData.Next)
                .Build())
            );
}
