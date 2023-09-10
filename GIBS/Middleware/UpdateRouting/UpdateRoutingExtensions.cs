using GIBS.Bot;
using GIBS.Middleware.UpdateRouting.UpdateTypeRouting;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace GIBS.Middleware.UpdateRouting;

internal static class UpdateRoutingExtensions
{
    internal static ITelegramBotBuilder AddUpdateRouting(this ITelegramBotBuilder builder)
    {
        builder.Services.TryAddScoped<UpdateRoutingMiddleware>();
        builder.Services.TryAddScoped<UpdateReaderMiddleware>();
        builder.Services.TryAddScoped<UpdateTypeRouterMiddleware>();

        return builder;
    }


    internal static WebApplication UseUpdateRouting(this WebApplication app)
    {
        app.MapControllers();
        app.UseMiddleware<UpdateRoutingMiddleware>().UseRouting();
        return app;
    }

}
