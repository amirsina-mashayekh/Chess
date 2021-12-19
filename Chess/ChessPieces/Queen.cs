using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.ChessPieces
{
    internal class Queen : ChessPiece
    {
        public override int ValuePoints => 9;

        public override string FullName => "Queen";

        public override char Letter => 'Q';

        public Queen(PiecePlayer player, ChessPosition position) : base(player, position) { }

        public override string ToString()
        {
            return "Queen, " + base.ToString();
        }

        public override List<ChessPosition> GetAvailableMoves()
        {
            int col = Position.File;
            int row = Position.Rank;
            List<ChessPosition> moves = new List<ChessPosition>();

            for (int i = ChessPosition.MinFile; i <= ChessPosition.MaxFile; i++)
            {
                if (i == col) { continue; }
                moves.Add(new ChessPosition(i, row));
            }

            for (int i = ChessPosition.MinRank; i <= ChessPosition.MaxRank; i++)
            {
                if (i == row) { continue; }
                moves.Add(new ChessPosition(col, i));
                int rowDiff = Math.Abs(i - row);
                if (col - rowDiff >= ChessPosition.MinFile)
                {
                    moves.Add(new ChessPosition(col - rowDiff, i));
                }
                if (col + rowDiff <= ChessPosition.MaxFile)
                {
                    moves.Add(new ChessPosition(col + rowDiff, i));
                }
            }

            return moves;
        }
    }
}
