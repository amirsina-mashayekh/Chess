using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.ChessPieces
{
    internal class Rook : ChessPiece
    {
        /// <summary>
        /// Gets or sets whether the piece is moved.
        /// </summary>
        public bool IsMoved { get; set; }

        private ChessPosition _position;

        public override ChessPosition Position
        {
            get { return _position; }
            set
            {
                if (_position != value)
                {
                    _position = value;
                    IsMoved = true;
                }
            }
        }

        public override int ValuePoints => 5;

        public override string FullName => "Rook";

        public override char Letter => 'R';

        public Rook(PiecePlayer player, ChessPosition position) : base(player, position)
        {
            IsMoved = false;
        }

        public override string ToString()
        {
            return "Rook, " + base.ToString();
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
            }

            return moves;
        }
    }
}
