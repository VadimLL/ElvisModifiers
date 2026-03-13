// Copyright (c) 2025 LeaLan (VadimLL). All rights reserved.
// License: MIT

using System;

using ElvisModifiersLib; 

[AttributeUsage(Constants.OnlyYouSetTargets, AllowMultiple = true)]
public class OnlyYouSetAttribute : Attribute
{
    public OnlyYouSetAttribute(Type type, params string[] members) { }
}

// variant only for C# 11 and above:
[AttributeUsage(Constants.OnlyYouSetTargets, AllowMultiple = true)]
public class OnlyYouSetAttribute<T> : Attribute
{
    public OnlyYouSetAttribute(params string[] members) { }
}


/*-----------------------------------------------------------*/


[AttributeUsage(Constants.OnlyYouSetTargets, AllowMultiple = true)]
public class OnlyAliasSetAttribute : Attribute
{
    public OnlyAliasSetAttribute(Type type, params string[] members) { }
}

// variant only for C# 11 and above:
[AttributeUsage(Constants.OnlyYouSetTargets, AllowMultiple = true)]
public class OnlyAliasSetAttribute<T> : Attribute
{
    public OnlyAliasSetAttribute(params string[] members) { }
}
