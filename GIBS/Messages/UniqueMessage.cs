using Telegram.Bot.Types;

/// <summary>
/// Represents <see cref="Message"/> unique across all chats.
/// </summary>
/// <remarks>
/// Both <paramref name="ChatId"/> and <paramref name="MessageId"/> are needed to uniquely identify messages
/// across all chats because <paramref name="MessageId"/> is unique only for the given <paramref name="ChatId"/>.
/// </remarks>
public record struct UniqueMessage(ChatId ChatId, int MessageId)
{
    public static UniqueMessage From(Message message)
        => new(message.Chat.Id, message.MessageId);
}
