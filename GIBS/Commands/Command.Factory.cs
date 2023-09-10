using GIBS.Commands.LexicalAnalysis.Tokens;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace GIBS.Commands;

public partial class Command
{
    public class Factory
    {
        readonly Parser _parser;

        readonly ILogger _logger;

        public Factory(Parser parser, ILogger<Factory> logger)
        {
            _parser = parser;
            _logger = logger;
        }

        internal Command Create(CommandToken commandToken) => Create(commandToken.Lexeme);

        public Command Create(string command)
        {
            if (TryCreate(command) is Command command_)
                return command_;
            else
            {
                var exception = new ArgumentException($"{nameof(command)} doesn't represent {nameof(Command)} ({command}).");
                _logger.LogCritical(exception, $"{nameof(Command)} can't be created.");
                throw exception;
            }
        }

        internal Command? TryCreate(string command)
        {
            command = command.StartsWith(Command.Prefix) ? command : Prefix + command;
            return _parser.TryParse(command);
        }
    }
}
