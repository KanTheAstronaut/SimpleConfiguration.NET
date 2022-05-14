
# SimpleConfiguration.NET
[![name](https://shields.io/nuget/v/SimpleConfiguration.NET)](https://www.nuget.org/packages/SimpleConfiguration.NET) [![name](https://shields.io/nuget/v/SimpleConfigurationLegacy.NET)](https://www.nuget.org/packages/SimpleConfigurationLegacy.NET)

A small library that adds simple configuration options with high customizability.

- [Features](#features)
- [Installation](#installation)
- [Usage](#usage)
- [FAQ](#faq)
- [Quirks](#quirks)
- [License](#license)
## Features

- Supports any serializable type
- Supports synchronous and asynchronous functions
- Highly customizable
- Supports change notifications
- Supports method chaining


## Installation

Install the standard version with nuget via the Package Manager Console

```
Install-Package SimpleConfiguration.NET
```
or the legacy version
```
Install-Package SimpleConfigurationLegacy.NET
```
## Usage

First define the type that you want to use as your configuration object (skip this if it's already defined)

Throughout this example the `Demo` class will be used
```c#
public class Demo
{
    public string String { get; set; }
    public bool Bool { get; set; }
    public int Int { get; set; }
}
```

Create a new `ConfigurationOptions` object

> If you don't define `SettingsName` the library will use reflection to get the name of the class which may cause issues if it's subject to change such as due to obfuscation.
```c#
ConfigurationOptions options = new ConfigurationOptions()
{
    ProgramName = "demoprogram", // This controls the folder name
    SettingsExtension = "test", // This controls the file extension
    SettingsName = "demo", // This controls the file name
};
```

Create a `Configuration<T>` object where `T` is the type that you want to use with the `options` field that we've created earlier

> The type that you choose to use must have a public parameterless constructor
```c#
Configuration<Demo> configuration = new Configuration<Demo>(options);
```

You can then load an existing setting file that matches the options
```c#
configuration.Load();
```
or you can set the `Data` property yourself
> Any changes that are done to the properties or fields of the `Data` field do not invoke `OnDataChanged`, however, setting the property itself will invoke the event. For more information check [this](#quirks)
```c#
configuration.Data = new Demo() {
    String = "demo",
    Bool = true,
    Int = 69
};
```
You can listen to any changes done to the `Data` property by adding an event handler to the `OnDataChanged` event
```c#
configuration.OnDataChanged += Configuration_OnDataChanged;

private void Configuration_OnDataChanged(Demo dataBefore, Demo dataAfter)
{
    Console.WriteLine($"BEFORE: {dataBefore.String} | AFTER: {dataAfter.String}");
}
```
You can also set properties or fields like this
```c#
configuration.Set(a => a.Int = 69);
```
and then save everything to disk
```c#
configuration.Save();
```
and delete the saved file at any time
```c#
configuration.Delete();
```
## Quirks
When setting any property or field of the configuration type through the `Data` property like so
```c#
configuration.Data.Bool = true;
```
the `OnDataChanged` event will not be invoked since the property's setter, that invokes the event, is not called so if the event is important to you you should use the `Set` method like so
```c#
configuration.Set(a => a.Bool = true);
```
## FAQ

#### What is the difference between SimpleConfiguration.NET and SimpleConfigurationLegacy.NET

SimpleConfiguration.NET supports both asynchronous and synchronous functions and uses .netstandard 2.1 which doesn't support that many frameworks whereas the legacy version uses .netstandard 2.0 which supports most frameworks but does not support asynchronous I/O operations so all asynchronous functions were removed from it.

For more detailed compatibility differences please check [this](https://dotnet.microsoft.com/en-us/platform/dotnet-standard#versions)
#### Will this library ever be expanded into a larger configuration library?

No, this library is meant to be simple, hence the name, and will always remain simple, however, I may create a different library with more features later on.


## License

MIT License

Copyright (c) 2022 Kan

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
