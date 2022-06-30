using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Bookshelf {

	internal static class IOManager {

		public static List<T> LoadList<T> (string name) {
			if (!File.Exists(name)) {
				return new List<T>();
			}

			string file = File.ReadAllText(name);
			List<T>? deserialized = JsonSerializer.Deserialize<List<T>>(file);

			return deserialized ?? new List<T>();
		}

		public static async void Save (this object obj, string name) {
			await using FileStream createStream = File.Create(name);
			await JsonSerializer.SerializeAsync(createStream, obj);
			await createStream.DisposeAsync();
		}

	}

}