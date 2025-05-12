using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using CeeShard.Parsing;

namespace CeeShard;

public partial class Parser
{
    private Token[] tokens;
    // you could just shift the entire array by one every time you call advance() but thats bad for performance
    private int currentTokenIndex = 0;

    private Token at()
    {
        return tokens[currentTokenIndex];
    }
    
    private Token advance()
    {
        return tokens[currentTokenIndex++];
    }
    
    private Token expect(TokenType expectedType)
    {
        Token previousToken = advance();

        if(previousToken.Type != expectedType)
        {
            throw new Exception($"Expected {Enum.GetName(expectedType)} but got {previousToken.Value} (at line {previousToken.Line})");
        }
        
        return previousToken;
    }

    public LangProgram produceAST(Token[] tokens)
    {
        this.tokens = tokens;
        currentTokenIndex = 0;
        
        List<Stmt> tree = new();

        while(currentTokenIndex < tokens.Length - 1)
        {
            tree.Add(ParseStmt());
        }
        
        LangProgram program = new LangProgram(tree.ToArray());
        
        return program;
    }
}