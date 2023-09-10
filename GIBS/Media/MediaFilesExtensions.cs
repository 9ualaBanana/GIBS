using GIBS.Bot;
using GIBS.MediaFiles;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace GIBS.Media;

static class MediaFilesExtensions
{
    internal static ITelegramBotBuilder AddMediaFiles(this ITelegramBotBuilder builder)
    {
        builder.Services.TryAddScoped<MediaFile.Factory>();
        builder.Services.TryAddSingleton<MediaFilesCache>();
        builder.Services.TryAddSingleton<MediaFile.Downloader>();

        return builder;
    }
}
