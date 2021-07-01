using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;

namespace YoutubeDownLoader
{
	public class HttpHelpers
	{
		public IDictionary<string, string> ParseQueryString(string queryString)
		{
			var queryStrings = queryString.Split ('?') [1].Split ('&');
			var parsedQueryStrings = new Dictionary<string, string> ();

			foreach (var query in queryStrings) 
			{
				var keyValueQuery = query.Split ('=');
				parsedQueryStrings.Add (keyValueQuery [0], keyValueQuery [1]);
			}
			return parsedQueryStrings;
		}

		public string RemoveCharacter(string input, char character)
		{
			// return Regex.Replace(input, "\\", "");
			StringBuilder output = new StringBuilder();
			for (int i = 0; i < input.Length; i++) {
				if(input[i] == character) i++;
				if (i >= input.Length)
					break;
				
				output.Append (input [i]);
			}
			return output.ToString ();
		}

		public string[] ParseJsonProperties(string input, string property)
		{
			var separator = ',';
			StringBuilder parsedProperties = new StringBuilder ();
			int j = 0;
			string rawProperty = "";

			for (int i = 0; i < input.Length; i++) {
				if (j >= property.Length)
					j = 0;
				if (input [i] == property [j])
					j++;
				else
					j = 0;
				if (j == property.Length-1) {
					while (input [i] != separator && input[i] != null) {
						rawProperty += input [i];
						i++;
					}
					parsedProperties.Append (this.SplitJsonProperty(rawProperty) + separator);
					rawProperty = "";
				}
			}
			return parsedProperties.ToString ().Split (separator);
		}

		private string SplitJsonProperty(string rawProperty)
		{
			string property = "";
			var rawPropertySplit = rawProperty.Split (':');
			if (rawPropertySplit.Length > 2) {
				for (int i = 1; i < rawPropertySplit.Length; i++) {
					property += rawPropertySplit [i] + ":";
				}
				property = property.Substring (0, property.Length - 1);
			} else
				property = rawPropertySplit [1];
			
			return property;
		}
	}
}

