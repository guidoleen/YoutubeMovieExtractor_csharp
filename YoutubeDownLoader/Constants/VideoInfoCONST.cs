using System;
using System.Collections.Generic;

namespace YoutubeDownLoader
{
	public static class VideoInfoCONST
	{
		public const string VideoInfoNameSeperator = "_";

		public const string RegexExtractPlayerJsSrc = @"(""jsUrl"":"")(.*?)("",)"; // @"(\""jsUrl\"":"")(.*)(\.js"",)"; // From html source
		public const string RegexExtractRawUrl = @"(""probeUrl"":"")(.*)(,)"; // From video_info

		public const string RegexExtractRawVideoInfo = @"(""formats"":\[)(.*)(\])";
		public const string RegexExtractSignature = @"(signature=)([a-zA-Z0-9.]{1,200})(&)";

		public const string RegexExtractItag = @"{(\\\""itag\\""|""itag""):([0-9]{1,})(,)";

		public const string Mp3 = ".mp3";
		public const string Mp4 = ".mp4";
		public const string Mov = ".mov";
		public const string Ogg = ".ogg";

		public const string YoutubeUrl = @"https://www.youtube.com/";
		private const string VideoInfoUrlPart = @"get_video_info?html5=1&video_id=";

		public const string YoutubeVideoInfoUrl = YoutubeUrl + VideoInfoUrlPart;

		public const string YoutubeEmbedUrl = YoutubeUrl + @"embed/"; // Old stuff - Deprecated
		public const string YoutubeEmbedPlayerJsUrl = @"https://s.ytimg.com/yts/jsbin/www-embed-player-vfl7jE1l_.js"; // Old stuff - Deprecated

		public static Dictionary<string, string> Utf8CharEncodingValues = new Dictionary<string,string>()
		{
			{"%26",@"&"},
			{"%3D",@"="},
			{"%2F",@"/"},
			{"%3F",@"?"},
			{"%7B",@"{"},
			{"%7D",@"}"},
			{"%22","\""},
			{"%3A",@":"},
			{"%2C",@","},
			{"%5D",@"]"},
			{"%5B",@"["},
			{"%25",@"%"},
			{"%5C",@"\"},
			{"\\u0026",@"&"},
			{"%2B",@"+"}
		};
	}
}

