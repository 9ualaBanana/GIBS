using GIBS.Tokenization;
using System.Text.RegularExpressions;

namespace GIBS.Commands.LexicalAnalysis.Tokens;

internal record UnquotedCommandArgumentToken : CommandToken_
{
    internal class LexemeScanner : LexemeScanner<CommandToken_>
    {
        internal override Regex Pattern => new("^[^/\"\\s][^\\s]*", RegexOptions.Compiled);

        protected override CommandToken_ Token(string lexeme) => new UnquotedCommandArgumentToken(lexeme);
    }

    UnquotedCommandArgumentToken(string lexeme)
        : base(lexeme)
    {
    }
}
