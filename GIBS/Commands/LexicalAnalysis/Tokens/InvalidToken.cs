using GIBS.Tokenization;
using System.Text.RegularExpressions;

namespace GIBS.Commands.LexicalAnalysis.Tokens;

internal record InvalidToken : CommandToken_
{
    internal class LexemeScanner : LexemeScanner<CommandToken_>
    {
        internal override Regex Pattern => new(@"^\S+", RegexOptions.Compiled);

        protected override CommandToken_ Token(string lexeme) => new InvalidToken(lexeme);
    }

    InvalidToken(string lexeme)
        : base(lexeme)
    {
    }
}
