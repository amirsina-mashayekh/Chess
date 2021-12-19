using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.ChessPieces
{
    internal class Pawn : ChessPiece
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
                if (Player == PiecePlayer.White && value.Rank == ChessPosition.MinRank
                    || Player == PiecePlayer.Black && value.Rank == ChessPosition.MaxRank)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }

                if (_position != value)
                {
                    _position = value;
                    IsMoved = true;
                }
            }
        }

        public override int ValuePoints => 1;

        public override string FullName => "Pawn";

        public override char Letter => 'P';

        public Pawn(PiecePlayer player, ChessPosition position) : base(player, position)
        {
            IsMoved = false;
        }

        public override List<ChessPosition> GetAvailableMoves()
        {
            int col = Position.File;
            int row = Position.Rank;
            int dir = Player == PiecePlayer.White? 1 : -1;
            List<ChessPosition> moves = new List<ChessPosition>
            {
                new ChessPosition(col, row + dir * 1)
            };

            if (!IsMoved)
            {
                moves.Add(new ChessPosition(col, row + dir * 2));
            }
            if (col + 1 <= ChessPosition.MaxFile)
            {
                moves.Add(new ChessPosition(col + 1, row + dir * 1));
            }
            if (col - 1 <= ChessPosition.MaxFile)
            {
                moves.Add(new ChessPosition(col - 1, row + dir * 1));
            }

            return moves;
        }
    }
}
