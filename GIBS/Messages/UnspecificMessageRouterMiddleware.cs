using GIBS.Middleware.UpdateRouting.MessageRouting;
using Telegram.Bot.Types;

namespace GIBS.Messages;

public class UnspecificMessageRouterMiddleware : MessageRouter
{
    protected override string PathFragment => MessagesController.PathFragment;

    public override bool Matches(Message message) => true;
}
