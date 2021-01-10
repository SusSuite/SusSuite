# Developing a SusSuite Plugin

## Setup

Install the **SusSuite.Core** NuGet Package  
[![NuGet](https://img.shields.io/nuget/vpre/SusSuite.Core)](https://www.nuget.org/packages/SusSuite.Core/)

Your plugin will need to create an SusSuitePlugin object and register it with SusSuiteManager

```csharp

[ImpostorPlugin(...)]
public class JesterPlugin : PluginBase
{
    private readonly SusSuiteManager _susSuiteManager;
    private readonly SusSuitePlugin _myPluginInfo;

    public JesterPlugin(SusSuiteManager susSuiteManager)
    {
        _susSuiteManager = susSuiteManager;

        _myPluginInfo = new SusSuitePlugin()
        {
            Name = "Jester",
            Description = "...",
            Author = "Gavin Steinhoff",
            Version = "1.0.0",
            PluginType = PluginType.GameMode,
            PluginColor = "[00aaffff]"
        };
    }

    public override ValueTask EnableAsync()
    {
        _susSuiteManager.PluginManager.RegisterPlugin(_myPluginInfo);
        return default;
    }

    public override ValueTask DisableAsync()
    {
        _susSuiteManager.PluginManager.UnRegisterPlugin(_myPluginInfo);
        return default;
    }
}

```

## How to Use

Your plugin is stored in the SusSuiteManger. You can get your plugin via its name.  
You can then get a SusSuiteCore object for your plugin:

```csharp
private readonly SusSuiteCore _susSuiteCore;
public JesterEventListener(SusSuiteManager susSuiteManager)
{
    var susSuiteCorePlugin = susSuiteManager.PluginManager.GetPlugin("Jester");
    _susSuiteCore = susSuiteManager.GetSusSuiteCore(susSuiteCorePlugin);
}
```

### Games

How to know if your plugin's game mode is enabled for a room:

```csharp
if (!_susSuiteCore.PluginService.IsGameModeEnabled(e.Game)) return;
```

Storing object data for a room:

```csharp
var jesterData = new JesterData();
_susSuiteCore.PluginService.SetData(e.Game, jesterData);
```

Getting room data:

```csharp
_susSuiteCore.PluginService.TryGetData<JesterData>(e.Game, out var jesterData); 
```

### Logging

SusSuiteCore provides a logging wrapper to provide better logging.

```csharp
_susSuiteCore.LoggerService.LogInformation("Log Stuff");
```

This will log:
> SusSuite => ExamplePlugin => Log Stuff

The logger supports everything the default logger provides:

```csharp
_susSuiteCore.LoggerService.LogDebug("Info: {0}", "message");
_susSuiteCore.LoggerService.LogError(ex, "error");
_susSuiteCore.LoggerService.Log(LogLevel.Information, "Hello");
```

### Helpers

Provide an easy way to send public and private room chat messages.  
Every new parameter is a new line.  
The message will appear to be coming a player with your plugin's name.

```csharp
_susSuiteCore.PluginService.SendMessageAsync(e.Game, "The Jester has been chosen!");
_susSuiteCore.PluginService.SendPrivateMessageAsync(jester, "pssst...", "You are the Jester", "Get Voted Out");
```

Provides a fun way to end the game:

```csharp
_susSuiteCore.PluginService.EndGame(e.Game, "Jester Wins!", 11000, PluginService.WinType.Impostor);
```

### Config

SusSuite makes it easy getting data from json config files.

```csharp
var jesterConfig = _susSuiteCore.ConfigService.GetConfig<JesterConfig>();
```

This looks for a json file **\plugins\Jester\Jester.json**

You can also pass in a name of a file, to find **\plugins\Jester\JesterAnotherConfig.json**

```csharp
var jesterConfig = _susSuiteCore.ConfigService.GetConfig<JesterConfig>("JesterAnotherConfig");
```

#### Validation

You can make sure your plugin will only run with a valid config:

```csharp
var validation = new JsonSerializerOptions();
validation.Converters.Add(new SusSuiteConfigPropertyConverter());

var serverConfig = _susSuiteCore.ConfigService.GetConfig<SusSuiteConfig>(validation);
var serverConfig = _susSuiteCore.ConfigService.GetConfig<SusSuiteConfig>("SusSuiteServer", validation);
```

This class will need to be added to every class you want to validate:

```csharp
public class SusSuiteConfig
 {
    public string ServerName { get; set; }
    public string ServerColor { get; set; }
}

public class SusSuiteConfigPropertyConverter : JsonConverter<SusSuiteConfig>
{
    public override SusSuiteConfig Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
    {
        var susSuiteConfig = JsonSerializer.Deserialize<SusSuiteConfig>(ref reader);

        var rx = new Regex(@"\[[0-9a-fA-F]{8}\]");
        if (!rx.IsMatch(susSuiteConfig.ServerColor)) throw new JsonException("ServerColor is not in proper format.");

        return susSuiteConfig;
    }

    public override void Write(Utf8JsonWriter writer, SusSuiteConfig susSuiteConfig, JsonSerializerOptions options)
    {
        //This is where you set your default values
        susSuiteConfig.ServerName = "SusSuiteServer";
        susSuiteConfig.ServerColor = "[ffaabbff]";
        JsonSerializer.Serialize(writer, susSuiteConfig);
    }
}
```

When validation fails, a .broken.json file will be made with the json that passed validation and a new json file will be made with default values.
