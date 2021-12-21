﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Chess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chess.ChessPieces;

namespace Chess.Tests
{
    [TestClass()]
    public class ChessBoardTests
    {
        private void PrintBoard(ChessBoard board)
        {
            for (int i = ChessPosition.MaxRow; i >= ChessPosition.MinRow; i--)
            {
                for (int j = ChessPosition.MinColumn; j <= ChessPosition.MaxColumn; j++)
                {
                    ChessPiece piece = board.GetPositionOccupier(new ChessPosition(j, i));
                    if (piece != null)
                    {
                        if (piece.Player == ChessPlayer.White)
                        {
                            Console.Write(char.ToLower(piece.Letter));
                        }
                        else { Console.Write(piece.Letter); }
                    }
                    else { Console.Write("-"); }
                    Console.Write(" ");
                }
                Console.WriteLine("");
            }
            Console.WriteLine("");
        }

        [TestMethod()]
        public void AvailableMovesTest()
        {
            ChessBoard board = new ChessBoard();
            Queen wQueen = board.GetPositionOccupier('d', 1) as Queen;
            Queen bQueen = board.GetPositionOccupier('d', 8) as Queen;
            Assert.AreEqual(0, board.AvailableMoves(wQueen).Count);

            wQueen.Position.File = 'e';
            wQueen.Position.Rank = 4;
            bQueen.Position.File = 'f';
            bQueen.Position.Rank = 5;
            PrintBoard(board);
            Assert.AreEqual(17, board.AvailableMoves(wQueen).Count);

            Pawn wPawnE = board.GetPositionOccupier('e', 2) as Pawn;
            Assert.AreEqual(3, board.AvailableMoves(wPawnE).Count);
        }

        [TestMethod()]
        public void InCheckTest()
        {
            ChessBoard board = new ChessBoard();
            King wKing = board.GetPositionOccupier('e', 1) as King;
            Assert.IsFalse(board.InCheck(ChessPlayer.White));

            wKing.Position.Rank = 4;
            PrintBoard(board);
            Assert.IsFalse(board.InCheck(ChessPlayer.White));

            wKing.Position.Rank = 6;
            PrintBoard(board);
            Assert.IsTrue(board.InCheck(ChessPlayer.White));

            board.GetPositionOccupier('d', 7).IsCaptured = true;
            board.GetPositionOccupier('e', 7).IsCaptured = true;
            board.GetPositionOccupier('f', 7).IsCaptured = true;
            PrintBoard(board);
            Assert.IsTrue(board.InCheck(ChessPlayer.White));

            board.GetPositionOccupier('c', 8).IsCaptured = true;
            PrintBoard(board);
            Assert.IsFalse(board.InCheck(ChessPlayer.White));
        }

        [TestMethod()]
        public void ValidMoves_CheckTest()
        {
            ChessBoard board = new ChessBoard();

            King wKing = board.GetPositionOccupier('e', 1) as King;
            Assert.AreEqual(0, board.ValidMoves(wKing).Count);

            // Check validation tests
            wKing.Position.File = 'h';
            wKing.Position.Rank = 6;
            board.GetPositionOccupier('h', 8).IsCaptured = true;
            board.GetPositionOccupier('f', 7).IsCaptured = true;
            PrintBoard(board);
            Assert.AreEqual(3, board.ValidMoves(wKing).Count);

            wKing.Position.File = 'e';
            wKing.Position.Rank = 3;
            Queen wQueen = board.GetPositionOccupier('d', 1) as Queen;
            Queen bQueen = board.GetPositionOccupier('d', 8) as Queen;
            wQueen.Position.File = 'e';
            wQueen.Position.Rank = 4;
            bQueen.Position.File = 'e';
            bQueen.Position.Rank = 6;
            PrintBoard(board);
            Assert.AreEqual(2, board.ValidMoves(wQueen).Count);
        }

        [TestMethod()]
        public void ValidMoves_CastlingTest()
        {
            // Castling validation tests
            ChessBoard board = new ChessBoard();
            King wKing = board.GetPositionOccupier('e', 1) as King;
            board.GetPositionOccupier('f', 1).IsCaptured = true;
            PrintBoard(board);
            Assert.AreEqual(1, board.ValidMoves(wKing).Count);

            board.GetPositionOccupier('g', 1).IsCaptured = true;
            PrintBoard(board);
            Assert.AreEqual(2, board.ValidMoves(wKing).Count);

            board.GetPositionOccupier('e', 2).IsCaptured = true;
            board.GetPositionOccupier('f', 2).IsCaptured = true;
            PrintBoard(board);
            Assert.AreEqual(4, board.ValidMoves(wKing).Count);

            Queen bQueen = board.GetPositionOccupier('d', 8) as Queen;
            bQueen.Position.File = 'e';
            bQueen.Position.Rank = 6;
            PrintBoard(board);
            Assert.AreEqual(2, board.ValidMoves(wKing).Count);

            bQueen.Position.File = 'a';
            bQueen.Position.Rank = 6;
            PrintBoard(board);
            Assert.AreEqual(1, board.ValidMoves(wKing).Count);

            bQueen.Position.File = 'b';
            bQueen.Position.Rank = 6;
            PrintBoard(board);
            Assert.AreEqual(2, board.ValidMoves(wKing).Count);
            bQueen.IsCaptured = true;

            Rook wrRook = board.GetPositionOccupier('h', 1) as Rook;
            wrRook.Position.IsMoved = true;
            Assert.AreEqual(3, board.ValidMoves(wKing).Count);

            wrRook.Position.IsMoved = false;
            wKing.Position.IsMoved = true;
            Assert.AreEqual(3, board.ValidMoves(wKing).Count);
        }

        [TestMethod()]
        public void ValidMoves_PawnTest()
        {
            // Pawn validation test
            ChessBoard board = new ChessBoard();
            Pawn wdPanw = board.GetPositionOccupier('e', 2) as Pawn;
            Assert.AreEqual(2, board.ValidMoves(wdPanw).Count);

            Queen bQueen = board.GetPositionOccupier('d', 8) as Queen;
            bQueen.Position.Rank = 3;
            PrintBoard(board);
            Assert.AreEqual(3, board.ValidMoves(wdPanw).Count);

            bQueen.Position.Rank = 4;
            wdPanw.Position.Rank = 3;
            PrintBoard(board);
            Assert.AreEqual(2, board.ValidMoves(wdPanw).Count);

            bQueen.Position.File = 'e';
            PrintBoard(board);
            Assert.AreEqual(0, board.ValidMoves(wdPanw).Count);
        }

        [TestMethod()]
        public void MovePieceTest()
        {
            ChessBoard board = new ChessBoard();
            Queen bQueen = board.GetPositionOccupier('d', 8) as Queen;

            Assert.ThrowsException<ArgumentException>(() => board.MovePiece(bQueen, 'e', 6));

            bQueen.Position.File = 'e';
            bQueen.Position.Rank = 6;
            Pawn wePawn = board.GetPositionOccupier('e', 2) as Pawn;
            board.MovePiece(bQueen, wePawn.Position.Column, wePawn.Position.Row);
            PrintBoard(board);
            Assert.IsTrue(wePawn.IsCaptured);
            Assert.IsTrue(board.InCheck(ChessPlayer.White));

            King wKing = board.GetPositionOccupier('e', 1) as King;
            Assert.ThrowsException<ArgumentException>(() => board.MovePiece(wKing, 'f', 1));
            board.MovePiece(wKing, 'e', 2);
            PrintBoard(board);
            Assert.IsTrue(bQueen.IsCaptured);

            board.MovePiece(wKing, 'e', 3);
            PrintBoard(board);

            // TODO: En passant
        }
    }
}