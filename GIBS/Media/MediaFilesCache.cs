using GIBS.MediaFiles;
using HeyRed.Mime;
using Microsoft.Extensions.Caching.Memory;

namespace GIBS.Media;

public class MediaFilesCache
{
    TimeSpan Expiration => TimeSpan.FromMilliseconds(_expiration);
    const int _expiration = 3_600_000;

    readonly MediaFile.Downloader _mediaFileDownloader;
    readonly IMemoryCache _cache;
    readonly IWebHostEnvironment _environment;

    readonly ILogger<MediaFilesCache> _logger;

    /// <summary>
    /// Local path to the directory that represents <see cref="MediaFilesCache"/>.
    /// </summary>
    /// <remarks>
    /// Accessing this property creates a corresponding directory if it doesn't exist yet.
    /// </remarks>
    public string Location
    {
        get
        {
            if (!Directory.Exists(_location))
                Directory.CreateDirectory(_location);
            return _location;
        }
    }
    readonly string _location;

    public MediaFilesCache(
        MediaFile.Downloader mediaFileDownloader,
        IMemoryCache cache,
        IWebHostEnvironment environment,
        ILogger<MediaFilesCache> logger)
    {
        _mediaFileDownloader = mediaFileDownloader;
        _cache = cache;
        _environment = environment;
        _location = RootedPath("cache");
        _logger = logger;
    }

    #region MediaFile

    public async Task<Entry> AddAsync(MediaFile mediaFile, CancellationToken cancellationToken)
        => await AddAsync(mediaFile, Expiration, cancellationToken);

    public async Task<Entry> AddAsync(MediaFile mediaFile, TimeSpan expiration, CancellationToken cancellationToken)
    {
        var entry = Entry.Of(this, mediaFile.Extension.ToString());
        await _mediaFileDownloader.UseAsyncToDownload(mediaFile, entry.File.FullName, cancellationToken);
        return Cache(entry, expiration);
    }

    #endregion

    #region IFormFile

    public async Task<Entry> AddAsync(IFormFile mediaFile, CancellationToken cancellationToken)
        => await AddAsync(mediaFile, Expiration, cancellationToken);

    public async Task<Entry> AddAsync(IFormFile mediaFile, TimeSpan expiration, CancellationToken cancellationToken)
    {
        var entry = Entry.Of(this, MimeTypesMap.GetExtension(mediaFile.ContentType));
        await DownloadAsync(mediaFile);
        return Cache(entry, expiration);


        async Task<FileInfo> DownloadAsync(IFormFile mediaFile)
        {
            using var downloadedMediaFile = entry.File.OpenWrite();
            await mediaFile.CopyToAsync(downloadedMediaFile, cancellationToken);

            return new FileInfo(downloadedMediaFile.Name);
        }
    }

    #endregion

    Entry Cache(Entry entry, TimeSpan expiration)
    {
        using var _ = _cache.Create(entry)
            .SetAbsoluteExpiration(expiration)
            .RegisterPostEvictionCallback((index, cachedMediaFile, _, _) =>
            {
                (cachedMediaFile as Entry)!.File.Delete();
                _logger.LogTrace("Media file with index {Index} has expired", index);
            });

        _logger.LogTrace("Media file with index {Index} was added to the cache", entry.Index);

        return entry;
    }

    public Entry? TryRetrieveMediaFileWith(Guid index)
    { _cache.TryGetValue(index, out Entry? cachedMediaFileEntry); return cachedMediaFileEntry; }

    string CachedPath(params string[] paths)
        => RootedPath(paths.Prepend(Location).ToArray());

    string RootedPath(params string[] paths)
        => Path.Combine(paths.Prepend(_environment.ContentRootPath).ToArray());


    /// <summary>
    /// Represents <see cref="MediaFilesCache"/> entry stored at <see cref="CachedPath(string[])"/> and backed by <see cref="File"/>.
    /// </summary>
    public record Entry
    {
        public readonly FileInfo File;

        /// <summary>
        /// Creates <see cref="Entry"/> of <paramref name="cache"/> stored at <see cref="CachedPath(string[])"/> and
        /// backed by <see cref="File"/> with <paramref name="extension"/>.
        /// </summary>
        /// <param name="cache"><see cref="MediaFilesCache"/> for which <see cref="Entry"/> will be created.</param>
        /// <param name="extension">Extension of the <see cref="File"/> by which resulting <see cref="Entry"/> will be backed.</param>
        /// <returns>
        /// <see cref="Entry"/> stored at <see cref="CachedPath(string[])"/> that is backed by <see cref="File"/> with <paramref name="extension"/>.
        /// </returns>
        internal static Entry Of(MediaFilesCache cache, string extension)
        {
            if (!extension.StartsWith('.'))
                extension = $".{extension}";

            return new Entry(
                new FileInfo(cache.CachedPath(Guid.NewGuid().ToString()) + extension)
                );
        }

        Entry(FileInfo file)
        {
            File = file;
        }

        /// <summary>
        /// Instances of <see cref="Entry"/> are stored under a unique <see cref="Index"/>
        /// which is also a name of <see cref="File"/> by which they are backed.
        /// </summary>
        public Guid Index => Guid.Parse(Path.GetFileNameWithoutExtension(File.Name));
    }
}

static class MediaFilesCacheExtensions
{
    internal static ICacheEntry Create(this IMemoryCache cache, MediaFilesCache.Entry entry)
        => cache.CreateEntry(entry.Index).SetValue(entry);
}
