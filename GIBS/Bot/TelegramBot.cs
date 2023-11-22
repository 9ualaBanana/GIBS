using GIBS.Bot.MessagePagination;
using Microsoft.Extensions.Options;
using System.Net;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace GIBS.Bot;

public partial class TelegramBot : TelegramBotClient
{
    public readonly ILogger Logger;

    readonly Options _options;
    readonly MessagePaginator _messagePaginator;

    public TelegramBot(IOptions<Options> options, MessagePaginator messagePaginator, ILogger<TelegramBot> logger)
        : base(options.Value.Token)
    {
        _options = options.Value;
        _messagePaginator = messagePaginator;
        Logger = logger;
    }

    /// <summary>
    /// Must be called before <see cref="WebApplication.Run(string?)"/>.
    /// </summary>
    internal async Task InitializeAsync(
        InputFileStream? certificate = default,
        int? maxConnections = default,
        IEnumerable<UpdateType>? allowedUpdates = default,
        bool? dropPendingUpdates = default,
        CancellationToken cancellationToken = default)
    {
        await this.SetWebhookAsync(_options.WebhookUri.ToString(), certificate, default, maxConnections, allowedUpdates, dropPendingUpdates, cancellationToken);
        Logger.LogTrace("Webhook is set for {Host}", _options.WebhookUri);
    }

    public async Task<Message> SendImageAsync_(
        ChatId chatId,
        InputOnlineFile image,
        string? caption = null,
        IReplyMarkup? replyMarkup = null,
        bool? disableNotification = null,
        bool? protectContent = null,
        CancellationToken cancellationToken = default) => await this.SendPhotoAsync(
            chatId,
            image,
            caption?.Sanitize(),
            ParseMode.MarkdownV2,
            captionEntities: null,
            disableNotification,
            protectContent,
            replyMarkup: replyMarkup,
            cancellationToken: cancellationToken);

    public async Task<Message> SendVideoAsync_(
        ChatId chatId,
        InputOnlineFile video,
        IReplyMarkup? replyMarkup = null,
        int? duration = null,
        int? width = null,
        int? height = null,
        InputMedia? thumb = default,
        string? caption = null,
        bool? supportsStreaming = default,
        bool? disableNotification = default,
        bool? protectContent = default,
        CancellationToken cancellationToken = default) => await this.SendVideoAsync(
            chatId,
            video,
            duration,
            width,
            height,
            thumb,
            caption?.Sanitize(),
            ParseMode.MarkdownV2,
            captionEntities: null,
            supportsStreaming,
            disableNotification,
            protectContent,
            replyMarkup: replyMarkup,
            cancellationToken: cancellationToken);

    /// <inheritdoc cref="SendAlbumAsync_(ChatId, IAlbumInputMedia[], string?, bool?, bool?, int?, bool?, CancellationToken)"/>
    public async Task<Message[]> SendAlbumAsync_(
        ChatId chatId,
        IEnumerable<IAlbumInputMedia> media,
        string? caption = null,
        bool? disableNotification = default,
        bool? protectContent = default,
        int? replyToMessageId = null,
        bool? allowSendingWithoutReply = default,
        CancellationToken cancellationToken = default)
        => await SendAlbumAsync_(chatId, media.ToArray(), caption, disableNotification, protectContent, replyToMessageId, allowSendingWithoutReply, cancellationToken);
    /// <remarks>
    /// If <paramref name="caption"/> is not <see langword="null"/>,
    /// it becomes the caption for the album and all captions of its individual media files are removed.
    /// </remarks>
    public async Task<Message[]> SendAlbumAsync_(
        ChatId chatId,
        IAlbumInputMedia[] media,
        string? caption = null,
        bool? disableNotification = default,
        bool? protectContent = default,
        int? replyToMessageId = null,
        bool? allowSendingWithoutReply = default,
        CancellationToken cancellationToken = default)
    {
        if (caption is not null)
            media.Caption(caption);

        return (await SendAlbumsAsync().AggregateAsync(new List<Message>(), (albums, album) => { albums.AddRange(album); return albums; }, cancellationToken)).ToArray();


        async IAsyncEnumerable<Message[]> SendAlbumsAsync()
        {
            const int AlbumSize = 10;
            foreach (var album in media.Chunk(AlbumSize))
                yield return await this.SendMediaGroupAsync(chatId, album, disableNotification, protectContent, replyToMessageId, allowSendingWithoutReply, cancellationToken);
        }
    }

    public async Task<Message> SendMessageAsync_(
        ChatId chatId,
        string text,
        InlineKeyboardMarkup? replyMarkup = null,
        bool? disableWebPagePreview = default,
        bool? disableNotification = default,
        bool? protectContent = default,
        int? replyToMessageId = null,
        bool? allowSendingWithoutReply = default,
        CancellationToken cancellationToken = default)
        => await (MessagePaginator.MustBeUsedToSend(text) ?
        _messagePaginator.SendPaginatedMessageAsyncUsing(this, chatId, text, replyMarkup, disableWebPagePreview, disableNotification, protectContent, replyToMessageId, allowSendingWithoutReply, cancellationToken) :
        SendMessageAsyncCore(chatId, text, replyMarkup, disableWebPagePreview, disableNotification, protectContent, replyToMessageId, allowSendingWithoutReply, cancellationToken));

    internal async Task<Message> SendMessageAsyncCore(
        ChatId chatId,
        string text,
        IReplyMarkup? replyMarkup = null,
        bool? disableWebPagePreview = default,
        bool? disableNotification = default,
        bool? protectContent = default,
        int? replyToMessageId = null,
        bool? allowSendingWithoutReply = default,
        CancellationToken cancellationToken = default) => await this.SendTextMessageAsync(
            chatId,
            text.Sanitize(),
            ParseMode.MarkdownV2,
            entities: null,
            disableWebPagePreview,
            disableNotification,
            protectContent,
            replyToMessageId,
            allowSendingWithoutReply,
            replyMarkup,
            cancellationToken);

    /// <returns>
    /// Edited <see cref="Message"/> or <see langword="null"/> if its contents haven't been modified.
    /// </returns>
    public async Task<Message?> EditMessageAsync_(
        ChatId chatId,
        int messageId,
        string text,
        InlineKeyboardMarkup? replyMarkup = null,
        bool? disableWebPagePreview = default,
        CancellationToken cancellationToken = default)
    {
        try { return await EditMessageAsyncCore(); }
        catch (ApiRequestException ex)
            when ((HttpStatusCode)ex.ErrorCode is HttpStatusCode.BadRequest && ex.Message.Contains("message is not modified"))
        { Logger.LogDebug("Message ({ID}) hasn't been modified.", messageId); return null; }

        async Task<Message> EditMessageAsyncCore()
        {
            var editedMessage = await this.EditMessageTextAsync(
                chatId,
                messageId,
                text.Sanitize(),
                ParseMode.MarkdownV2,
                entities: null,
                disableWebPagePreview,
                replyMarkup,
                cancellationToken);
            if (replyMarkup is not null)
                editedMessage = await this.EditMessageReplyMarkupAsync(
                    chatId,
                    messageId,
                    replyMarkup,
                    cancellationToken);

            return editedMessage;
        }
    }
}
