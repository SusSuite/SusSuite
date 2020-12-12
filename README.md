# SusSuite

SusSuite is a library to help write Impostor plugins.

[![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/SusSuite.Core)](https://www.nuget.org/packages/SusSuite.Core/)

## Setup

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
    susSuiteCore.PluginName = "ExamplePlugin";
}
```

## How to Use

### Logging
SusSuite provides a logging wrapper to provide better logging.
```csharp
susSuiteCore.Logger.LogInformation("Log Stuff");
```
This will log:
>  SusSuite => ExamplePlugin => Log Stuff

### Config
SusSuite provides getting json config files.
GetConfig will create a file if one does not exist.
The file will be located at:
> \plugins\ExamplePlugin\ExampleSettings.json

```csharp
var settings = susSuiteCore.ConfigService.GetConfig<ExampleSettings>("ExampleSettings");
```

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























