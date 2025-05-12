using CeeShard.Runtime;

using System;

namespace CeeShard;

public class PrettyPrint
{
    private string binaryExpr(BinaryExpr expr)
    {
        // i love chatgpt code and garbage formatting cuz idk wtf this is
        var leftExpr = expr.Left is NumericLiteral left ? Convert.ToString(left.Value) : this.binaryExpr(expr.Left as BinaryExpr);
        var rightExpr = expr.Right is NumericLiteral right ? Convert.ToString(right.Value) : this.binaryExpr(expr.Right as BinaryExpr);

        return $"{leftExpr} {expr.Operator} {rightExpr}";
    }

    public string ParseExpression(Expr expr)
    {
        switch(expr.Kind)
        {
            case NodeType.BinaryExpr: return binaryExpr(expr as BinaryExpr);
            case NodeType.NumericLiteral: return (expr as NumericLiteral).Value.ToString();
            case NodeType.Identifier: return (expr as Identifier).Symbol + " (variable)";
            case NodeType.NullLiteral: return "null";
            
            default: throw new Exception($"PrettyPrint: Unknown expression type: {expr.Kind}");
        }
    }

    private string objectExpr(ObjectRuntimeValue obj)
    {
        string str = "{";
        foreach(var prop in obj.Properties) str += $"\n\t{prop.Key}: {ParseExpression(prop.Value)}";
        str += "\n}";
        return str;
    }

    public string ParseExpression(RuntimeValue val)
    {
        switch(val.Type)
        {
            case RuntimeValueType.Number: return (val as NumberRuntimeValue).Value.ToString();
            case RuntimeValueType.String: return (val as StringRuntimeValue).Value;
            case RuntimeValueType.Object: return objectExpr(val as ObjectRuntimeValue);
            case RuntimeValueType.Null: return "null";
            
            default: throw new Exception($"PrettyPrint: Unknown value type: {val.Type}");
        }
    }
    
}