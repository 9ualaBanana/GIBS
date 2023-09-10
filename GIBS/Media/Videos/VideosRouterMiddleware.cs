using GIBS.Middleware.UpdateRouting.MessageRouting;
using Telegram.Bot.Types;

namespace GIBS.Media.Videos;

public class VideosRouterMiddleware : MessageRouter
{
    protected override string PathFragment => VideosController.PathFragment;

    public override bool Matches(Message message) => message.IsVideo();
}
