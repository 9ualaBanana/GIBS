using GIBS.Tokenization;
using System.Text.RegularExpressions;

namespace GIBS.Commands.LexicalAnalysis.Tokens;

internal record CommandToken : CommandToken_
{
    internal class LexemeScanner : LexemeScanner<CommandToken_>
    {
        internal override Regex Pattern { get; } = new(@$"^{Command.Prefix}[a-z_]{{2,}}", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        protected override CommandToken_ Token(string lexeme) => new CommandToken(lexeme);
    }

    CommandToken(string lexeme)
        : base(lexeme)
    {
    }
}
