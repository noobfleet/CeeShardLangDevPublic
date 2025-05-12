using System.Collections.Generic;
using System.Security.Claims;

namespace CeeShard;

// these are for organization
public class Stmt
{
    public NodeType Kind;
}
public class Expr : Stmt { }

/*
 *
 * STATEMENTS
 * 
 */

public class LangProgram : Stmt
{
    public LangProgram(Stmt[] stmts)
    {
        Kind = NodeType.Program;
        Body = stmts;
    }

    public Stmt[] Body;
}

public class VarDeclaration : Stmt
{
    public VarDeclaration()
    {
        Kind = NodeType.VarDeclaration;
    }
    public bool IsConstant;
    public string Identifier;
    public Expr Value;
}

public class IfStatement : Stmt
{
    public IfStatement()
    {
        Kind = NodeType.IfStatement;
    }
    public Expr Condition;
    public FunctionLiteral Body;
    public FunctionLiteral ElseBody;
}

/*
 *
 * EXPRESSIONS
 * 
*/

public class AssignmentExpr : Expr
{
    public AssignmentExpr()
    {
        Kind = NodeType.AssignmentExpr;
    }
    public Expr Assignee;
    public Expr Value;
}

public class BinaryExpr : Expr
{
    public BinaryExpr()
    {
        Kind = NodeType.BinaryExpr;
    }
    public Expr Left;
    public Expr Right;
    public string Operator;
}

public class CallExpr : Expr
{
    public CallExpr()
    {
        Kind = NodeType.CallExpr;
    }
    
    public Expr Caller;
    public Expr[] Args = [];
}

public class MemberExpr : Expr
{
    public MemberExpr()
    {
        Kind = NodeType.MemberExpr;
    }
    public Identifier Object;
    public Expr Property;
    public bool IsComputed = false;
}

/*
 *
 * LITERALS
 * 
*/

public class Identifier : Expr
{
    public Identifier()
    {
        Kind = NodeType.Identifier;
    }
    public string Symbol;
}

public class NumericLiteral : Expr
{
    public NumericLiteral()
    {
        Kind = NodeType.NumericLiteral;
    }
    public double Value;
}

public class StringLiteral : Expr
{
    public StringLiteral()
    {
        Kind = NodeType.StringLiteral;
    }
    public string Value;
}

public class BooleanLiteral : Expr
{
    public BooleanLiteral()
    {
        Kind = NodeType.BooleanLiteral;
    }
    
    public bool Value;
}

public class NullLiteral : Expr
{
    public NullLiteral()
    {
        Kind = NodeType.NullLiteral;
    }
}

public class ObjectLiteral : Expr
{
    public Dictionary<string, Expr> Properties;

    public ObjectLiteral()
    {
        Kind = NodeType.ObjectLiteral;
    }
}

public class FunctionLiteral : Expr
{
    public Stmt[] Body;
    public Identifier[] Parameters;

    public FunctionLiteral()
    {
        Kind = NodeType.FunctionLiteral;
    }
}
