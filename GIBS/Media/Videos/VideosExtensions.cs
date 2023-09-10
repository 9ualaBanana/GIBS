using GIBS.Bot;
using GIBS.Middleware.UpdateRouting.MessageRouting;

namespace GIBS.Media.Videos;

static class VideosExtensions
{
    internal static ITelegramBotBuilder AddVideosCore(this ITelegramBotBuilder builder)
    {
        builder.Services.AddScoped<IMessageRouter, VideosRouterMiddleware>();
        builder.AddMediaFiles();

        return builder;
    }
}
