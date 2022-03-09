using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace WebView.Hosting
{
	public class MSEdge : AbstractWebViewer
	{
		public MSEdge(string executablePath, ILogger<MSEdge> logger) : base(executablePath, logger)
		{
		}

		public async override Task ViewSite(WebViewRequest options, CancellationToken cancellationToken)
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
				if (options.Private) psi.ArgumentList.Add("--inprivate");
				psi.ArgumentList.Add("--no-first-run");
				psi.ArgumentList.Add("--bwsi");
				if (options.ProfileDirectory != null)
				{
					var firstTime = Path.Join(options.ProfileDirectory, "FirstLaunchAfterInstallation");
					if (!File.Exists(firstTime))
						File.Create(firstTime).Dispose();
					psi.ArgumentList.Add($"--user-data-dir={options.ProfileDirectory}");
				}
			}, cancellationToken);
		}
	}
}
