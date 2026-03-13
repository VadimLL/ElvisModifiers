// Copyright (c) 2025 LeaLan (VadimLL). All rights reserved.
// License: MIT

using System;
//using System.Linq.Expressions;

using ElvisModifiersLib;

[AttributeUsage(Constants.OnlyYouTargets, AllowMultiple = true)]
public class OnlyYouAttribute : Attribute // OnlyUAttribute // OUAttribute
{
    public OnlyYouAttribute(Type type, params string[] members) { }
    
    //public OnlyYouAttribute(params Expression[] expressions) { }
}

// variant only for C# 11 and above:
[AttributeUsage(Constants.OnlyYouTargets, AllowMultiple = true)]
public class OnlyYouAttribute<T> : Attribute
{
    public OnlyYouAttribute(params string[] members) { }
}


/*-----------------------------------------------------------*/


[AttributeUsage(Constants.OnlyYouTargets, AllowMultiple = true)]
public class OnlyAliasAttribute : Attribute
{
    public OnlyAliasAttribute(Type type, params string[] aliases) { }
}

// variant for C# 11 and above:
[AttributeUsage(Constants.OnlyYouTargets, AllowMultiple = true)]
public class OnlyAliasAttribute<T> : Attribute
{
    public OnlyAliasAttribute(params string[] aliases) { }
}
