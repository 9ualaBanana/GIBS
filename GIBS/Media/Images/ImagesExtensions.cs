using GIBS.Bot;
using GIBS.Middleware.UpdateRouting.MessageRouting;

namespace GIBS.Media.Images;

static class ImagesExtensions
{
    internal static ITelegramBotBuilder AddImagesCore(this ITelegramBotBuilder builder)
    {
        builder.Services.TryAddScoped_<IMessageRouter, ImagesRouterMiddleware>();
        builder.AddMediaFiles();

        return builder;
    }
}
