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

		// Load data from url in chunks and saves it in a file
		public static bool LoadAndSaveChunkedDataFromUrl(string url, string directory, string fileName)
		{
			int byteBuffer = 1024;
			byte[] data;
			int dataStream;

			try
			{
				WebRequest request = WebRequest.Create (new Uri (url));
				var filePath = FilePathString(directory, fileName);

				using(Stream streamResponse = request.GetResponse ().GetResponseStream ())
				{
					using (FileStream streamTarget = new FileStream(filePath, FileMode.Append))
					{
						data = new byte[byteBuffer];
						while ((dataStream = streamResponse.Read (data, 0, byteBuffer)) > 0) 
						{
							streamTarget.Write (data, dataStream, byteBuffer);
						}
					}
				}
			}
			catch(Exception e)
			{
				return false;
			}
			return true;
		}

		// Save Bytes to file
		public static void SaveStreamToDisk(byte[] downloadedStream, string directory, string fileName)
		{
			if (downloadedStream.Length == null) return;

			try
			{
				var filePath = FilePathString(directory, fileName);
				File.WriteAllBytes (filePath, downloadedStream);
			}
			catch(Exception e) {
				Console.WriteLine (e.ToString());
			}
		}

		// Create filepath string
		private static string FilePathString(string directory, string fileName)
		{
			return (directory [directory.Length - 1] != '/') ? directory + '/' + fileName : directory + fileName;
		}
	}
}

