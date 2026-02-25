internal static class ElvisAttributes
{
    public const string OnlyYouAttribute = nameof(OnlyYouAttribute);
    public const string OnlyAliasAttribute = nameof(OnlyAliasAttribute);
    public const string FriendAliasAttribute = nameof(FriendAliasAttribute);

    public const string Code10 = $@"
using System;

[AttributeUsage(
      AttributeTargets.Class
    | AttributeTargets.Interface
    | AttributeTargets.Method
    | AttributeTargets.Property
    | AttributeTargets.Field
    , AllowMultiple = true)]
public class {OnlyYouAttribute} : Attribute
{{
    public {OnlyYouAttribute}(Type type, params string[] members) {{ }}
}}

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = true)]
public class {OnlyAliasAttribute} : Attribute
{{
    public {OnlyAliasAttribute}(Type type, params string[] aliases) {{ }}
}}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class {FriendAliasAttribute} : Attribute
{{
    public {FriendAliasAttribute}(string alias) {{ }}
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
    , AllowMultiple = true)]
public class {OnlyYouAttribute}<T> : Attribute
{{
    public {OnlyYouAttribute}(params string[] members) {{ }}
}}

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = true)]
public class {OnlyAliasAttribute}<T> : Attribute
{{
    public {OnlyAliasAttribute}(params string[] aliases) {{ }}
}}
";
}

