# GodotLayersSourceGenerator
Source generator that will emit Godot layer names as constant values for use in masks using a Source Generator.

![image](https://github.com/AntonBergaker/GodotLayersSourceGenerator/assets/12459883/59317bdb-8f16-4e67-b30b-77641a5938ae)

Will generate code so you can grab these layers statically and typed using C# code.
```csharp
using Godot;

var myMask1 = LayerNames.Render2D.ImLayer1Mask;
var myMask2 = LayerNames.Render2D.ImLayer2Mask;
```

## Installation
1) Grab the Source Generator from [nuget](https://www.nuget.org/packages/GodotLayersSourceGenerator).
2) Add the project.godot as an additional file to your .csproj project file. Like this snippet:
```xml
<ItemGroup>
  <AdditionalFiles Include="project.godot" />
</ItemGroup>
```
