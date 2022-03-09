namespace WebView.Hosting
{
	public enum ViewMode
	{
		/// <summary>
		/// View by opening in the browser
		/// </summary>
		Browser,
		/// <summary>
		/// Views with a popup window with no browser chrome
		/// </summary>
		App,
		/// <summary>
		/// Views in a locked fullscreen mode
		/// </summary>
		Kiosk,
	}
}