using Telegram.Bot.Types;

namespace GIBS.Bot.MessagePagination;

/// <summary>
/// Represents a sent <see cref="Types.Message"/> with its content as <see cref="ChunkedText"/>.
/// </summary>
/// <remarks>
/// Intended for using it to edit the <see cref="Message"/> via bot to display different chunks of its <see cref="Content"/>
/// because Telegram limits the number of bytes that can be sent in one message.
/// </remarks>
internal class ChunkedMessage : IEquatable<ChunkedMessage>
{
    /// <summary>
    /// The sent message whose content is <see cref="Content"/> or its part
    /// if its <see cref="ChunkedText.IsChunked"/> property is <see langword="true"/>.
    /// </summary>
    internal readonly Message Message;
    internal readonly ChunkedText Content;

    /// <summary>
    /// Initializes <see cref="ChunkedMessage"/> from <paramref name="sentMessage"/>
    /// whose content is <paramref name="sentMessageContent"/> or its part
    /// if its <see cref="ChunkedText.IsChunked"/> property is <see langword="true"/>.
    /// </summary>
    internal ChunkedMessage(Message sentMessage, ChunkedText sentMessageContent)
    {
        Message = sentMessage;
        Content = sentMessageContent;
    }

    // We don't need a constructor that defines single Message parameter because we always
    // preinitialize an instance of ChunkedText to send as the content of that Message.

    #region Equaity

    public static bool operator ==(ChunkedMessage this_, ChunkedMessage that) => this_.Equals(that);
    public static bool operator !=(ChunkedMessage this_, ChunkedMessage that) => !this_.Equals(that);
    public override bool Equals(object? obj) => Equals(obj as ChunkedMessage);
    public bool Equals(ChunkedMessage? other)
        => other is not null && UniqueMessage.From(Message) == UniqueMessage.From(other.Message);
    /// <summary>
    /// Same as <see cref="MessageExtensions.HashCode(Message)"/>
    /// </summary>
    public override int GetHashCode() => Message.HashCode();

    #endregion

    #region Conversions

    public static implicit operator Message(ChunkedMessage this_)
        => this_.Message;

    #endregion
}

internal static class MessageExtensions
{
    /// <summary>
    /// Hash code based on ID of the <see cref="Chat"/> where the <paramref name="message"/> came from and the ID of the message itself.
    /// </summary>
    internal static int HashCode(this Message message) => UniqueMessage.From(message).GetHashCode();
}
