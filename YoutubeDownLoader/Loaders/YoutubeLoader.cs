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
			var loadedRawHtmlData = YoutubeStreamDownloader.LoadDataFromUrl (url);
			if (loadedRawHtmlData == null)
				return;
			var loadedRawHtmlStringData = System.Text.Encoding.UTF8.GetString (loadedRawHtmlData);

			// Extract player source URL from "jsUrl" (e.g. https://www.youtube.com/yts/jsbin/player-vflYXLM5n/en_US/base.js).
			var extractedPlayerSource = this.ExtractedPlayerSource (loadedRawHtmlStringData); // >> OldStuff >> VideoInfoCONST.YoutubeEmbedPlayerJsUrl; // >> OLD STUFF >> ExtractedPlayerSource(loadedRawEmbedStringData);

			// Download and parse player source code.
			var loadedJsData = System.Text.Encoding.UTF8.GetString (YoutubeStreamDownloader.LoadDataFromUrl (extractedPlayerSource));

			// Request video metadata (e.g. https://www.youtube.com/get_video_info?video_id=e_S9VvJM1PI&sts=17488&hl=en). Try with el=detailpage if it fails.
			var loadedVideoInfoData = YoutubeStreamDownloader.LoadDataFromUrl (this.FetchFileFromUrl (url, VideoInfoCONST.YoutubeVideoInfoUrl));

			if (loadedVideoInfoData == null)
				return;
			var loadedVideoInfoStringData = System.Text.Encoding.UTF8.GetString (loadedVideoInfoData);
			this.Utf8CharEncoding (ref loadedVideoInfoStringData);

			// Create VideoInfo
			var videoInfos = this.CreateVideoInfos (loadedVideoInfoStringData);

			// Get deciphered signature(s)
			var decipheredSignatures = Decipherer.DecipherWithJsPlayer (loadedJsData, videoInfos [0].Signatures);

			// First update the deciphered signatures in the videoInfo objects
			this.UpdateVideoInfosSignatures(videoInfos, decipheredSignatures);

			// Choose a stream from an itag (eg. 22), download it and save it's data to disk.
			this.DownloadYoutubeStreamToDisk(videoInfos[1], @"/Users/guidoleen/Desktop/WegNaGebruik/Downloads");

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
			var videoInfoId = new HttpHelpers ().ParseJsonProperties (new HttpHelpers ().RemoveCharacter (extractedGeneralVideoInfo, '\\'), "\"videoId\"");
			var videoInfoSignatures = this.ExtractedSignatures (extractedGeneralVideoInfo);

			var videoInfos = new VideoInfo[videoInfoItag.Length];
			for (int i = 0; i < videoInfoItag.Length; i++) {
				videoInfos[i] = new VideoInfo (
					itag: (!String.IsNullOrEmpty(videoInfoItag[i])) ? Int32.Parse(videoInfoItag[i]) : 0,
					mimeType: videoInfoMimeType[i],
					urls: new string[]{videoInfoUrl[i]},
					videoId: videoInfoId[0], // Can have more id's. Get the first id which is the id from the desired movie. 
					signatures: videoInfoSignatures
				);
			}

			return videoInfos;
		}

		// Update videoInfo (deciphered signature)
		private void UpdateVideoInfosSignatures(VideoInfo[] videoInfos, string[] decipheredSignatures)
		{
			foreach (var videoInfo in videoInfos) {
				videoInfo.Signatures = decipheredSignatures;
			}
		}

		// Fetch movie data
		private byte[] FetchRawMovieDataFromVideoInfo(VideoInfo videoInfo)
		{
			// Get Encoded Url from videoInfo
			var urlVideoStreamUrl = videoInfo.StreamUrl;
			this.Utf8CharEncoding (ref urlVideoStreamUrl);

			urlVideoStreamUrl = new HttpHelpers().RemoveCharacter(urlVideoStreamUrl, '"') + "&sig="; 

			foreach (var videoInfoSignature in videoInfo.Signatures) {
				var loadedDataFromUrl = YoutubeStreamDownloader.LoadDataFromUrl (urlVideoStreamUrl + videoInfoSignature);
				if (loadedDataFromUrl != null)
					return loadedDataFromUrl;
			}

			return null;
		}

		// Write movie data to disk
		private void WriteYoutubeDataToDisk(byte[] downloadedStream, string directory, string fileName)
		{
			YoutubeStreamDownloader.SaveStreamToDisk (downloadedStream, directory, fileName);
		}

		// Download Movie to Disk
		private void DownloadYoutubeStreamToDisk(VideoInfo videoInfo, string directory)
		{
			var data = this.FetchRawMovieDataFromVideoInfo(videoInfo);

			if(data == null) 
				throw new Exception ("Could not download this Youtube movie.");
			
			this.WriteYoutubeDataToDisk (data, directory, videoInfo.VideoNameFromId);
		}
	}
}

//////// OLD STUFF /////////
// Download video’s embed page (e.g. https://www.youtube.com/embed/e_S9VvJM1PI).
//var loadedRawEmbedHtmlData = this.LoadDataFromUrl (this.FetchFileFromUrl(url, VideoInfoCONST.YoutubeEmbedUrl));
//var loadedRawEmbedStringData = System.Text.Encoding.UTF8.GetString(loadedRawEmbedHtmlData);
//Console.WriteLine (loadedRawEmbedStringData);

// >> If there’s a reference to DASH manifest, extract the URL and decipher it if necessary as well.
// >> Download the DASH manifest and extract additional streams.

// Use itag to classify streams by their properties.