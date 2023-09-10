using GIBS.Bot;
using System.Security.Claims;
using Telegram.Bot.Types;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace GIBS;

public abstract class UpdateHandler_ : IHandler
{
    protected readonly TelegramBot Bot;
    protected readonly ILogger Logger;

    protected Update Update => Context.GetUpdate();
    protected ClaimsPrincipal User => Context.User;
    protected CancellationToken RequestAborted => Context.RequestAborted;
    protected HttpContext Context => _httpContextAccessor.HttpContext!;

    readonly IHttpContextAccessor _httpContextAccessor;

    protected UpdateHandler_(TelegramBot bot, IHttpContextAccessor httpContextAccessor, ILogger logger)
    {
        Bot = bot;
        Logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    public abstract Task HandleAsync();
}
