// Copyright (c) 2025 LeaLan (VadimLL). All rights reserved.
// License: MIT

using System;

using ElvisModifiersLib;

/// <summary>
/// Exclude (switch off) [Only*<T>] on the T for some member
/// </summary>
[AttributeUsage(Constants.ExcludeTargets, AllowMultiple = true)]
public class ExcludeAttribute : Attribute
{
    public ExcludeAttribute() { }
    public ExcludeAttribute(Type type) { }
    public ExcludeAttribute(string type) { }
}

[AttributeUsage(Constants.ExcludeTargets, AllowMultiple = true)]
public class ExcludeAttribute<T> : Attribute
{
    public ExcludeAttribute() { }
}

/*-----------------------------------------------------------*/

/// <summary>
/// Not implemented yet
/// </summary>
[AttributeUsage(Constants.ExcludeTargets, AllowMultiple = true)]
public class ExcludeMembersAttribute : Attribute
{
    public ExcludeMembersAttribute(Type type, string member0, params string[] members) { }
    public ExcludeMembersAttribute(string type, string member0, params string[] members) { }
}

[AttributeUsage(Constants.ExcludeTargets, AllowMultiple = true)]
public class ExcludeMembersAttribute<T> : Attribute
{
    public ExcludeMembersAttribute(string member0, params string[] members) { }
}
