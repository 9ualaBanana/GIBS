using GIBS.CallbackQueries;
using GIBS.CallbackQueries.Serialization;
using Telegram.Bot.Types.Enums;

namespace GIBS.Bot.MessagePagination;

public class MessagePaginatorCallbackQueryHandler : CallbackQueryHandler<MessagePaginatorCallbackQuery, MessagePaginatorCallbackData>
{
    readonly ChunkedMessagesAutoStorage _chunkedMessagesAutoStorage;
    readonly MessagePaginatorControlButtons _controlButtons;

    public MessagePaginatorCallbackQueryHandler(
        ChunkedMessagesAutoStorage chunkedMessagesAutoStorage,
        MessagePaginatorControlButtons controlButtons,
        CallbackQuerySerializer serializer,
        TelegramBot bot,
        IHttpContextAccessor httpContextAccessor,
        ILogger<MessagePaginatorCallbackQueryHandler> logger)
        : base(serializer, bot, httpContextAccessor, logger)
    {
        _chunkedMessagesAutoStorage = chunkedMessagesAutoStorage;
        _controlButtons = controlButtons;
    }

    public override async Task HandleAsync(MessagePaginatorCallbackQuery callbackQuery)
    {
        if (_chunkedMessagesAutoStorage.TryGet(Message, out var paginatedMessage))
        {
            if (callbackQuery.Data is MessagePaginatorCallbackData.Previous)
                paginatedMessage.Content.MovePointerToBeginningOfPreviousChunk();
            var controlButtons = _controlButtons.For(paginatedMessage);
            var nextPage = paginatedMessage.Content.NextChunk.Sanitize();

            await Bot.EditMessageTextAsync(ChatId, paginatedMessage.Message.MessageId, nextPage, ParseMode.MarkdownV2);
            await Bot.EditMessageReplyMarkupAsync(ChatId, paginatedMessage.Message.MessageId, controlButtons);
        }
    }
}

public record MessagePaginatorCallbackQuery : CallbackQuery<MessagePaginatorCallbackData>
{
}

public enum MessagePaginatorCallbackData
{
    Previous,
    Next
}
