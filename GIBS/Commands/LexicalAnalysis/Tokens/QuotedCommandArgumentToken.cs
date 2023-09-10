using GIBS.Tokenization;
using System.Text.RegularExpressions;

namespace GIBS.Commands.LexicalAnalysis.Tokens;

internal record QuotedCommandArgumentToken : CommandToken_
{
    internal class LexemeScanner : LexemeScanner<CommandToken_>
    {
        internal override Regex Pattern => new("^\".*?\"", RegexOptions.Compiled);

        protected override CommandToken_ Token(string lexeme) => new QuotedCommandArgumentToken(lexeme);
    }

    QuotedCommandArgumentToken(string lexeme)
        : base(lexeme)
    {
    }

    protected override string Evaluate(string lexeme) => lexeme.Trim('"');
}
