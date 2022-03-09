using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using System.Linq;
using System;
namespace WebView.Hosting
{
	/// <summary>
	/// Interfaces between <see cref="IServer"/> and <see cref="IWebViewer"/> to perform browsing on local website
	/// </summary>
	public class WebViewerService : IHostedService
	{
		private readonly ILogger<WebViewerService> logger;
		private readonly IHostApplicationLifetime lifetime;
		private readonly IServer server;
		private readonly IWebViewer webViewer;
		public Action<WebViewRequest> ConfigWebView { get; set; }

		public WebViewerService(ILogger<WebViewerService> logger, IHostApplicationLifetime lifetime, IServer server, IWebViewer webViewer)
		{
			this.logger = logger;
			this.lifetime = lifetime;
			this.server = server;
			this.webViewer = webViewer;
			ConfigWebView = _ => { };
		}

		Task IHostedService.StartAsync(CancellationToken cancellationToken)
		{
			lifetime.ApplicationStarted.Register(async () =>
			{
				try
				{
					var url = server.Features.Get<IServerAddressesFeature>()?.Addresses?.FirstOrDefault() ?? throw new System.Exception("No Url Found");
					var request = new WebViewRequest(url);
					ConfigWebView(request);
					await webViewer.ViewSite(request, lifetime.ApplicationStopping);
				}
				catch (Exception e)
				{
					logger.LogError(e, "Exception while running WebViewerService");
				}
				finally
				{
					lifetime.StopApplication();//program is stopped when this task completes
				}
			});
			return Task.CompletedTask;
		}

		Task IHostedService.StopAsync(CancellationToken cancellationToken)
		{
			return Task.CompletedTask;
		}
	}
}