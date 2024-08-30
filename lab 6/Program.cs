using System;
using System.Text;

class Program
{
	static void Main(string[] args)
	{
		Console.OutputEncoding = Encoding.UTF8;
		GameController gameController = new GameController();

		while (true)
		{
			gameController.StartNewGame();
			Console.Write("0 - Закрыть, 1 - Перезапустить игру: ");
			if (Console.ReadLine() == "0")
			{
				break;
			}
		}
	}
}

class GameController
{
	private Board board;
	private bool[,] revealed;
	private bool gameOver;

	public void StartNewGame()
	{
		int width = 10;
		int height = 10;
		int mines = 10;

		board = BoardFactory.CreateBoard(width, height, mines);
		revealed = new bool[height, width];
		gameOver = false;

		while (!gameOver)
		{
			Console.Clear();
			PrintBoard();
			ProcessInput();
			CheckGameStatus();
		}
	}

	private void PrintBoard()
	{
		Console.Write("  ");
		for (int j = 0; j < board.Width; j++)
		{
			Console.Write(j + " ");
		}
		Console.WriteLine();

		for (int i = 0; i < board.Height; i++)
		{
			Console.Write(i + " ");
			for (int j = 0; j < board.Width; j++)
			{
				if (revealed[i, j])
				{
					Console.Write(board.GetCell(i, j) + " ");
				}
				else
				{
					Console.Write(". ");
				}
			}
			Console.WriteLine();
		}
	}

	private void ProcessInput()
	{
		Console.Write("Введите координаты (строка столбец): ");
		string[] input = Console.ReadLine().Split(' ');
		int row = int.Parse(input[0]);
		int col = int.Parse(input[1]);

		if (board.GetCell(row, col) == '*')
		{
			Console.WriteLine("Вы наткнулись на мину! Игра окончена.");
			revealed[row, col] = true;
			gameOver = true;
		}
		else
		{
			board.RevealCell(revealed, row, col);
		}
	}

	private void CheckGameStatus()
	{
		if (board.IsGameWon(revealed))
		{
			Console.WriteLine("Поздравляем! Вы победили!");
			gameOver = true;
		}
	}
}

class Board
{
	private char[,] cells;
	public int Width { get; }
	public int Height { get; }

	public Board(int width, int height, int mines)
	{
		Width = width;
		Height = height;
		cells = new char[height, width];
		InitializeBoard(mines);
	}

	public char GetCell(int row, int col)
	{
		return cells[row, col];
	}

	private void InitializeBoard(int mines)
	{
		for (int i = 0; i < Height; i++)
		{
			for (int j = 0; j < Width; j++)
			{
				cells[i, j] = ' ';
			}
		}

		Random rand = RandomSingleton.Instance;
		for (int i = 0; i < mines; i++)
		{
			int row, col;
			do
			{
				row = rand.Next(Height);
				col = rand.Next(Width);
			} while (cells[row, col] == '*');

			cells[row, col] = '*';
		}

		for (int i = 0; i < Height; i++)
		{
			for (int j = 0; j < Width; j++)
			{
				if (cells[i, j] != '*')
				{
					cells[i, j] = (char)('0' + CountAdjacentMines(i, j));
				}
			}
		}
	}

	private int CountAdjacentMines(int row, int col)
	{
		int count = 0;

		for (int i = -1; i <= 1; i++)
		{
			for (int j = -1; j <= 1; j++)
			{
				int newRow = row + i;
				int newCol = col + j;

				if (newRow >= 0 && newRow < Height && newCol >= 0 && newCol < Width && cells[newRow, newCol] == '*')
				{
					count++;
				}
			}
		}

		return count;
	}

	public void RevealCell(bool[,] revealed, int row, int col)
	{
		if (revealed[row, col])
		{
			return;
		}

		revealed[row, col] = true;

		if (cells[row, col] == '0')
		{
			for (int i = -1; i <= 1; i++)
			{
				for (int j = -1; j <= 1; j++)
				{
					int newRow = row + i;
					int newCol = col + j;

					if (newRow >= 0 && newRow < Height && newCol >= 0 && newCol < Width)
					{
						RevealCell(revealed, newRow, newCol);
					}
				}
			}
		}
	}

	public bool IsGameWon(bool[,] revealed)
	{
		int revealedCells = 0;

		for (int i = 0; i < Height; i++)
		{
			for (int j = 0; j < Width; j++)
			{
				if (revealed[i, j])
				{
					revealedCells++;
				}
			}
		}

		return revealedCells == Width * Height - CountMines();
	}

	private int CountMines()
	{
		int count = 0;

		for (int i = 0; i < Height; i++)
		{
			for (int j = 0; j < Width; j++)
			{
				if (cells[i, j] == '*')
				{
					count++;
				}
			}
		}

		return count;
	}
}

class BoardFactory
{
	public static Board CreateBoard(int width, int height, int mines)
	{
		return new Board(width, height, mines);
	}
}

class RandomSingleton
{
	private static Random instance;

	private RandomSingleton() { }

	public static Random Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new Random();
			}
			return instance;
		}
	}
}