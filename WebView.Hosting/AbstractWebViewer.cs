using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace WebView.Hosting
{
	/// <summary>
	/// Abstraction of Web Viewer derived from locally installed web browsers
	/// </summary>
	public abstract class AbstractWebViewer : IWebViewer
	{
		protected readonly ILogger logger;
		/// <summary>
		/// Path to executable web browser represented by this Web Viewer
		/// </summary>
		public string ExecutablePath { get; protected set; }
		/// <summary>
		/// All children should call this using the base constructor
		/// </summary>
		/// <param name="executablePath">String path to web browser</param>
		/// <param name="logger">Relevant ILogger&lt;T&gt; should be dependency injected</param>
		protected AbstractWebViewer(string executablePath, ILogger logger)
		{
			ExecutablePath = executablePath;
			this.logger = logger;
		}
		/// <summary>
		/// Utility function to be called by implementing children
		/// <list type="bullet">
		/// <item>Runs the browser executable using <see cref="Process"/></item>
		/// <item>Kills process if <see cref="CancellationToken">cancellationToken</see> is triggered</item>
		/// <item>Cleans up <see cref="WebViewRequest.ProfileDirectory">options.ProfileDirectory</see> if <see cref="WebViewRequest.DeleteProfileDirectory">options.DeleteProfileDirectory</see> is <c>true</c></item>
		/// </list>
		/// </summary>
		/// <param name="options">Describes the behavior needed from the Web Viewer</param>
		/// <param name="configProc">Performs custom configurations to the <see cref="ProcessStartInfo"/> before running</param>
		/// <param name="cancellationToken">Cancels the process and kills it</param>
		/// <returns>async Task</returns>
		protected async Task RunProcAsync(WebViewRequest options, Action<ProcessStartInfo> configProc, CancellationToken cancellationToken)
		{
			if (options.ProfileDirectory != null && !Directory.Exists(options.ProfileDirectory))
				Directory.CreateDirectory(options.ProfileDirectory);
			using (var proc = new Process())
			{
				proc.StartInfo.FileName = this.ExecutablePath;
				configProc(proc.StartInfo);
				proc.Start();
				await proc.WaitForExitAsync(cancellationToken);
				if (cancellationToken.IsCancellationRequested)
				{
					proc.Kill();
				}
			}
			if (options.ProfileDirectory != null && options.DeleteProfileDirectory)
			{
				for (int i = 0; i < 10; i++)
				{
					try
					{
						Directory.Delete(options.ProfileDirectory, true);
						break;
					}
					catch (IOException e)
					{
						logger.LogError(e, "Exception while deleting {ProfileDirectory}", options.ProfileDirectory);
						await Task.Delay(100, cancellationToken);
					}
				}
			}
		}
		/// <summary>
		/// Override this to implement behavior
		/// </summary>
		/// <param name="options">Describes the behavior needed from the Web Viewer</param>
		/// <param name="cancellationToken">Triggers to interrupt process</param>
		/// <returns>async Task</returns>
		public abstract Task ViewSite(WebViewRequest options, CancellationToken cancellationToken);
	}
}
