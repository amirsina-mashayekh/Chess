using System;
using System.Collections.Generic;

namespace Chess.ChessUtil.ChessPieces
{
    /// <summary>
    /// Represents a bishop chess piece.
    /// </summary>
    public class Bishop : ChessPiece
    {
        public Bishop(ChessPlayer player, ChessPosition position) : base(player, position)
        {
        }

        public override List<ChessPosition> GetMoves()
        {
            int col = Position.Column;
            int row = Position.Row;
            List<ChessPosition> moves = new List<ChessPosition>();

            for (int i = ChessPosition.MinRow; i <= ChessPosition.MaxRow; i++)
            {
                if (i == row)
                {
                    continue;
                }

                int rowDiff = Math.Abs(i - row);
                if (col - rowDiff >= ChessPosition.MinColumn)
                {
                    moves.Add(new ChessPosition(col - rowDiff, i));
                }
                if (col + rowDiff <= ChessPosition.MaxColumn)
                {
                    moves.Add(new ChessPosition(col + rowDiff, i));
                }
            }

            return moves;
        }
    }
}