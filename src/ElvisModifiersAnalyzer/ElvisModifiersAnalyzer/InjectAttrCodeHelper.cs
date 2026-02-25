// Copyright (c) 2025 LeaLan (VadimLL). All rights reserved.
// License: MIT

using System;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

using static DebugHelper;

namespace ElvisModifiersAnalyzer;

public static class InjectAttrCodeHelper
{
    //static LanguageVersion? version;

    public static void AutoDetectCsVersionAndInject(IncrementalGeneratorInitializationContext context)
    {        
        // Create a single item provider using compilation
        var csharpVersionProvider = context.CompilationProvider
            .Select((compilation, cancellationToken) =>
            {
                // Get parse options from compilation
                var parseOptions = compilation.SyntaxTrees
                    .FirstOrDefault()?.Options as CSharpParseOptions;
                return parseOptions?.LanguageVersion;
            });

        // Register output - will execute once per compilation
        context.RegisterSourceOutput(csharpVersionProvider,
            (productionContext, version) =>
            {
                if (version is not null)
                {
                    JustInject(context, (LanguageVersion)version);
                }

                Report(productionContext, $"version = {version}");
            });
    }

    /* using:
     * add to csproj file of target project:
       <PropertyGroup>
         ...
         <InjectAttrCodeCsVersion>11</InjectAttrCodeCsVersion>
       </PropertyGroup>
       <ItemGroup>
         ...
         <CompilerVisibleProperty Include="InjectAttrCodeCsVersion" />
       </ItemGroup>     
     * where 11 number is C# version of attributes code to inject
     */
    public static void UseParamForInject(IncrementalGeneratorInitializationContext context)
    {
        // Get analyzer config options
        var optionsProvider = context.AnalyzerConfigOptionsProvider;

        // Create source
        context.RegisterSourceOutput(
            context.CompilationProvider.Combine(optionsProvider),
            (spc, source) =>
            {
                var (compilation, options) = source;

                //Report(spc, string.Join(", ", options.GlobalOptions.Keys));

                // Read your custom parameter
                if (options.GlobalOptions.TryGetValue("build_property.InjectAttrCodeCsVersion",
                    out var paramValue))
                {
                    if(int.TryParse(paramValue, out int versionNum))
                    {
                        JustInject(context, versionNum >= 11 
                            ? LanguageVersion.CSharp11 : LanguageVersion.CSharp10);
                    }

                    Report(spc, $"v = {paramValue}");
                }
            });
    }

    static bool IsGenericAttrSupport(in LanguageVersion? version)
        => version is null ? false : version >= LanguageVersion.CSharp11;

    static readonly object _lock = new object();    
    static volatile bool wasInject = false;
    public static void JustInject(in IncrementalGeneratorInitializationContext context, LanguageVersion version)
    {
        if (wasInject)
        {
            return;
        }

        //lock (_lock)
        {
            //if (wasInject)
            //{
            //    return;
            //}

            wasInject = true;
            // Add the attribute to the compilation
            context.RegisterPostInitializationOutput(ctx => ctx.AddSource(
                "ElvisModifiersAnalyzer_Attributes.g.cs",
                Microsoft.CodeAnalysis.Text.SourceText.From(
                    IsGenericAttrSupport(version) 
                        ? ElvisAttributes.Code11 : ElvisAttributes.Code10,
                    System.Text.Encoding.UTF8))
            );
            Beep();
        }
    }
}
