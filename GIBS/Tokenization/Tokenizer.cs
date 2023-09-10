namespace GIBS.Tokenization;

/// <summary>
/// Uses registered <see cref="LexemeScanner{TToken}"/> implementations
/// to transform <see cref="string"/> input into a stream of <typeparamref name="TToken"/>s.
/// </summary>
public class Tokenizer<TToken> where TToken : Token
{
    readonly IEnumerable<LexemeScanner<TToken>> _lexemeScanners;

    public Tokenizer(IEnumerable<LexemeScanner<TToken>> lexemeScanners)
    {
        _lexemeScanners = lexemeScanners;
    }

    /// <summary>
    /// Transforms <paramref name="input"/> into a stream of <typeparamref name="TToken"/>s.
    /// </summary>
    internal IEnumerable<TToken> Tokenize(string input)
    {
        var tokenizerInput = new TokenizerInput(input);

        if (_lexemeScanners.Any())
            while (!tokenizerInput.IsExhausted)
                foreach (var lexemeScanner in _lexemeScanners)
                    if (lexemeScanner.Scan(tokenizerInput) is TToken token)
                    { yield return token; break; }
    }
}
