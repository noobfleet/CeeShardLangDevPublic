namespace CeeShard;

public enum TokenType
{
    Number,
    String,
    Identifier,
    Null,
    // Grouping
    Operator,
    Dot,
    Comma,
    CloseParen,
    OpenParen,
    Equals,
    CloseBracket,
    OpenBracket,
    CloseCurlyBracket,
    OpenCurlyBracket,
    // keywords
    VarDeclaration,
    ConstantDeclaration,
    FunctionDeclaration,
    IfStatement,
    WhileStatement,
    ForStatement,
    Else,
    EndOfStmt,
    EndOfFile
}

public enum NodeType
{
    // Statements (no return value)
    Program,
    VarDeclaration,
    IfStatement,
    WhileStatement,
    ForStatement,
    // Expressions (generally has a return value)
    AssignmentExpr,
    MemberExpr,
    BinaryExpr,
    CallExpr,
    // Literals
    NumericLiteral,
    StringLiteral,
    BooleanLiteral,
    Identifier,
    ObjectLiteral,
    FunctionLiteral,
    NullLiteral,
}
