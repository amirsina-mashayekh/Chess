using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.ChessPieces
{
    /// <summary>
    /// Represents a queen chess piece.
    /// </summary>
    public class Queen : ChessPiece
    {
        public override int ValuePoints => 9;

        public override string FullName => "Queen";

        public override char Letter => 'Q';

        public Queen(ChessPlayer player, ChessPosition position) : base(player, position) { }

        public override List<ChessPosition> GetMoves()
        {
            int col = Position.Column;
            int row = Position.Row;
            List<ChessPosition> moves = new List<ChessPosition>();

            for (int i = ChessPosition.MinColumn; i <= ChessPosition.MaxColumn; i++)
            {
                if (i == col) { continue; }
                moves.Add(new ChessPosition(i, row));
            }

            for (int i = ChessPosition.MinRow; i <= ChessPosition.MaxRow; i++)
            {
                if (i == row) { continue; }
                moves.Add(new ChessPosition(col, i));
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
