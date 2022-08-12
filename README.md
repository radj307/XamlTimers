# XamlTimers

WPF behaviors that update bindings or execute a callback method on a configurable interval.  
You can toggle the timers at any time; or even bind the `EnableTimer` property to another control, such as a `CheckBox`.  

# Usage

Requires Microsoft's WPF Behaviors NuGet package:  

 - [`Microsoft.Xaml.Behaviors.Wpf`](https://www.nuget.org/packages/Microsoft.Xaml.Behaviors.Wpf) &nbsp;&nbsp;&nbsp; *(XAML Namespace: `http://schemas.microsoft.com/xaml/behaviors`)*


## Namespace

### C#
```csharp
using XamlTimers;
```

### XAML
```xaml
xmlns:behavior="clr-namespace:XamlTimers;assembly=XamlTimers"
```

## Behaviors

*Properties that don't have default values are **required**!*

### `IntervalUpdateBinding`

#### Properties

| Property        | Default | Description |
|-----------------|---------|-------------|
| `EnableTimer`   | `true`  | When `true`, the timer is enabled & the specified binding is updated every time the `Interval` has elapsed;<br/>When `false`, the timer is disabled & no updates occur. |
| `Interval`      |         | The timer interval, in milliseconds. *(type `double`)* |
| `Property`      |         | The `DependencyProperty` on the attached object to update the databindings of. If the specified property doesn't exist on the attached object, nothing happens.<br/>This should be specified in the form `"{x:Static <CONTROL>.<PROPERTYNAME>Property}"`. |
| `ThrowWhenPropertyIsNull` | `true` | When `true`, an `ArgumentNullException` is thrown by the update method when `Property` is `null`; when `false`, no exception is thrown and the binding update silently fails. |
| `ThrowWhenPropertyIsMissing` | `true` | When `true`, an `ArgumentNullException` is thrown by the update method when `Property` doesn't exist on the attached object; when `false`, no exception is thrown and the binding update silently fails. |

### `IntervalCallback`

#### Properties

| Property        | Default | Description |
|-----------------|---------|-------------|
| `EnableTimer`   | `true`  | When `true`, the timer is enabled & `TimerCallback` is called every time the `Interval` has elapsed;<br/>When `false`, the timer is disabled & `TimerCallback` is not called. |
| `Interval`      |         | The timer interval, in milliseconds. *(type `double`)* |
| `TimerCallback` |         | A callback delegate of type `System.Timers.ElapsedEventHandler` that will be invoked every time the timer is triggered. |

### `BaseIntervalBehavior`

If you want to create your own timer class, you can inherit from the abstract base class with your own implementation.  
See the [source code](BaseIntervalBehavior.cs) for implementation details & examples.  
