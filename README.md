# Params_Logger
Simple in use library for logging called meber methods, exceptions, properties with parameters and their values, and saving them in clear and transparent format.

## Purpose

Library was created for logging method calls, exceptions, property changes and additional data selected by the user from a solution, and saving the logs in a file with clear, transparent and readable format. Currently logger works only with .Net Framework.

## Features

1. Parsing method and properties parameter values with their names, with using the minimum amount of code.
2. Saving the parameters logs with easy to read format.
3. Setup library configuration in separate log.config file.
4. Synchronous calls of the library interface.
5. Possibility to place log.config anywhere in the project.
6. Creating singleton object of the logger.
7. Logged data types:
  - property name with value,
  - called method name with property names and values,
  - special information strings called by developer,
  - exceptions.
8. Configuration covers:
  - log file path and name,
  - use only in debug or not,
  - deleting logs on application start,
  - log in console and/or file,
  - console display only INFO logs
  
# Usage

## Setup

To use the library you need to install it with [NuGet](https://www.nuget.org/packages/Params_Logger/ "NuGet"). After installation you can create singleton of `ParamsLogger` in your class and all other classes of your project:
```csharp
public class YourViewModel : IAsyncInitialization
{
        private static readonly ILogger _log = ParamsLogger.LogInstance.GetLogger(); //singleton

        public YourViewModel()
        {
            //ctor
        }
}
```

Somewhere inside of your project you need to create **log.config** file. Setup example:
```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <appSettings>
    <add
        key="debugOnly"
        value="true"
        />
    <add
        key="logFile"
        value="log.txt"
        />
    <add
        key="deleteLogs"
        value="true"
        />
    <add
        key="fileLog"
        value="true"
        />
    <add
        key="consoleLog"
        value="true"
        />
    <add
        key="infoOnlyConsole"
        value="true"
        />
  </appSettings>
</configuration>

```
Where:
 - `debugOnly` means that will save logs with debugger attached only, default value: `true`,
 - `logFile` means where will save the logs, default value: is project main folder with file name `log.txt`,
 - `deleteLogs` is for deleting log file on application startup, default value: `true`,
 - `fileLog` is a statement, that you want to save logs into log file, default value: `true`,
 - `consoleLog` is a statement, that you want to display logs in console, default value: `true`,
 - `infoOnlyConsole` if `consoleLog` is `true`, you can decide to display only **INFO** logs, default value: `true`.
 
 ## Logging

1. Logging called method with parameter names and values example:
```csharp
public void ExecutePauseButton(bool paused)
        {
            _log.Called(paused); //log
            if (paused == true)
            {
                _controlsService.GetStartedConfiguration();
            }
            else
            {
                _controlsService.GetPausedConfiguration();
            }
        }
```
Calling method with bool parameter gives sample output:
```
07:05:49.398|CALLED|ButtonsService|ExecutePauseButton((Boolean)paused=False)
```

2. Logging property with its name and value example:
```csharp
private bool _paused;
        public bool Paused
        {
            get => _paused;
            set
            {
                _paused = value;
                _log.Prop(_paused);
            }
        }
```
Setting up propaerty value with bool parameter gives sample output:
```
07:05:49.323|PROP|BrowserViewModel|set_Paused(True)
```

3. Logging special information string:
```csharp
_log.Info("Paused");
```
It gives sample output:
```
07:05:52.44|INFO|BrowseService|Pause((Paused))
```

4. Logging exceptions:
```csharp
public async Task LoopCollectingAsync()
        {
            try
            {
                InputCorrection = false;
                await GetNewRecordAsync();
                await LoopCollectingAsync();
            }
            catch (Exception e)
            {
                _log.Error(e.Message);
            }
        }
```
When method catches exception, it gives sample output:
```
08:41:55.370|ERROR|BrowseService|LoopCollectingAsync((The operation was canceled.))
```
## Sample output
![example](https://i.imgur.com/INe4tR3.png)

# Technology

1. Approaches:
  - is called synchronously,
  - logs are saved asynchronously,
  - locking log object on creation,
  - singleton pattern for logger class,
  - facade used for hiding dependencies,
  - factory pattern for creation of tyhe logger,
  - logging unhandled fatal exceptions.
  
2. Library is using:
  - .Net Standard 2.0,
  - System.Reflection,
  - System.Configuration.ConfigurationManager.
  
  ## Production
  
  30 Aug 2019
