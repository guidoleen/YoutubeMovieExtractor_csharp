using System;
using System.Text.RegularExpressions;
using System.Net;
using System.Threading.Tasks;

namespace YoutubeDownLoader
{
	public class YoutubeLoader
	{
		public void WriteYoutubeMovieToDisk(string url)
		{
			// Download video’s default page (e.g. https://www.youtube.com/watch?v=NWY7ZRACbUU).
			var loadedRawHtmlData = this.LoadDataFromUrl (url);
			if (loadedRawHtmlData == null) return;
			var loadedRawStringData = System.Text.Encoding.UTF8.GetString(loadedRawHtmlData);
			Console.WriteLine (loadedRawStringData);

			// Extract player source URL from "jsUrl" (e.g. https://www.youtube.com/yts/jsbin/player-vflYXLM5n/en_US/base.js).
			var extractedPlayerSource = this.ExtractedPlayerSource(loadedRawStringData); // >> OldStuff >> VideoInfoCONST.YoutubeEmbedPlayerJsUrl; // >> OLD STUFF >> ExtractedPlayerSource(loadedRawEmbedStringData);

			// Download and parse player source code.
			var loadedJsData = System.Text.Encoding.UTF8.GetString (this.LoadDataFromUrl(extractedPlayerSource));

			// Request video metadata (e.g. https://www.youtube.com/get_video_info?video_id=e_S9VvJM1PI&sts=17488&hl=en). Try with el=detailpage if it fails.
			var loadedVideoInfoData = this.LoadDataFromUrl (this.FetchFileFromUrl(url, VideoInfoCONST.YoutubeVideoInfoUrl));
			if (loadedVideoInfoData == null) return;
			var loadedVideoInfoStringData = System.Text.Encoding.UTF8.GetString (loadedVideoInfoData);
			this.Utf8CharEncoding (ref loadedVideoInfoStringData);

			// Create VideoInfo
			var videoInfos = this.CreateVideoInfos(loadedVideoInfoStringData);

			var deciphered = Decipherer.DecipherWithJsPlayer (loadedJsData, videoInfos [0].Signatures [0]);

			// Get the value of sts (e.g. 17488).



Console.WriteLine ("NEWLINE - \r\n");

// Console.WriteLine (loadedVideoInfoStringData);

			// Parse the URL-encoded metadata and extract information about streams.

			// If they have signatures, use the player source to decipher them and update the URLs.


			// >> If there’s a reference to DASH manifest, extract the URL and decipher it if necessary as well.
			// >> Download the DASH manifest and extract additional streams.

			// Use itag to classify streams by their properties.

			// Choose a stream and download it in segments.
		}

		// Load from url
		private byte[] LoadDataFromUrl(string url)
		{
			byte[] html = null;

			try
			{
				using(WebClient client = new WebClient ())
				{
					string htmlValue = client.DownloadString(new Uri(url));
					html = System.Text.Encoding.UTF8.GetBytes(htmlValue);
				}
			}
			catch(Exception e) {
				return null;
			}
			return html;
		}

		// Get fileinfo from eg. VideoInfo
		private string FetchFileFromUrl(string inputUrl, string outputUrl)
		{
			try
			{
				// ?v=8szprzt2LsI
				var videoParams = inputUrl.Split('?')[1].Split('&');
				string videoId = "";
				foreach (var videoIdFromParam in videoParams) {
					if(videoIdFromParam.Contains("v="))
						videoId = videoIdFromParam.Substring(2);
				}
				return outputUrl + videoId;

			}
			catch(Exception e) {
				return e.ToString ();
			}
		}

		// Encode Data with Utf8 encoding
		private void Utf8CharEncoding(ref string value)
		{
			foreach (var Utf8EncodingValue in VideoInfoCONST.Utf8CharEncodingValues) {
				value = value.Replace (Utf8EncodingValue.Key, Utf8EncodingValue.Value);
			}
		}

		// Extract player source (js)
		private string ExtractedPlayerSource(string htmlSource)
		{
			// Extract the first occurence of the js playersrc.
			var extractedPlayerSource = new Regex (VideoInfoCONST.RegexExtractPlayerJsSrc).Matches(htmlSource)[0].Groups[2].ToString();
			return VideoInfoCONST.YoutubeUrl + ((extractedPlayerSource[0] == '/') ? extractedPlayerSource.Substring(1) : extractedPlayerSource);
		}

		// Extract urlRaw video Url - for Property VideoInfo.DownloadUrl
		private string ExtractedRawVideoUrl(string videoInfoSource)
		{
			return new Regex (VideoInfoCONST.RegexExtractRawUrl).Match(videoInfoSource).Groups[2].ToString();
		}

		// Extract Signature(s) from VideoInfo
		private string[] ExtractedSignatures(string videoInfoSource)
		{
			var signaturesFromRegex = new Regex (VideoInfoCONST.RegexExtractSignature).Matches(videoInfoSource);
			var extractedSignatures = new string[signaturesFromRegex.Count];
			for (int i = 0; i < signaturesFromRegex.Count; i++) {
				extractedSignatures[i] = signaturesFromRegex [i].Groups [2].ToString ();
			}
			return extractedSignatures;
		}

		// Extract Raw VideoInfo
		private string ExtractedRawVideoInfo(string videoInfoSource)
		{
			return new Regex (VideoInfoCONST.RegexExtractRawVideoInfo).Match(videoInfoSource).Groups[2].ToString();
		}

		// Create VideoInfo
		private VideoInfo[] CreateVideoInfos(string loadedVideoInfoStringData)
		{
			var extractedRawVideoUrl = this.ExtractedRawVideoUrl(loadedVideoInfoStringData);
			var extractedSignatures = this.ExtractedSignatures(loadedVideoInfoStringData);
			var extractedGeneralVideoInfo = this.ExtractedRawVideoInfo(loadedVideoInfoStringData);

			var videoInfoItag = new HttpHelpers ().ParseJsonProperties (new HttpHelpers ().RemoveCharacter (extractedGeneralVideoInfo, '\\'), "\"itag\"");
			var videoInfoMimeType = new HttpHelpers ().ParseJsonProperties (new HttpHelpers ().RemoveCharacter (extractedGeneralVideoInfo, '\\'), "\"mimeType\"");
			var videoInfoUrl = new HttpHelpers ().ParseJsonProperties (new HttpHelpers ().RemoveCharacter (extractedGeneralVideoInfo, '\\'), "\"url\"");
			var videoInfoSignatures = this.ExtractedSignatures (extractedGeneralVideoInfo);

			var videoInfos = new VideoInfo[videoInfoItag.Length];
			for (int i = 0; i < videoInfoItag.Length; i++) {
				videoInfos[i] = new VideoInfo (
					itag: videoInfoItag[i] != "" ? Int32.Parse(videoInfoItag[i]) : 0,
					mimeType: videoInfoMimeType[i],
					urls: new string[]{videoInfoUrl[i]},
					signatures: videoInfoSignatures
				);
			}
			return videoInfos;
		}

		// Fetch movie data

		// Write movie data to disk
	}
}

//////// OLD STUFF /////////
// Download video’s embed page (e.g. https://www.youtube.com/embed/e_S9VvJM1PI).
//var loadedRawEmbedHtmlData = this.LoadDataFromUrl (this.FetchFileFromUrl(url, VideoInfoCONST.YoutubeEmbedUrl));
//var loadedRawEmbedStringData = System.Text.Encoding.UTF8.GetString(loadedRawEmbedHtmlData);
//Console.WriteLine (loadedRawEmbedStringData);