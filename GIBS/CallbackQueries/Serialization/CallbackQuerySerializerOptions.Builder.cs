using NLog;
using ILogger = NLog.ILogger;

namespace Telegram.Callbacks;

internal partial record CallbackQuerySerializerOptions
{
    internal class Builder
    {
        readonly CallbackQuerySerializerOptions _options = new();

        readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        #region Directors

        internal CallbackQuerySerializerOptions BuildDefault()
            => this
            .DataAndArgumentsSeparator(_DefaultValueAndArgumentsSeparator)
            .ArgumentsSeparator(_DefaultArgumentsSeparator)
            .Build();

        const char _DefaultValueAndArgumentsSeparator = ' ';
        const char _DefaultArgumentsSeparator = ',';

        #endregion

        /// <param name="_">
        /// Must be different than <see cref="CallbackQuerySerializerOptions.DataAndArgumentsSeparator"/>
        /// set via <see cref="DataAndArgumentsSeparator(char)"/>.
        /// </param>
        internal Builder ArgumentsSeparator(char _)
        { _options.ArgumentsSeparator = _; return this; }

        /// <param name="_">
        /// Must be different than <see cref="CallbackQuerySerializerOptions.ArgumentsSeparator"/>
        /// set via <see cref="ArgumentsSeparator(char)"/>.
        /// </param>
        internal Builder DataAndArgumentsSeparator(char _)
        { _options.DataAndArgumentsSeparator = _; return this; }

        internal CallbackQuerySerializerOptions Build()
        {
            if (_options.DataAndArgumentsSeparator == _options.ArgumentsSeparator)
            {
                var exception = new InvalidOperationException(
                    $"{nameof(_options.DataAndArgumentsSeparator)} and {nameof(_options.ArgumentsSeparator)} can not be the same.");
                _logger.Fatal(exception);
                throw exception;
            }
            else return _options;    
        }
    }
}
