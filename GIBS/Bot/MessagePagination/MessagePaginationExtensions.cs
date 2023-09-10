using GIBS.CallbackQueries;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace GIBS.Bot.MessagePagination;

static class MessagePaginationExtensions
{
    internal static ITelegramBotBuilder AddMessagePagination(this ITelegramBotBuilder builder)
    {
        builder.AddCallbackQueriesCore();
        builder.Services.TryAddScoped_<ICallbackQueryHandler, MessagePaginatorCallbackQueryHandler>();
        builder.Services.TryAddSingleton<MessagePaginator>();
        builder.Services.TryAddSingleton<ChunkedMessagesAutoStorage>();
        builder.Services.TryAddSingleton<MessagePaginatorControlButtons>();

        return builder;
    }
}
