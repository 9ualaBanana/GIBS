using GIBS.Bot;
using Telegram.Bot.Types;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace GIBS.Messages;

public abstract class MessageHandler : MessageHandler_, ISwitchableService<MessageHandler, Message>
{
    protected MessageHandler(
        TelegramBot bot,
        IHttpContextAccessor httpContextAccessor,
        ILogger logger)
        : base(bot, httpContextAccessor, logger)
    {
    }

    public abstract bool Matches(Message message);
}
