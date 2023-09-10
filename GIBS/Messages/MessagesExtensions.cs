using GIBS.Bot;
using GIBS.Middleware.UpdateRouting.MessageRouting;
using GIBS.Middleware.UpdateRouting.UpdateTypeRouting;

namespace GIBS.Messages;

static class MessagesExtensions
{
    internal static ITelegramBotBuilder AddMessagesCore(this ITelegramBotBuilder builder)
    {
        builder.Services
            .TryAddScoped_<IUpdateTypeRouter, MessageRouterMiddleware>()
            .TryAddScoped_<IMessageRouter, UnspecificMessageRouterMiddleware>();

        return builder;
    }
}
