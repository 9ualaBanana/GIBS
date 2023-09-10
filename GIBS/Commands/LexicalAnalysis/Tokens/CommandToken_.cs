using GIBS.Tokenization;

namespace GIBS.Commands.LexicalAnalysis.Tokens;

public abstract record CommandToken_ : Token
{
    protected CommandToken_(string lexeme)
        : base(lexeme)
    {
    }
}
