using ChessDotNet;
using System.Collections.ObjectModel;

namespace Game.Bots
{
	class MiniMaxBot
	{
		public Move? Minimax(ChessGame game, int depth, int alpha, int beta, bool maximizingPlayer)
		{
			if (depth == 0 || game.IsCheckmated(Player.White) || game.IsCheckmated(Player.Black) || game.IsDraw())
				return null;

			ReadOnlyCollection<Move> moves = game.GetValidMoves(game.WhoseTurn);
			Move? bestMove = null;

			if (maximizingPlayer)
			{
				int maxEval = int.MinValue;

				foreach (Move move in moves)
				{
					ChessGame clone = new ChessGame(game.GetFen());
					clone.MakeMove(move, true);

					Move? tempBestMove = Minimax(clone, depth - 1, alpha, beta, false);

					int eval = Evaluator.Evaluate(clone.GetFen());
					if (eval > maxEval)
					{
						maxEval = eval;
						bestMove = move;
					}

					alpha = Math.Max(alpha, eval);
					if (beta <= alpha)
						break;
				}

				return bestMove;
			}
			else
			{
				int minEval = int.MaxValue;

				foreach (Move move in moves)
				{
					ChessGame clone = new ChessGame(game.GetFen());
					clone.MakeMove(move, true);

					Move? tempBestMove = Minimax(clone, depth - 1, alpha, beta, true);

					int eval = Evaluator.Evaluate(clone.GetFen());
					if (eval < minEval)
					{
						minEval = eval;
						bestMove = move;
					}

					beta = Math.Min(beta, eval);
					if (beta <= alpha)
						break;
				}

				return bestMove;
			}
		}
	}
}
