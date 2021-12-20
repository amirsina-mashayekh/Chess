using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.ChessPieces
{
    internal class King : ChessPiece
    {
        public override int ValuePoints =>
            throw new NotSupportedException("King has no value as it cannot be captured.");

        /// <summary>
        /// Always returns false as king cannot be captured.
        /// </summary>
        public override bool IsCaptured => false;

        public override string FullName => "King";

        public override char Letter => 'K';

        /// <summary>
        /// Initializes a new instance of the <c>King</c> class.
        /// </summary>
        /// <param name="player">The player which the piece belongs to.</param>
        /// <param name="position">The position of piece.</param>
        public King(PiecePlayer player, ChessPosition position) : base(player, position) { }

        public override List<ChessPosition> GetAvailableMoves()
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
                    if (i == 0 && j == 0) { continue; }
                    try
                    {
                        moves.Add(new ChessPosition(c, r));
                    }
                    catch (ArgumentOutOfRangeException) { }
                }
            }

            return moves;
        }
    }
}
