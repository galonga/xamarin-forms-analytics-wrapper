# xamarin-forms-analytics-wrapper

## What is this?
xamarin-forms-analytics-wrapper is a library which warps Google Analytics tracking to Xamarin.Forms.

I'm very eager about your feedback, so do not hesitate to create an issue or feel free to improve my code via a contribution.

### Setup and Usage
1. Install the [package via nuget](https://www.nuget.org/packages/xamarin-forms-analytics-wrapper/) into your PCL and platform specific projects.
2. Get your Analytics-Id (iOS AppDelegate.cs, Android: MainActivity.cs)
2. Add the registration call to your platform specific main class (see sample).

Example Android implementation of the Init call:
```cs
protected override void OnCreate(Bundle bundle)
{
	base.OnCreate(bundle);
	Forms.Init(this, bundle);
	var gaService = AnalyticsService.GetGASInstance();
	gaService.Init("UA-12345675-1", this, 3);
	gaService.OptOut = false;
}
```

Example iOS implementation of the Init call:
```cs
public override bool FinishedLaunching(UIApplication uiApplication, NSDictionary launchOptions)
{
    Forms.Init();
    var gaService = AnalyticsService.GetGASInstance();
    gaService.Init("UA-12345675-2", 3);
    gaService.OptOut = false;
}
```
More examples on my [blog](https://galonga.de/xamarin-forms-analytics-wrapper/).

#### FAKE options / Tasks

Execute `bin/fake <taskname>` to run a task or `bin/fake --<optionname>` for fake cli options. First run `bin/fake install`.

Available tasks:

```
* Restore
  Clean solution and afterwards restore all packages

* Build
  Build all projects of solution

```

### Create Nuget Package

Add changes to ```xamarin-forms-analytics-wrapper.nuspec``` file and create package with following command:

```nuget pack -Verbosity detailed -Prop Configuration=Release -Version 0.x.x```
