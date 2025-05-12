using System;

namespace CeeShard.Runtime;

public partial class Interpreter
{
    private void EvaluateVarDeclaration(VarDeclaration declaration, ref Environment env)
    {
        RuntimeValue value;

        if(declaration.Value != null) value = evaluate(declaration.Value, ref env);
        else value = new NullRuntimeValue();

        env.DeclareVariable(declaration.Identifier, value, declaration.IsConstant);
    }

    private void EvaluateIfStatement(IfStatement stmt, ref Environment env)
    {
        if((evaluate(stmt.Condition, ref env) as BooleanRuntimeValue).Value) new Interpreter(true).Run(new LangProgram(EvaluateFunctionLiteral(stmt.Body, ref env).Body), ref env, showOutput);
            else if(stmt.ElseBody != null) new Interpreter(true).Run(new LangProgram(EvaluateFunctionLiteral(stmt.ElseBody, ref env).Body), ref env, showOutput);
}

    private RuntimeValue evaluateStatement(Stmt astNode, ref Environment env)
    {
        currentIndex++;
        if(astNode == null) return new NullRuntimeValue();
        switch(astNode.Kind)
        {
            case NodeType.VarDeclaration: EvaluateVarDeclaration(astNode as VarDeclaration, ref env); return new NullRuntimeValue();
            case NodeType.CallExpr: EvaluateCallExpr(astNode as CallExpr, ref env); return new NullRuntimeValue();
            case NodeType.AssignmentExpr: EvaluateAssignmentExpr(astNode as AssignmentExpr, ref env); return new NullRuntimeValue();
            case NodeType.IfStatement: EvaluateIfStatement(astNode as IfStatement, ref env); return new NullRuntimeValue();
        }
        
        throw new Exception($"Unexpected statement type: {astNode.Kind}");
    }
}