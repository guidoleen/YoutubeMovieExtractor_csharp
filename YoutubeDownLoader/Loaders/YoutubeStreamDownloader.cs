using System;
using System.Net;
using System.IO;

namespace YoutubeDownLoader
{
	public static class YoutubeStreamDownloader
	{
		// Load data from url
		public static byte[] LoadDataFromUrl(string url)
		{
			byte[] data = null;

			try
			{
				using(WebClient client = new WebClient ())
				{
					string dataValue = client.DownloadString(new Uri(url));
					data = System.Text.Encoding.UTF8.GetBytes(dataValue);
				}
			}
			catch(Exception e) {
				return null;
			}
			return data;
		}

		// Save Bytes to file
		public static void SaveStreamToDisk(byte[] downloadedStream, string directory, string fileName)
		{
			if (downloadedStream.Length == null) return;

			try
			{
				var filePath = (directory [directory.Length - 1] != '/') ? directory + '/' + fileName : directory + fileName;
				File.WriteAllBytes (filePath, downloadedStream);
			}
			catch(Exception e) {
				Console.WriteLine (e.ToString());
			}
		}
	}
}

