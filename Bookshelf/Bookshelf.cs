namespace Bookshelf;

internal class Bookshelf {

	public int BookCount => _shelf.Count;
	
	private readonly List<Book> _shelf = new();
	private readonly string _filename = "Bookshelf.json";

	public Bookshelf () {}

	public Bookshelf (string filename) {
		_filename = filename;
	}

	public void Load () {
		_shelf.AddRange(IOManager.LoadList<Book>(_filename));
	}

	public void Save () {
		_shelf.Save(_filename);
	}

	public List<Book> Get (Genre genre) {
		return _shelf.FindAll(book => book.Genre == genre);
	}

	public void Add (Book book) {
		_shelf.Add(book);
	}

	public void Remove (Book book) {
		_shelf.Remove(book);
	}

	public void Clear () {
		_shelf.Clear();
	}

}