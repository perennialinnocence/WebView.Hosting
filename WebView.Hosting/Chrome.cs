using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace WebView.Hosting
{
	public class Chrome : AbstractWebViewer
	{
		public Chrome(string executablePath, ILogger<Chrome> logger) : base(executablePath, logger)
		{
		}

		public override async Task ViewSite(WebViewRequest options, CancellationToken cancellationToken)
		{
			await RunProcAsync(options, psi =>
			 {
				 switch (options.ViewMode)
				 {
					 case ViewMode.App:
						 psi.ArgumentList.Add($"--app={options.Url}");
						 break;
					 case ViewMode.Browser:
						 psi.ArgumentList.Add(options.Url);
						 break;
					 case ViewMode.Kiosk:
						 psi.ArgumentList.Add(options.Url);
						 psi.ArgumentList.Add("--kiosk");
						 break;
				 }
				 if (options.Private) psi.ArgumentList.Add("--incognito");
				 psi.ArgumentList.Add("--no-first-run");
				 psi.ArgumentList.Add("--bwsi");
				 if (options.ProfileDirectory != null)
				 {
					 psi.ArgumentList.Add($"--user-data-dir={options.ProfileDirectory}");
				 }
			 }, cancellationToken);
		}
	}
}
