using HeyRed.Mime;
using Telegram.Bot.Types.InputFiles;

namespace GIBS.MediaFiles;

/// <summary>
/// Represents a media file stored in one of the supported by Telegram forms.
/// </summary>
/// <remarks>
/// Media file can be stored on Telegram servers and identified with <see cref="FileId"/> or at any other <see cref="Location"/>.
/// </remarks>
public sealed partial class MediaFile
{
    public readonly long? Size;

    public string Extension { get; }

    public string MimeType { get; }

    /// <summary>
    /// Identifier for the file stored on Telegram servers which can be used to download or reuse this file.
    /// </summary>
    readonly string? FileId;

    /// <summary>
    /// URL where the file is stored and can be downloaded from.
    /// </summary>
    readonly Uri? Location;

    #region Initialization

    MediaFile(long? size, string extension, string fileId)
    {
        Size = size;
        Extension = extension;
        MimeType = MimeTypesMap.GetMimeType(extension);
        FileId = fileId;
        Location = null;
    }

    MediaFile(string extension, Uri location)
    {
        Size = null;
        Extension = extension;
        MimeType = MimeTypesMap.GetMimeType(extension);
        FileId = null;
        Location = location;
    }

    #endregion

    #region Conversions

    public static implicit operator InputOnlineFile(MediaFile this_) => this_.FileId is not null ? this_.FileId : this_.Location!.ToString();

    #endregion
}
