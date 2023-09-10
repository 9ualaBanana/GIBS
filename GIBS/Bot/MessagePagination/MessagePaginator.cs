using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace GIBS.Bot.MessagePagination;

public class MessagePaginator
{
    internal const int MessageLengthLimit = 4096;

    readonly ChunkedMessagesAutoStorage _chunkedMessagesAutoStorage;
    readonly MessagePaginatorControlButtons _controlButtons;

    public MessagePaginator(
        ChunkedMessagesAutoStorage chunkedMessagesAutoStorage,
        MessagePaginatorControlButtons controlButtons)
    {
        _chunkedMessagesAutoStorage = chunkedMessagesAutoStorage;
        _controlButtons = controlButtons;
    }

    internal static bool MustBeUsedToSend(string text) => text.Length > MessageLengthLimit;

    internal async Task<Message> SendPaginatedMessageAsyncUsing(
        TelegramBot bot,
        ChatId chatId,
        string text,
        InlineKeyboardMarkup? replyMarkup = null,
        bool? disableWebPagePreview = default,
        bool? disableNotification = default,
        bool? protectContent = default,
        int? replyToMessageId = null,
        bool? allowSendingWithoutReply = default,
        CancellationToken cancellationToken = default)
    {
        var chunkedText = new ChunkedText(text);
        replyMarkup = BuildReplyMarkupFor(chunkedText, replyMarkup);

        var message = await bot.SendMessageAsyncCore(chatId, chunkedText.NextChunk,
            replyMarkup,
            disableWebPagePreview, disableNotification, protectContent, replyToMessageId, allowSendingWithoutReply, cancellationToken);

        if (chunkedText.IsChunked)
            _chunkedMessagesAutoStorage.Add(new(message, chunkedText));

        return message;
    }


    InlineKeyboardMarkup? BuildReplyMarkupFor(ChunkedText chunkedText, InlineKeyboardMarkup? replyMarkup)
    {
        var controlButtons = _controlButtons.For(chunkedText);
        return replyMarkup is null ?
            controlButtons.ToArray() :
            replyMarkup.InlineKeyboard.Append(controlButtons).ToArray();
    }
}
