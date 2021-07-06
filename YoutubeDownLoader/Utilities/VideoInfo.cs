using System;
using System.Collections.Generic;
using System.Linq;

namespace YoutubeDownLoader
{
	public class VideoInfo
	{
		private string _videoId;
		public string VideoNameFromId {
			get
			{
				if (_videoId == null)
					_videoId = "ForgotYourFileName";
				return _videoId + VideoInfoCONST.VideoInfoNameSeperator + this.Itag.ToString () + FileTypeExtenstion;
			}
		}
		public string HtmlPlayerVersion { get; set; }
		public bool RequiresDecryption { get; set; }
		//itag — integer code that identifies the type of stream.
		public int Itag { get; private set; }
		//type — MIME type and codecs.
		public string MimeType { get; private set; }
		//url — URL that serves the stream.
		public IEnumerable<string> StreamUrls { get; private set; }
		public string StreamUrl { 
			get 
			{
				return StreamUrls.FirstOrDefault ();
			}
		}

		//s — cipher signature used to protect the stream (if present).
		public string[] Signatures { get; set; }
		public int FallBackHost { get; private set; }

		public string FileTypeExtenstion { 
			get 
			{ 
				switch (this.MimeType) {
					case "video/webm": return YoutubeDownLoader.VideoInfoCONST.Mov;
					case "mp3": return YoutubeDownLoader.VideoInfoCONST.Mp3;
					case "mp4": return YoutubeDownLoader.VideoInfoCONST.Mp4;
					case "vorbis": return YoutubeDownLoader.VideoInfoCONST.Ogg;
				}
				return "";
			} 
		}

		public VideoInfo (int itag, string mimeType, IEnumerable<string> urls, string videoId, string[] signatures)
		{
			this.Itag = itag;
			this.MimeType = mimeType;
			this.StreamUrls = urls;
			this._videoId = videoId;
			this.Signatures = signatures;
		}
	}
}


