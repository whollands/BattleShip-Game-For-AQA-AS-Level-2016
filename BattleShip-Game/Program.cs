/*
 * 
 * Bugs:
 * 
 * - Inputted co-ordinates can go out of range
 * - Crashes if non-ints entered
 * 
 * Improvements:
 * - save to file
 * - letters on top row
 * 
*/

using System;
using System.IO;

class Program
{
	public struct ShipType
	{
		public string Name;
		public int Size;
		public int HitCount;
	}
		
	private static string GetFileName()
	{
		string path = "";
		bool fileExists = false;

		while(!fileExists)
			// keep asking for file while not found
		{
			Console.Write("Enter file name or path: ");
			path = Console.ReadLine();

			if(File.Exists(path))
			{
				fileExists = true;
			}
			else
			{
				Console.WriteLine("File not found.");
			}
		}

		return path;

	} 


	private static string CompressFile(string Filename)
	{
		string compressedText = "";
		string uncompressedText = "";
		string line = "";
		int count = 1;

		using (StreamReader f = new StreamReader(Filename))
		{
			while ((line = f.ReadLine()) != null)
			{
				uncompressedText += line;
			}

			for(int i = 1; i < uncompressedText.Length; i++)
			{

				if(uncompressedText[i] == uncompressedText[i - 1])
				{
					count++;
					// count the number of repeats of the character
				}
				else
				{
					compressedText += Convert.ToString(count) + Convert.ToString(uncompressedText[i - 1]);
					// write the compressed text to the file

					count = 1;
					// reset count
				}
					
			}

			compressedText += Convert.ToString(count) + Convert.ToString(uncompressedText[compressedText.Length]);
			// append the final character to be compressed to the end of the file
		}

		return compressedText;
		// return the compressed text back as a string
	}

	private static void SaveGame(ref char[,] Board)
	{
		string fileName = "";

		Console.Write("Enter a name for this game: ");
		fileName = Console.ReadLine();

		// check if file exists
		// 

		using (StreamWriter f = new StreamWriter(fileName + ".txt"))
		{
			for(int row = 0; row < 10; row ++)
			{
				for(int col = 0; col < 10; col ++)
				{
					f.Write(Board[row, col]);
				}
				f.Write("\n");
			}
		}

	}

	private static void GetRowColumn(ref int Row, ref int Column)
	{
		char inputRow = ' ';
		bool validatedRow = false;

		string inputCol = "";
		bool validatedCol = false;

		while(validatedCol == false)
		{
			Console.Write("Please enter column: ");
			inputCol = Console.ReadLine();

			if(int.TryParse(inputCol, out Column))
			{
				if(Column < 0 || Column > 9)
				{
					Console.WriteLine("Co-ordinate must be a number between 0 and 9.\n");
				}
				else
				{
					validatedCol = true;
				}

			}
			else
			{
				Console.WriteLine("Co-ordinate must be a number.\n");
			}
				
		}

		while(validatedRow == false)
		{
			Console.Write("Please enter row: ");

			if(char.TryParse(Console.ReadLine().ToLower(), out inputRow))
			{
				Row = (int)inputRow - 97;

				if(Row < 0 || Row > 9)
				{
					Console.WriteLine("Row must be between A and J.\n");
				}
				else
				{
					validatedRow = true;
				}

			}
			else
			{
				Console.WriteLine("Row must be a letter between A and J.\n");
			}

		}
	}

	private static void MakePlayerMove(ref char[,] Board, ref ShipType[] Ships)
	{
		int Row = 0;
		int Column = 0;
		GetRowColumn(ref Row, ref Column);
		if (Board[Row, Column] == 'm' || Board[Row, Column] == 'h')
		{
			Console.WriteLine("Sorry, you have already shot at the square (" + Column + "," + Row + "). Please try again.");
		}
		else if (Board[Row, Column] == '-')
		{
			Console.WriteLine("Sorry, (" + Column + "," + Row + ") is a miss.");
	
			Board[Row, Column] = 'm';
		}
		else if (Board[Row, Column] == 'M')
		{
			

			Console.WriteLine("You hit a mine! (" + Column + "," + Row + ").");

			Board[Row, Column] = '*';
			// mark the mine that was hit on the board with an * because it's pretty :D
			// (also that way the PrintBoard() function knows whether a mine is hit or needs to be hidden.

				for(int SearchRow = 0; SearchRow <= 9; SearchRow++)
				{
					for(int SearchCol = 0; SearchCol <= 9; SearchCol++)
					{
						if(
							(Column - 1 == SearchCol && Row - 1 == SearchRow) ||
							(Column == SearchCol && Row - 1 == SearchRow) ||
							(Column + 1 == SearchCol && Row - 1 == SearchRow) ||
							// search the 3 cells above mine
	
							(Column - 1 == SearchCol && Row == SearchRow) ||
							(Column + 1 == SearchCol && Row == SearchRow) ||
							// search cells left and right of mine
	
							(Column - 1 == SearchCol && Row + 1 == SearchRow) ||
							(Column == SearchCol && Row + 1 == SearchRow) ||
							(Column + 1 == SearchCol && Row + 1 == SearchRow)
							// search the 3 cells below mine
						)
						{
							if(
								Board[SearchRow, SearchCol] == 'A' || 
								Board[SearchRow, SearchCol] == 'B' || 
								Board[SearchRow, SearchCol] == 'S' || 
								Board[SearchRow, SearchCol] == 'D' || 
								Board[SearchRow, SearchCol] == 'P'
								// if... we find a ship there...
							)
							{
								Board[SearchRow, SearchCol] = 'h';
								// mark it as a hit
							}
							if(Board[SearchRow, SearchCol] == '-')
							{
								Board[SearchRow, SearchCol] = 'm';
								// otherwise mark it as a miss
							}

						}
					} // END for search col
				} // END for search row


		}
		else
		{
			Console.WriteLine("Hit at (" + Column + "," + Row + ").");
			Board[Row, Column] = 'h';
		}
	}

	private static void SetUpBoard(ref char[,] Board)
	{
		for (int Row = 0; Row < 10; Row++)
		{
			for (int Column = 0; Column < 10; Column++)
			{
				Board[Row, Column] = '-';
			}
		}
	}

	private static void LoadGame(string TrainingGame, ref char[,] Board)
	{
		string Line = "";
		StreamReader BoardFile = new StreamReader(TrainingGame);
		for (int Row = 0; Row < 10; Row++)
		{
			Line = BoardFile.ReadLine();
			for (int Column = 0; Column < 10; Column++)
			{
				Board[Row, Column] = Line[Column];
			}
		}
		BoardFile.Close();
	}

	private static void PlaceRandomShips(ref char[,] Board, ShipType[] Ships)
	{
		Random RandomNumber = new Random();
		bool Valid;
		char Orientation = ' ';
		int Row = 0;
		int Column = 0;
		int HorV = 0;
		foreach (var Ship in Ships)
		{
			Valid = false;
			while (Valid == false)
			{
				Row = RandomNumber.Next(0, 10);
				Column = RandomNumber.Next(0, 10);
				HorV = RandomNumber.Next(0, 2);
				if (HorV == 0)
				{
					Orientation = 'v';
				}
				else
				{
					Orientation = 'h';
				}
				Valid = ValidateBoatPosition(Board, Ship, Row, Column, Orientation);
			}
			Console.WriteLine("Computer placing the " + Ship.Name);
			PlaceShip(ref Board, Ship, Row, Column, Orientation);
		}
	}

	private static void PlaceRandomMines(ref char[,] Board, int NumOfMines)
	{
		Random RandomNumber = new Random();
		bool validMine = false;
		int Row = 0;
		int Column = 0;

		for(int i = 0; i < NumOfMines; i++)
			// repeat process for each mine
		{
			while(!validMine)
				// repeat until mine has been placed
			{
				Row = RandomNumber.Next(0, 10);
				Column = RandomNumber.Next(0, 10);

				if(Board[Row, Column] == '-')
					// check if mine has been placed there
				{
					Board[Row, Column] = 'M';
					validMine = true;
				}
			}
			validMine = false;
			// reset for next mine
		}
	} 

	private static void PlaceShip(ref char[,] Board, ShipType Ship, int Row, int Column, char Orientation)
	{
		if (Orientation == 'v')
		{
			for (int Scan = 0; Scan < Ship.Size; Scan++)
			{
				Board[Row + Scan, Column] = Ship.Name[0];
			}
		}
		else if (Orientation == 'h')
		{
			for (int Scan = 0; Scan < Ship.Size; Scan++)
			{
				Board[Row, Column + Scan] = Ship.Name[0];
			}
		}
	}

	private static bool ValidateBoatPosition(char[,] Board, ShipType Ship, int Row, int Column, char Orientation)
	{
		if (Orientation == 'v' && Row + Ship.Size > 10)
		{
			return false;
		}
		else if (Orientation == 'h' && Column + Ship.Size > 10)
		{
			return false;
		}
		else
		{
			if (Orientation == 'v')
			{
				for (int Scan = 0; Scan < Ship.Size; Scan++)
				{
					if (Board[Row + Scan, Column] != '-')
					{
						return false;
					}
				}
			}
			else if (Orientation == 'h')
			{
				for (int Scan = 0; Scan < Ship.Size; Scan++)
				{
					if (Board[Row, Column + Scan] != '-')
					{
						return false;
					}
				}
			}
		}
		return true;
	}

	private static bool CheckWin(char[,] Board)
	{
		for (int Row = 0; Row < 10; Row++)
		{
			for (int Column = 0; Column < 10; Column++)
			{
				if (Board[Row, Column] == 'A' || Board[Row, Column] == 'B' || Board[Row, Column] == 'S' || Board[Row, Column] == 'D' || Board[Row, Column] == 'P')
				{
					return false;
				}
			}
		}
		return true;
	}

	private static void PrintBoard(char[,] Board)
	{
		string RowLetter = "";

		Console.WriteLine();
		Console.WriteLine("The board looks like this: ");
		Console.WriteLine();


		Console.Write("  ");
		for (int Column = 0; Column < 10; Column++)
		{
			Console.Write("  " + Column + " ");
		}
		Console.WriteLine();

		Console.BackgroundColor = ConsoleColor.Blue;
		// background to blue

		Console.ForegroundColor = ConsoleColor.DarkBlue;
		Console.WriteLine("  +---+---+---+---+---+---+---+---+---+---+");
		Console.ForegroundColor = ConsoleColor.Black;

		Console.BackgroundColor = ConsoleColor.White;

		for (int Row = 0; Row < 10; Row++)
		{
			RowLetter = Convert.ToString((char)(Row + 97)).ToUpper();
			// convert integer to ascii, then to letter
		
			Console.BackgroundColor = ConsoleColor.White;

			Console.Write(RowLetter);

			Console.BackgroundColor = ConsoleColor.Blue;

			Console.ForegroundColor = ConsoleColor.DarkBlue;
			Console.Write(" | ");
			Console.ForegroundColor = ConsoleColor.Black;


			for (int Column = 0; Column < 10; Column++)
			{
				if (Board[Row, Column] == '-')
				{
					Console.Write(" ");
				}
				else if (
					Board[Row, Column] == 'A' ||
					Board[Row, Column] == 'B' ||
					Board[Row, Column] == 'S' ||
					Board[Row, Column] == 'D' ||
					Board[Row, Column] == 'P' ||
					Board[Row, Column] == 'M'

					// uncomment lines above to hide mines on the board.
					// for testing, mines are shown as “M” on the board or “*” if they are hit
				)
				{
					Console.Write(" ");
				}
				else
				{
					switch(Board[Row, Column])
					{
					case 'h': Console.ForegroundColor = ConsoleColor.Red; break;
					case 'm': Console.ForegroundColor = ConsoleColor.DarkBlue; break;
					case '*': Console.ForegroundColor = ConsoleColor.DarkYellow; break;
					}

					Console.Write(Board[Row, Column]);
					Console.ForegroundColor = ConsoleColor.Black;

				}

				Console.ForegroundColor = ConsoleColor.DarkBlue;
				Console.Write(" | ");
				Console.ForegroundColor = ConsoleColor.Black;
				
			}

			Console.WriteLine("");
			Console.ForegroundColor = ConsoleColor.DarkBlue;
			Console.WriteLine("  +---+---+---+---+---+---+---+---+---+---+");
			Console.ForegroundColor = ConsoleColor.Black;

			Console.BackgroundColor = ConsoleColor.White;
		}
	} 

	private static void DisplayMenu()
	{
		Console.WriteLine("MAIN MENU");
		Console.WriteLine("");
		Console.WriteLine("1. Start new game");
		Console.WriteLine("2. Load game from file");
		Console.WriteLine("3. Compress game file");
		Console.WriteLine("9. Quit");
		Console.WriteLine();
	}

	private static int GetMainMenuChoice()
	{
		int choice = 0;
		string input = "";
		bool validated = false;

		while(validated == false)
		// loop untill user has entered valid choice
		{
			Console.Write("Please enter your choice: ");
			input = Console.ReadLine();
		
			if(input == "1" || input == "2" || input == "3" || input == "9")
			// check if menu option exists
			{
				choice = int.Parse(input);
				validated = true;
				// user has entered valid input!
			}
			else
			{
				Console.WriteLine("That menu option does not exist!\n");
			}
		}

		Console.WriteLine();
		return choice;
	}

	private static void PlayGame(ref char[,] Board, ref ShipType[] Ships)
	{
		bool GameWon = false;
		while (GameWon == false)
		{
			PrintBoard(Board);
			MakePlayerMove(ref Board, ref Ships);
			GameWon = CheckWin(Board);
			if (GameWon == true)
			{
				Console.WriteLine("All ships sunk!");
				Console.WriteLine();
			}
		}
	}

	private static void SetUpShips(ref ShipType[] Ships)
	{
		Ships[0].Name = "Aircraft Carrier";
		Ships[0].Size = 5;
		Ships[0].HitCount = 0;
		Ships[1].Name = "Battleship";
		Ships[1].Size = 4;
		Ships[0].HitCount = 0;
		Ships[2].Name = "Submarine";
		Ships[2].Size = 3;
		Ships[0].HitCount = 0;
		Ships[3].Name = "Destroyer";
		Ships[3].Size = 3;
		Ships[0].HitCount = 0;
		Ships[4].Name = "Patrol Boat";
		Ships[4].Size = 2;
		Ships[0].HitCount = 0;
	}

	static void Main(string[] args)
	{

		string TrainingGame = "";
		ShipType[] Ships = new ShipType[5];
		char[,] Board = new char[10, 10];
		int MenuOption = 0;

		while (MenuOption != 9)
		{
			SetUpBoard(ref Board);
			SetUpShips(ref Ships);
			DisplayMenu();
			MenuOption = GetMainMenuChoice();
			if (MenuOption == 1)
			{
				PlaceRandomShips(ref Board, Ships);
				PlaceRandomMines(ref Board, 3);
				PlayGame(ref Board, ref Ships);
			}
			if (MenuOption == 2)
			{
				LoadGame(GetFileName(), ref Board);
				PlayGame(ref Board, ref Ships);
			}
			if (MenuOption == 3)
			{
				Console.WriteLine("\n" + CompressFile(GetFileName()));
				Console.WriteLine();
			}
		}


	}
}