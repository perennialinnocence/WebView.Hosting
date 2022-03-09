# WebView.Hosting
Introduces your locally installed web browsers to `Microsoft.Extensions.Hosting.Abstractions` and `Microsoft.AspNetCore.Hosting.Server.Abstractions`

Instead of relying on an IDE to find a web browser to run on, find and inject an available installed browser into your `IHost` and run it.

To an existing `WebApplication` or `IHost`-based website call `.ViewLocalWebsite()` to start viewing the site as part of the executable itself.  
Then you don't need `Properties/launchSettings.json` anymore to test your website in Visual Studio, or `configurations[].serverReadyAction.action = "openExternally"` in `launch.json` in VSCode

Or on a generic `IHost` call `.AddWebViewer()` and inject it into anything.
