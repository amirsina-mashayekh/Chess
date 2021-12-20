using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.ChessPieces
{
    internal class Bishop : ChessPiece
    {
        public override int ValuePoints => 3;

        public override string FullName => "Bishop";

        public override char Letter => 'B';

        public Bishop(PiecePlayer player, ChessPosition position) : base(player, position) { }

        public override List<ChessPosition> GetAvailableMoves()
        {
            int col = Position.Column;
            int row = Position.Row;
            List<ChessPosition> moves = new List<ChessPosition>();

            for (int i = ChessPosition.MinRow; i <= ChessPosition.MaxRow; i++)
            {
                if (i == row) { continue; }
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
