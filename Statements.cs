using System;
using CeeShard.Parsing;

namespace CeeShard;

public partial class Parser
{
    private Stmt parseVarDeclaration()
    {
        bool isConst = (advance().Type == TokenType.ConstantDeclaration);
        string identifier = expect(TokenType.Identifier).Value;
        
        VarDeclaration varDecl = new();
        varDecl.Identifier = identifier;
        varDecl.IsConstant = isConst;

        switch(at().Type)
        {
            case TokenType.EndOfStmt:
            {
                if(isConst) throw new Exception($"Constant variable {identifier} cannot be declared with null value");
                varDecl.Value = new NullLiteral();
                return varDecl;
            }
            case TokenType.OpenParen:
            {
                advance();
                
                Expr[] args = parseCallExprArgs();
                foreach(Expr arg in args)
                    if(arg.Kind != NodeType.Identifier) throw new Exception($"Cannot have parameter name of type {arg.Kind} in function '{identifier}'");
                
                expect(TokenType.CloseParen);

                if(at().Type == TokenType.EndOfStmt)
                {
                    varDecl.Value = new NullLiteral();
                    return varDecl;
                }

                break;
            }
            default: { expect(TokenType.Equals); break; }
        }
        
        varDecl.Value = ParseExpr();
        
        if(varDecl.Value.Kind != NodeType.FunctionLiteral) expect(TokenType.EndOfStmt);
        
        return varDecl;
    }

    private Stmt parseIfStatement()
    {
        advance();
        expect(TokenType.OpenParen);
        
        IfStatement ifStmt = new();
        
        ifStmt.Condition = parseConditionalExpr();
        expect(TokenType.CloseParen);
        
        ifStmt.Body = parseFunctionLiteral() as FunctionLiteral;

        if(at().Type == TokenType.Else)
        {
            advance();
            ifStmt.ElseBody = parseFunctionLiteral() as FunctionLiteral;
        }
        
        return ifStmt;
    }
    
    private Stmt ParseStmt()
    {
        switch(at().Type)
        {
            case TokenType.ConstantDeclaration: return parseVarDeclaration();
            case TokenType.VarDeclaration: return parseVarDeclaration();
            case TokenType.FunctionDeclaration: return parseVarDeclaration();
            case TokenType.IfStatement: return parseIfStatement();
        }

        Expr expr = ParseExpr();
        
        switch(expr.Kind)
        {
            case NodeType.CallExpr: expect(TokenType.EndOfStmt); break;
            case NodeType.AssignmentExpr: expect(TokenType.EndOfStmt); break;
            default: throw new Exception($"Expected statement but got {expr.Kind}");
        }
        
        return expr;
    }
}