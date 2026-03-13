// Copyright (c) 2025 LeaLan (VadimLL). All rights reserved.
// License: MIT

using System;

namespace ElvisModifiersLib;

internal static class Constants
{
    internal const AttributeTargets OnlyYouTargets =
          AttributeTargets.Class
        | AttributeTargets.Interface
        | AttributeTargets.Method
        | AttributeTargets.Property
        | AttributeTargets.Field
        | AttributeTargets.Constructor
        | AttributeTargets.Event;

    internal const AttributeTargets OnlyYouSetTargets =
          AttributeTargets.Class
        | AttributeTargets.Interface
        | AttributeTargets.Property
        | AttributeTargets.Field;

    internal const AttributeTargets ExcludeTargets =
          AttributeTargets.Method
        | AttributeTargets.Property
        | AttributeTargets.Field
        | AttributeTargets.Constructor
        | AttributeTargets.Event;
}
