namespace CeeShard.Runtime;

public static class NativeFunctions
{
    private static NullRuntimeValue consoleLog(RuntimeValue[] strs)
    {
        foreach(RuntimeValue s in strs)
        {
            if(s.Type != RuntimeValueType.String && s.Type != RuntimeValueType.Number) return new NullRuntimeValue();
            Console.WriteLine((s as StringRuntimeValue).Value);
        }
        
        return new NullRuntimeValue();
    }

    private static StringRuntimeValue consoleRead(RuntimeValue[] _) => new StringRuntimeValue(Console.ReadLine());
    private static NullRuntimeValue consoleError(RuntimeValue[] strs)
    {
        foreach(RuntimeValue s in strs)
        {
            if (s.Type != RuntimeValueType.String && s.Type != RuntimeValueType.Number) return new NullRuntimeValue();
                throw new Exception((s as StringRuntimeValue).Value);
        }
        
        return new NullRuntimeValue();
    }

    private static NullRuntimeValue consoleClear(RuntimeValue[] _)
    {
        Console.Clear();
        return new NullRuntimeValue();
    }

    public static ObjectRuntimeValue consoleLib()
    {
        Dictionary<string, RuntimeValue> dict = new Dictionary<string, RuntimeValue>();
        
        dict["Log"] = new NativeFunctionRuntimeValue(consoleLog);
        dict["Error"] = new NativeFunctionRuntimeValue(consoleError);
        dict["Read"] = new NativeFunctionRuntimeValue(consoleRead);
        dict["Clear"] = new NativeFunctionRuntimeValue(consoleClear);
        
        return new ObjectRuntimeValue(dict);
    }
}
