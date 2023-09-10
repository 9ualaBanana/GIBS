namespace GIBS.Bot;

public partial class TelegramBot
{
    public record Options
    {
        internal const string Configuration = "TelegramBot";

        public string Token { get; init; } = default!;

        public string Username { get; init; } = default!;

        public Uri Host { get; init; } = default!;

        internal Uri WebhookUri => _webhookUri ??= new(Host, new PathString($"/{Token}").ToUriComponent());
        Uri? _webhookUri;
    }
}

static class TelegramBotOptionsExtensions
{
    internal static ITelegramBotBuilder ConfigureOptions(this ITelegramBotBuilder builder)
    {
        builder.Services.AddOptions<TelegramBot.Options>().BindConfiguration(TelegramBot.Options.Configuration);
        return builder;
    }
}
