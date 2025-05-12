namespace CeeShard.Runtime;

public partial class Interpreter
{
    private NumberRuntimeValue EvaluateNumber(NumericLiteral number, ref Environment env) => new NumberRuntimeValue(number.Value);
    private RuntimeValue EvaluateIdentifier(Identifier identifier, ref Environment env) => env.GetVariable(identifier.Symbol);
    private StringRuntimeValue EvaluateString(StringLiteral str, ref Environment env) => new StringRuntimeValue(str.Value);

    private ObjectRuntimeValue EvaluateObject(ObjectLiteral obj, ref Environment env)
    { // work on parsing this first, the ast object needs a properties field
        ObjectRuntimeValue val = new();
        
        foreach(KeyValuePair<string, Expr> key in obj.Properties)
        {
            val.Properties.Add(key.Key, evaluate(key.Value, ref env));
        }
        
        return val;
    }

    private FunctionRuntimeValue EvaluateFunctionLiteral(FunctionLiteral func, ref Environment env)
    { // this is basically just a shell class
        FunctionRuntimeValue val = new();
        val.Body = func.Body;
        val.Parameters = func.Parameters;
        return val;
    }
        
    private RuntimeValue EvaluateBinaryExpr(BinaryExpr expr, ref Environment env)
    {
        if(expr.Left.Kind == NodeType.NullLiteral || expr.Right.Kind == NodeType.NullLiteral)
            return new NullRuntimeValue();
        
        double left = (evaluate(expr.Left, ref env) as NumberRuntimeValue).Value;
        double right = (evaluate(expr.Right, ref env) as NumberRuntimeValue).Value;
        
        switch(expr.Operator)
        {
            case "+": return new NumberRuntimeValue(left + right);
            case "-": return new NumberRuntimeValue(left - right);
            case "*": return new NumberRuntimeValue(left * right);
            case "/": return new NumberRuntimeValue(left / right);
            case "^": return new NumberRuntimeValue((int)left ^ (int)right);
            case "%": return new NumberRuntimeValue(left % right);
            case "&&":
            {
                RuntimeValue leftBool = evaluate(expr.Left, ref env);
                RuntimeValue rightBool = evaluate(expr.Right, ref env);
                if (leftBool.Type != RuntimeValueType.Boolean || rightBool.Type != RuntimeValueType.Boolean) throw new Exception("Conditional expression result is not of type boolean");
                return new BooleanRuntimeValue((leftBool as BooleanRuntimeValue).Value && (rightBool as BooleanRuntimeValue).Value);
            }
            case "||":
            {
                RuntimeValue leftBool = evaluate(expr.Left, ref env);
                RuntimeValue rightBool = evaluate(expr.Right, ref env);
                if (leftBool.Type != RuntimeValueType.Boolean || rightBool.Type != RuntimeValueType.Boolean) throw new Exception("Conditional expression result is not of type boolean");
                return new BooleanRuntimeValue((leftBool as BooleanRuntimeValue).Value || (rightBool as BooleanRuntimeValue).Value);
            }
            case "==":
            {
                RuntimeValue leftBool = evaluate(expr.Left, ref env);
                RuntimeValue rightBool = evaluate(expr.Right, ref env);
                return new BooleanRuntimeValue(leftBool.GetHashCode() == rightBool.GetHashCode()); // need to find another way to implement this
            }
            case "!":
            {
                RuntimeValue boolean = evaluate(expr.Left, ref env);
                if (boolean.Type != RuntimeValueType.Boolean) throw new Exception("Conditional expression result is not of type boolean");
                return new BooleanRuntimeValue(!(boolean as BooleanRuntimeValue).Value);
            }
        }
        
        return new NullRuntimeValue();
    }

    private RuntimeValue EvaluateCallExpr(CallExpr expr, ref Environment env)
    {
        
        Interpreter interpreter = new(true);
        Environment environment = new(env);
        
        RuntimeValue function = evaluate(expr.Caller, ref env);

        RuntimeValue[] args = new RuntimeValue[expr.Args.Length];
        for(int i = 0; i < expr.Args.Length; i++)
            args[i] = evaluate(expr.Args[i], ref env);
        
        if(function.Type == RuntimeValueType.NativeFunction)
        {
            return (function as NativeFunctionRuntimeValue).Callback(args);
        }

        for(int i = 0; i < expr.Args.Length; i++)
            environment.DeclareVariable((function as FunctionRuntimeValue).Parameters[i].Symbol, args[i], false);

        return interpreter.Run(new LangProgram((function as FunctionRuntimeValue).Body), ref environment, showOutput);
    }

    private RuntimeValue EvaluateAssignmentExpr(AssignmentExpr assignment, ref Environment env)
    {
        RuntimeValue value;

        if(assignment.Value != null) value = evaluate(assignment.Value, ref env);
        else value = new NullRuntimeValue();
        switch(assignment.Assignee.Kind)
        {
            case NodeType.Identifier:
            {
                return env.AssignVariable((assignment.Assignee as Identifier).Symbol, value);
            }
            case NodeType.MemberExpr:
            {
                return env.AssignValueToObjectKey(
                    (assignment.Assignee as MemberExpr).Object.Symbol,
                    ((assignment.Assignee as MemberExpr).Property as Identifier).Symbol,
                    value
                );
            }
            default: throw new Exception($"Cannot assign variable with type {assignment.Assignee.Kind}");
        }
    }

    private RuntimeValue evaluate(Stmt astNode, ref Environment env)
    {
        if(astNode == null) return new NullRuntimeValue();
        switch(astNode.Kind)
        {
            case NodeType.NumericLiteral: return EvaluateNumber(astNode as NumericLiteral, ref env);
            case NodeType.NullLiteral: return new NullRuntimeValue();
            case NodeType.Identifier: return EvaluateIdentifier(astNode as Identifier, ref env);
            case NodeType.MemberExpr: return (EvaluateIdentifier((astNode as MemberExpr).Object, ref env) as ObjectRuntimeValue).Properties[((astNode as MemberExpr).Property as Identifier).Symbol];
            case NodeType.CallExpr: return EvaluateCallExpr(astNode as CallExpr, ref env);
            case NodeType.StringLiteral: return EvaluateString(astNode as StringLiteral, ref env);
            case NodeType.ObjectLiteral: return EvaluateObject(astNode as ObjectLiteral, ref env);
            case NodeType.FunctionLiteral: return EvaluateFunctionLiteral(astNode as FunctionLiteral, ref env);
            case NodeType.BinaryExpr: return EvaluateBinaryExpr(astNode as BinaryExpr, ref env);
            case NodeType.AssignmentExpr: EvaluateAssignmentExpr(astNode as AssignmentExpr, ref env); return new NullRuntimeValue();
        }
        
        throw new Exception($"Unexpected node type: {astNode.Kind}");
    }
}
