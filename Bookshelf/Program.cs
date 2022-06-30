using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Mathy;

namespace Bookshelf {

	internal static class Program {

		private static readonly BookGenerator.Config Config = new() {
			MinTitleWords = 1,
			MaxTitleWords = 6,
			MinAuthors = 1,
			MaxAuthors = 3,
			MinAuthorNameWords = 2,
			MaxAuthorNameWords = 3,
			TextGeneratorConfig = new TextGenerator.Config {
				PreferFewer = true,
				MinWords = 2,
				MaxWords = 12,
				MinSentences = 5,
				MaxSentences = 12,
				MinParagraphs = 5,
				MaxParagraphs = 20
			}
		};

		private static readonly BookGenerator.Config ConfigDemo = new() {
			MinTitleWords = 1,
			MaxTitleWords = 3,
			MinAuthors = 1,
			MaxAuthors = 2,
			MinAuthorNameWords = 2,
			MaxAuthorNameWords = 2,
			TextGeneratorConfig = new TextGenerator.Config {
				PreferFewer = true,
				MinWords = 2,
				MaxWords = 6,
				MinSentences = 5,
				MaxSentences = 8,
				MinParagraphs = 5,
				MaxParagraphs = 10
			}
		};

		private static readonly BookGenerator.Config SelectedConfig = Config;
		private const bool SkipIntro = false;
		private static readonly int BookContentMaxWidth = Math.Min(Console.WindowWidth, 120);

		[SuppressMessage("ReSharper", "HeuristicUnreachableCode")]
		private static void Main () {
			Console.Clear();
			string nl = Environment.NewLine;

			string[] intro = {
				"  Welcome to the", " Magic Bookshelf", ", a place with almost infinite books" +
				                                        $",{nl}  written by over a million different authors, right at your fingertips!",
				$"{nl}                                                   (Lorem Ipsum edition)"
			};

			Write(intro[0], ConsoleColor.Green);
			Write(intro[1], ConsoleColor.Cyan);
			Write(intro[2], ConsoleColor.Green);
			Thread.Sleep(SkipIntro ? 0 : 2500);
			Write(intro[3], ConsoleColor.Green);
			Thread.Sleep(SkipIntro ? 0 : 500);
			int topAfterIntro = Console.CursorTop + 1;
			Write($"{nl}Press any key to begin ");

			if (!SkipIntro) {
				Console.ReadKey(true);
			}

			Position(0, topAfterIntro);
			Write("Choose an option (1/2): ");
			(int leftOption, int topOption) = Console.GetCursorPosition();
			Write($"{nl}  [1] Load an existing bookshelf.", ConsoleColor.Green);
			Write($"{nl}  [2] Discover a new bookshelf.", ConsoleColor.Green);
			Position(leftOption, topOption);
			Bookshelf shelf = new();

			while (true) {
				string? readInput = Console.ReadLine();

				switch (readInput) {
					case "1":
						Console.Clear();
						Write("Analyzing your Magic Bookshelf");
						int leftAnalyzing = Console.CursorLeft;
						shelf.Load();
						int bookCount = shelf.BookCount;

						for (int i = 0; i < 25; i++) {
							if (i % 12 == 0) {
								ReplaceAhead(leftAnalyzing, 0, "   ");
							} else if (i % 3 == 0) {
								Write(".");
							}

							Thread.Sleep(SkipIntro ? 0 : 150);
						}

						Position(0, 0);
						WriteLine($"Found {bookCount} books in your archive.");
						Thread.Sleep(SkipIntro ? 0 : 1000);

						break;
					case "2":
						Console.Clear();
						Write("Analyzing a new Magic Bookshelf");
						leftAnalyzing = Console.CursorLeft;
						Rand rand = new();
						bookCount = rand.Next(20, 31);

						for (int i = 0; i < bookCount; i++) {
							if (i % 12 == 0) {
								ReplaceAhead(leftAnalyzing, 0, "   ");
							} else if (i % 3 == 0) {
								Write(".");
							}

							shelf.Add(BookGenerator.Get(SelectedConfig));
							Thread.Sleep(SkipIntro ? 0 : 150);
						}

						shelf.Save();
						Console.Clear();
						WriteLine($"Discovered {bookCount} books this time.");
						Thread.Sleep(SkipIntro ? 0 : 1000);

						break;
					default:
						ReplaceAhead(leftOption, topOption, "    Invalid option, please try again.");

						continue;
				}

				break;
			}



			while (true) {
				Write($"Select a category (1-{GenreUtil.GenreCount - 1}): ");
				(int leftCategory, int topCategory) = Console.GetCursorPosition();
				WriteLine("    Enter empty to quit.");
				Genre genre;

				for (int i = 1; i < GenreUtil.Genres.Length; i++) {
					genre = GenreUtil.Genres[i];
					int count = shelf.Get(genre).Count;
					string suffix = count == 1 ? "" : "s";
					Write($"  [{(int)genre}] {GenreUtil.Label(genre)}", ConsoleColor.Green);
					WriteLine($" ({count} book{suffix})");
				}

				Position(leftCategory, topCategory);
				string? readInput = Console.ReadLine();

				if (string.IsNullOrEmpty(readInput)) {
					Console.Clear();
					Console.WriteLine("Exiting bookshelf.");

					return;
				}

				if (!Enum.TryParse(readInput, out genre)) {
					ReplaceAhead(leftCategory, topCategory, "    Invalid category, please try again.");

					continue;
				}

				Console.Clear();
				string infoText = "             Enter empty to return.";

				while (true) {
					Position(0, 1);
					WriteLine($"{GenreUtil.Adjective(genre)}books:");
					List<Book> books = shelf.Get(genre);

					for (int i = 0; i < books.Count; i++) {
						Book book = books[i];
						BookFormatter.Write(book, $"  [{i + 1}] ", nl);
					}

					WriteLine("Commands:" +
					          $"{nl}  [read <num>] e.g.: read 2" +
					          $"{nl}  [edit <num>] e.g.: edit 2" +
					          $"{nl}  [delete <num>] e.g.: delete 2" +
					          $"{nl}  [discover <num>] add new books to this category, e.g.: discover 5",
						ConsoleColor.Gray);

					Position(0, 0);
					Write("Enter a command: ");
					(int leftCommand, int topCommand) = Console.GetCursorPosition();
					Write(infoText);
					Position(leftCommand, topCommand);
					readInput = Console.ReadLine();

					if (string.IsNullOrEmpty(readInput)) {
						Console.Clear();

						break;
					}

					string[] splitInput = readInput.Split(' ');

					if (splitInput.Length != 2 || !int.TryParse(splitInput[1], out int inputInt)
					                           || inputInt < 0 ||
					                           (inputInt > books.Count && splitInput[0] != "discover")) {
						infoText = "             Invalid command, please try again.";

						continue;
					}

					switch (splitInput[0]) {
						case "read":
							Console.Clear();
							Book book = books[inputInt - 1];
							BookFormatter.Write(book, suffix: nl);
							int topBook = Console.CursorTop;
							string[] text = book.Content.Split(nl);

							for (int i = 0; i < text.Length; i++) {
								string line = text[i];
								BookFormatter.WriteTextWrap(line, BookContentMaxWidth);
								bool lastLine = i == text.Length - 1;

								if (Console.CursorTop - topBook <= 8 && !lastLine) {
									continue;
								}

								string continueText = "Press any key to " +
								                      (lastLine ? "close the book." : "continue reading.");

								int offset = (int)(BookContentMaxWidth / 2d - continueText.Length / 2d);
								Write(new string(' ', offset));
								Write(continueText, ConsoleColor.Gray);
								Console.ReadKey(true);

								while (Console.CursorTop > 1) {
									Console.Clear();
								}
							}

							break;
						case "edit":
							string editInfoText = "    Enter empty to return.";
							book = books[inputInt - 1];

							while (true) {
								Console.Clear();
								Write("Select a field to edit: ");
								(int leftEdit, int topEdit) = Console.GetCursorPosition();
								WriteLine(editInfoText);
								Write("  [1] Title: ");
								(int leftEditTitle, int topEditTitle) = Console.GetCursorPosition();
								WriteLine(book.Title, ConsoleColor.Green);
								string suffix = book.Authors.Length > 1 ? "s" : "";
								Write($"  [2] Author{suffix}: ");
								(int leftEditAuthors, int topEditAuthors) = Console.GetCursorPosition();
								BookFormatter.WriteArray(book.Authors, ConsoleColor.Green);
								Write($"{nl}  [3] Category: ");
								(int leftEditCategory, int topEditCategory) = Console.GetCursorPosition();
								WriteLine(GenreUtil.Label(book.Genre), ConsoleColor.Green);
								int topBottom = Console.CursorTop;
								Position(leftEdit, topEdit);
								readInput = Console.ReadLine();

								if (string.IsNullOrEmpty(readInput)) {
									Console.Clear();

									break;
								}

								switch (readInput) {
									case "1":
										ClearAhead(leftEditTitle, topEditTitle);
										readInput = Console.ReadLine();

										if (string.IsNullOrEmpty(readInput)) {
											editInfoText = "    Invalid title, please try again.";

											continue;
										}

										Book newBook = book with {
											Title = readInput
										};

										shelf.Remove(book);
										shelf.Add(newBook);
										shelf.Save();
										book = newBook;
										editInfoText = "    Saved new title.";

										continue;
									case "2":
										Position(0, topBottom);
										WriteLine("Enter comma-separated authors.", ConsoleColor.Gray);
										ClearAhead(leftEditAuthors, topEditAuthors);
										readInput = Console.ReadLine();

										if (string.IsNullOrEmpty(readInput)) {
											editInfoText = "    Invalid field, please try again.";

											continue;
										}

										string[] authors = readInput.Split(',');

										for (int i = 0; i < authors.Length; i++) {
											authors[i] = authors[i].Trim();
										}

										newBook = book with {
											Authors = authors
										};

										shelf.Remove(book);
										shelf.Add(newBook);
										shelf.Save();
										book = newBook;
										suffix = authors.Length > 1 ? "s" : "";
										editInfoText = $"    Saved new author{suffix}.";

										continue;
									case "3":
										Position(0, topBottom);
										WriteLine("Categories:");

										for (int i = 1; i < GenreUtil.Genres.Length; i++) {
											genre = GenreUtil.Genres[i];
											WriteLine($"  [{(int)genre}] {GenreUtil.Label(genre)}", ConsoleColor.Green);
										}

										Position(leftEditCategory, topEditCategory);
										Write($"    Enter category (1-{GenreUtil.GenreCount - 1})");
										Position(leftEditCategory, topEditCategory);

										readInput = Console.ReadLine();

										if (!Enum.TryParse(readInput, out genre)) {
											editInfoText = "    Invalid category, please try again.";

											continue;
										}

										newBook = book with {
											Genre = genre
										};

										shelf.Remove(book);
										shelf.Add(newBook);
										shelf.Save();
										book = newBook;
										editInfoText = "    Saved new category.";

										continue;
									default:
										editInfoText = "    Invalid field, please try again.";

										continue;
								}
							}

							break;
						case "delete":
							Console.Clear();
							book = books[inputInt - 1];
							Position(0, 1);
							Write("You are about to delete ", ConsoleColor.DarkRed);
							WriteLine(book.Title, ConsoleColor.Green);
							Position(0, 0);
							Write("Are you sure? (yes/no) ");
							readInput = Console.ReadLine();

							if (string.IsNullOrEmpty(readInput) || readInput.ToLower() != "yes") {
								Console.Clear();
								Write("Nothing was deleted.", ConsoleColor.Green);
								Thread.Sleep(1000);

								break;
							}

							shelf.Remove(book);
							shelf.Save();
							Console.Clear();
							Write("The book was deleted.", ConsoleColor.Green);
							Thread.Sleep(1000);

							break;
						case "discover":
							Console.Clear();
							Write("Searching for more books");
							int leftAnalyzing = Console.CursorLeft;

							for (int i = 0; i < inputInt; i++) {
								if (i % 12 == 0) {
									ReplaceAhead(leftAnalyzing, 0, "   ");
								} else if (i % 3 == 0) {
									Write(".");
								}

								book = BookGenerator.Get(SelectedConfig) with { Genre = genre };
								shelf.Add(book);
								Thread.Sleep(SkipIntro ? 0 : 150);
							}

							shelf.Save();
							Console.Clear();
							WriteLine($"Discovered {inputInt} new books.");
							Thread.Sleep(SkipIntro ? 0 : 1000);

							break;
					}

					Console.Clear();
				}
			}
		}

		#region Helpers

		private static void WriteLine (string text) {
			Console.WriteLine(text);
		}

		private static void WriteLine (string text, ConsoleColor color) {
			Console.ForegroundColor = color;
			Console.WriteLine(text);
			Console.ForegroundColor = ConsoleColor.White;
		}

		private static void Write (string text) {
			Console.Write(text);
		}

		private static void Write (string text, ConsoleColor color) {
			Console.ForegroundColor = color;
			Console.Write(text);
			Console.ForegroundColor = ConsoleColor.White;
		}

		private static void Position (int left, int top) {
			Console.SetCursorPosition(left, top);
		}

		private static void ClearAhead (int left, int top) {
			ReplaceAhead(left, top, new string(' ', Console.WindowWidth - left));
		}

		private static void ReplaceAhead (int left, int top, string text, bool line = false) {
			Console.SetCursorPosition(left, top);
			Console.Write(text);

			if (line) {
				ClearAhead(left + text.Length, top);
			}

			Console.SetCursorPosition(left, top);
		}

		#endregion

	}

}