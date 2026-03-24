// Copyright (c) 2025 LeaLan (VadimLL). All rights reserved.
// License: MIT

public static class ElvisAttributes
{
    public const string OnlyYouAttribute = nameof(OnlyYouAttribute);
    public const string OnlyAliasAttribute = nameof(OnlyAliasAttribute);
    public const string OnlyYouSetAttribute = nameof(OnlyYouSetAttribute);
    public const string OnlyAliasSetAttribute = nameof(OnlyAliasSetAttribute);
    public const string AliasAttribute = nameof(AliasAttribute);
    public const string ExcludeAttribute = nameof(ExcludeAttribute);
    public const string ExcludeMembersAttribute = nameof(ExcludeMembersAttribute);
    public const string OnlyNsAttribute = nameof(OnlyNsAttribute);

    public const string Code10 = $@"
using System;

[AttributeUsage(
      AttributeTargets.Class
    | AttributeTargets.Interface
    | AttributeTargets.Method
    | AttributeTargets.Property
    | AttributeTargets.Field
    | AttributeTargets.Constructor
    | AttributeTargets.Event
    , AllowMultiple = true)]
public class {OnlyYouAttribute} : Attribute
{{
    public {OnlyYouAttribute}(Type type, params string[] members) {{ }}
}}

[AttributeUsage(
      AttributeTargets.Class
    | AttributeTargets.Interface
    | AttributeTargets.Method
    | AttributeTargets.Property
    | AttributeTargets.Field
    | AttributeTargets.Constructor
    | AttributeTargets.Event
    , AllowMultiple = true)]
public class {OnlyAliasAttribute} : Attribute
{{
    public {OnlyAliasAttribute}(Type type, params string[] aliases) {{ }}
}}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class {AliasAttribute} : Attribute
{{
    public {AliasAttribute}(string alias) {{ }}
}}
";

    public const string Code11 = $@"
{Code10}

[AttributeUsage(
      AttributeTargets.Class
    | AttributeTargets.Interface
    | AttributeTargets.Method
    | AttributeTargets.Property
    | AttributeTargets.Field
    | AttributeTargets.Constructor
    | AttributeTargets.Event
    , AllowMultiple = true)]
public class {OnlyYouAttribute}<T> : Attribute
{{
    public {OnlyYouAttribute}(params string[] members) {{ }}
}}

[AttributeUsage(
      AttributeTargets.Class
    | AttributeTargets.Interface
    | AttributeTargets.Method
    | AttributeTargets.Property
    | AttributeTargets.Field
    | AttributeTargets.Constructor
    | AttributeTargets.Event
    , AllowMultiple = true)]
public class {OnlyAliasAttribute}<T> : Attribute
{{
    public {OnlyAliasAttribute}(params string[] aliases) {{ }}
}}
";
}

