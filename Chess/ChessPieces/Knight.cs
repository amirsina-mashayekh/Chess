﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.ChessPieces
{
    internal class Knight : ChessPiece
    {
        public override int ValuePoints => 3;

        public override string FullName => "Knight";

        public override char Letter => 'N';

        public Knight(PiecePlayer player, ChessPosition position) : base(player, position) { }

        private static readonly int[] rowMoves = new int[] { 1, 2, 2, 1, -1, -2, -2, -1 };

        private static readonly int[] colMoves = new int[] { -2, -1, 1, 2, 2, 1, -1, -2 };

        public override List<ChessPosition> GetAvailableMoves()
        {
            int col = Position.File;
            int row = Position.Rank;
            List<ChessPosition> moves = new List<ChessPosition>();

            for (int i = 0; i < 8; i++)
            {
                try
                {
                    moves.Add(new ChessPosition(col + colMoves[i], row + rowMoves[i]));
                }
                catch (ArgumentOutOfRangeException) { }
            }

            return moves;
        }
    }
}
