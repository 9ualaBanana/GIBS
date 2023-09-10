using GIBS.Bot;
using GIBS.CallbackQueries.Serialization;
using GIBS.MediaFiles;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace GIBS.Media.Images;

public abstract class ImageHandler_ : MediaHandler_
{
    protected ImageHandler_(
        MediaFilesCache cache,
        MediaFile.Factory factory,
        CallbackQuerySerializer callbackQuerySerializer,
        TelegramBot bot,
        IHttpContextAccessor httpContextAccessor,
        ILogger logger)
        : base(cache, factory, callbackQuerySerializer, bot, httpContextAccessor, logger)
    {
    }
}
