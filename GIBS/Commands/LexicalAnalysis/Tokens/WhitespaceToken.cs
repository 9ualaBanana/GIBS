using GIBS.Tokenization;
using System.Text.RegularExpressions;

namespace GIBS.Commands.LexicalAnalysis.Tokens;

internal record WhitespaceToken : CommandToken_
{
    internal class LexemeScanner : LexemeScanner<CommandToken_>
    {
        internal override Regex Pattern => new(@"\s+", RegexOptions.Compiled);

        protected override CommandToken_ Token(string lexeme) => new WhitespaceToken(lexeme);
    }

    WhitespaceToken(string lexeme)
        : base(lexeme)
    {
    }
}
