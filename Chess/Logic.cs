using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chess.ChessPieces;

namespace Chess
{
    internal static class Logic
    {
        /// <summary>
        /// Collection of all game pieces.
        /// </summary>
        public static ChessPiece[] pieces = new ChessPiece[]
        {
            new Rook(PiecePlayer.White, new ChessPosition('a', 1)),
            new Knight(PiecePlayer.White, new ChessPosition('b', 1)),
            new Bishop(PiecePlayer.White, new ChessPosition('c', 1)),
            new Queen(PiecePlayer.White, new ChessPosition('d', 1)),
            new King(PiecePlayer.White, new ChessPosition('e', 1)),
            new Bishop(PiecePlayer.White, new ChessPosition('f', 1)),
            new Knight(PiecePlayer.White, new ChessPosition('g', 1)),
            new Rook(PiecePlayer.White, new ChessPosition('h', 1)),
            new Pawn(PiecePlayer.White, new ChessPosition('a', 2)),
            new Pawn(PiecePlayer.White, new ChessPosition('b', 2)),
            new Pawn(PiecePlayer.White, new ChessPosition('c', 2)),
            new Pawn(PiecePlayer.White, new ChessPosition('d', 2)),
            new Pawn(PiecePlayer.White, new ChessPosition('e', 2)),
            new Pawn(PiecePlayer.White, new ChessPosition('f', 2)),
            new Pawn(PiecePlayer.White, new ChessPosition('g', 2)),
            new Pawn(PiecePlayer.White, new ChessPosition('h', 2)),
            new Rook(PiecePlayer.Black, new ChessPosition('a', 8)),
            new Knight(PiecePlayer.Black, new ChessPosition('b', 8)),
            new Bishop(PiecePlayer.Black, new ChessPosition('c', 8)),
            new Queen(PiecePlayer.Black, new ChessPosition('d', 8)),
            new King(PiecePlayer.Black, new ChessPosition('e', 8)),
            new Bishop(PiecePlayer.Black, new ChessPosition('f', 8)),
            new Knight(PiecePlayer.Black, new ChessPosition('g', 8)),
            new Rook(PiecePlayer.Black, new ChessPosition('h', 8)),
            new Pawn(PiecePlayer.Black, new ChessPosition('a', 7)),
            new Pawn(PiecePlayer.Black, new ChessPosition('b', 7)),
            new Pawn(PiecePlayer.Black, new ChessPosition('c', 7)),
            new Pawn(PiecePlayer.Black, new ChessPosition('d', 7)),
            new Pawn(PiecePlayer.Black, new ChessPosition('e', 7)),
            new Pawn(PiecePlayer.Black, new ChessPosition('f', 7)),
            new Pawn(PiecePlayer.Black, new ChessPosition('g', 7)),
            new Pawn(PiecePlayer.Black, new ChessPosition('h', 7))
        };
    }
}
