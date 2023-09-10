using Telegram.Bot.Types;

namespace GIBS.Media.Videos;

static class VideosHelper
{
    internal static bool IsVideo(this Message message)
        => message.Document.IsVideo() || message.Video is not null;

    internal static bool IsVideo(this Document? document)
        => document?.MimeType?.StartsWith("video") is true;
}
