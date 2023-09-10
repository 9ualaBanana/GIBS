using GIBS.Bot;
using GIBS.Commands.SyntacticAnalysis;
using GIBS.Middleware.UpdateRouting.MessageRouting;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace GIBS.Commands;

static class CommandsExtensions
{
    internal static ITelegramBotBuilder AddCommandsCore(this ITelegramBotBuilder builder)
    {
        builder.Services.TryAddScoped_<IMessageRouter, CommandRouterMiddleware>();
        builder.Services.TryAddScoped<Command.Factory>();
        builder.Services.TryAddSingleton<Command.Received>();
        builder.AddCommandsParsing();

        return builder;
    }
}
