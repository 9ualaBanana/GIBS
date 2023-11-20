using GIBS.Bot;
using Telegram.Bot;

namespace GIBS.MediaFiles;

public sealed partial class MediaFile
{
    public class Downloader
    {
        readonly TelegramBot _bot;
        readonly HttpClient _httpClient;

        public Downloader(TelegramBot bot, IHttpClientFactory httpClientFactory)
        {
            _bot = bot;
            _httpClient = httpClientFactory.CreateClient();
        }

        internal async Task<FileInfo> UseAsyncToDownload(MediaFile mediaFile, string destinationPath, CancellationToken cancellationToken)
        {
            using var downloadedMediaFile = File.Create(destinationPath);

            if (mediaFile.FileId is not null)
                await _bot.GetInfoAndDownloadFileAsync(mediaFile.FileId, downloadedMediaFile, cancellationToken);
            else await
                (await _httpClient.GetStreamAsync(mediaFile.Location, cancellationToken))
                .CopyToAsync(downloadedMediaFile, cancellationToken);

            return new FileInfo(downloadedMediaFile.Name);
        }
    }
}
