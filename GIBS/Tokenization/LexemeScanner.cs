using System.Text.RegularExpressions;

namespace GIBS.Tokenization;

/// <summary>
/// Builder responsible for construction of concrete <typeparamref name="TToken"/>s from scanned lexemes that represent them.
/// </summary>
public abstract class LexemeScanner<TToken>
    where TToken : Token
{
    /// <summary>
    /// Constructs concrete <typeparamref name="TToken"/> from <paramref name="tokenizerInput"/>
    /// if scanned lexeme matches this <see cref="LexemeScanner{TToken}"/>.
    /// </summary>
    /// <returns>
    /// <typeparamref name="TToken"/> constructed from the scanned lexeme that matches this <see cref="LexemeScanner{TToken}"/>;
    /// <see langword="null"/> if the scanned lexeme doesn't match this <see cref="LexemeScanner{TToken}"/>.
    /// </returns>
    internal TToken? Scan(TokenizerInput tokenizerInput)
        => Pattern.Match(tokenizerInput.Untokenized) is Match match && match.Success ?
        tokenizerInput.Consume(Token(match.Value)) as TToken : null;

    internal abstract Regex Pattern { get; }

    /// <summary>
    /// Constructs concrete <typeparamref name="TToken"/> from the <paramref name="lexeme"/> matching this <see cref="LexemeScanner{TToken}"/>.
    /// </summary>
    /// <param name="lexeme">The lexeme to construct <see cref="Token"/> from.</param>
    /// <returns>
    /// Concrete <typeparamref name="TToken"/> constructed from <paramref name="lexeme"/> matching this <see cref="LexemeScanner{TToken}"/>.
    /// </returns>
    protected abstract TToken Token(string lexeme);
}
