using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting.Server;

namespace WebView.Hosting
{
	public static class WebViewHelper
	{
		/// <summary>
		/// Finds a web browser and makes it available as a <see cref="IWebViewer"/> singleton
		/// </summary>
		/// <param name="services">Dependency Injection</param>
		/// <param name="preferredBrowserTypes">List of <see cref="IWebViewer"/> child types which are preferred, in order of preference</param>
		/// <returns></returns>
		public static IServiceCollection AddWebViewer(this IServiceCollection services, params Type[] preferredBrowserTypes) =>
			services.AddSingleton<IWebViewer>(isp => GetWebViewer(isp, preferredBrowserTypes));
		/// <summary>
		/// Set up browser behavior for local <see cref="IServer"/>
		/// </summary>
		/// <param name="services">Dependency Injection</param>
		/// <param name="preferredBrowserTypes">List of <see cref="IWebViewer"/> child types which are preferred, in order of preference</param>
		/// <returns></returns>
		public static IServiceCollection ViewLocalWebsite(this IServiceCollection services, params Type[] preferredBrowserTypes) =>
			services.AddWebViewer(preferredBrowserTypes)
				.AddHostedService<WebViewerService>();
		/// <summary>
		/// Set up browser behavior for local <see cref="IServer"/>
		/// </summary>
		/// <param name="services">Dependency Injection</param>
		/// <param name="configrequest">Configures <see cref="WebViewRequest"/> before it is sent to <see cref="IWebViewer.ViewSite"/></param>
		/// <param name="preferredBrowserTypes">List of <see cref="IWebViewer"/> child types which are preferred, in order of preference</param>
		/// <returns></returns>
		public static IServiceCollection ViewLocalWebsite(this IServiceCollection services, Action<WebViewRequest> configrequest, params Type[] preferredBrowserTypes) =>
			services.AddWebViewer(preferredBrowserTypes)
				.AddHostedService<WebViewerService>(isp => new WebViewerService(
					isp.GetRequiredService<ILogger<WebViewerService>>(),
					isp.GetRequiredService<IHostApplicationLifetime>(),
					isp.GetRequiredService<IServer>(),
					isp.GetRequiredService<IWebViewer>())
				{ ConfigWebView = configrequest }
				);
		private static Type[] defaultBrowserPreference = new Type[] { typeof(Chrome), typeof(MSEdge), typeof(Firefox) };
		private static IWebViewer GetWebViewer(IServiceProvider isp, Type[] preferredBrowserTypes)
		{
			var output = new List<IWebViewer>();
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				const string apppath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths";
				foreach (var key in Registry.LocalMachine.OpenSubKey(apppath)?.GetSubKeyNames()
					?? throw new Exception($"Registry HKEY_LOCAL_MACHINE\\{apppath} was not found"))
				{
					var path = Registry.LocalMachine.OpenSubKey($"{apppath}\\{key}")?.GetValue("")?.ToString();
					if (path != null)
					{
						if (key.Equals("msedge.exe", StringComparison.OrdinalIgnoreCase))
							output.Add(new MSEdge(path, isp.GetRequiredService<ILogger<MSEdge>>()));
						else if (key.Equals("chrome.exe", StringComparison.OrdinalIgnoreCase))
							output.Add(new Chrome(path, isp.GetRequiredService<ILogger<Chrome>>()));
						else if (key.Equals("firefox.exe", StringComparison.OrdinalIgnoreCase))
							output.Add(new Firefox(path, isp.GetRequiredService<ILogger<Firefox>>()));
					}
				}
			}
			else
			{
				throw new NotImplementedException("Only Windows supported");
			}
			foreach (var preferredType in preferredBrowserTypes)
				foreach (var availableViewer in output)
					if (availableViewer.GetType().IsAssignableTo(preferredType))
						return availableViewer;
			foreach (var preferredType in defaultBrowserPreference)
				foreach (var availableViewer in output)
					if (availableViewer.GetType().IsAssignableTo(preferredType))
						return availableViewer;
			foreach (var availableViewer in output)
				return availableViewer;
			throw new Exception("No WebViewer Found");
		}
	}
}