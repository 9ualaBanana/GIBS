using GIBS.Bot;
using GIBS.Middleware.UpdateRouting.UpdateTypeRouting;
using Telegram.Bot.Types;

namespace GIBS.Middleware.UpdateRouting.MessageRouting;

public interface IMessageRouter : ISwitchableMiddleware<IMessageRouter, Message>
{
}

public abstract class MessageRouter : IMessageRouter
{
    protected abstract string PathFragment { get; }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        context.Request.Path += $"/{PathFragment}";
        await next(context);
    }

    public abstract bool Matches(Message message);
}

static class MessageRouterExtensions
{
    internal static ITelegramBotBuilder AddMessageRouter<TMessageRouter>(this ITelegramBotBuilder builder)
        where TMessageRouter : class, IMessageRouter
    {
        builder.Services
            .AddScoped<IUpdateTypeRouter, MessageRouterMiddleware>()
            .AddScoped<IMessageRouter, TMessageRouter>();
        return builder;
    }
}
