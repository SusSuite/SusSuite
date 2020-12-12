# SusSuite

SusSuite is a library to help write Impostor plugins.

[![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/SusSuite.Core)](https://www.nuget.org/packages/SusSuite.Core/)

## Use

Your plugin startup needs to include:
```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddSingleton<ISusSuiteCore, SusSuiteCore>();
    var jsonSerializerOptions = new JsonSerializerOptions();
    services.AddSingleton(jsonSerializerOptions);
}
```

Your plugin class needs to include:
```csharp
public ExamplePLugin(ISusSuiteCore susSuiteCore)
{
    _susSuiteCore.PluginName = "ExamplePlugin";
}
```
