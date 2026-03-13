// Copyright (c) 2025 LeaLan (VadimLL). All rights reserved.
// License: MIT

using System;

//namespace ElvisModifiersLib;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor, AllowMultiple = true)]
public class AliasAttribute : Attribute
{
    public AliasAttribute(string alias) { }
}
