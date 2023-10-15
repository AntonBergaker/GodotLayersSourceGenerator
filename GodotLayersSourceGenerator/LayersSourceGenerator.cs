using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

namespace GodotLayersSourceGenerator;

[Generator]
public class LayersSourceGenerator : IIncrementalGenerator {
    public void Initialize(IncrementalGeneratorInitializationContext context) {
        //Debugger.Launch();

        var projectFiles = context.AdditionalTextsProvider.Where(CheckProjectPath);

        var layerNames = projectFiles.Select(GetLayerNames);

        var projectFileCount = projectFiles.Collect().Select((x, _) => x.Length);

        context.RegisterSourceOutput(projectFileCount, EmitNoImportWarning);
        context.RegisterSourceOutput(layerNames, EmitCategoryNames);
    }

    bool CheckProjectPath(AdditionalText text) {
        return Path.GetFileName(text.Path) == "project.godot";
    }


    private List<string> GetLayerNames(AdditionalText a, CancellationToken token) {
        var content = a.GetText(token)?.ToString();

        if (content == null) {
            return new();
        }

        var lines = content.Split('\n');
        int layerNamesStart = Array.IndexOf(lines, "[layer_names]");
        if (layerNamesStart <= 0) {
            return new();
        }

        var list = new List<string>();

        // Scan all the layers
        for (int i = layerNamesStart+1; i < lines.Length; i++) {
            var line = lines[i];
            if (string.IsNullOrWhiteSpace(line)) {
                continue;
            }
            // Start of a new section, break out
            if (line[0] == '[') {
                break;
            }
            list.Add(line);
        }

        return list;
    }

    private void EmitNoImportWarning(SourceProductionContext context, int count) {
        if (count != 0) {
            return;
        }

        var descriptor = new DiagnosticDescriptor(
            "MissingGodotProjectFile",
            "The project.godot file is not included in the project",
            "The project.godot file is not included in the project. When it's not added LayerSourceGenerator can not generate any layer names. "+
            "To fix add <AdditionalFiles Include=\"project.godot\"/> to the .csproj file."
            ,
            "Imports", 
            DiagnosticSeverity.Warning, 
            isEnabledByDefault: true
        );

        context.ReportDiagnostic(
            Diagnostic.Create(descriptor, null)
        );
    }

    private void EmitCategoryNames(SourceProductionContext context, List<string> layers) {
        var categories = new Dictionary<string, Dictionary<int, string>>();

        foreach (var layer in layers) {
            var categorySlashIndex = layer.IndexOf('/');
            var category = layer.Substring(0, categorySlashIndex);
            
            if (categories.TryGetValue(category, out var layersInCategory) == false) {
                layersInCategory = categories[category] = new Dictionary<int, string>();
            }

            var afterCategory = layer.Substring(categorySlashIndex + 1);
            var equalsIndex = afterCategory.IndexOf('=');
            var layerIndex = afterCategory.Substring(0, equalsIndex).Remove(0, 6);
            var layerName = afterCategory.Substring(equalsIndex + 1).Trim('"');

            layersInCategory.Add(int.Parse(layerIndex), layerName);
        }

        // Helper method to read dictionary so we can oneline building classes later
        Dictionary<int, string>? GetOrDefault(string key) {
            if (categories.TryGetValue(key, out var val)) {
                return val;
            }
            return null;
        }

        // Ugly ol namespace format for maximum portability
        var source = $$"""
            namespace Godot {
                public static class LayerNames {
                    {{BuildClass(2, "Render2D", GetOrDefault("2d_render"))}}

                    {{BuildClass(2, "Render3D", GetOrDefault("3d_render"))}}

                    {{BuildClass(2, "Physics2D", GetOrDefault("2d_physics"))}}

                    {{BuildClass(2, "Navigation2D", GetOrDefault("2d_navigation"))}}

                    {{BuildClass(2, "Physics3D", GetOrDefault("3d_physics"))}}

                    {{BuildClass(2, "Navigation3D", GetOrDefault("3d_navigation"))}}

                    {{BuildClass(2, "Avoidance", GetOrDefault("avoidance"))}}
                }
            }

            """;

        context.AddSource("Godot.LayerNames.g.cs", source);
    }

    private string BuildClass(int indentation, string name, Dictionary<int, string>? dict) {
        if (dict == null) {
            return "";
        }

        var sb = new StringBuilder();

        sb.AppendLine($"public static class {name} {{");

        for (int i = 1; i <= 32; i++) {
            if (dict.TryGetValue(i, out var layer) == false) {
                continue;
            }
            var indent = new string(' ', indentation * 4 + 4);
            var maskValue = (1) << (i-1);
            sb.AppendLine($"{indent}public const uint {layer}Mask = 0x{maskValue:X};");
        }

        sb.AppendLine(new string(' ', indentation * 4) + "}");

        return sb.ToString();
    }
}