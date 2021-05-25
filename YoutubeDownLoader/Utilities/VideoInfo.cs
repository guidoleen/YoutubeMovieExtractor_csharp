using System;
using System.Collections.Generic;
using System.Linq;

namespace YoutubeDownLoader
{
	public class VideoInfo
	{
		public string DownloadUrl { get; set; }
		public string HtmlPlayerVersion { get; set; }
		public bool RequiresDecryption { get; set; }
		//itag — integer code that identifies the type of stream.
		public int Itag { get; private set; }
		//type — MIME type and codecs.
		public string MimeType { get; private set; }
		//url — URL that serves the stream.
		public IEnumerable<string> Urls { get; private set; }
		public string Url { 
			get 
			{
				return Urls.FirstOrDefault ();
			}
		}

		//s — cipher signature used to protect the stream (if present).
		public string[] Signatures { get; private set; }
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

		public VideoInfo (int itag, string mimeType, IEnumerable<string> urls, string[] signatures)
		{
			this.Itag = itag;
			this.MimeType = mimeType;
			this.Urls = urls;
			this.Signatures = signatures;
		}
	}
}

// https://gist.github.com/sidneys/7095afe4da4ae58694d128b1034e01e2
// https://www.youtube.com/get_video_info?video_id=tt66izA5y28
// https://tyrrrz.me/blog/reverse-engineering-youtube

//			tag=43
//			type=video/webm; codecs="vp8.0, vorbis"
//			fallback_host=redirector.googlevideo.com
//			url=https://r12---sn-3c27sn7k.googlevideo.com/videoplayback?itag=43&lmt=1367519763212098&ipbits=0&key=yt6&mime=video%2Fwebm&expire=1511401259&mn=sn-3c27sn7k&mm=31&ms=au&mv=m&mt=1511379591&ei=y9IVWuuyKI-YdLvnm8AO&sparams=dur%2Cei%2Cgcr%2Cid%2Cinitcwndbps%2Cip%2Cipbits%2Citag%2Clmt%2Cmime%2Cmm%2Cmn%2Cms%2Cmv%2Cnh%2Cpl%2Cratebypass%2Crequiressl%2Csource%2Cexpire&ip=255.255.255.255&id=o-AJuM11wvxuVl2WBgfb3nr6zbmXsFGQvhMelDobZ_KOrE&nh=IgpwcjAxLmticDAxKgkxMjcuMC4wLjE&requiressl=yes&gcr=ua&source=youtube&ratebypass=yes&pl=24&initcwndbps=1112500&dur=0.000
//			s=9599599594B0133328AA570AE0129E58478D7BCE9D226F.15ABC404267945A3F64FB4E42074383FC4FA80F5
//			quality=medium


