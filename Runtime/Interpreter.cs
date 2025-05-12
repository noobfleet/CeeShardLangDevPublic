using System;
using System.Data;
using System.Reflection;

namespace CeeShard.Runtime;

// rn this will interpret the ast itself, after making everything work correctly towards bytecode instead
public partial class Interpreter(bool isInsideFunction = false)
{
    private Stmt[] tree;
    private int currentIndex;
    private bool showOutput;
    
    private bool isInsideFunction = isInsideFunction;

    public RuntimeValue Run(LangProgram program, bool showOutput = true)
    {
        tree = program.Body;
        currentIndex = 0;
        this.showOutput = showOutput;
        
        Environment environment = new Environment();
        
        RuntimeValue result = new NullRuntimeValue();
        
        // for cli
        PrettyPrint p = new();
        if(showOutput)
        {
            while(currentIndex < tree.Length)
            {
                Console.WriteLine(p.ParseExpression(result = evaluateStatement(tree[currentIndex], ref environment)));
            }
        }
        else
        {
            while(currentIndex < tree.Length)
            {
                 result = evaluateStatement(tree[currentIndex], ref environment);
            }
        }
        
        return result;
    }
    
    public RuntimeValue Run(LangProgram program, ref Environment environment, bool showOutput = false)
    {
        tree = program.Body;
        currentIndex = 0;
        this.showOutput = showOutput;
        
        RuntimeValue result = new NullRuntimeValue();
        
        // for cli
        PrettyPrint p = new();
        if(showOutput)
        {
            while(currentIndex < tree.Length)
            {
                Console.WriteLine(p.ParseExpression(result = evaluateStatement(tree[currentIndex], ref environment)));
            }
        }
        else
        {
            while(currentIndex < tree.Length)
            {
                result = evaluateStatement(tree[currentIndex], ref environment);
            }
        }
        
        return result;
    }
}
