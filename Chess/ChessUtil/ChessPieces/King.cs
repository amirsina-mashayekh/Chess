using System;
using System.Collections.Generic;

namespace Chess.ChessUtil.ChessPieces
{
    /// <summary>
    /// Represents a king chess piece.
    /// </summary>
    public class King : ChessPiece
    {
        /// <summary>
        /// Always returns false as king cannot be captured.
        /// </summary>
        public override bool IsCaptured => false;

        /// <summary>
        /// Initializes a new instance of the <c>King</c> class.
        /// </summary>
        /// <param name="player">The player which the piece belongs to.</param>
        /// <param name="position">The position of piece.</param>
        public King(ChessPlayer player, ChessPosition position) : base(player, position) { }

        public override List<ChessPosition> GetMoves()
        {
            int col = Position.Column;
            int row = Position.Row;
            List<ChessPosition> moves = new List<ChessPosition>();

            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    int c = col + j;
                    int r = row + i;
                    if (i == 0 && j == 0) continue;
                    if (ChessPosition.ColumnIsValid(c) && ChessPosition.RowIsValid(r))
                    {
                        moves.Add(new ChessPosition(c, r));
                    }
                }
            }

            return moves;
        }
    }
}