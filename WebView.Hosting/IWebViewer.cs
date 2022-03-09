using System.Threading;
using System.Threading.Tasks;
namespace WebView.Hosting
{
	public interface IWebViewer
	{
		/// <summary>
		/// Provides capability for viewing a website
		/// </summary>
		/// <param name="options">Describes the behavior needed from the Web Viewer</param>
		/// <param name="cancellation">Triggers to interrupt process</param>
		/// <returns>async Task</returns>
		Task ViewSite(WebViewRequest options, CancellationToken cancellation);
	}
}
