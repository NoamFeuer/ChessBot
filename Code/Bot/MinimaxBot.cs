using ChessDotNet;
using System.Collections.ObjectModel;

namespace Game.Bots
{
	class MiniMaxBot
	{
		Evaluation evaluation = new Evaluation();

		// Minimax function that returns the best move
		public Move? Minimax(ChessGame game, int depth, int alpha, int beta, bool maximizingPlayer)
		{
			// Base case: check for game over or depth limit
			if (depth == 0 || game.IsCheckmated(Player.White) || game.IsCheckmated(Player.Black) || game.IsDraw())
			{
				return null;  // No move to return, only evaluating at leaf nodes
			}

			ReadOnlyCollection<Move> moves = game.GetValidMoves(game.WhoseTurn);
			Move? bestMove = null;

			if (maximizingPlayer)
			{
				int maxEval = int.MinValue;

				foreach (Move move in moves)
				{
					// Clone the game state and apply the move
					ChessGame clone = new ChessGame(game.GetFen());
					clone.MakeMove(move, true);

					// Recursively get the best move
					Move? tempBestMove = Minimax(clone, depth - 1, alpha, beta, false);

					// If the current move results in a better evaluation, update the best move
					int eval = evaluation.EvaluateBoard(clone.GetFen());
					if (eval > maxEval)
					{
						maxEval = eval;
						bestMove = move;
					}

					// Alpha-Beta pruning
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
					// Clone the game state and apply the move
					ChessGame clone = new ChessGame(game.GetFen());
					clone.MakeMove(move, true);

					// Recursively get the best move
					Move? tempBestMove = Minimax(clone, depth - 1, alpha, beta, true);

					// If the current move results in a worse evaluation, update the best move
					int eval = evaluation.EvaluateBoard(clone.GetFen());
					if (eval < minEval)
					{
						minEval = eval;
						bestMove = move;
					}

					// Alpha-Beta pruning
					beta = Math.Min(beta, eval);
					if (beta <= alpha)
						break;
				}

				return bestMove;
			}
		}
	}
}
