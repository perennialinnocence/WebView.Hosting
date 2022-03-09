namespace WebView.Hosting
{
	public class WebViewRequest
	{
		/// <summary>
		/// string URL to instruct the Web Viewer to visit after starting
		/// </summary>
		public string Url { get; set; }
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="url">Url is required on instantiation</param>
		public WebViewRequest(string url)
		{
			Url = url;
		}
		/// <summary>
		/// ViewMode defaults to Viewmode.App
		/// </summary>
		public ViewMode ViewMode { get; set; } = ViewMode.App;
		/// <summary>
		/// Private defaults to tru
		/// </summary>
		public bool Private { get; set; } = true;
		/// <summary>
		/// ProfileDirectory defaults to null, which means no value will be provided to the browser and it will use the user's profile
		/// <br/>
		/// Otherwise this defines a directory where the browser will be instructed to store user data
		/// </summary>
		public string? ProfileDirectory { get; set; } = null;
		/// <summary>
		/// DeleteProfileDirectory defaults to true.
		/// <br/>
		/// When this is true and ProfileDirectory exists, the ProfileDirectory will be deleted afterwards
		/// </summary>
		public bool DeleteProfileDirectory { get; set; } = true;
	}
}