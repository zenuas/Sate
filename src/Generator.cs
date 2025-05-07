using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Sate;

[Generator(LanguageNames.CSharp)]
public class Generator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var project_dir = context.AnalyzerConfigOptionsProvider
            .Select(static (x, token) => (
                ProjectDir: x.GlobalOptions.TryGetValue("build_property.ProjectDir", out var project_dir) ? project_dir : "",
                RootNamespace: x.GlobalOptions.TryGetValue("build_property.RootNamespace", out var root_namespace) ? root_namespace : ""
            ));

        context.RegisterSourceOutput(
            context
                .AdditionalTextsProvider
                .Combine(project_dir)
                .Where(x => x.Left.Path.EndsWith(".ts.sql"))
                .Select((x, token) => (AdditionalText: x.Left, SourceText: x.Left.GetText(token), Option: x.Right))
                .Where(x => x.SourceText != null)
                .Select((x, token) =>
                {
                    var file_name = Path.GetFileName(x.AdditionalText.Path);
                    var dir_path = Path.GetDirectoryName(x.AdditionalText.Path);
                    var relative_path = dir_path.StartsWith(x.Option.ProjectDir) ? dir_path.Substring(x.Option.ProjectDir.Length) : "";
                    var class_name = file_name.Substring(0, file_name.IndexOf('.'));
                    var ns_name = (relative_path == "" ? x.Option.RootNamespace : $"{x.Option.RootNamespace}.{PathToNamespace(relative_path)}");
                    return (
                        HintName: $"{ns_name}.{class_name}.cs",
                        Source: GenerateSource(ns_name, class_name, x.SourceText!.Lines.Select(x => x.ToString()))
                    );
                }),
            (sp, x) => sp.AddSource(x.HintName, x.Source)
        );
    }

    public static string PathToNamespace(string path) => path.Replace(Path.DirectorySeparatorChar, '.');

    public static string GenerateSource(string ns_name, string class_name, IEnumerable<string> sql)
    {
        return $@"
namespace {ns_name}
{{
    public static class {class_name}
    {{
        public static void Build(dynamic _)
        {{
            System.Console.WriteLine(""dyn"");
        }}
    }}
}}
";
    }
}
