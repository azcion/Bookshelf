namespace Bookshelf; 

internal static class GenreUtil {

	public static readonly Genre[] Genres;
	public static readonly string[] GenreNames;
	public static readonly int GenreCount;
	public static readonly int MaxGenreValue;
	
	static GenreUtil () {
		Genres = Enum.GetValues<Genre>();
		GenreCount = Genres.Length;
		MaxGenreValue = (int)Genres[^1];
		GenreNames = Enum.GetNames<Genre>();
		GenreNames[(int)Genre.SciFi] = "Sci-Fi";
	}

	public static string Label (Genre genre) {
		return GenreNames[(int)genre];
	}

	public static string Adjective (Genre genre) {
		return genre switch {
			Genre.Cooking => "Cook",
			_ => Label(genre) + " "
		};
	}

	public static string Article (Genre genre) {
		return genre switch {
			Genre.Adventure => "an",
			_ => "a"
		};
	}

}