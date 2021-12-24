using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.ChessPieces
{
    /// <summary>
    /// Represents a rook chess piece.
    /// </summary>
    public class Rook : ChessPiece
    {
        public override int ValuePoints => 5;

        public override string FullName => "Rook";

        public override char Letter => 'R';

        public Rook(ChessPlayer player, ChessPosition position) : base(player, position) { }

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
            }

            return moves;
        }
    }
}
