using System;

namespace YoutubeDownLoader
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			string youtubeUrl = "";
			YoutubeLoader youtubeLoader = new YoutubeLoader ();

			while(youtubeUrl != "q")
			{
				
				Console.WriteLine ("Insert youtube url here");

				youtubeUrl = Console.ReadLine ();

				youtubeLoader.WriteYoutubeMovieToDisk (youtubeUrl);
			}
		}
	}
}