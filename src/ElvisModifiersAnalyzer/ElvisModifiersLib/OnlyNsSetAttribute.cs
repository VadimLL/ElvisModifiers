// Copyright (c) 2025 LeaLan (VadimLL). All rights reserved.
// License: MIT

using System;

using ElvisModifiersLib;

[AttributeUsage(Constants.OnlyYouSetTargets, AllowMultiple = true)]
public class OnlyNsSetAttribute : Attribute
{
    public OnlyNsSetAttribute(string ns) { }
}
