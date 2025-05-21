using System;

class Evaluation
{
	public int EvaluateBoard(string fen)
	{
		int evaluation = 0;

		evaluation += EvaluatePieceValues(fen);

		return evaluation;
	}

	int EvaluatePieceValues(string fen)
	{

		string[] parts = fen.Split(' ');
		if (parts.Length < 1)
			return 0;

		string boardLayout = parts[0];
		int evaluation = 0;

		foreach (char symbol in boardLayout)
		{
			if (char.IsDigit(symbol))
				continue;
			else if (symbol == '/')
				continue;
			else
			{
				int pieceValue = GetPieceValue(symbol.ToString());
				if (char.IsUpper(symbol))
					evaluation += pieceValue;
				else
					evaluation -= pieceValue;
			}
		}

		return evaluation;
	}

	int GetPieceValue(string piece)
	{
		switch (piece.ToLower())
		{
			case "p": return 10;
			case "n": return 30;
			case "b": return 32;
			case "r": return 50;
			case "q": return 90;
			case "k": return 900;
			default: return 0;
		}
	}

	
}