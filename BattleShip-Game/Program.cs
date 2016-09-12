/*
 * 
 *
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
		
	const int BoardSize = 10;

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

		using (StreamWriter f = new StreamWriter(fileName + ".txt"))
		{
			for(int row = 0; row < BoardSize; row ++)
			{
				for(int col = 0; col < BoardSize; col ++)
				{
					f.Write(Board[row, col]);
				}
				f.Write("\n");
			}
		}

	}

	private static void GetRowColumn(ref int Row, ref int Column)
	{

		string Input = "";
		char RowChar = ' ';

		Console.Write("Enter Co-ordinates: ");
		Input = Console.ReadLine();

		while(	Input.Length != 2 || 
				Input[0] - 97 > 9 || 
				Input[0] - 97 < 0 ||
				Input[1] - 48 > 9 ||
				Input[1] - 48 < 0
			)
		{
			Console.WriteLine("Invalid co-ordinate. e.g: A1 or E5");
			Console.Write("Enter Co-ordinates: ");
			Input = Console.ReadLine();
		}

		Input = Input.ToLower();

		RowChar = Input[0];

		Row = (int)RowChar - 97;
		Column = Convert.ToInt32(Input[1]) - 48;
		Console.WriteLine(Column);
	
	}

	private static void MakePlayerMove(ref char[,] Board, ref char[,] OriginalBoard, ref ShipType[] Ships)
	{
		int Row = 0;
		int Column = 0;
		bool sunkShip = false;

		GetRowColumn(ref Row, ref Column);
		if (Board[Row, Column] == 'm' || Board[Row, Column] == 'h' || Board[Row, Column] == 'x')
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
								Board[SearchRow, SearchCol] != 'm' && 
								Board[SearchRow, SearchCol] != 'h' && 
								Board[SearchRow, SearchCol] != '-' && 
								Board[SearchRow, SearchCol] != '*'
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

			switch(Board[Row, Column])
			{
			case 'A': 
				
				Ships[0].HitCount++;
				if(Ships[0].Size == Ships[0].HitCount)
				{
					Console.WriteLine("You sunk an Aircraft carrier!");

					for(int row = 0; row < BoardSize; row++)
					{
						for(int col = 0; col < BoardSize; col++)
						{
							if(OriginalBoard[row, col] == 'A')
							{
								Board[row, col] = 'x';
							}
						}
					}

					sunkShip = true;
				}
				break;
			case 'B':
				Ships[1].HitCount++; 

				if(Ships[1].Size == Ships[1].HitCount)
				{
					Console.WriteLine("You sunk a Battleship!");

					for(int row = 0; row < BoardSize; row++)
					{
						for(int col = 0; col < BoardSize; col++)
						{
							if(OriginalBoard[row, col] == 'B')
							{
								Board[row, col] = 'x';
							}
						}
					}

					sunkShip = true;
				}


				break;
			case 'S': 
				Ships[2].HitCount++; 
				if(Ships[2].Size == Ships[2].HitCount)
				{
					Console.WriteLine("You sunk a Submarine!");

					for(int row = 0; row < BoardSize; row++)
					{
						for(int col = 0; col < BoardSize; col++)
						{
							if(OriginalBoard[row, col] == 'S')
							{
								Board[row, col] = 'x';
							}
						}
					}

					sunkShip = true;
				}
				break;
			case 'D':
				Ships[3].HitCount++; 

				if(Ships[3].Size == Ships[3].HitCount)
				{
					Console.WriteLine("You sunk a Destroyer!");

					for(int row = 0; row < BoardSize; row++)
					{
						for(int col = 0; col < BoardSize; col++)
						{
							if(OriginalBoard[row, col] == 'D')
							{
								Board[row, col] = 'x';
							}
						}
					}

					sunkShip = true;
				}
				break;

			case 'P': 
				Ships[4].HitCount++; 
				if(Ships[4].Size == Ships[4].HitCount)
				{
					Console.WriteLine("You sunk a Patrol boat!");

					for(int row = 0; row < BoardSize; row++)
					{
						for(int col = 0; col < BoardSize; col++)
						{
							if(OriginalBoard[row, col] == 'P')
							{
								Board[row, col] = 'x';
							}
						}
					}

					sunkShip = true;

				}
				break;
			}

			if(sunkShip == false)
			{
				Console.WriteLine("Hit at (" + Column + "," + Row + ").");
				Board[Row, Column] = 'h';
			}
		}
	}

	private static void SetUpBoard(ref char[,] Board)
	{
		for (int Row = 0; Row < BoardSize; Row++)
		{
			for (int Column = 0; Column < BoardSize; Column++)
			{
				Board[Row, Column] = '-';
			}
		}
	}

	private static void LoadGame(string TrainingGame, ref char[,] Board)
	{
		string Line = "";
		StreamReader BoardFile = new StreamReader(TrainingGame);
		for (int Row = 0; Row < BoardSize; Row++)
		{
			Line = BoardFile.ReadLine();
			for (int Column = 0; Column < BoardSize; Column++)
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
				Row = RandomNumber.Next(0, BoardSize);
				Column = RandomNumber.Next(0, BoardSize);
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
				Row = RandomNumber.Next(0, BoardSize);
				Column = RandomNumber.Next(0, BoardSize);

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
		if (Orientation == 'v' && Row + Ship.Size > BoardSize)
		{
			return false;
		}
		else if (Orientation == 'h' && Column + Ship.Size > BoardSize)
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
		for (int Row = 0; Row < BoardSize; Row++)
		{
			for (int Column = 0; Column < BoardSize; Column++)
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
		for (int Column = 0; Column < BoardSize; Column++)
		{
			Console.Write("  " + Column + " ");
		}
		Console.WriteLine();

		Console.ForegroundColor = ConsoleColor.DarkBlue;
		Console.WriteLine("  +---+---+---+---+---+---+---+---+---+---+");
		Console.ForegroundColor = ConsoleColor.Black;

		for (int Row = 0; Row < BoardSize; Row++)
		{
			RowLetter = Convert.ToString((char)(Row + 97)).ToUpper();
			// convert integer to ascii, then to letter
		

			Console.Write(RowLetter);

			Console.ForegroundColor = ConsoleColor.DarkBlue;
			Console.Write(" | ");
			Console.ForegroundColor = ConsoleColor.Black;


			for (int Column = 0; Column < BoardSize; Column++)
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
					case 'x': Console.ForegroundColor = ConsoleColor.DarkGreen; break;
					case '*': Console.ForegroundColor = ConsoleColor.DarkYellow; break;
					}

					Console.Write(Board[Row, Column]);

				}

				Console.ForegroundColor = ConsoleColor.DarkBlue;
				Console.Write(" | ");
				Console.ForegroundColor = ConsoleColor.Black;
				
			}

			Console.WriteLine("");
			Console.ForegroundColor = ConsoleColor.DarkBlue;
			Console.WriteLine("  +---+---+---+---+---+---+---+---+---+---+");
			Console.ForegroundColor = ConsoleColor.Black;
		}
	} 

	private static void DisplayMenu()
	{
		Console.WriteLine("BATTLESHIPS MAIN MENU");
		Console.WriteLine("");
		Console.WriteLine("1. Play regular Battleships");
		Console.WriteLine("2. Play Salvo!");
		Console.WriteLine("3. Load game from file");
		Console.WriteLine("4. Compress game file");
		Console.WriteLine("9. Quit");
		Console.WriteLine();
	}

	private static void PlayGame(ref char[,] Board, ref ShipType[] Ships)
	{
		char[,] OriginalBoard = new char[BoardSize, BoardSize];

		for(int row = 0; row < BoardSize; row ++)
		{
			for(int col = 0; col < BoardSize; col++)
			{
				OriginalBoard[row, col] = Board[row, col];
			}
		}
		// using "OriginalBoard = Board" seems to mirror the two arrays!
		// so just copied them using a couple of for loops

		bool GameWon = false;

		while (GameWon == false)
		{
			PrintBoard(Board);
			MakePlayerMove(ref Board, ref OriginalBoard, ref Ships);
			GameWon = CheckWin(Board);
			if (GameWon == true)
			{
				Console.WriteLine("All ships sunk!");
				Console.WriteLine();
			}
		}
	}


	private static void PlaySalvo(ref char[,] Board, ref ShipType[] Ships)
	{
		bool GameWon = false;
		bool MadeHit = true;


		while (MadeHit == true)
		{
			MadeHit = false;
			PrintBoard(Board);
			MakeSalvoMove(ref Board, ref Ships, ref MadeHit);
		}

		GameWon = CheckWin(Board);

		if (GameWon == true)
		{
			PrintBoard(Board);

			Console.WriteLine("All ships sunk!");
			Console.WriteLine();
		}
		else
		{
			PrintBoard(Board);

			Console.WriteLine("Sorry, you loose!");
			Console.WriteLine();
		}
	}

	private static void MakeSalvoMove(ref char[,] Board, ref ShipType[] Ships, ref bool MadeHit)
	{
		int Row = 0;
		int Column = 0;

		for(int i = 0; i < 5; i++)
		{

			GetRowColumn(ref Row, ref Column);

			if (Board[Row, Column] == 'm' || Board[Row, Column] == 'h')
			{
				Console.WriteLine("Sorry, you have already shot at the square (" + Column + "," + Row + "). Please try again.");
				i--;
			}
			else if (Board[Row, Column] == '-')
			{
				Board[Row, Column] = 'm';
			}
			else
			{
				Board[Row, Column] = 'h';
				MadeHit = true;
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
		Ships[1].HitCount = 0;

		Ships[2].Name = "Submarine";
		Ships[2].Size = 3;
		Ships[2].HitCount = 0;

		Ships[3].Name = "Destroyer";
		Ships[3].Size = 3;
		Ships[3].HitCount = 0;

		Ships[4].Name = "Patrol Boat";
		Ships[4].Size = 2;
		Ships[4].HitCount = 0;
	}

	static void Main(string[] args)
	{

		string TrainingGame = "";
	

		ShipType[] Ships = new ShipType[5];
	
		char[,] Board = new char[BoardSize, BoardSize];
		string MenuOption = "0";

		while (MenuOption != "9")
		{
			SetUpBoard(ref Board);
			SetUpShips(ref Ships);
			DisplayMenu();



			Console.Write("Enter menu option: ");
			MenuOption = Console.ReadLine();

			if (MenuOption == "1")
			// play regular
			{
				PlaceRandomShips(ref Board, Ships);
				PlaceRandomMines(ref Board, 3);
				PlayGame(ref Board, ref Ships);
			}
			else if (MenuOption == "2")
			// play Salvo!
			{
				PlaceRandomShips(ref Board, Ships);
				PlaySalvo(ref Board, ref Ships);
			}
			else if (MenuOption == "3")
			{
			// load game
				LoadGame(GetFileName(), ref Board);
				PlayGame(ref Board, ref Ships);
			}
			else if (MenuOption == "4")
			// compress file
			{
				Console.WriteLine("\n" + CompressFile(GetFileName()));
				Console.WriteLine();
			}
			else
			{
				Console.WriteLine("Invalid menu option.");
			}
		}


	}
}