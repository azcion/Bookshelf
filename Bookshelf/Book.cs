using System.Text.Json.Serialization;

namespace Bookshelf; 

internal readonly struct Book {

	public string Title { get; init; }
	public string[] Authors { get; init; }
	public Genre Genre { get; init; }
	public string Content { get; }

	[JsonConstructor]
	public Book (string title, string[] authors, Genre genre, string content) {
		Title = title;
		Authors = authors;
		Genre = genre;
		Content = content;
	}

}