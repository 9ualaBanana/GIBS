using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace GIBS.CallbackQueries;

[ApiController]
[Route($"/{PathFragment}")]
public class CallbackQueriesController : ControllerBase
{
    internal const string PathFragment = "callback_query";

    readonly ILogger _logger;

    public CallbackQueriesController(ILogger<CallbackQueriesController> logger)
    {
        _logger = logger;
    }

    [HttpPost]
    public async Task Handle([FromServices] IEnumerable<ICallbackQueryHandler> callbackQueryHandlers)
    {
        try { await Handle(); }
        catch (Exception)
        { _logger.LogCritical("Callback query wasn't handled"); throw; }


        async Task Handle()
        {
            var callbackQuery = HttpContext.GetUpdate().CallbackQuery!;

            if (callbackQuery.Data is not null)

                if (callbackQueryHandlers.Switch(callbackQuery) is ICallbackQueryHandler callbackQueryHandler)
                    await callbackQueryHandler.HandleAsync();
                else throw new NotImplementedException(
                    $"None of registered implementations of {nameof(ICallbackQueryHandler)} matched received callback query: {callbackQuery.Data}"
                    );

            else throw new NullReferenceException($"{nameof(CallbackQuery.Data)} must contain a serialized {typeof(CallbackQuery<>)}");
        }
    }
}
