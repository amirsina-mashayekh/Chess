﻿using System.Collections.Generic;

namespace Chess.ChessUtil.ChessPieces
{
    /// <summary>
    /// Represents a pawn chess piece.
    /// </summary>
    public class Pawn : ChessPiece
    {
        public Pawn(ChessPlayer player, ChessPosition position) : base(player, position)
        {
        }

        public override List<ChessPosition> GetMoves()
        {
            int col = Position.Column;
            int row = Position.Row;
            int dir = Player == ChessPlayer.White ? 1 : -1;
            List<ChessPosition> moves = new List<ChessPosition>();

            if (row != (dir == 1 ? ChessPosition.MaxRow : ChessPosition.MinRow))
            {
                moves.Add(new ChessPosition(col, row + dir * 1));

                if (!Position.IsMoved)
                {
                    moves.Add(new ChessPosition(col, row + dir * 2));
                }
                if (col + 1 <= ChessPosition.MaxColumn)
                {
                    moves.Add(new ChessPosition(col + 1, row + dir * 1));
                }
                if (col - 1 >= ChessPosition.MinColumn)
                {
                    moves.Add(new ChessPosition(col - 1, row + dir * 1));
                }
            }

            return moves;
        }

        /// <summary>
        /// Indicates whether the pawn should be promoted.
        /// </summary>
        /// <returns><c>true</c> if pawn is at the opponet end of board. <c>false</c> otherwise.</returns>
        public bool ShouldPromote()
        {
            return (Player == ChessPlayer.White && Position.Row == ChessPosition.MaxRow)
                || (Player == ChessPlayer.Black && Position.Row == ChessPosition.MinRow);
        }
    }
}