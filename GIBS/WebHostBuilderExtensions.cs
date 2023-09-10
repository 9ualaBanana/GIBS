namespace GIBS;

static class WebHostBuilderExtensions
{
    public static IServiceCollection Services(this IWebHostBuilder builder)
    {
        IServiceCollection services = default!; builder.ConfigureServices(_ => services = _);
        return services;
    }
}
