using GIBS.Bot;
using GIBS.Commands.LexicalAnalysis;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace GIBS.Commands.SyntacticAnalysis;

internal static class CommandsSyntacticAnalysisExtensions
{
    internal static ITelegramBotBuilder AddCommandsParsing(this ITelegramBotBuilder builder)
    {
        builder.AddCommandsTokenization();
        builder.Services.TryAddScoped<Command.Parser>();

        return builder;
    }
}
