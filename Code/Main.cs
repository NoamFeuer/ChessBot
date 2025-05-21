using Raylib_cs;
using Game.Bots;
using ChessDotNet;

namespace Game
{
	class GameClass
	{
		static void Main()
		{
			Raylib.InitWindow(600, 600, "Chess");

			Image icon = Raylib.LoadImage("./Textures/Knight_White.png");
			Raylib.SetWindowIcon(icon);

			Board board = new Board();
			MiniMaxBot bot = new MiniMaxBot();

			while (!Raylib.WindowShouldClose())
			{
				board.Update();

				if (!board.IsWhiteTurn() &&
					!board.chessGame.IsCheckmated(ChessDotNet.Player.Black) &&
					!board.chessGame.IsCheckmated(ChessDotNet.Player.White) &&
					!board.chessGame.IsDraw() &&
					!board.isPromoting)
				{
					Move bestMove = bot.Minimax(board.chessGame, 5, int.MinValue, int.MaxValue, false);

					if (bestMove != null)
						board.chessGame.MakeMove(bestMove, true);

					board.FlipTurn();
				}

				Raylib.BeginDrawing();
				Raylib.ClearBackground(Color.RayWhite);

				board.Draw();

				if (board.chessGame.IsCheckmated(ChessDotNet.Player.White))
					Raylib.DrawText("Black Won By Checkmate!", 10, 10, 40, Color.Black);
				else if (board.chessGame.IsCheckmated(ChessDotNet.Player.Black))
					Raylib.DrawText("White Won By Checkmate!", 10, 10, 40, Color.Black);
				else if (board.chessGame.IsDraw())
					Raylib.DrawText("Draw, No One Wins", 10, 10, 40, Color.Black);

				Raylib.EndDrawing();
			}

			board.UnloadTextures();
			Raylib.UnloadImage(icon);
			Raylib.CloseWindow();
		}
	}
}
