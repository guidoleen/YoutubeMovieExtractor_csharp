using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace YoutubeDownLoader
{
	internal static class Decipherer
	{
		public static string[] DecipherWithJsPlayer(string jsPlayerSrc, string[] signatures)
		{
			// Find the name of the function that handles deciphering
			var deciperFunctionName = GetInitialFunctionDecipherName(jsPlayerSrc);
			if (string.IsNullOrWhiteSpace(deciperFunctionName))
				throw new Exception("Could not find the entry function for signature deciphering.");

			// Get the functionsequence to define the decipher algorithm
			var jsFunctionsSequence = GetJsFunctionsSequence(deciperFunctionName, jsPlayerSrc);

			// Map the js functions to the C# equivalent
			var mappedJsFunctions = MapJsFunction(jsFunctionsSequence, jsPlayerSrc);
			if (mappedJsFunctions == null || mappedJsFunctions.Count != jsFunctionsSequence.Length)
				throw new Exception ("Could not find and map the js functions to the C# equivalent.");

			// Execute the Mapped C# equivalent
			var deciperedSignatures = new string[signatures.Length];

			for (int i = 0; i < deciperedSignatures.Length; i++) {
				deciperedSignatures[i] = DecipherOnMappedFunctions(mappedJsFunctions, signatures[i]).ToString();
			}
			return deciperedSignatures;
		}

		// 1) Get initial function name for the js function sequence. Name eg. wy or encodeURIComponent
		private static string GetInitialFunctionDecipherName(string jsPlayerSrc)
		{
			/// TESTED AND WORKED ON PLAYER:
			// (?P<sig>[a-zA-Z0-9$]+)\s*=\s*function\(\s*a\s*\)\s*{\s*a\s*=\s*a\.split\(\s*""\s*\)
			// \b[a-zA-Z0-9]+\s*&&\s*[a-zA-Z0-9]+\.set\([^,]+\s*,\s*(?P<sig>[a-zA-Z0-9$]+)\(

			string[] functionPatterns = {
				@"([a-zA-Z0-9$]+)\s*=\s*function\(\s*a\s*\)\s*{\s*a\s*=\s*a\.split\(\s*""""\s*\)"
			};

			foreach (var functionPattern in functionPatterns) {
				var regex = new Regex(functionPattern);
				var match = regex.Match (jsPlayerSrc);

				if(match.Success)
					return match.Groups[1].Value;
			}

			return null;
		}

		// 2) Get Js functions sequence from initial function name.
		// The functionName eg. PW and the index param's get returned.
		private static Tuple<string, int>[] GetJsFunctionsSequence(string initialFunctionName, string jsPlayerSrc)
		{
			// Regex: INITIAL_FUNCTION_NAME=function\(\w\){[a-z=\.\(\"\)]*;(.*);(?:.+)} OR  @"INITIAL_FUNCTION_NAME=function\(\w+\)\{(.*?)\}"
			var regex = Regex.Escape(initialFunctionName) + @"=function\(\w\){[a-z=\.\(\""\)]*;(.*);(?:.+)}";
			var jsFunctionsSequenceWithParams = Regex.Match(jsPlayerSrc, regex).Groups[1].Value.Split(';');

			Tuple<string, int>[] jsFunctionsSequence = new Tuple<string, int> [jsFunctionsSequenceWithParams.Length];

			for (int i = 0; i < jsFunctionsSequence.Length; i++) {
				var jsFunctionParam = jsFunctionsSequenceWithParams[i].Split('.')[1].Split(',');
				jsFunctionsSequence [i] = Tuple.Create(Regex.Match (jsFunctionParam [0], @"(.*)(\(.*)").Groups [1].Value,
					Int32.Parse (Regex.Match (jsFunctionParam [1], @"\d").Value)
				);
			}

			return jsFunctionsSequence;
		}

		// 3) Map found functions to C# swap, splice or reverse equivalent
		private static List<IJsToCSharpFunctions> MapJsFunction(Tuple<string, int>[] jsFunctionsSequence, string jsPlayerSrc)
		{
			List<IJsToCSharpFunctions> mappedJsFunctions = new List<IJsToCSharpFunctions> ();

			// Regex: FUNCTION_NAME:(.*) // eg. >> PW:(.*)\bsplice\b
			string[] regexFunctions = {
				@":(.*)\bsplice\b",
				@":(.*)\breverse\b",
				@":(.*)\blength\b"
			};

			for (int i = 0; i < jsFunctionsSequence.Length; i++) {
				var regex = new Regex (jsFunctionsSequence [i].Item1 + regexFunctions [0]);
				if (regex.Match (jsPlayerSrc).Success) {
					mappedJsFunctions.Add (new CsSplice(){ Index = jsFunctionsSequence [i].Item2 });
					continue;
				}
					
				regex = new Regex (jsFunctionsSequence [i].Item1 + regexFunctions [1]);
				if (regex.Match (jsPlayerSrc).Success) {
					mappedJsFunctions.Add (new CsReverse());
					continue;
				}

				regex = new Regex (jsFunctionsSequence [i].Item1 + regexFunctions [2]);
				if (regex.Match (jsPlayerSrc).Success) {
					mappedJsFunctions.Add (new CsSwap(){ Index = jsFunctionsSequence [i].Item2 });
					continue;
				}
					
				return null;
			}

			return mappedJsFunctions;
		}

		// 4) Execute mapped c# functions
		private static string DecipherOnMappedFunctions(List<IJsToCSharpFunctions> mappedCSharpFunctions, string signature)
		{
			string decipheredSignature = signature;
			foreach (var mappedFunction in mappedCSharpFunctions) {
				var deciphered = decipheredSignature.ToString ();
				decipheredSignature = mappedFunction.InvokeFunction (deciphered);

			}

			return decipheredSignature;
		}
	}


	/// <summary>
	/// 
	/// Js to C sharp functions.
	/// </summary>
	internal interface IJsToCSharpFunctions
	{
		string InvokeFunction (string signature);
	}

	internal class CsReverse : IJsToCSharpFunctions
	{
		// Js equivalent: function(a, b) { a.reverse() }
		public string InvokeFunction (string signature)
		{
			string reversedSignature = string.Empty;

			for (int j = signature.Length-1; j >= 0; j--) {
				reversedSignature += signature [j];
			}
			return reversedSignature;
		}
	}

	internal class CsSwap : IJsToCSharpFunctions
	{
		public int Index { get; set; }

		// Js equivalent: function(a, b) { var c=a[0];a[0]=a[b%a.length];a[b]=c }
		public string InvokeFunction (string signature)
		{
			string swappedSignature = string.Empty;

			var a = signature.ToCharArray ();
			var c = a[0];
			var swapIndex = Index % a.Length;

			a[0] = a[swapIndex];
			a[swapIndex] = c;

			for (int i = 0; i < a.Length; i++) {
				swappedSignature += a [i];
			}

			return swappedSignature;
		}
	}

	internal class CsSplice : IJsToCSharpFunctions
	{
		public int Index { get; set; }

		// Js equivalent: function(a, b) { a.splice(0, b) }
		public string InvokeFunction (string signature)
		{
			return signature.Substring(Index).ToString();
		}
	}
}