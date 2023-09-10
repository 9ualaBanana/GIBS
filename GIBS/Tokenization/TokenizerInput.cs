namespace GIBS.Tokenization;

internal class TokenizerInput
{
    internal readonly string Whole;

    internal string Untokenized => Whole[_pointer..];

    internal bool IsExhausted => Untokenized.Length == 0;

    int _pointer;

    internal TokenizerInput(string value)
    {
        Whole = value.Trim();
        _pointer = 0;
    }

    internal Token Consume(Token token)
    { _pointer += token.Lexeme.Length; return token; }
}
