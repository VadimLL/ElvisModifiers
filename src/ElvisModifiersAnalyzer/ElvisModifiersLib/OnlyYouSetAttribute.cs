// Copyright (c) 2025 LeaLan (VadimLL). All rights reserved.
// License: MIT

using System;

//namespace ElvisModifiersLib;

[AttributeUsage(
      AttributeTargets.Class
    | AttributeTargets.Interface
    | AttributeTargets.Property
    | AttributeTargets.Field
    , AllowMultiple = true)]
public class OnlyYouSetAttribute : Attribute
{
    public OnlyYouSetAttribute(Type type, params string[] members) { }
}

// next variant only for C# 11 and above:
[AttributeUsage(
      AttributeTargets.Class
    | AttributeTargets.Interface
    | AttributeTargets.Property
    | AttributeTargets.Field
    , AllowMultiple = true)]
public class OnlyYouSetAttribute<T> : Attribute
{
    public OnlyYouSetAttribute(params string[] members) { }
}


/*-----------------------------------------------------------*/


[AttributeUsage(
      AttributeTargets.Class
    | AttributeTargets.Interface
    | AttributeTargets.Property
    | AttributeTargets.Field
    , AllowMultiple = true)]
public class OnlyAliasSetAttribute : Attribute
{
    public OnlyAliasSetAttribute(Type type, params string[] members) { }
}

// next variant only for C# 11 and above:
[AttributeUsage(
      AttributeTargets.Class
    | AttributeTargets.Interface
    | AttributeTargets.Property
    | AttributeTargets.Field
    , AllowMultiple = true)]
public class OnlyAliasSetAttribute<T> : Attribute
{
    public OnlyAliasSetAttribute(params string[] members) { }
}
