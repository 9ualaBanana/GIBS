using GIBS.Bot;
using Microsoft.Extensions.Options;
using Telegram.Bot.Types;

namespace GIBS.Middleware.UpdateRouting;

/// <summary>
/// Redirects requests from Telegram containing <see cref="Update"/>s to corresponding update routing branch.
/// </summary>
/// <remarks>
/// Abstracts routing by rewriting the request path to remove <see cref="TelegramBot.Options.Token"/>
/// and move the rest to <see cref="HttpRequest.PathBase"/> which is not considered in routing.
/// </remarks>
public class UpdateRoutingMiddleware : IMiddleware
{
    readonly TelegramBot.Options _botOptions;
    readonly UpdateReaderMiddleware _updateReaderMiddleware;

    public UpdateRoutingMiddleware(
        IOptions<TelegramBot.Options> botOptions,
        UpdateReaderMiddleware updateReaderMiddleware)
    {
        _botOptions = botOptions.Value;
        _updateReaderMiddleware = updateReaderMiddleware;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (context.Request.Path.Value?.Contains(_botOptions.Token) is true)
        {
            context.Request.PathBase = PathString.FromUriComponent(context.Request.Path.Value.TrimEnd('/'));
            context.Request.Path = PathString.Empty;
            await _updateReaderMiddleware.InvokeAsync(context, next);
        }
        else await next(context);
    }
}
