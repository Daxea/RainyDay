using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace RainyDay
{
    public class Lexer
    {
        private string _text;
        private int _position;
        private char _currentChar;

        private List<Exception> _errors;
        public Exception[] Errors => _errors.ToArray();

        public Lexer(string text)
        {
            _text = text;
            _position = 0;
            _currentChar = _text[0];
            _errors = new List<Exception>();
        }

        #region Advance Position

        private void AdvancePosition()
        {
            _position++;
            _currentChar = _position >= _text.Length ? char.MinValue : _text[_position];
        }

        private void AdvancePosition(int num)
        {
            while (num-- > 0)
                AdvancePosition();
        }

        private char Peek()
        {
            var peekPos = _position + 1;
            if (peekPos >= _text.Length)
                return char.MinValue;
            return _text[peekPos];
        }

        #endregion
        #region Skip Content

        private void SkipWhitespace()
        {
            while (_currentChar != char.MinValue && char.IsWhiteSpace(_currentChar))
                AdvancePosition();
        }

        private void SkipLineComment()
        {
            while ((_currentChar != '\n') && (_currentChar != char.MinValue))
                AdvancePosition();
            AdvancePosition();
        }

        private void SkipBlockComment()
        {
            while ((_currentChar != '*' && Peek() != '/') && (_currentChar != char.MinValue))
                AdvancePosition();
            AdvancePosition();
            if (_currentChar == '/')
                AdvancePosition();
        }

        #endregion
        #region Create Tokens

        private Token Number()
        {
            var result = string.Empty;
            while (_currentChar != char.MinValue && char.IsDigit(_currentChar))
            {
                result += _currentChar;
                AdvancePosition();
            }

            if (_currentChar == '.')
            {
                result += _currentChar;
                AdvancePosition();

                while (_currentChar != char.MinValue && char.IsDigit(_currentChar))
                {
                    result += _currentChar;
                    AdvancePosition();
                }

                return new Token("Single", float.Parse(result));
            }

            return new Token("Int32", int.Parse(result));
        }

        private Token Identifier()
        {
            var result = string.Empty;
            while (_currentChar != char.MinValue && char.IsLetterOrDigit(_currentChar))
            {
                result += _currentChar;
                AdvancePosition();
            }
            var key = result.ToLowerInvariant();
            if (Tokens.Reserved.ContainsKey(key))
                return Tokens.Reserved[key];
            return new Token(Tokens.Identifier, result);
        }

        private Token String()
        {
            var result = string.Empty;
            while (_currentChar != char.MinValue && _currentChar != '"')
            {
                if (_currentChar == '\\' && Peek() == '"')
                {
                    AdvancePosition(2);
                    result += "\"";
                    continue;
                }

                result += _currentChar;
                AdvancePosition();
            }
            AdvancePosition();
            return new Token(Tokens.String, result);
        }

        private Token Character()
        {
            var result = _currentChar;
            AdvancePosition();
            if (_currentChar != '\'')
            {
                ReportError(new Exception("Single quotes can only contain a single character or escape sequence!"));
                return null;
            }
			AdvancePosition();
            return new Token(Tokens.Character, result);
        }

        public Token GetNextToken()
        {
            while (_currentChar != char.MinValue)
            {
                if (char.IsWhiteSpace(_currentChar))
                {
                    SkipWhitespace();
                    continue;
                }

                if (_currentChar == '/')
                {
                    if (Peek() == '/')
                    {
                        AdvancePosition(2);
                        SkipLineComment();
                        continue;
                    }
                    if (Peek() == '*')
                    {
                        AdvancePosition(2);
                        SkipBlockComment();
                        continue;
                    }
                }

                if (char.IsDigit(_currentChar))
                    return Number();

                if (char.IsLetter(_currentChar))
                    return Identifier();

                if (_currentChar == '"')
                {
                    AdvancePosition();
                    return String();
                }

                if (_currentChar == '\'')
                {
                    AdvancePosition();
                    return Character();
                }

                if (_currentChar == '[') return GetToken(Tokens.ArrayStart);
                if (_currentChar == ']') return GetToken(Tokens.ArrayEnd);

                if (_currentChar == '{') return GetToken(Tokens.BlockStart);
                if (_currentChar == '}') return GetToken(Tokens.BlockEnd);

                if (_currentChar == '(') return GetToken(Tokens.ParamStart);
                if (_currentChar == ')') return GetToken(Tokens.ParamEnd);

                if (_currentChar == ',') return GetToken(Tokens.Comma);
                if (_currentChar == ';') return GetToken(Tokens.Semi);
                if (_currentChar == ':') return GetToken(Tokens.Colon);

                if (_currentChar == '=')
                {
                    if (Peek() == '=')
                    {
                        AdvancePosition();
                        return GetToken(Tokens.Equal);
                    }

					if (Peek() == '>')
					{
						AdvancePosition();
						return GetToken(Tokens.Lambda);
					}

                    return GetToken(Tokens.Assign);
                }

                if (_currentChar == '!')
                {
                    if (Peek() == '=')
                    {
                        AdvancePosition();
                        return GetToken(Tokens.NotEqual);
                    }

                    return GetToken(Tokens.Not);
                }

                if (_currentChar == '>')
                {
                    if (Peek() == '=')
                    {
                        AdvancePosition();
                        return GetToken(Tokens.GreaterThanOrEqualTo);
                    }

                    return GetToken(Tokens.GreaterThan);
                }

                if (_currentChar == '<')
                {
                    if (Peek() == '=')
                    {
                        AdvancePosition();
                        return GetToken(Tokens.LessThanOrEqualTo);
                    }

                    return GetToken(Tokens.LessThan);
                }

                if (_currentChar == '+')
                {
                    if (Peek() == '=')
                    {
                        AdvancePosition();
                        return GetToken(Tokens.IncrementBy);
                    }

                    if (Peek() == '+')
                    {
                        AdvancePosition();
                        return GetToken(Tokens.IncrementByOne);
                    }

                    return GetToken(Tokens.Add);
                }

                if (_currentChar == '-')
                {
                    if (Peek() == '=')
                    {
                        AdvancePosition();
                        return GetToken(Tokens.DecrementBy);
                    }

                    if (Peek() == '-')
                    {
                        AdvancePosition();
                        return GetToken(Tokens.DecrementByOne);
                    }

                    return GetToken(Tokens.Subtract);
                }

                if (_currentChar == '*')
                {
                    if (Peek() == '=')
                    {
                        AdvancePosition();
                        return GetToken(Tokens.MultiplyBy);
                    }

                    return GetToken(Tokens.Multiply);
                }

                if (_currentChar == '/')
                {
                    if (Peek() == '=')
                    {
                        AdvancePosition();
                        return GetToken(Tokens.DivideBy);
                    }

                    return GetToken(Tokens.Divide);
                }

				if (_currentChar == '.') return GetToken(Tokens.Dot);

                ReportError(new Exception($"Tokenization failed at character '{_currentChar}'"));
                return null;
            }

            return new Token(Tokens.ScriptEnd);

            Token GetToken(string type, object value = null)
            {
                AdvancePosition();
                return new Token(type, value);
            }
        }

        #endregion
        #region Errors

        private void ReportError(Exception error) => _errors.Add(error);

        #endregion
        #region Static Members

        public static Lexer FromFile(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException($"No script found at path {path}");

            return new Lexer(File.ReadAllText(path));
        }

        #endregion
    }
}