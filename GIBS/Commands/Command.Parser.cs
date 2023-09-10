using GIBS.Commands.LexicalAnalysis.Tokens;
using GIBS.Tokenization;

namespace GIBS.Commands;

public partial class Command
{
    /// <summary>
    /// Exposes <see cref="TryParse(string)"/> method that returns <see cref="Command"/>
    /// if the input <see cref="string"/> was a syntactically correct command.
    /// </summary>
    public class Parser
    {
        readonly Tokenizer<CommandToken_> _tokenizer;

        public Parser(Tokenizer<CommandToken_> tokenizer)
        {
            _tokenizer = tokenizer;
        }

        /// <summary>
        /// Uses <see cref="Tokenizer{TToken}"/> to make an attempt in constructing <see cref="Command"/>
        /// from <paramref name="command"/> if it is a syntactically correct command.
        /// </summary>
        /// <param name="command"><see cref="string"/> that should represent a command.</param>
        /// <returns>
        /// <see cref="Command"/> if <paramref name="command"/> is a syntactically correct command.
        /// </returns>
        internal Command? TryParse(string command)
        {
            var tokens = _tokenizer.Tokenize(command).GetEnumerator();

            if (tokens.MoveNext() && tokens.Current is CommandToken commandToken)
            {
                List<Token> arguments = new();
                while (tokens.MoveNext())
                    if (tokens.Current is QuotedCommandArgumentToken || tokens.Current is UnquotedCommandArgumentToken)
                        arguments.Add(tokens.Current);
                var quotedArguments = arguments.OfType<QuotedCommandArgumentToken>().Select(arg => arg.Value);
                var unquotedArguments = arguments.OfType<UnquotedCommandArgumentToken>().Select(arg => arg.Value);

                return new Command(commandToken, quotedArguments, unquotedArguments);
            }

            return null;
        }
    }
}
