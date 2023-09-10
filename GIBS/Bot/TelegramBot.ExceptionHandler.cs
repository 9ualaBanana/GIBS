using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Options;
using NLog;
using System.Text;
using ILogger = NLog.ILogger;

namespace GIBS.Bot;

public static partial class TelegramBotExceptionHandler
{
    readonly static ILogger _logger = LogManager.GetCurrentClassLogger();

    public static ITelegramBotBuilder ConfigureExceptionHandlerOptions(this ITelegramBotBuilder builder)
    {
        builder.Services.AddOptions<Options>()
            .BindConfiguration($"{TelegramBot.Options.Configuration}:{Options.Configuration}");
        return builder;
    }


    public static IApplicationBuilder UseTelegramBotExceptionHandler(this IApplicationBuilder app)
        => app.UseExceptionHandler(_ => _.Run(async context =>
        {
            await TelegramBotExceptionHandler.InvokeAsync(context);

            // We tell Telegram the Update is handled.
            context.Response.StatusCode = 200;
            await context.Response.StartAsync();
        }));

    internal static async Task InvokeAsync(HttpContext context)
    {
        try
        {
            var bot = context.RequestServices.GetRequiredService<TelegramBot>();
            var subscribers = context.RequestServices.GetRequiredService<IOptionsSnapshot<Options>>().Value.Subscribers;

            foreach (var subscriber in subscribers)
                await bot.SendMessageAsync_(subscriber, ExceptionMessage(),
                    disableNotification: true, cancellationToken: context.RequestAborted
                    );
        }
        catch (Exception ex)
        { _logger.Error(ex, $"{nameof(TelegramBotExceptionHandler)} must not throw"); }


        string ExceptionMessage()
        {
            var exception = context.Features.GetRequiredFeature<IExceptionHandlerFeature>();
            return
                $"""
                `{exception.Path}` handler thrown an unhandled exception.

                {ExceptionDetails()}
                """;


            string ExceptionDetails()
            {
                var exceptionDetails = new StringBuilder()
                    .AppendLine($"*{exception.Error.Message}*");
                foreach (var innerException in InnerExceptions())
                    exceptionDetails.Append(" ---> ").AppendLine($"*{innerException.Message}*");
                exceptionDetails.AppendLine(exception.Error.StackTrace?.Replace(@"\", @"\\").Replace("`", @"\`"));

                return exceptionDetails.ToString();


                IEnumerable<Exception> InnerExceptions()
                {
                    var innerExceptions = new List<Exception>();
                    Exception? innerException = exception.Error;
                    while ((innerException = innerException?.InnerException) is not null)
                        innerExceptions.Add(innerException);

                    return innerExceptions;
                }
            }
    }
    }
}
