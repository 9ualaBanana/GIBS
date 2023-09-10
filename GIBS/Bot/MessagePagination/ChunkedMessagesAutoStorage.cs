using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using Telegram.Bot.Types;

namespace GIBS.Bot.MessagePagination;

/// <summary>
/// Stores <see cref="ChunkedMessage"/>s for a configured amount of time
/// which is being reset for each specific <see cref="ChunkedMessage"/> when it is accessed.
/// </summary>
public class ChunkedMessagesAutoStorage
{
    /// <summary>
    /// Maps the hash of regular <see cref="Message"/>s received in <see cref="Update"/>s
    /// to <see cref="ChunkedMessage"/>s stored inside <see cref="_notExpiredMessages"/> solely for efficient lookup.
    /// </summary>
    readonly Dictionary<UniqueMessage, ChunkedMessage> _chunkedMessagesRegistry;
    // We can't store messages in memory indefinitely so they should expire if not needed.
    readonly AutoStorage<ChunkedMessage> _notExpiredMessages;

    public ChunkedMessagesAutoStorage()
    {
        _chunkedMessagesRegistry = new();
        _notExpiredMessages = new AutoStorage<ChunkedMessage>(defaultStorageTime: TimeSpan.FromMinutes(30));
        _notExpiredMessages.ItemStorageTimeElapsed += (_, e)
            => _chunkedMessagesRegistry.Remove(UniqueMessage.From(e.Value.Message));
    }

    /// <remarks>
    /// This method won't add <paramref name="chunkedMessage"/> if it's already present in the storage.
    /// </remarks>
    internal void Add(ChunkedMessage chunkedMessage)
    {
        _notExpiredMessages.Add(chunkedMessage);
        _chunkedMessagesRegistry.TryAdd(UniqueMessage.From(chunkedMessage), chunkedMessage);
    }

    internal bool TryGet(Message message, [MaybeNullWhen(false)] out ChunkedMessage storedChunkedMessage)
        => TryGet(UniqueMessage.From(message), out storedChunkedMessage);

    internal bool TryGet(UniqueMessage message, [MaybeNullWhen(false)] out ChunkedMessage storedChunkedMessage)
    {
        if (_chunkedMessagesRegistry.TryGetValue(message, out storedChunkedMessage))
            if (!_notExpiredMessages.TryResetStorageTime(storedChunkedMessage))
                // We didn't make it in time to reset the storage time so the message is already expired.
                storedChunkedMessage = null;

        return storedChunkedMessage is not null;
    }
}
