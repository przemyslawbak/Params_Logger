# Params_Logger
Simple in use library for logging called meber methods, exceptions, properties with parameters and their values, and saving them in clear and transparent format.

## Purpose

Library was created for logging method calls, exceptions, property changes and additional data selected by the user from a solution, and saving the logs in a file with clear, transparent and readable format. Currently logger works only with .Net Framework.

## Features

1. Parsing method and properties parameter values with their names, with using the minimum amount of code.
2. Saving the logs with easy to read format.
3. Setup library configuration in separate log.config file.
4. Synchronous calls of the library interface.
5. Possibility to place log.config anywhere in the project.
6. Saving unhandled exceptions.
7. Logged data types:
  - property name with value,
  - called method name with property names and values,
  - special information strings called by developer,
  - exceptions.
8. Configuration covers:
  - log file path and name,
  - use only in debug or not,
  - deleting logs on application start.
  
# Usage

## Setup

To use the library you need to install it with **NuGet** and inject it into the class constructor. Also run `await _log.Initialization;` at the beginning of your code to make sure that ParamsLogger will be loaded before rest of the code. To do it, you can use exposed `Initialization` property to use [Asynchronous Initialization Pattern](https://blog.stephencleary.com/2013/01/async-oop-2-constructors.html "Asynchronous Initialization Pattern"), as in the example:
```csharp


public class BrowserViewModel : IAsyncInitialization
{
        private readonly IParamsLogger _log;

        public YourViewModel(IParamsLogger log)
        {
            _log = log;
            Initialization = InitializeProgramAsync();
        }
        
        public Task Initialization { get; private set; } //for Asynchronous Initialization Pattern
        
        public async Task InitializeProgramAsync()
        {
            await _log.Initialization; //awaiting for init of logger before continue with your code

            await Task.Delay(100000);
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
  </appSettings>
</configuration>

```
Where:
 - `debugOnly` means that will save logs with debugger attached only, default value: `true`,
 - `logFile` means where will save the logs, default value: is project main folder with file name `log.txt`,
 - `deleteLogs` is for deleting log file on application startup, default value: `true`.
 
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
  - is called synchronously and saving records into collection,
  - library is processing data on Timer short intervals,
  - logs are saved asynchronously,
  - logging unhandled fatal exceptions.
  
2. Library is using:
  - .Net Standard 2.0,
  - System.Reflection,
  - System.Configuration.ConfigurationManager.
  
  ## Production
  
  30 Aug 2019
