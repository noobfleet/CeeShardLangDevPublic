namespace CeeShard.Runtime;

public enum RuntimeValueType
{
    Number,
    String,
    Boolean,
    Null,
    Object,
    Function,
    NativeFunction,
}

public class RuntimeValue
{
    public RuntimeValueType Type { get; internal set; }
}

public class NullRuntimeValue : RuntimeValue
{
    public NullRuntimeValue()
    {
        Type = RuntimeValueType.Null;
    }
}

public class NumberRuntimeValue : RuntimeValue
{
    public double Value;

    public NumberRuntimeValue(double value)
    {
        Type = RuntimeValueType.Number;
        Value = value;
    }
}

public class StringRuntimeValue : RuntimeValue
{
    public string Value;

    public StringRuntimeValue(string value)
    {
        Type = RuntimeValueType.String;
        Value = value;
    }
}

public class BooleanRuntimeValue : RuntimeValue
{
    public bool Value;

    public BooleanRuntimeValue(bool value)
    {
        Type = RuntimeValueType.Boolean;
        Value = value;
    }
}

public class ObjectRuntimeValue : RuntimeValue
{
    public Dictionary<string, RuntimeValue> Properties = new();

    public ObjectRuntimeValue()
    {
        Type = RuntimeValueType.Object;
    }

    public ObjectRuntimeValue(Dictionary<string, RuntimeValue> properties)
    {
        Properties = properties;
    }
}

public class FunctionRuntimeValue : RuntimeValue
{
    public Stmt[] Body;
    public Identifier[] Parameters;

    public FunctionRuntimeValue()
        : this(null) {}
    
    public FunctionRuntimeValue(Stmt[] body)
    {
        Body = body;
        Type = RuntimeValueType.Function;
    }
}

public delegate RuntimeValue Callback(RuntimeValue[] args);

public class NativeFunctionRuntimeValue : RuntimeValue
{
    public Callback Callback;

    public NativeFunctionRuntimeValue(Callback callback)
    {
        Callback = callback;
        Type = RuntimeValueType.NativeFunction;
    }
}
