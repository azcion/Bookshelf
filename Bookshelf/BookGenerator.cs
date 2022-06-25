using System.Globalization;
using Mathy;

namespace Bookshelf; 

internal static class BookGenerator {

	internal struct Config {

		public int MinTitleWords { get; init; }
		public int MaxTitleWords { get; init; }
		public int MinAuthors { get; init; }
		public int MaxAuthors { get; init; }
		public int MinAuthorNameWords { get; init; }
		public int MaxAuthorNameWords { get; init; }
		public TextGenerator.Config TextGeneratorConfig { get; init; }

	}
	
	public static Book Get (Config config) {
		Rand rand = new();
		string title = TextGenerator.Get(
			config.MinTitleWords, config.MaxTitleWords).TitleCase();
		
		int numAuthors = Smooth.InverseSmoothStop2(
			config.MinAuthors, 
			config.MaxAuthors, 
			rand.NextDouble()).Round();
		
		string[] authors = new string[numAuthors];

		for (int author = 0; author < authors.Length; author++) {
			authors[author] = TextGenerator.Get(
				config.MinAuthorNameWords, config.MaxAuthorNameWords, true).TitleCase();
		}

		Genre genre = (Genre)rand.Next(1, GenreUtil.MaxGenreValue + 1);
		string content = TextGenerator.Get(config.TextGeneratorConfig);
		
		return new Book(title, authors, genre, content);
	}

	private static string TitleCase (this string text) {
		return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(text);
	}

	

}