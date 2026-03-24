// Copyright (c) 2025 LeaLan (VadimLL). All rights reserved.
// License: MIT

using System;

using ElvisModifiersLib;

[AttributeUsage(Constants.OnlyYouTargets, AllowMultiple = true)]
public class OnlyNsAttribute : Attribute
{
    public OnlyNsAttribute(string ns) { }
}
