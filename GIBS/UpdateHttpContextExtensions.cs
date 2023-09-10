using GIBS.Middleware.UpdateRouting;
using NLog;
using Telegram.Bot.Types;
using ILogger = NLog.ILogger;

namespace GIBS;

public static class UpdateHttpContextExtensions
{
    readonly static ILogger _logger = LogManager.GetCurrentClassLogger();

    /// <summary>
    /// Caches <paramref name="update"/> inside <see cref="HttpContext.Items"/>
    /// from where it can be retrieved using <see cref="GetUpdate(HttpContext)"/>.
    /// </summary>
    /// <remarks>
    /// Intended to be called once per request by <see cref="UpdateReaderMiddleware"/>.
    /// Following call attempts will result in an exception.
    /// </remarks>
    /// <exception cref="ArgumentException">Thrown when this method is called more than once per request.</exception>
    internal static void SetUpdate(this HttpContext context, Update update)
        => context.Items.Add(_cacheKey, update);

    /// <summary>
    /// Retrieves <see cref="Update"/> instance that is cached inside <see cref="HttpContext.Items"/>.
    /// </summary>
    /// <remarks>
    /// Attempting to <see cref="GetUpdate(HttpContext)"/> before it was cached by <see cref="SetUpdate(HttpContext, Update)"/> results in an exception.
    /// </remarks>
    /// <exception cref="InvalidOperationException"/>
    public static Update GetUpdate(this HttpContext context)
    {
        if (context.Items[_cacheKey] is Update update)
            return update;
        else
        {
            var exception = new InvalidOperationException($"{nameof(SetUpdate)} before attempting to {nameof(GetUpdate)}.");
            _logger.Fatal(exception);
            throw exception;
        }
    }

    public static bool ContainsUpdate(this HttpContext context)
        => context.Items[_cacheKey] is not null;

    static readonly object _cacheKey = new();
}
