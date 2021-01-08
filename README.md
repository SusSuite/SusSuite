# SusSuite

SusSuite is a library to help write Impostor plugins.

[![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/SusSuite.Core)](https://www.nuget.org/packages/SusSuite.Core/)
[![Build Status](https://dev.azure.com/steinhoff/SusSuite/_apis/build/status/SusSuite.SusSuite?branchName=master)](https://dev.azure.com/steinhoff/SusSuite/_build/latest?definitionId=7&branchName=master)

## Setup

Your plugin will need to have an class that inherits SusSuiteCore and and initialization method:
```csharp
public class SusExamplePlugin : SusSuiteCore
{
    public SusExamplePlugin(ILogger<SusTranslatorPlugin> logger, JsonSerializerOptions jsonSerializerOptions) : base(logger, jsonSerializerOptions)
    {
        PluginName = "Exmaple";
    }
}
```
You can edit SusClient settings here as well:
```csharp
...
PluginName = "Exmaple";
ConfigServiceDefaultLogLevel = LogLevel.Information;
...
```

Your plugin startup needs to include:
```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddSingleton<SusExamplePlugin>();
    var jsonSerializerOptions = new JsonSerializerOptions();
    services.AddSingleton(jsonSerializerOptions);
}
```

## How to Use

Since your implementation of SusSuite (SusExamplePlugin) is dependency injected, it can be used in all your classes:
```csharp
private readonly SusExamplePlugin _susSuite;
public ExampleClass(SusExamplePlugin susSuiteCore)
{
    _susSuiteCore = susSuiteCore;
}
```

### Logging
SusSuite provides a logging wrapper to provide better logging.
```csharp
_susSuiteCore.Logger.LogInformation("Log Stuff");
```
This will log:
>  SusSuite => ExamplePlugin => Log Stuff

Many types of logs are available (info,debug,error), as well as parameters:
```csharp
_susSuiteCore.Logger.LogError("Error: {0}", "error");
```
This will log:
>  SusSuite => ExamplePlugin => Error: error

You can also use Log and pass any log level:
```csharp
_susSuiteCore.Logger.Log(LogLevel.Information, "Hello");
```
This will log:
>  SusSuite => ExamplePlugin => Hello

### Config
SusSuite provides getting json config files.
GetConfig will create a file if one does not exist with the plugin name.
The file will be located at:
> \plugins\ExamplePlugin\Example.json

```csharp
var settings = _susSuiteCore.ConfigService.GetConfig<ExampleSettings>();
```

You can also pass in a name of a file to store more config files:
```csharp
var settings = _susSuiteCore.ConfigService.GetConfig<ExampleSettings>("AnotherConfig");
```
> \plugins\ExamplePlugin\AnotherConfig.json

The config can be edited and will take affect on a server restart.

The config can also be validated. This class will need to be added to every class you want to validate:

```csharp
public class ExampleSettingPropertyConverter : JsonConverter<TranslatorSettings>
{
    public override ExampleSettings Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
    {
        // Don't pass in options when recursively calling Deserialize.
        ExampleSettings exampleSettings = JsonSerializer.Deserialize<TranslatorSettings>(ref reader);

        if (string.IsNullOrEmpty(exampleSettings.Endpoint)) throw new JsonException("Endpoint is null");

        return exampleSettings;
    }

    public override void Write(Utf8JsonWriter writer, ExampleSettings exampleSettings, JsonSerializerOptions options)
    {
        exampleSettings.Endpoint = "https://example.com";

        // Don't pass in options when recursively calling Serialize.
        JsonSerializer.Serialize(writer, exampleSettings);
    }
}
```

Then your startup will need to be edited to include the validation:
```csharp
var jsonSerializerOptions = new JsonSerializerOptions();
jsonSerializerOptions.Converters.Add(new ExampleSettingPropertyConverter());
services.AddSingleton(jsonSerializerOptions);
```
