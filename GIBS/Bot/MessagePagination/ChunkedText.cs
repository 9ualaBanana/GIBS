namespace GIBS.Bot.MessagePagination;

/// <summary>
/// Represents a <see cref="string"/> that can be manipulated as chunks of specified size.
/// </summary>
internal class ChunkedText : IEquatable<ChunkedText>
{
    const int DefaultChunkSize = 4096;

    readonly string _content;
    readonly int _chunkSize;
    int _pointer;

    internal bool IsChunked => _content.Length > _chunkSize;
    internal bool IsAtMiddle => !IsAtFirstChunk && !IsAtLastChunk;
    internal bool IsAtFirstChunk => _pointer == 0;
    internal bool IsAtLastChunk =>
        _pointer == _content.Length || _pointer + _chunkSize >= _content.Length;


    internal ChunkedText(string content, int chunkSize = DefaultChunkSize)
    {
        if (chunkSize == 0) throw new ArgumentOutOfRangeException(
            nameof(chunkSize), chunkSize, $"{nameof(chunkSize)} can't be 0.");

        _content = content;
        _chunkSize = chunkSize;
        _pointer = 0;
    }

    internal void MovePointerToBeginningOfPreviousChunk()
    {
        _pointer -= _chunkSize * 2;
        if (_pointer < 0) _pointer = 0;
    }

    /// <remarks>
    /// Also moves the pointer to a beginning of the chunk that follows the returned one.
    /// </remarks>
    internal string NextChunk
    {
        get
        {
            int chunkEnd = _pointer + _chunkSize;
            if (chunkEnd > _content.Length) chunkEnd = _content.Length;

            int pointerBeforeMove = _pointer;
            _pointer = chunkEnd;

            return _content[pointerBeforeMove..chunkEnd];
        }
    }

    #region Equality

    public static bool operator ==(ChunkedText this_, ChunkedText that) => this_.Equals(that);
    public static bool operator !=(ChunkedText this_, ChunkedText that) => !this_.Equals(that);
    public override bool Equals(object? obj) => Equals(obj as ChunkedText);
    public bool Equals(ChunkedText? other) => Equals(other);
    public override int GetHashCode() => _content.GetHashCode();

    #endregion

    #region Conversions

    public static implicit operator string(ChunkedText chunkedMessage) => chunkedMessage._content;

    #endregion
}
