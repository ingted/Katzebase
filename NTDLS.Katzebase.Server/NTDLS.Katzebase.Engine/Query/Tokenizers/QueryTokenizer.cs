﻿using NTDLS.Katzebase.Client;
using NTDLS.Katzebase.Client.Exceptions;
using NTDLS.Katzebase.Client.Types;
using System.Text.RegularExpressions;
using static NTDLS.Katzebase.Engine.Library.EngineConstants;

namespace NTDLS.Katzebase.Engine.Query.Tokenizers
{
    public class QueryTokenizer
    {
        static readonly char[] DefaultTokenDelimiters = [',', '='];

        private readonly string _text;
        private int _position = 0;
        private readonly int _startPosition = 0;

        public string Text => _text;
        public int Position => _position;
        public int Length => _text.Length;
        public int StartPosition => _startPosition;
        public KbInsensitiveDictionary<string> StringLiterals { get => _stringLiterals; }
        public KbInsensitiveDictionary<string> NumericLiterals { get => _numericLiterals; }
        public List<string> Breadcrumbs { get; private set; } = new();
        public char? NextCharacter => _position < _text.Length ? _text[_position] : null;
        public bool IsEnd() => _position >= _text.Length;
        public KbInsensitiveDictionary<KbConstant?> UserParameters { get; set; }

        public delegate bool NextCharDel(char c);
        public bool IsNextCharacter(NextCharDel del)
        {
            var next = NextCharacter;
            if (next == null)
            {
                return false;
            }
            return del((char)next);
        }

        /// <summary>
        /// After the constructor is called, this will contain the same hash
        ///     for the same query regardless of string or numeric constants.
        /// </summary>
        public string LogicHash { get; private set; }

        private readonly KbInsensitiveDictionary<string> _stringLiterals;
        private readonly KbInsensitiveDictionary<string> _numericLiterals;

        public QueryTokenizer(string text, KbInsensitiveDictionary<KbConstant?>? userParameters = null)
        {
            UserParameters = userParameters ?? new();

            _text = text.Trim().TrimEnd(';').Trim();
            CleanQueryText(ref _text, out _stringLiterals, out _numericLiterals);

            LogicHash = Library.Helpers.GetSHA256Hash(_text);
        }

        public QueryTokenizer(string text, int startPosition, KbInsensitiveDictionary<KbConstant?>? userParameters = null)
        {
            UserParameters = userParameters ?? new();

            _text = text;
            _position = startPosition;
            _startPosition = startPosition;
            CleanQueryText(ref _text, out _stringLiterals, out _numericLiterals);

            LogicHash = Library.Helpers.GetSHA256Hash(_text);
        }

        public string GetLiteralValue(string value, bool wrapInSingleQuotes = false)
        {
            if (StringLiterals.TryGetValue(value, out string? stringLiteral))
            {
                if (wrapInSingleQuotes)
                {
                    return $"{stringLiteral.ToLowerInvariant()}";
                }
                return stringLiteral.ToLowerInvariant();
            }
            else if (NumericLiterals.TryGetValue(value, out string? numericLiteral))
            {
                if (wrapInSingleQuotes)
                {
                    return $"{numericLiteral.ToLowerInvariant()}";
                }
                return numericLiteral.ToLowerInvariant();
            }
            else if (UserParameters.TryGetValue(value, out KbConstant? userParameter))
            {
                if (wrapInSingleQuotes)
                {
                    return $"{userParameter?.Value?.ToLowerInvariant()}";
                }
                return userParameter?.Value?.ToLowerInvariant() ?? string.Empty;
            }
            else return value.ToLowerInvariant();
        }

        public void SwapFieldLiteral(ref string givenValue)
        {
            if (string.IsNullOrEmpty(givenValue) == false && StringLiterals.TryGetValue(givenValue, out string? value))
            {
                givenValue = value;

                if (givenValue.StartsWith('\'') && givenValue.EndsWith('\''))
                {
                    givenValue = givenValue.Substring(1, givenValue.Length - 2);
                }
            }
        }

        public void SetPosition(int position)
        {
            _position = position;
            if (_position > _text.Length)
            {
                throw new KbParserException("Skip position is greater than query length.");
            }
        }

        public char CurrentChar()
        {
            if (_position >= Length)
            {
                return '\0';
            }
            return (_text.Substring(_position, 1)[0]);
        }

        public bool IsNextCharacter(char ch)
        {
            if (_position >= Length)
            {
                return false;
            }
            return (_text.Substring(_position, 1)[0] == ch);
        }

        public string Remainder()
        {
            return _text.Substring(_position).Trim();
        }

        public bool TryGetNextIndexOf(char[] characters, out int index)
        {
            int restorePosition = _position;

            index = -1;

            for (; _position < _text.Length; _position++)
            {
                if (characters.Contains(_text[_position]))
                {
                    index = _position;
                    break;
                }
            }

            _position = restorePosition;
            return index != -1;
        }

        public int GetNextIndexOf(char[] characters)
        {
            int restorePosition = _position;

            for (; _position < _text.Length; _position++)
            {
                if (characters.Contains(_text[_position]))
                {
                    int index = _position;
                    _position = restorePosition;
                    return index;
                }
            }

            _position = restorePosition;

            throw new Exception($"Expected character not found [{string.Join("],[", characters)}].");
        }


        public string GetMatchingBraces()
        {
            return GetMatchingBraces('(', ')');
        }

        public string GetMatchingBraces(char open, char close)
        {
            int scope = 0;

            SkipWhiteSpace();

            if (_text[_position] != open)
            {
                throw new Exception($"Expected character not found [{open}].");
            }

            int startPosition = _position + 1;

            for (; _position < _text.Length; _position++)
            {
                if (_text[_position] == open)
                {
                    scope++;
                }
                else if (_text[_position] == close)
                {
                    scope--;
                }

                if (scope == 0)
                {
                    var result = _text.Substring(startPosition, _position - startPosition).Trim();

                    _position++;
                    SkipWhiteSpace();

                    return result;
                }
            }

            throw new Exception($"Expected matching braces not found [{open}] and [{close}], ended at scope [{scope}].");
        }

        public string SubString(int endPosition)
        {
            var result = _text.Substring(_position, endPosition - _position);
            _position = endPosition;
            return result;
        }

        public string GetNext(char[] delimiters)
        {
            var token = string.Empty;

            if (_position == _text.Length)
            {
                Breadcrumbs.Add(string.Empty);
                return string.Empty;
            }

            for (; _position < _text.Length; _position++)
            {
                if (char.IsWhiteSpace(_text[_position]) || delimiters.Contains(_text[_position]) == true)
                {
                    break;
                }

                token += _text[_position];
            }

            SkipWhiteSpace();

            token = token.Trim();

            Breadcrumbs.Add(token);
            return token;
        }

        /// <summary>
        /// Returns the index of the first found of the given string. Throws exception if not found and does not affect the token position.
        /// </summary>
        /// <param name="strings"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int GetNextIndexOf(string[] strings)
        {
            foreach (var str in strings)
            {
                int index = _text.IndexOf(str, _position, StringComparison.InvariantCultureIgnoreCase);
                if (index >= 0)
                {
                    return index;
                }
            }

            throw new Exception($"Expected string not found [{string.Join("],[", strings)}].");
        }


        public string GetNext()
        {
            return GetNext(DefaultTokenDelimiters);
        }

        public int GetNextAsInt()
        {
            string token = GetNext();

            if (NumericLiterals.TryGetValue(token, out var literal))
            {
                token = literal;
            }

            if (int.TryParse(token, out int value) == false)
            {
                throw new KbParserException("Invalid query. Found [" + token + "], expected numeric row limit.");
            }

            SkipWhiteSpace();

            return value;
        }

        public bool IsNextStartOfQuery()
        {
            return IsNextStartOfQuery(out var _);
        }

        public bool IsNextStartOfQuery(out QueryType type)
        {
            var token = PeekNext().ToLowerInvariant();

            return Enum.TryParse(token, true, out type) //Enum parse.
                && Enum.IsDefined(typeof(QueryType), type) //Is enum value über lenient.
                && int.TryParse(token, out _) == false; //Is not number, because enum parsing is "too" flexible.
        }

        public string PeekNext()
        {
            int originalPosition = _position;
            var result = GetNext(DefaultTokenDelimiters);
            _position = originalPosition;
            return result;
        }

        public string PeekNext(char[] delimiters)
        {
            int originalPosition = _position;
            var result = GetNext(delimiters);
            _position = originalPosition;
            return result;
        }

        /// <summary>
        /// Skips to the next standard delimiter. Throws an exception if one is not found.
        /// </summary>
        public void SkipNext()
        {
            GetNext(DefaultTokenDelimiters);
        }

        /// <summary>
        /// Skips to the next given character. Throws an exception if one is not found.
        /// </summary>
        public void SkipNext(char character)
        {
            SkipNext([character]);
        }

        /// <summary>
        /// Skips to the next given character. Throws an exception if one is not found.
        /// </summary>
        public void SkipNext(char[] characters)
        {
            for (; _position < _text.Length; _position++)
            {
                if (char.IsWhiteSpace(_text[_position]))
                {
                    continue;
                }

                if (characters.Contains(_text[_position]))
                {
                    _position++;
                    return;
                }
            }

            throw new Exception($"Expected character not found [{string.Join("],[", characters)}].");
        }

        public void SkipToEnd()
        {
            _position = _text.Length;
        }

        public void SkipDelimiters()
        {
            SkipDelimiters(DefaultTokenDelimiters);
        }

        public void SkipDelimiters(char delimiter)
        {
            SkipDelimiters([delimiter]);
        }

        public void SkipDelimiters(char[] delimiters)
        {
            while (_position < _text.Length && (char.IsWhiteSpace(_text[_position]) || delimiters.Contains(_text[_position]) == true))
            {
                _position++;
            }
        }

        public void SkipWhile(char[] chs)
        {
            while (_position < _text.Length && (chs.Contains(_text[_position]) || char.IsWhiteSpace(_text[_position])))
            {
                _position++;
            }
        }

        public void SkipWhile(char ch)
        {
            while (_position < _text.Length && (_text[_position] == ch || char.IsWhiteSpace(_text[_position])))
            {
                _position++;
            }
        }

        public void SkipNextCharacter()
        {
            _position++;

            while (_position < _text.Length && char.IsWhiteSpace(_text[_position]))
            {
                _position++;
            }
        }

        public void SkipWhiteSpace()
        {
            while (_position < _text.Length && char.IsWhiteSpace(_text[_position]))
            {
                _position++;
            }
        }

        /// <summary>
        /// Removes all unnecessary whitespace, newlines, comments and replaces literals with tokens to prepare query for parsing.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="swapLiteralsBackIn"></param>
        /// <returns></returns>
        public static void CleanQueryText(ref string query,
            out KbInsensitiveDictionary<string> stringLiterals,
            out KbInsensitiveDictionary<string> numericLiterals)
        {
            query = KbTextUtility.RemoveComments(query);

            stringLiterals = SwapOutStringLiterals(ref query);

            //We replace numeric constants and we want to make sure we have 
            //  no numbers next to any conditional operators before we do so.
            query = query.Replace("!=", "$$NotEqual$$");
            query = query.Replace(">=", "$$GreaterOrEqual$$");
            query = query.Replace("<=", "$$LesserOrEqual$$");

            query = query.Replace("(", " ( ");
            query = query.Replace(")", " ) ");

            query = query.Replace(">", " > ");
            query = query.Replace("<", " < ");
            query = query.Replace("=", " = ");
            query = query.Replace("$$NotEqual$$", " != ");
            query = query.Replace("$$GreaterOrEqual$$", " >= ");
            query = query.Replace("$$LesserOrEqual$$", " <= ");
            query = query.Replace("||", " || ");
            query = query.Replace("&&", " && ");

            numericLiterals = SwapOutNumericLiterals(ref query);

            int length;
            do
            {
                length = query.Length;
                query = query.Replace("\t", " ");
                query = query.Replace("  ", " ");
            }
            while (length != query.Length);

            query = query.Trim();

            query = query.Replace("(", " ( ").Replace(")", " ) ");

            RemoveComments(ref query);

            TrimAllLines(ref query);
            RemoveEmptyLines(ref query);
            RemoveNewlines(ref query);
            RemoveDoubleWhitespace(ref query);

            query = query.Trim();
        }

        /// <summary>
        /// Replaces literals with tokens to prepare query for parsing.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public static KbInsensitiveDictionary<string> SwapOutStringLiterals(ref string query)
        {
            var mappings = new KbInsensitiveDictionary<string>();
            var regex = new Regex("\"([^\"\\\\]*(\\\\.[^\"\\\\]*)*)\"|\\'([^\\'\\\\]*(\\\\.[^\\'\\\\]*)*)\\'");
            int literalKey = 0;

            while (true)
            {
                var match = regex.Match(query);

                if (match.Success)
                {
                    string key = $"$s_{literalKey++}$";
                    mappings.Add(key, match.ToString());

                    query = Helpers.Text.ReplaceRange(query, match.Index, match.Length, key);
                }
                else
                {
                    break;
                }
            }

            return mappings;
        }

        public static KbInsensitiveDictionary<string> SwapOutNumericLiterals(ref string query)
        {
            var mappings = new KbInsensitiveDictionary<string>();
            var regex = new Regex(@"(?<=\s|^)(?:\d+\.?\d*|\.\d+)(?=\s|$)");
            int literalKey = 0;

            while (true)
            {
                var match = regex.Match(query);

                if (match.Success)
                {
                    string key = $"$n_{literalKey++}$";
                    mappings.Add(key, match.ToString());

                    query = Helpers.Text.ReplaceRange(query, match.Index, match.Length, key);
                }
                else
                {
                    break;
                }
            }

            return mappings;
        }

        public static void RemoveDoubleWhitespace(ref string query)
        {
            query = Regex.Replace(query, @"\s+", " ");
        }

        public static void RemoveNewlines(ref string query)
        {
            query = query.Replace("\r\n", " ");
        }

        public static void SwapInLiteralStrings(ref string query, KbInsensitiveDictionary<string> mappings)
        {
            foreach (var mapping in mappings)
            {
                query = query.Replace(mapping.Key, mapping.Value);
            }
        }

        public static void RemoveComments(ref string query)
        {
            query = "\r\n" + query + "\r\n";

            var blockComments = @"/\*(.*?)\*/";
            //var lineComments = @"//(.*?)\r?\n";
            var lineComments = @"--(.*?)\r?\n";
            var strings = @"""((\\[^\n]|[^""\n])*)""";
            var verbatimStrings = @"@(""[^""]*"")+";

            query = Regex.Replace(query,
                blockComments + "|" + lineComments + "|" + strings + "|" + verbatimStrings,
                me =>
                {
                    if (me.Value.StartsWith("/*") || me.Value.StartsWith("--"))
                        return me.Value.StartsWith("--") ? Environment.NewLine : "";
                    // Keep the literal strings
                    return me.Value;
                },
                RegexOptions.Singleline);
        }

        public static void RemoveEmptyLines(ref string query)
        {
            query = Regex.Replace(query, @"^\s+$[\r\n]*", "", RegexOptions.Multiline);
        }

        public static void TrimAllLines(ref string query)
        {
            query = string.Join("\r\n", query.Split('\n').Select(o => o.Trim()));
        }
    }
}
