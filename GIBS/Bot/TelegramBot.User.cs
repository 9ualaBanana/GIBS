using System.Security.Claims;
using Telegram.Bot.Types;

namespace GIBS.Bot;

public partial class TelegramBot
{
    /// <summary>
    /// Represents <see cref="ClaimsPrincipal"/> that belong to a Telegram bot user uniquely identified by <see cref="ChatId"/>.
    /// </summary>
    public partial class User : ClaimsPrincipal
    {
        readonly public ChatId ChatId;

        public User(ClaimsPrincipal claimsPrincipal, ChatId chatId)
            : base(claimsPrincipal)
        {
            ChatId = chatId;
        }
    }
}

public static class TelegramBotUserExtensions
{
    public static TelegramBot.User ToTelegramBotUserWith(this ClaimsPrincipal user, ChatId chatId)
        => new(user, chatId);
}
