using System;
using System.Collections.Generic;

namespace CeeShard.Runtime;

public class Environment
{
    public Environment Parent;

    private Dictionary<string, RuntimeValue> Variables = new();
    private Dictionary<string, bool> Constants = new();

    public Environment() : this(null) { }

    public Environment(Environment parent)
    {
        Parent = parent;

        if(Parent != null) return;
        
        DeclareVariable("Console", NativeFunctions.consoleLib(), true);
    }
    
    public void DeclareVariable(string name, RuntimeValue value = null, bool isConstant = false)
    {
        if(Variables.TryAdd(name, value) == false) throw new Exception($"Cannot define variable '{name}'because it already exists");
        if(isConstant)  Constants[name] = true;
    }

    public RuntimeValue AssignVariable(string name, RuntimeValue value)
    {
        Environment env = ResolveEnvironment(name);
        if(!env.Variables.ContainsKey(name)) throw new Exception($"Cannot assign variable '{name}' before declaration");
        if(env.Constants.ContainsKey(name)) throw new Exception($"Cannot reassign constant '{name}'");
        
        return env.Variables[name] = value;
    }
    
    // im assuming this is faster than calling GetVariable and then AssignVariable
    public RuntimeValue AssignValueToObjectKey(string name, string key, RuntimeValue value)
    {
        Environment env = ResolveEnvironment(name);
        if(!env.Variables.ContainsKey(name)) throw new Exception($"Cannot assign variable '{name}' before declaration");
        if(env.Constants.ContainsKey(name)) throw new Exception($"Cannot reassign constant '{name}'");
        if(env.Variables[name].Type != RuntimeValueType.Object) throw new Exception($"Cannot index variable '{name}' that is not an array");
        
        return (env.Variables[name] as ObjectRuntimeValue).Properties[key] = value;
    }
    
    public RuntimeValue GetVariable(string name)
    {
        Environment env = ResolveEnvironment(name);
        
        if(env == null) return new NullRuntimeValue();
        
        return env.Variables[name] != null ? env.Variables[name] : new NullRuntimeValue();
    }
    
    // get the environment that declared the variable
    private Environment ResolveEnvironment(string name)
    {
        if(Variables.ContainsKey(name)) return this;
        if(Parent == null)
        {
            throw new Exception($"Unknown variable: {name}");
        }
        
        Environment currentEnv = Parent;
        while(currentEnv != null)
        {
            if(currentEnv.Variables.ContainsKey(name)) return currentEnv;
            currentEnv = currentEnv.Parent;
        }

        throw new Exception($"Unknown variable: {name}");
    }
}