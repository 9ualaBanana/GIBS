namespace GIBS.Bot;

public static partial class TelegramBotExceptionHandler
{
    /// <param name="Subscribers"><see cref="ChatId"/>s of users that should be notified about unhandled exceptions.</param>
    public record Options(HashSet<long> Subscribers)
    {
        internal const string Configuration = "Exceptions";

        public Options()
            : this((HashSet<long>)null!)
        {
        }
    }
}
