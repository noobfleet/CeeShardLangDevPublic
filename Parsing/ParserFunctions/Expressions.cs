using System;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using CeeShard.Parsing;

namespace CeeShard;

public partial class Parser
{
    private Expr parsePrimaryExpr()
    {
        switch(at().Type)
        {
            case TokenType.Identifier:
                Identifier ident = new();
                ident.Symbol = advance().Value;
                return ident;
            case TokenType.Number:
                NumericLiteral num = new();
                num.Value = Convert.ToDouble(advance().Value);
                return num;
            case TokenType.String:
                StringLiteral str = new();
                str.Value = advance().Value;
                return str;
            case TokenType.Null:
                advance();
                return new NullLiteral();
            case TokenType.OpenParen:
                advance();
                Expr value = ParseExpr();
                expect(TokenType.CloseParen);
                return value;
            
            default: throw new Exception($"Expected expression but got {at().Value} (at line {at().Line})");
        }
    }

    private Expr parseMemberExpr()
    {
        Expr obj = parsePrimaryExpr();
        MemberExpr memberExpr = new();
        bool isMemberExpr = false;
        
        memberExpr.Object = obj as Identifier;
        while(at().Type == TokenType.Dot || at().Type == TokenType.OpenBracket)
        {
            switch(at().Type)
            {
                case TokenType.Dot:
                    advance();
                    memberExpr.Property = parsePrimaryExpr();
                    
                    isMemberExpr = true;

                    if(memberExpr.Property.Kind != NodeType.Identifier) throw new Exception($"Expected property identifier but got {memberExpr.Property.Kind}");
                    break;
                case TokenType.OpenBracket:
                    advance();
                    memberExpr.Property = ParseExpr();
                    
                    expect(TokenType.CloseBracket);
                    
                    isMemberExpr = true;
                    break;
            }
            
            if(memberExpr.Property.Kind != NodeType.NumericLiteral) memberExpr.IsComputed = true;
        }

        return isMemberExpr ? memberExpr : obj;
    }

    private Expr parseCallExpr(Expr caller)
    {
        CallExpr callExpr = new CallExpr();
        callExpr.Caller = caller;

        expect(TokenType.OpenParen);
        
        callExpr.Args = parseCallExprArgs();
        
        expect(TokenType.CloseParen);
        
        return callExpr;
    }

    private Expr[] parseCallExprArgs()
    {
        if(at().Type == TokenType.CloseParen) return [];

        List<Expr> args = [ParseExpr()];

        while(at().Type == TokenType.Comma)
        {
            advance();
            args.Add(ParseExpr());
        }
        
        return args.ToArray();
    }

    private Expr parseCallMemberExpr()
    {
        Expr obj = parseMemberExpr();

        if(at().Type == TokenType.OpenParen) return parseCallExpr(obj);

        return obj;
    }
    
    private Expr parseExponentialExpr()
    {
        Expr left = parseCallMemberExpr();

        if(at().Value == "^")
        {
            string op = advance().Value;
            Expr right = ParseExpr();
            
            BinaryExpr binaryExpr = new BinaryExpr();
            binaryExpr.Operator = op;
            binaryExpr.Left = left;
            binaryExpr.Right = right;
            return binaryExpr;
        }
        
        return left;
    }
    
    private Expr parseMultiplicativeExpr()
    {
        Expr left = parseExponentialExpr();

        if(at().Value == "*" || at().Value == "/" || at().Value == "%")
        {
            string op = advance().Value;
            Expr right = ParseExpr();
            
            BinaryExpr binaryExpr = new BinaryExpr();
            binaryExpr.Operator = op;
            binaryExpr.Left = left;
            binaryExpr.Right = right;
            return binaryExpr;
        }
        
        return left;
    }
    
    private Expr parseAdditiveExpr()
    {
        Expr left = parseMultiplicativeExpr();

        if(at().Value == "+" || at().Value == "-")
        {
            string op = advance().Value;
            Expr right = ParseExpr();
            
            BinaryExpr binaryExpr = new BinaryExpr();
            binaryExpr.Operator = op;
            binaryExpr.Left = left;
            binaryExpr.Right = right;
            return binaryExpr;
        }
        
        return left;
    }

    private Expr parseObjectExpr()
    {
        if(at().Type != TokenType.OpenBracket) return parseAdditiveExpr();

        advance();
        
        Dictionary<string, Expr> properties = [];
        int count = 0;
        while(at().Type != TokenType.EndOfFile && at().Type != TokenType.CloseBracket)
        {
            Expr key = parseObjectExpr();

            switch(at().Type)
            {
                case TokenType.Comma:
                case TokenType.CloseBracket:
                {
                    advance();
                    properties.Add(count++.ToString(), key);
                    continue;
                }
                default:
                {
                    if(key.Kind != NodeType.Identifier && key.Kind != NodeType.NumericLiteral) throw new Exception($"Expected property identifier but got {key.Kind}");
                    expect(TokenType.Equals);
                    
                    Expr value = ParseExpr();
                    properties.Add(key.Kind == NodeType.Identifier ? (key as Identifier).Symbol : (key as NumericLiteral).Value.ToString(), value);
                    
                    if(at().Type != TokenType.CloseBracket) expect(TokenType.Comma);
                    continue;
                }
            }
        }

        expect(TokenType.CloseBracket);
        
        ObjectLiteral obj = new();
        obj.Properties = properties;
        return obj;
    }

    private Expr parseConditionalSingleOperatorExpr(Expr left, string op)
    {
        advance();

        BinaryExpr expr = new();
        expr.Left = left;
        expr.Right = ParseExpr();
        expr.Operator = op;
        return expr;
    }

    private string parseToOperator()
    {
        switch(at().Value)
        {
            case "!":
                return tokens[currentTokenIndex + 1].Type == TokenType.Equals ? "!=" : "!";
            case "&": return "&&";
            case "|": return "||";
        }

        throw new Exception($"Unrecogized operator: '{at().Value}'");
    }

    private Expr parseConditionalExpr()
    {
        Expr left = parseObjectExpr();

        switch(at().Type)
        {
            case TokenType.Equals:
            {
                if(tokens[currentTokenIndex + 1].Type == TokenType.Equals) // checks for ==
                {
                    advance();
                    return parseConditionalSingleOperatorExpr(left, "==");
                }

                return left;
            }
            case TokenType.Operator:
            {
                return parseConditionalSingleOperatorExpr(left, parseToOperator());
            }
            
            default: return left;
        }
    }

    private Expr parseAssignmentExpr()
    {
        Expr left = parseConditionalExpr();

        if(at().Type == TokenType.Equals)
        {
            advance();
            AssignmentExpr assignmentExpr = new();
            assignmentExpr.Assignee = left;
            assignmentExpr.Value = ParseExpr(); // change this to parseassignmentexpr maybe
            return assignmentExpr;
        }
        
        return left;
    }

    private Expr parseFunctionLiteral()
    {
        if(at().Type != TokenType.OpenCurlyBracket) return parseAssignmentExpr();
        
        advance();
        
        List<Stmt> body = new();
        while(at().Type != TokenType.CloseCurlyBracket && currentTokenIndex < tokens.Length - 1)
        {
            body.Add(ParseStmt());
        }
        
        expect(TokenType.CloseCurlyBracket);
        
        FunctionLiteral func = new();
        func.Body = body.ToArray();
        
        return func;
    }
    
    private Expr ParseExpr()
    {
        return parseFunctionLiteral();
    }
}
