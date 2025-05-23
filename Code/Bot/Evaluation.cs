using System;
using ChessDotNet;

public static class Evaluator
{
	static readonly int PawnValue = 100;
	static readonly int KnightValue = 320;
	static readonly int BishopValue = 330;
	static readonly int RookValue = 500;
	static readonly int QueenValue = 900;
	static readonly int KingValue = 20000;

	static readonly int[] PawnPST = new int[64]
	{
		0, 0, 0, 0, 0, 0, 0, 0,
		5, 10, 10, -20, -20, 10, 10, 5,
		5, -5, -10, 0, 0, -10, -5, 5,
		0, 0, 0, 20, 20, 0, 0, 0,
		5, 5, 10, 25, 25, 10, 5, 5,
		10, 10, 20, 30, 30, 20, 10, 10,
		50, 50, 50, 50, 50, 50, 50, 50,
		0, 0, 0, 0, 0, 0, 0, 0
	};

	static readonly int[] KnightPST = new int[64]
	{
		-50,-40,-30,-30,-30,-30,-40,-50,
		-40,-20,  0,  0,  0,  0,-20,-40,
		-30,  0, 10, 15, 15, 10,  0,-30,
		-30,  5, 15, 20, 20, 15,  5,-30,
		-30,  0, 15, 20, 20, 15,  0,-30,
		-30,  5, 10, 15, 15, 10,  5,-30,
		-40,-20,  0,  5,  5,  0,-20,-40,
		-50,-40,-30,-30,-30,-30,-40,-50
	};

	static readonly int[] BishopPST = new int[64]
	{
		-20,-10,-10,-10,-10,-10,-10,-20,
		-10,  0,  0,  0,  0,  0,  0,-10,
		-10,  0,  5, 10, 10,  5,  0,-10,
		-10,  5,  5, 10, 10,  5,  5,-10,
		-10,  0, 10, 10, 10, 10,  0,-10,
		-10, 10, 10, 10, 10, 10, 10,-10,
		-10,  5,  0,  0,  0,  0,  5,-10,
		-20,-10,-10,-10,-10,-10,-10,-20
	};

	static readonly int[] RookPST = new int[64]
	{
		0, 0, 0, 0, 0, 0, 0, 0,
		5, 10, 10, 10, 10, 10, 10, 5,
		-5, 0, 0, 0, 0, 0, 0, -5,
		-5, 0, 0, 0, 0, 0, 0, -5,
		-5, 0, 0, 0, 0, 0, 0, -5,
		-5, 0, 0, 0, 0, 0, 0, -5,
		-5, 0, 0, 0, 0, 0, 0, -5,
		0, 0, 0, 5, 5, 0, 0, 0
	};

	static readonly int[] QueenPST = new int[64]
	{
		-20,-10,-10, -5, -5,-10,-10,-20,
		-10,  0,  0,  0,  0,  0,  0,-10,
		-10,  0,  5,  5,  5,  5,  0,-10,
		-5,  0,  5,  5,  5,  5,  0, -5,
		0,  0,  5,  5,  5,  5,  0, -5,
		-10,  5,  5,  5,  5,  5,  0,-10,
		-10,  0,  5,  0,  0,  0,  0,-10,
		-20,-10,-10, -5, -5,-10,-10,-20
	};

	static readonly int[] KingPST = new int[64]
	{
		-30,-40,-40,-50,-50,-40,-40,-30,
		-30,-40,-40,-50,-50,-40,-40,-30,
		-30,-40,-40,-50,-50,-40,-40,-30,
		-30,-40,-40,-50,-50,-40,-40,-30,
		-20,-30,-30,-40,-40,-30,-30,-20,
		-10,-20,-20,-20,-20,-20,-20,-10,
		20, 20,  0,  0,  0,  0, 20, 20,
		20, 30, 10,  0,  0, 10, 30, 20
	};

	public static int Evaluate(string fen)
	{
		ChessGame game = new ChessGame(fen);
		int score = 0;

		for (int rank = 1; rank <= 8; rank++)
		{
			for (char file = 'a'; file <= 'h'; file++)
			{
				Position pos = new Position((ChessDotNet.File)(file - 'a'), (byte)rank);
				Piece piece = game.GetPieceAt(pos);
				if (piece != null)
				{
					int index = SquareToIndex(file, rank);
					int value = GetPieceValue(piece.GetFenCharacter());
					int pst = GetPSTValue(piece, index);

					score += piece.Owner == Player.White ? value + pst : -(value + pst);
				}
			}
		}

		return score;
	}

	static int SquareToIndex(char file, int rank)
	{
		int fileIndex = file - 'a';
		int rankIndex = 8 - rank;
		return rankIndex * 8 + fileIndex;
	}

	static int GetPieceValue(char fenChar)
	{
		switch (char.ToLower(fenChar))
		{
			case 'p': return PawnValue;
			case 'n': return KnightValue;
			case 'b': return BishopValue;
			case 'r': return RookValue;
			case 'q': return QueenValue;
			case 'k': return KingValue;
			default: return 0;
		}
	}

	static int GetPSTValue(Piece piece, int index)
	{
		int mirrored = 63 - index;

		switch (char.ToLower(piece.GetFenCharacter()))
		{
			case 'p': return piece.Owner == Player.White ? PawnPST[index] : PawnPST[mirrored];
			case 'n': return piece.Owner == Player.White ? KnightPST[index] : KnightPST[mirrored];
			case 'b': return piece.Owner == Player.White ? BishopPST[index] : BishopPST[mirrored];
			case 'r': return piece.Owner == Player.White ? RookPST[index] : RookPST[mirrored];
			case 'q': return piece.Owner == Player.White ? QueenPST[index] : QueenPST[mirrored];
			case 'k': return piece.Owner == Player.White ? KingPST[index] : KingPST[mirrored];
			default: return 0;
		}
	}
}
