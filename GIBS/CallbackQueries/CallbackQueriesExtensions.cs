using GIBS.Bot;
using GIBS.CallbackQueries.Serialization;
using GIBS.Middleware.UpdateRouting.UpdateTypeRouting;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Telegram.Callbacks;

namespace GIBS.CallbackQueries;

static class CallbackQueriesExtensions
{
    internal static ITelegramBotBuilder AddCallbackQueriesCore(
        this ITelegramBotBuilder builder,
        CallbackQuerySerializerOptions? options = default)
    {
        builder.Services.TryAddScoped_<IUpdateTypeRouter, CallbackQueryRouterMiddleware>();
        builder.Services.TryAddSingleton(services => new CallbackQuerySerializer(
            options ??
            services.GetService<CallbackQuerySerializerOptions>() ??
            new CallbackQuerySerializerOptions.Builder().BuildDefault())
        );

        return builder;
    }
}
