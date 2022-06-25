using System.Diagnostics.CodeAnalysis;
using System.Text;
using Mathy;

namespace Bookshelf; 

internal static class TextGenerator {

	internal struct Config {

		public bool PreferFewer { get; init; }
		public int MinWords { get; init; }
		public int MaxWords { get; init; }
		public int MinSentences { get; init; }
		public int MaxSentences { get; init; }
		public int MinParagraphs { get; init; }
		public int MaxParagraphs { get; init; }

	}
	
	[SuppressMessage("ReSharper", "StringLiteralTypo")]
	private static readonly string[] Words = {
		"a", "ac", "accumsan", "ad", "aenean", "aliquam", "aliquet", "amet", "ante", "aptent", "arcu", "at", "auctor",
		"augue", "bibendum", "blandit", "class", "commodo", "condimentum", "congue", "consectetur", "consequat",
		"conubia", "convallis", "cras", "curabitur", "cursus", "dapibus", "diam", "dictum", "dictumst", "dignissim",
		"dolor", "donec", "dui", "duis", "efficitur", "egestas", "eget", "eleifend", "elementum", "elit", "enim",
		"erat", "eros", "est", "et", "etiam", "eu", "euismod", "ex", "facilisi", "facilisis", "fames", "faucibus",
		"felis", "fermentum", "feugiat", "finibus", "fringilla", "fusce", "gravida", "habitant", "habitasse", "hac",
		"hendrerit", "himenaeos", "iaculis", "id", "imperdiet", "in", "inceptos", "integer", "interdum", "ipsum",
		"justo", "lacinia", "lacus", "laoreet", "lectus", "leo", "libero", "ligula", "litora", "lobortis", "lorem",
		"luctus", "maecenas", "magna", "malesuada", "massa", "mattis", "mauris", "maximus", "metus", "mi", "molestie",
		"mollis", "morbi", "nam", "nec", "neque", "netus", "nibh", "nisi", "nisl", "non", "nostra", "nulla", "nullam",
		"nunc", "odio", "orci", "ornare", "pellentesque", "per", "pharetra", "phasellus", "placerat", "platea", "porta",
		"porttitor", "posuere", "praesent", "pretium", "proin", "pulvinar", "purus", "quam", "quis", "quisque",
		"rhoncus", "risus", "rutrum", "sagittis", "sapien", "scelerisque", "sed", "sem", "semper", "senectus", "sit",
		"sociosqu", "sodales", "sollicitudin", "suscipit", "suspendisse", "taciti", "tellus", "tempor", "tempus",
		"tincidunt", "torquent", "tortor", "tristique", "turpis", "ullamcorper", "ultrices", "ultricies", "urna", "ut",
		"varius", "vehicula", "vel", "velit", "venenatis", "vestibulum", "vitae", "vivamus", "viverra", "volutpat",
		"vulputate"
	};

	public static string Get (int minWords, int maxWords, bool preferFewer = false) {
		Rand rand = new();
		StringBuilder result = new();
		
		int numWords = preferFewer
			? Smooth.InverseSmoothStop2(minWords, maxWords, rand.NextDouble()).Round()
			: rand.Next(minWords, maxWords + 1);

		for(int w = 0; w < numWords; w++) {
			if (w > 0) {
				result.Append(' ');
			}
					
			result.Append(Words[rand.Next(Words.Length)]);
		}

		return result.ToString();
	}

	public static string Get (Config config) {
		Rand rand = new();
		StringBuilder result = new();
		
		int numParagraphs = config.PreferFewer
			? Smooth.InverseSmoothStop2(
				config.MinParagraphs, 
				config.MaxParagraphs, 
				rand.NextDouble()).Round()
			: rand.Next(
				config.MinParagraphs, 
				config.MaxParagraphs + 1);
		
		int numSentences = config.PreferFewer
			? Smooth.InverseSmoothStop2(
				config.MinSentences, 
				config.MaxSentences, 
				rand.NextDouble()).Round()
			: rand.Next(
				config.MinSentences, 
				config.MaxSentences + 1);

		for(int p = 0; p < numParagraphs; p++) {
			for(int s = 0; s < numSentences; s++) {
				string sentence = Get(config.MinWords, config.MaxWords, config.PreferFewer);
				string firstCharacter = sentence[0].ToString().ToUpper();
				result.Append(firstCharacter);
				result.Append(sentence.AsSpan(1));
				result.Append(". ");
			}

			if (p < numParagraphs - 1) {
				result.Append(Environment.NewLine);
			}
		}

		return result.ToString();
	}

}