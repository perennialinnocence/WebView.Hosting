using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace WebView.Hosting
{
	public class Firefox : AbstractWebViewer
	{
		public Firefox(string executablePath, ILogger<Firefox> logger) : base(executablePath, logger)
		{
		}

		public async override Task ViewSite(WebViewRequest options, CancellationToken cancellationToken)
		{
			await RunProcAsync(options, psi =>
			{
				switch (options.ViewMode)
				{
					case ViewMode.App:
						throw new System.Exception("No App mode available for Firefox");
					case ViewMode.Browser:
						psi.ArgumentList.Add(options.Url);
						break;
					case ViewMode.Kiosk:
						psi.ArgumentList.Add(options.Url);
						psi.ArgumentList.Add("-kiosk");
						break;
				}
				if (options.Private) psi.ArgumentList.Add("--incognito");
				if (options.ProfileDirectory != null)
				{
					psi.ArgumentList.Add($"-profile");
					psi.ArgumentList.Add(options.ProfileDirectory);
				}
			}, cancellationToken);
		}
	}
}
