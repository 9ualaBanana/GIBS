using System.Diagnostics.CodeAnalysis;
using Telegram.Bot.Types;

namespace GIBS.Media.Images;

static class ImagesHelper
{
    internal static async Task<bool> IsImageAsync(this Message message, HttpClient httpClient, CancellationToken cancellationToken)
        => message.Document.IsImage() ||
        message.Photo is not null ||
        message.Text is not null && Uri.IsWellFormedUriString(message.Text, UriKind.Absolute) && await new Uri(message.Text).IsImageUriAsync(httpClient, cancellationToken);

    internal static async Task<bool> IsImageUriAsync([NotNullWhen(true)] this string? uri, HttpClient httpClient, CancellationToken cancellationToken)
        => uri is not null && await new Uri(uri).IsImageUriAsync(httpClient, cancellationToken);

    internal static async Task<bool> IsImageUriAsync(this Uri uri, HttpClient httpClient, CancellationToken cancellationToken)
    {
        try { return IsImageMimeType(await httpClient.GetMimeTypeAsync(uri, cancellationToken)); }
        catch { return false; }
    }

    internal static async Task<string?> GetMimeTypeAsync(this HttpClient httpClient, Uri uri, CancellationToken cancellationToken)
        => (await httpClient.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead, cancellationToken))
        .Content.Headers.ContentType?.MediaType;

    internal static bool IsImage(this Document? document)
        => document is not null && IsImageMimeType(document.MimeType);

    internal static bool IsImageMimeType(string? mimeType) => mimeType is not null &&
        (mimeType.StartsWith("image") || mimeType.Equals("application/postscript"));
}
