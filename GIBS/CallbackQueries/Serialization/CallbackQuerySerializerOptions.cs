namespace Telegram.Callbacks;

internal partial record CallbackQuerySerializerOptions
{
    internal char DataAndArgumentsSeparator { get; private set; }
    internal char ArgumentsSeparator { get; private set; }
}
