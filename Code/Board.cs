using Raylib_cs;
using ChessDotNet;
using System.Numerics;
using System.Collections.ObjectModel;

namespace Game;

class Board
{
	public ChessGame chessGame;
	public bool isPromoting = false;

	Dictionary<string, Texture2D> pieceTextures;
	Position? selectedPiecePosition;
	List<Position> possibleMoves;
	Position pendingPromotionStart;
	Position pendingPromotionEnd;
	Player promotionPlayer;
	bool isWhiteTurn = true;

	public Board()
	{
		chessGame = new ChessGame();
		pieceTextures = new Dictionary<string, Texture2D>();
		possibleMoves = new List<Position>();
		LoadTextures();
	}

	public void Update()
	{
		if (!isPromoting)
			HandleInput();
		else
			HandlePromotionMenu();
	}

	void HandleInput()
	{
		if (Raylib.IsMouseButtonPressed(MouseButton.Left))
		{
			Vector2 mousePos = Raylib.GetMousePosition();
			int x = (int)(mousePos.X / 75);
			int y = (int)(mousePos.Y / 75);

			if (x >= 0 && x < 8 && y >= 0 && y < 8)
			{
				Position clickedPos = new Position((ChessDotNet.File)x, (byte)(8 - y));
				Piece clickedPiece = chessGame.GetPieceAt(clickedPos);

				if (selectedPiecePosition == null)
				{
					if (clickedPiece != null &&
						(isWhiteTurn && clickedPiece.Owner == Player.White ||
						 !isWhiteTurn && clickedPiece.Owner == Player.Black))
					{
						selectedPiecePosition = clickedPos;
						ShowPossibleMoves(clickedPos);
					}
				}
				else
				{
					if (possibleMoves.Contains(clickedPos))
						MovePiece(clickedPos);
					else if (clickedPiece != null &&
							 (isWhiteTurn && clickedPiece.Owner == Player.White ||
							  !isWhiteTurn && clickedPiece.Owner == Player.Black))
					{
						selectedPiecePosition = clickedPos;
						ShowPossibleMoves(clickedPos);
					}
					else
					{
						selectedPiecePosition = null;
						possibleMoves.Clear();
					}
				}
			}
		}
	}

	void ShowPossibleMoves(Position piecePosition)
	{
		Piece piece = chessGame.GetPieceAt(piecePosition);
		possibleMoves.Clear();

		if (piece != null)
		{
			ReadOnlyCollection<Move> moves = chessGame.GetValidMoves(piecePosition);
			foreach (Move move in moves)
			{
				possibleMoves.Add(move.NewPosition);
			}
		}
	}

	public bool IsWhiteTurn()
	{
		return isWhiteTurn;
	}

	public void FlipTurn()
	{
		isWhiteTurn = !isWhiteTurn;
	}


	void MovePiece(Position targetPosition)
	{
		if (selectedPiecePosition == null)
			return;

		Piece movingPiece = chessGame.GetPieceAt(selectedPiecePosition);
		Player currentPlayer = isWhiteTurn ? Player.White : Player.Black;

		if (movingPiece != null)
		{
			char fenChar = movingPiece.GetFenCharacter();
			bool isPawn = fenChar == 'P' || fenChar == 'p';
			bool isPromotionRank = (currentPlayer == Player.White && targetPosition.Rank == 8) ||
								   (currentPlayer == Player.Black && targetPosition.Rank == 1);

			if (isPawn && isPromotionRank)
			{
				isPromoting = true;
				pendingPromotionStart = selectedPiecePosition;
				pendingPromotionEnd = targetPosition;
				promotionPlayer = currentPlayer;

				selectedPiecePosition = null;
				possibleMoves.Clear();
				return;
			}
			else
			{
				Move move = new Move(selectedPiecePosition, targetPosition, currentPlayer);
				if (chessGame.MakeMove(move, true) != MoveType.Invalid)
					isWhiteTurn = !isWhiteTurn;
			}
		}

		selectedPiecePosition = null;
		possibleMoves.Clear();
	}

	void HandlePromotionMenu()
	{
		int squareSize = 75;
		int baseX = 600 / 2 - 2 * squareSize;
		int baseY = 600 / 2 - squareSize / 2;
		string[] choices = { "Q", "R", "B", "N" };

		for (int i = 0; i < choices.Length; i++)
		{
			Rectangle rect = new Rectangle(baseX + i * squareSize, baseY, squareSize, squareSize);
			if (Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), rect) &&
				Raylib.IsMouseButtonPressed(MouseButton.Left))
			{
				char promotionChar = choices[i][0];
				Move move = new Move(pendingPromotionStart, pendingPromotionEnd, promotionPlayer, promotionChar);

				if (chessGame.MakeMove(move, true) != MoveType.Invalid)
					isWhiteTurn = !isWhiteTurn;

				isPromoting = false;
				return;
			}
		}
	}

	void LoadTextures()
	{
		pieceTextures["K"] = Raylib.LoadTexture("./Textures/King_White.png");
		pieceTextures["Q"] = Raylib.LoadTexture("./Textures/Queen_White.png");
		pieceTextures["R"] = Raylib.LoadTexture("./Textures/Rook_White.png");
		pieceTextures["B"] = Raylib.LoadTexture("./Textures/Bishop_White.png");
		pieceTextures["N"] = Raylib.LoadTexture("./Textures/Knight_White.png");
		pieceTextures["P"] = Raylib.LoadTexture("./Textures/Pawn_White.png");

		pieceTextures["k"] = Raylib.LoadTexture("./Textures/King_Black.png");
		pieceTextures["q"] = Raylib.LoadTexture("./Textures/Queen_Black.png");
		pieceTextures["r"] = Raylib.LoadTexture("./Textures/Rook_Black.png");
		pieceTextures["b"] = Raylib.LoadTexture("./Textures/Bishop_Black.png");
		pieceTextures["n"] = Raylib.LoadTexture("./Textures/Knight_Black.png");
		pieceTextures["p"] = Raylib.LoadTexture("./Textures/Pawn_Black.png");
	}

	void DrawBoard()
	{
		for (int y = 0; y < 8; y++)
		{
			for (int x = 0; x < 8; x++)
			{
				Color color = ((x + y) % 2 == 0) ? new Color(230, 228, 193, 255) : new Color(75, 115, 153, 255);
				Raylib.DrawRectangle(x * 75, y * 75, 75, 75, color);
			}
		}
	}

	void DrawPieces()
	{
		for (int y = 0; y < 8; y++)
		{
			for (int x = 0; x < 8; x++)
			{
				Position pos = new Position((ChessDotNet.File)x, (byte)(8 - y));
				Piece piece = chessGame.GetPieceAt(pos);

				if (piece != null)
				{
					string pieceKey = piece.GetFenCharacter().ToString();
					if (pieceTextures.ContainsKey(pieceKey))
					{
						Texture2D texture = pieceTextures[pieceKey];
						Raylib.DrawTexture(texture, x * 75 + 10, y * 75 + 10, Color.White);
					}
				}
			}
		}
	}

	void DrawPossibleMoves()
	{
		foreach (Position move in possibleMoves)
		{
			int x = (int)move.File;
			int y = 8 - move.Rank;
			Raylib.DrawRectangle(x * 75, y * 75, 75, 75, new Color(242, 209, 41, 150));
		}
	}

	void DrawPromotionMenu()
	{
		int squareSize = 75;
		int baseX = 600 / 2 - 2 * squareSize;
		int baseY = 600 / 2 - squareSize / 2;
		string[] choices = { "Q", "R", "B", "N" };

		for (int i = 0; i < choices.Length; i++)
		{
			Rectangle rect = new Rectangle(baseX + i * squareSize, baseY, squareSize, squareSize);
			Raylib.DrawRectangleRec(rect, Color.Gray);

			string key = (promotionPlayer == Player.White) ? choices[i] : choices[i].ToLower();
			if (pieceTextures.ContainsKey(key))
			{
				Texture2D texture = pieceTextures[key];
				Raylib.DrawTexture(texture, (int)rect.X + 10, (int)rect.Y + 10, Color.White);
			}
		}
	}

	public void Draw()
	{
		DrawBoard();
		DrawPieces();
		DrawPossibleMoves();

		if (isPromoting)
			DrawPromotionMenu();
	}

	public void UnloadTextures()
	{
		foreach (Texture2D texture in pieceTextures.Values)
		{
			Raylib.UnloadTexture(texture);
		}
	}
}