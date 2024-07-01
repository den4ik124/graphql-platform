using System.Runtime.CompilerServices;
using static HotChocolate.Fusion.Properties.FusionLanguageResources;
using TokenKind = HotChocolate.Fusion.FieldSelectionMapTokenKind;

namespace HotChocolate.Fusion;

/// <summary>
/// Parses nodes from source text representing a field selection map.
/// </summary>
internal ref struct FieldSelectionMapParser
{
    private readonly FieldSelectionMapParserOptions _options;

    private FieldSelectionMapReader _reader;
    private int _parsedNodes;

    public FieldSelectionMapParser(
        ReadOnlySpan<char> sourceText,
        FieldSelectionMapParserOptions? options = null)
    {
        if (sourceText.Length == 0)
        {
            throw new ArgumentException(SourceTextCannotBeEmpty, nameof(sourceText));
        }

        options ??= FieldSelectionMapParserOptions.Default;

        _options = options;
        _reader = new FieldSelectionMapReader(sourceText, options.MaxAllowedTokens);
    }

    public SelectedValueNode Parse()
    {
        _parsedNodes = 0;

        MoveNext(); // skip start of file

        return ParseSelectedValue();
    }

    /// <summary>
    /// Parses a <see cref="SelectedValueNode"/>.
    ///
    /// <code>
    /// SelectedValue ::
    ///     Path
    ///     SelectedObjectValue
    ///     Path . SelectedObjectValue
    ///     SelectedValue | SelectedValue
    /// </code>
    /// </summary>
    /// <returns>The parsed <see cref="SelectedValueNode"/>.</returns>
    private SelectedValueNode ParseSelectedValue()
    {
        var start = Start();

        SelectedObjectValueNode? selectedObjectValue = null;
        PathNode? path = null;

        // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
        switch (_reader.TokenKind)
        {
            case TokenKind.Name:
                path = ParsePath();
                break;

            case TokenKind.LeftBrace:
                selectedObjectValue = ParseSelectedObjectValue();
                break;

            default:
                throw new SyntaxException(_reader, UnexpectedToken, _reader.TokenKind);
        }

        SelectedValueNode? selectedValue = null;

        if (_reader.TokenKind == TokenKind.Pipe)
        {
            MoveNext(); // skip "|"
            selectedValue = ParseSelectedValue();
        }

        var location = CreateLocation(in start);

        if (path is not null)
        {
            return new SelectedValueNode(location, path, selectedValue);
        }

        if (selectedObjectValue is not null)
        {
            return new SelectedValueNode(location, selectedObjectValue, selectedValue);
        }

        throw new InvalidOperationException();
    }

    /// <summary>
    /// Parses a <see cref="PathNode"/>.
    ///
    /// <code>
    /// Path ::
    ///     - FieldName
    ///     - Path . FieldName
    ///     - FieldName &lt; TypeName &gt; . Path
    ///
    /// FieldName ::
    ///     - Name
    ///
    /// TypeName ::
    ///     - Name
    /// </code>
    /// </summary>
    /// <returns>The parsed <see cref="PathNode"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private PathNode ParsePath()
    {
        var start = Start();

        var fieldName = ParseName();

        NameNode? typeName = null;

        if (_reader.TokenKind == TokenKind.LeftAngleBracket)
        {
            MoveNext(); // skip "<"
            typeName = ParseName();
            Expect(TokenKind.RightAngleBracket);
        }

        PathNode? path = null;

        if (typeName is not null)
        {
            Expect(TokenKind.Period);
            path = ParsePath();
        }
        else if (_reader.TokenKind == TokenKind.Period)
        {
            MoveNext(); // skip "."
            path = ParsePath();
        }

        var location = CreateLocation(in start);

        return new PathNode(location, fieldName, typeName, path);
    }

    /// <summary>
    /// Parses a <see cref="SelectedObjectValueNode"/>.
    ///
    /// <code>
    /// SelectedObjectValue ::
    ///     { SelectedObjectField+ }
    /// </code>
    /// </summary>
    /// <returns>The parsed <see cref="SelectedObjectValueNode"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private SelectedObjectValueNode ParseSelectedObjectValue()
    {
        var start = Start();

        Expect(TokenKind.LeftBrace);

        var fields = new List<SelectedObjectFieldNode>();

        while (_reader.TokenKind != TokenKind.RightBrace)
        {
            fields.Add(ParseSelectedObjectField());
        }

        Expect(TokenKind.RightBrace);

        var location = CreateLocation(in start);

        return new SelectedObjectValueNode(location, fields);
    }

    /// <summary>
    /// Parses a <see cref="SelectedObjectFieldNode"/>.
    ///
    /// <code>
    /// SelectedObjectField ::
    ///     Name : SelectedValue
    /// </code>
    /// </summary>
    /// <returns>The parsed <see cref="SelectedObjectFieldNode"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private SelectedObjectFieldNode ParseSelectedObjectField()
    {
        var start = Start();

        var name = ParseName();
        Expect(TokenKind.Colon);
        var selectedValue = ParseSelectedValue();

        var location = CreateLocation(in start);

        return new SelectedObjectFieldNode(location, name, selectedValue);
    }

    /// <summary>
    /// Parses a <see cref="NameNode"/>.
    /// </summary>
    /// <returns>The parsed <see cref="NameNode"/>.</returns>
    /// <seealso href="https://spec.graphql.org/October2021/#sec-Names">Specification</seealso>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private NameNode ParseName()
    {
        var start = Start();
        var name = ExpectName();
        var location = CreateLocation(in start);

        return new NameNode(location, name);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private TokenInfo Start()
    {
        if (++_parsedNodes > _options.MaxAllowedNodes)
        {
            throw new SyntaxException(
                _reader,
                string.Format(MaxAllowedNodesExceeded, _options.MaxAllowedNodes));
        }

        return _options.NoLocations
            ? default
            : new TokenInfo(_reader.Start, _reader.End, _reader.Line, _reader.Column);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool MoveNext() => _reader.Read();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Expect(TokenKind tokenKind)
    {
        if (!_reader.Skip(tokenKind))
        {
            throw new SyntaxException(_reader, InvalidToken, tokenKind, _reader.TokenKind);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private string ExpectName()
    {
        if (_reader.TokenKind == TokenKind.Name)
        {
            var name = _reader.Value.ToString();
            MoveNext();

            return name;
        }

        throw new SyntaxException(_reader, InvalidToken, TokenKind.Name, _reader.TokenKind);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Location? CreateLocation(in TokenInfo start)
    {
        return _options.NoLocations
            ? null
            : new Location(start.Start, _reader.End, start.Line, start.Column);
    }
}
