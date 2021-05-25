using System;
using System.Collections.Generic;
// using Newtonsoft.Json.Linq;

namespace YoutubeDownLoader
{
	public class DownloadUrlResolver
	{
		private const string RateBypassFlag = "ratebypass";
		private const string SignatureQuery = "signature";

		/// <summary>
		/// Decrypts the signature in the <see cref="VideoInfo.DownloadUrl" /> property and sets it
		/// to the decrypted URL. Use this method, if you have decryptSignature in the <see
		/// cref="GetDownloadUrls" /> method set to false.
		/// </summary>
		/// <param name="videoInfo">The video info which's downlaod URL should be decrypted.</param>
		/// <exception cref="YoutubeParseException">
		/// There was an error while deciphering the signature.
		/// </exception>
		public static void DecryptDownloadUrl(VideoInfo videoInfo, string javaScript)
		{
			IDictionary<string, string> queries = new HttpHelpers().ParseQueryString(videoInfo.DownloadUrl);

			if (queries.ContainsKey(SignatureQuery))
			{
				string encryptedSignature = queries[SignatureQuery];

				string decrypted;

				try
				{
					decrypted = GetDecipheredSignature(javaScript, encryptedSignature);
				}

				catch (Exception ex)
				{
					throw new Exception("Could not decipher signature", ex);
				}

				videoInfo.DownloadUrl = ""; // HttpHelper.ReplaceQueryStringParameter(videoInfo.DownloadUrl, SignatureQuery, decrypted);
				videoInfo.RequiresDecryption = false;
			}
		}
	
		private static string GetDecipheredSignature(string javaScript, string signature)
		{
			return Decipherer.DecipherWithJsPlayer(javaScript, signature);
		}
	}
}