using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.ChessPieces
{
    public class Knight : ChessPiece
    {
        public override int ValuePoints => 3;

        public override string FullName => "Knight";

        public override char Letter => 'N';

        public Knight(ChessPlayer player, ChessPosition position) : base(player, position) { }

        private static readonly int[] rowMoves = new int[] { 1, 2, 2, 1, -1, -2, -2, -1 };

        private static readonly int[] colMoves = new int[] { -2, -1, 1, 2, 2, 1, -1, -2 };

        public override List<ChessPosition> GetMoves()
        {
            int col = Position.Column;
            int row = Position.Row;
            List<ChessPosition> moves = new List<ChessPosition>();

            for (int i = 0; i < 8; i++)
            {
                int c = col + colMoves[i];
                int r = row + rowMoves[i];
                if (ChessPosition.ColumnIsValid(c) && ChessPosition.RowIsValid(r))
                {
                    moves.Add(new ChessPosition(c, r));
                }
            }

            return moves;
        }
    }
}
