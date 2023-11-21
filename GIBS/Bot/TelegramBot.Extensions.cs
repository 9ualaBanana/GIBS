using GIBS.Middleware.UpdateRouting;
using System.Text;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;

namespace GIBS.Bot;

public static class TelegramBotExtensions
{
    public static IWebHostBuilder ConfigureTelegramBot(
        this IWebHostBuilder builder,
        Action<ITelegramBotBuilder> configureTelegramBot)
    {
        builder
            .ConfigureAppConfiguration(_ => _
                .AddJsonFile("botsettings.json", optional: false, reloadOnChange: false)
                .AddJsonFile($"botsettings.{Environments.Development}.json", optional: true, reloadOnChange: false));

        TelegramBot.Builder.Default(builder.Services(), configureTelegramBot);

        return builder;
    }

    public static WebApplication UseTelegramBot(this WebApplication app)
        => app.UseUpdateRouting();

    public static async Task RunAsync_(this WebApplication app,
        InputFileStream? certificate = default,
        int? maxConnections = default,
        IEnumerable<UpdateType>? allowedUpdates = default,
        bool? dropPendingUpdates = default,
        CancellationToken cancellationToken = default)
    {
        await app.Services.GetRequiredService<TelegramBot>()
            .InitializeAsync(certificate, maxConnections, allowedUpdates, dropPendingUpdates, cancellationToken);
        app.Run();
    }

    #region Properties

    public static ChatId ChatId(this Update update) =>
        update.Message?.Chat.Id ??
        update.CallbackQuery?.Message?.Chat.Id ??
        update.InlineQuery?.From.Id ??
        update.ChosenInlineResult?.From.Id ??
        update.ChannelPost?.Chat.Id ??
        update.EditedChannelPost?.Chat.Id ??
        update.ShippingQuery?.From.Id ??
        update.PreCheckoutQuery?.From.Id!;

    public static User From(this Update update) =>
        update.Message?.From ??
        update.CallbackQuery?.From ??
        update.InlineQuery?.From ??
        update.ChosenInlineResult?.From ??
        update.ChannelPost?.From ??
        update.EditedChannelPost?.From ??
        update.ShippingQuery?.From ??
        update.PreCheckoutQuery?.From!;

    /// <summary>
    /// Sets <paramref name="caption"/> of the <paramref name="album"/>
    /// also removing captions from individual media files constituting it.
    /// </summary>
    /// <remarks>
    /// <paramref name="album"/> caption is set via the first media file constituting it.
    /// If any other constituent media files have their <see cref="InputMediaBase.Caption"/> set,
    /// then they are treated as captions of individual media files and not the <paramref name="album"/>.
    /// </remarks>
    public static IEnumerable<IAlbumInputMedia> Caption(this IAlbumInputMedia[] album, string caption)
    {
        if (album.FirstOrDefault() is InputMediaBase album_)
        {
            album_.Caption = caption;
            foreach (var mediaFile in album.Skip(1))
                (mediaFile as InputMediaBase)!.Caption = null;
        }

        return album;
    }

    #endregion

    public static string Sanitize(this string unsanitizedString)
    {
        return unsanitizedString
            .Replace("|", @"\|")
            .Replace("[", @"\[")
            .Replace("]", @"\]")
            .Replace("{", @"\{")
            .Replace("}", @"\}")
            .Replace(".", @"\.")
            .Replace("-", @"\-")
            .Replace("+", @"\+")
            .Replace("_", @"\_")
            .Replace(">", @"\>")
            .Replace("(", @"\(")
            .Replace(")", @"\)")
            .Replace("=", @"\=")
            .Replace("!", @"\!")
            .Replace("#", @"\#");
    }

    public static StringBuilder AppendHeader(this StringBuilder builder, string header)
        => builder
        .AppendLine(header)
        .AppendLine(HorizontalDelimeter);

    public const string HorizontalDelimeter = "------------------------------------------------------------------------------------------------";
}
