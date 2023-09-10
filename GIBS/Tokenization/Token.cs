namespace GIBS.Tokenization;

public abstract record Token
{
    internal readonly string Lexeme;
    internal readonly string Value;

    protected Token(string lexeme)
    {
        Lexeme = lexeme;
        Value = Evaluate(lexeme);
    }

    protected virtual string Evaluate(string lexeme) => lexeme;
}
