using GIBS.Media.Images;
using HeyRed.Mime;
using Telegram.Bot.Types;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace GIBS.MediaFiles;

public sealed partial class MediaFile
{
    public class Factory
    {
        readonly HttpClient _httpClient;

        readonly ILogger _logger;

        public Factory(IHttpClientFactory httpClientFactory, ILogger<Factory> logger)
        {
            _httpClient = httpClientFactory.CreateClient();
            _logger = logger;
        }

        public async Task<MediaFile> CreateAsyncFrom(Message message, CancellationToken cancellationToken)
        {
            if (message.Video is Video video)  // Check for video must precede the one for image because Photo is not null for videos too.
                return CreateFrom(video);
            else if (message.Photo is PhotoSize[] image)
                return CreateFrom(image);
#pragma warning disable IDE0150 // Prefer 'null' check over type check
            if (message.Document is Document)
                return CreateFromDocumentAttachedTo(message);
#pragma warning restore IDE0150 // Prefer 'null' check over type check
            else if (await message.Text.IsImageUriAsync(_httpClient, cancellationToken))
                return await CreateAsyncFrom(new Uri(message.Text!), cancellationToken);
            else
            {
                string errorMessage = $"{nameof(message)} doesn't represent {nameof(MediaFile)}.";
                var exception = new ArgumentException(errorMessage);
                _logger.LogCritical(exception, string.Empty);
                throw exception;
            }
        }

        /// <summary>
        /// Creates <see cref="MediaFile"/> from an array of images sent via Telegram
        /// representing a single image with different resolutions sorted from lowest to highest.
        /// </summary>
        /// <param name="image">
        /// Array of images representing a single image with different resolutions sorted from lowest to highest
        /// from the last of which <see cref="MediaFile"/> will be created.
        /// </param>
        /// <remarks>Resulting <see cref="MediaFile"/> will have <see cref="Extension.jpeg"/> as all <see cref="PhotoSize"/> shall have it.</remarks>
        /// <returns><see cref="MediaFile"/> created from the highest resolution <paramref name="image"/>.</returns>
        static MediaFile CreateFrom(PhotoSize[] image) => CreateFrom(image.Last());

        /// <summary>
        /// Creates <see cref="MediaFile"/> from an image sent via Telegram.
        /// </summary>
        /// <param name="image">Image from which <see cref="MediaFile"/> will be created.</param>
        /// <returns><see cref="MediaFile"/> created from the <paramref name="image"/>.</returns>
        static MediaFile CreateFrom(PhotoSize image) => new(image.FileSize, Path.GetExtension(".jpeg"), image.FileId);

        static MediaFile CreateFrom(Video video) => new(video.FileSize, Path.GetExtension(".mp4"), video.FileId);

        /// <exception cref="ArgumentException"><see cref="Common.Extension"/> of the document can't be deduced.</exception>
        static MediaFile CreateFromDocumentAttachedTo(Message message)
        {
            Document document;
            ArgumentNullException.ThrowIfNull(document = message.Document!);

            string extension = document.MimeType is not null ? MimeTypesMap.GetExtension(document.MimeType)
                : Path.GetExtension(document.FileName) ?? Path.GetExtension(message.Caption) ??
                throw new ArgumentException("Extension of the document attached to the message can't be deduced.", nameof(message));

            return new(document.FileSize, extension, document.FileId);
        }

        public async Task<MediaFile> CreateAsyncFrom(Uri imageUri, CancellationToken cancellationToken)
        {
            try { return await CreateAsyncCoreFrom(imageUri); }
            catch (Exception exception)
            { _logger.LogCritical(exception, $"Couldn't create {nameof(MediaFile)} from {nameof(imageUri)}"); throw; }


            async Task<MediaFile> CreateAsyncCoreFrom(Uri imageUri)
            {
                var resourceMimeType = await _httpClient.GetMimeTypeAsync(imageUri, cancellationToken);

                if (ImagesHelper.IsImageMimeType(resourceMimeType))
                    return new MediaFile(MimeTypesMap.GetExtension(resourceMimeType), imageUri);
                else throw new ArgumentException(
                    $"{nameof(imageUri)} doesn't refer to an image based on MIME type of the resource.", nameof(imageUri)
                    );
            }
        }
    }
}
