# CShargs review

## First impressions

At first glance the API looks very friendly and simple to use. You can get a good idea of how it works by just skimming through the readme file. It uses a declarative approach, meaning you just annotate the variables you want to have populated and all the hard work gets done for you. It's clean and lightweight, I like it.

Provided is an extensive readme file, as well as a make target to build an autogenerated documentation from the source code using [Doxygen](https://www.doxygen.nl/index.html). The readme file contains everything you need to know to get started. The only thing I didn't quite get right away was the custom options, but a quick look into the generated docs cleared things up. In visual studio (and many other editors I'm sure) you can see parts of the documentation when calling the respective functions so you almost never need to look at the documentation directly.

## Functionality

The API supports all required basic functionalities, like the ability to specify a mandatory argument and actually parse the arguments. Notable features are:

- Any number of long or short aliases
- One alias can refer to multiple options
- Custom option types

The authors went above and beyond and added even more extra functionality. More interesting features:

- Explicit way to create option dependencies (`useWith`)
-  The ability to specify your own symbols for the long and short option prefix, the plain argument delimiter, and even the equals symbol
  - `--param=value` => `--param*value`
- Specify number required of plain arguments 

## Implementation

I don't have much to criticize here. Everything that you shouldn't touch is either private or internal and the classes that are not meant to be inherited are correctly marked as sealed. The code is readable and function names self explanatory. I admit that I never worked with custom attributes in c#, so I can't tell if they are used correctly, but I couldn't find anything weird or cumbersome in their implementation so I think they're fine. Parts of the API are in my opinion reasonably abstracted and the class hierarchy makes sense. Functions are most often reasonably short, with some exceptions, where a part of the function could have been abstracted into its own function. These would probably only be used once, but It would improve readability. I'm talking for example about the `GenerateHelp` method, where the author's comments suggest this as well. Still, this can never be perfect and its just a minor thing anyway. Variable naming is consistent, with the only change from the standard being private members named `var_` instead of `_var`.

## Test coverage

The authors ended up with around 170 unit tests. Given the test groups and test names, it looks like they cover all of the important parts and test then extensively. The current build passes all the tests.

## Extension

Implementing the date-time parser extension was actually incredibly easy. This is the entire configuration:

```csharp
class DateTimeParser : Parser
{
    [ValueOption("date", required: true, shortName: 'd', help: "A date and time in any reasonable format")]
    public DateTime Date { get; set; }

    [ValueOption("format", required: true, shortName: 'f', help: "A format string to use when printing the date")]
    public string Format { get; set; }  
}
```

The c# built-in `DateTime` object already has a method that parses a string called exactly like what the API expects, so no custom parsing is needed. To get the date in a given format simply call

```csharp
arguments.Date.ToString(arguments.Format);
```

where `arguments` is an instance of `DateTimeParser`. 

## Final remarks

The library is designed nicely, a declarative approach was definitely a good call for this project. It does what it promises and it does it in an elegant way. The code doesn't have any major design flaws or spaghetti areas. It is clear the authors spent a lot of time on it. Good job :)
