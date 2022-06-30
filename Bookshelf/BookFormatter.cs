using System;
using System.Collections.Generic;

namespace Bookshelf {

	internal static class BookFormatter {

		public static void Write (Book book, string prefix = "", string suffix = "") {
			Console.ForegroundColor = ConsoleColor.Green;
			Console.Write($"{prefix}{book.Title}");
			Console.ForegroundColor = ConsoleColor.White;
			Console.Write($", {GenreUtil.Article(book.Genre)} {GenreUtil.Adjective(book.Genre).ToLower()}book by ");
			WriteArray(book.Authors, ConsoleColor.Green);
			Console.Write(suffix);
			Console.ForegroundColor = ConsoleColor.White;
		}

		public static void WriteArray (string[] array, ConsoleColor color) {
			for (int i = 0; i < array.Length; i++) {

				if (i > 0 && array.Length > 1) {
					Console.Write(i == array.Length - 1 ? " and " : ", ");
				}

				Console.ForegroundColor = color;
				Console.Write(array[i]);
				Console.ForegroundColor = ConsoleColor.White;
			}
		}

		public static void WriteTextWrap (string text, int width) {
			string[] lines = text.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

			foreach (string line in lines) {
				string process = line;
				List<string> wrapped = new();

				while (process.Length > width) {
					int wrapAt = process.LastIndexOf(' ', Math.Min(width - 1, process.Length));

					if (wrapAt <= 0) {
						break;
					}

					wrapped.Add(process[..wrapAt]);
					process = process.Remove(0, wrapAt + 1);
				}

				foreach (string wrap in wrapped) {
					Console.WriteLine(wrap);
				}

				Console.WriteLine(process);
			}
		}

	}

}