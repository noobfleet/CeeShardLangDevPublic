using System.Collections.Generic;

namespace CeeShard.Parsing;

public struct Token
{
    public Token(TokenType type, string value, int line, int column)
    {
        this.Type = type;
        this.Value = value;
        this.Line = line;
        this.Column = column;
    }
        
    public TokenType Type;
    public string Value;
    public int Line;
    public int Column;
}

public class Lexer
{
    private Dictionary<string, TokenType> KeywordType;

    public Lexer()
    {
        KeywordType = new();
        
        KeywordType.Add("var", TokenType.VarDeclaration);
        KeywordType.Add("const", TokenType.ConstantDeclaration);
        KeywordType.Add("null", TokenType.Null);
        KeywordType.Add("function", TokenType.FunctionDeclaration);
        KeywordType.Add("if", TokenType.IfStatement);
        KeywordType.Add("else", TokenType.Else);
        KeywordType.Add("while", TokenType.WhileStatement);
        KeywordType.Add("for", TokenType.ForStatement);
    }

    private List<Token> tokens = new();
    
    public Token[] Tokenize(string str)
    {
        int i = 0;
        int line = 1;
        int col = 1;
        char c;
        while(i < str.Length)
        {
            c = str[i];
            col++;
            switch(c)
            {
                // whitespace
                case '\n': line++; col = 1; break;
                case '\r':
                case '\t':
                case ' ': break;
                // keyword symbols
                case '=':
                {
                    tokens.Add(new Token(TokenType.Equals, "=", line, col));
                    break;
                }
                case ';':
                {
                    tokens.Add(new Token(TokenType.EndOfStmt, ";", line, col));
                    break;
                }
                case '-':
                case '+':
                case '*':
                case '/':
                case '%':
                case '^':
                case '&':
                case '|':
                case '!':
                {
                    tokens.Add(new Token(TokenType.Operator, c.ToString(), line, col));
                    break;
                }
                case '(':
                {
                    tokens.Add(new Token(TokenType.OpenParen, "(", line, col));
                    break;
                }
                case ')':
                {
                    tokens.Add(new Token(TokenType.CloseParen, ")", line, col));
                    break;
                }
                case '[':
                {
                    tokens.Add(new Token(TokenType.OpenBracket, "[", line, col));
                    break;
                }
                case ']':
                {
                    tokens.Add(new Token(TokenType.CloseBracket, "]", line, col));
                    break;
                }
                case '{':
                {
                    tokens.Add(new Token(TokenType.OpenCurlyBracket, "{", line, col));
                    break;
                }
                case '}':
                {
                    tokens.Add(new Token(TokenType.CloseCurlyBracket, "}", line, col));
                    break;
                }
                case '.':
                {
                    tokens.Add(new Token(TokenType.Dot, ".", line, col));
                    break;
                }
                case ',':
                {
                    tokens.Add(new Token(TokenType.Comma, ",", line, col));
                    break;
                }

                case '"':
                {
                    int nextOccurence = str.IndexOf('"', ++i);
                    tokens.Add(new Token(TokenType.String, str.Substring(i, nextOccurence - i), line, col));
                    i = nextOccurence;
                    break;
                }

                default:
                {
                    if (char.IsNumber(c))
                    {
                        string num = "";
                        while(i < str.Length && char.IsNumber(str[i]))
                        {
                            num += str[i++];
                        }
                        i--;
                        tokens.Add(new Token(TokenType.Number, num, line, col));
                        break;
                    }
            
                    // this goes last
                    if(char.IsAsciiLetter(c))
                    {
                        string identifier = "";
                        while(i < str.Length && char.IsAsciiLetter(str[i]))
                        {
                            identifier += str[i++];
                        }
                        i--;

                        if(KeywordType.TryGetValue(identifier, out TokenType t))
                        {
                            tokens.Add(new Token(t, identifier, line, col));
                        }
                        else
                        {
                            tokens.Add(new Token(TokenType.Identifier, identifier, line, col));
                        }

                        break;
                    }
                    
                    throw new Exception($"Unrecognized character: {c}");
                }
            }

            i++;
        }
        
        tokens.Add(new Token(TokenType.EndOfStmt, "EOF", line, col));
        
        return tokens.ToArray();
    }
}