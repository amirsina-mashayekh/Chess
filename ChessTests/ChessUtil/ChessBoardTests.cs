﻿using Chess.ChessUtil.ChessPieces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Chess.ChessUtil.Tests
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
            Pawn wePanw = board.GetPositionOccupier('e', 2) as Pawn;
            Pawn wdPanw = board.GetPositionOccupier('d', 2) as Pawn;
            Assert.AreEqual(2, board.ValidMoves(wePanw).Count);

            Queen bQueen = board.GetPositionOccupier('d', 8) as Queen;
            bQueen.Position.Rank = 3;
            PrintBoard(board);
            Assert.AreEqual(3, board.ValidMoves(wePanw).Count);
            Assert.AreEqual(0, board.ValidMoves(wdPanw).Count);

            bQueen.Position.Rank = 4;
            wePanw.Position.Rank = 3;
            PrintBoard(board);
            Assert.AreEqual(2, board.ValidMoves(wePanw).Count);

            bQueen.Position.File = 'e';
            PrintBoard(board);
            Assert.AreEqual(0, board.ValidMoves(wePanw).Count);
        }

        [TestMethod()]
        public void CastlingTest()
        {
            ChessBoard board = new ChessBoard();
            foreach (ChessPiece piece in board.Pieces)
            {
                if (!(piece is King) && !(piece is Rook))
                {
                    piece.IsCaptured = true;
                }
            }
            PrintBoard(board);

            King wKing = board.GetPositionOccupier('e', 1) as King;
            King bKing = board.GetPositionOccupier('e', 8) as King;
            Rook wqRook = board.GetPositionOccupier('a', 1) as Rook;
            Rook wkRook = board.GetPositionOccupier('h', 1) as Rook;
            Rook bqRook = board.GetPositionOccupier('a', 8) as Rook;
            Rook bkRook = board.GetPositionOccupier('h', 8) as Rook;

            board.MovePiece(wKing, 'g', 1);
            PrintBoard(board);
            Assert.AreEqual(new ChessPosition('f', 1), wkRook.Position);

            Assert.ThrowsException<ArgumentException>(() => board.MovePiece(bKing, 'g', 8));
            board.MovePiece(bKing, 'c', 8);
            PrintBoard(board);
            Assert.AreEqual(new ChessPosition('d', 8), bqRook.Position);

            board.Undo();
            board.Redo();
            board.Undo();
            board.Undo();
            PrintBoard(board);
            Assert.AreEqual(new ChessPosition('h', 1), wkRook.Position);
            Assert.AreEqual(new ChessPosition('a', 8), bqRook.Position);

            board.MovePiece(wKing, 'c', 1);
            PrintBoard(board);
            Assert.AreEqual(new ChessPosition('d', 1), wqRook.Position);

            Assert.ThrowsException<ArgumentException>(() => board.MovePiece(bKing, 'c', 8));
            board.MovePiece(bKing, 'g', 8);
            PrintBoard(board);
            Assert.AreEqual(new ChessPosition('f', 8), bkRook.Position);
        }

        [TestMethod()]
        public void MovePieceTest()
        {
            ChessBoard board = new ChessBoard();
            Queen bQueen = board.GetPositionOccupier('d', 8) as Queen;
            Pawn wePawn = board.GetPositionOccupier('e', 2) as Pawn;

            Assert.ThrowsException<InvalidOperationException>(() => board.MovePiece(bQueen, 'e', 6));
            Assert.ThrowsException<ArgumentException>(() => board.MovePiece('a', 3, 'a', 4));
            board.MovePiece('a', 2, 'a', 4);
            PrintBoard(board);
            Assert.ThrowsException<ArgumentException>(() => board.MovePiece(bQueen, 'e', 6));

            bQueen.Position.File = 'e';
            bQueen.Position.Rank = 6;
            board.MovePiece(bQueen, wePawn.Position.Column, wePawn.Position.Row);
            PrintBoard(board);
            Assert.IsTrue(wePawn.IsCaptured);
            Assert.IsTrue(board.InCheck(ChessPlayer.White));

            King wKing = board.GetPositionOccupier('e', 1) as King;
            Assert.ThrowsException<ArgumentException>(() => board.MovePiece(wKing, 'f', 1));
            board.MovePiece(wKing, 'e', 2);
            PrintBoard(board);
            Assert.IsTrue(bQueen.IsCaptured);

            board.MovePiece('a', 7, 'a', 6);
            board.MovePiece(wKing, 'e', 3);
            PrintBoard(board);

            // TODO: En passant
        }

        [TestMethod()]
        public void CheckmateTest()
        {
            ChessBoard board = new ChessBoard();
            foreach (ChessPiece piece in board.Pieces)
            {
                if (!(piece is King) && !(piece is Queen))
                {
                    piece.IsCaptured = true;
                }
            }
            PrintBoard(board);

            King wKing = board.GetPositionOccupier('e', 1) as King;
            King bKing = board.GetPositionOccupier('e', 8) as King;
            Queen bQueen = board.GetPositionOccupier('d', 8) as Queen;

            board.MovePiece(wKing, 'f', 1);
            board.MovePiece(bKing, 'f', 7);
            PrintBoard(board);
            board.MovePiece(wKing, 'g', 1);
            board.MovePiece(bKing, 'g', 6);
            PrintBoard(board);
            board.MovePiece(wKing, 'h', 1);
            board.MovePiece(bKing, 'g', 5);
            PrintBoard(board);
            board.MovePiece(wKing, 'g', 1);
            board.MovePiece(bKing, 'h', 4);
            PrintBoard(board);
            board.MovePiece(wKing, 'h', 1);
            board.MovePiece(bKing, 'h', 3);
            PrintBoard(board);
            board.MovePiece(wKing, 'g', 1);
            board.MovePiece(bQueen, 'd', 2);
            PrintBoard(board);
            board.MovePiece(wKing, 'h', 1);
            Assert.IsNull(board.Winner);
            board.MovePiece(bQueen, 'h', 2);
            PrintBoard(board);
            Assert.IsTrue(board.Ended);
            Assert.AreEqual(ChessPlayer.Black, board.Winner);
            Assert.IsTrue(board.MovesHistory.Last.Previous.Value.Symbols.EndsWith("#"));
            Assert.AreEqual("0-1", board.MovesHistory.Last.Value.Symbols);
            Assert.ThrowsException<InvalidOperationException>(() => board.MovePiece('d', 1, 'd', 8));
        }

        [TestMethod()]
        public void StalemateTest()
        {
            ChessBoard board = new ChessBoard();
            foreach (ChessPiece piece in board.Pieces)
            {
                if (!(piece is King) && !(piece is Queen))
                {
                    piece.IsCaptured = true;
                }
            }
            PrintBoard(board);

            King wKing = board.GetPositionOccupier('e', 1) as King;
            King bKing = board.GetPositionOccupier('e', 8) as King;
            Queen bQueen = board.GetPositionOccupier('d', 8) as Queen;

            board.MovePiece(wKing, 'e', 2);
            board.MovePiece(bKing, 'f', 7);
            PrintBoard(board);
            board.MovePiece(wKing, 'f', 1);
            board.MovePiece(bKing, 'g', 6);
            PrintBoard(board);
            board.MovePiece(wKing, 'g', 1);
            board.MovePiece(bKing, 'g', 5);
            PrintBoard(board);
            board.MovePiece(wKing, 'h', 1);
            board.MovePiece(bKing, 'h', 4);
            PrintBoard(board);
            board.MovePiece(wKing, 'g', 1);
            board.MovePiece(bKing, 'h', 3);
            PrintBoard(board);
            board.MovePiece('d', 1, 'd', 3);
            board.MovePiece(bQueen, 'd', 3);
            PrintBoard(board);
            board.MovePiece(wKing, 'h', 1);
            board.MovePiece(bQueen, 'g', 3);
            PrintBoard(board);
            Assert.IsTrue(board.Ended);
            Assert.IsNull(board.Winner);
            Assert.IsFalse(board.MovesHistory.Last.Previous.Value.Symbols.EndsWith("#"));
            Assert.AreEqual("1/2-1/2", board.MovesHistory.Last.Value.Symbols);
        }

        [TestMethod()]
        public void EnPassantTest()
        {
            ChessBoard board = new ChessBoard();
            Pawn wbPawn = board.GetPositionOccupier('b', 2) as Pawn;
            Pawn baPawn = board.GetPositionOccupier('a', 7) as Pawn;
            Pawn bcPawn = board.GetPositionOccupier('c', 7) as Pawn;

            board.MovePiece(wbPawn, 'b', 4);
            board.MovePiece(bcPawn, 'c', 5);
            board.MovePiece(wbPawn, 'b', 5);
            board.MovePiece(baPawn, 'a', 5);
            PrintBoard(board);
            Assert.AreEqual(2, board.ValidMoves(wbPawn).Count);
            board.MovePiece(wbPawn, 'a', 6);
            PrintBoard(board);
            Assert.IsTrue(baPawn.IsCaptured);
            Assert.AreEqual(2, board.ValidMoves(wbPawn).Count);
        }

        [TestMethod()]
        public void UndoTest()
        {
            ChessBoard board = new ChessBoard();
            foreach (ChessPiece piece in board.Pieces)
            {
                if (!(piece is King) && !(piece is Queen))
                {
                    piece.IsCaptured = true;
                }
            }
            PrintBoard(board);

            King wKing = board.GetPositionOccupier('e', 1) as King;
            King bKing = board.GetPositionOccupier('e', 8) as King;
            Queen bQueen = board.GetPositionOccupier('d', 8) as Queen;

            Assert.ThrowsException<InvalidOperationException>(() => board.Undo());

            board.MovePiece(wKing, 'f', 1);
            board.MovePiece(bKing, 'f', 7);
            PrintBoard(board);
            board.MovePiece(wKing, 'g', 1);
            board.MovePiece(bKing, 'g', 6);
            PrintBoard(board);
            board.MovePiece(wKing, 'h', 1);
            board.MovePiece(bKing, 'g', 5);
            PrintBoard(board);
            board.MovePiece(wKing, 'g', 1);
            board.MovePiece(bKing, 'h', 4);
            PrintBoard(board);
            board.MovePiece(wKing, 'h', 1);
            board.MovePiece(bKing, 'h', 3);
            PrintBoard(board);
            board.MovePiece(wKing, 'g', 1);
            board.MovePiece(bQueen, 'd', 2);
            PrintBoard(board);
            board.MovePiece(wKing, 'h', 1);
            board.MovePiece(bQueen, 'h', 2);
            PrintBoard(board);

            ChessMove mateMove = board.MovesHistory.Last.Value;
            Assert.IsTrue(board.Undo());
            PrintBoard(board);
            Assert.AreEqual(ChessPlayer.Black, board.Turn);
            Assert.AreEqual(new ChessPosition('d', 2), bQueen.Position);
            Assert.IsFalse(board.Ended);
            Assert.IsNull(board.Winner);

            board.MovePiece(bQueen, 'f', 2);
            PrintBoard(board);
            Assert.IsNull(board.MovesHistory.Find(mateMove));

            while (board.Undo())
            {
                PrintBoard(board);
            }

            PrintBoard(board);
            Assert.AreEqual(ChessPlayer.White, board.Turn);
            Assert.AreEqual(new ChessPosition('e', 1), wKing.Position);
        }

        [TestMethod()]
        public void RedoTest()
        {
            ChessBoard board = new ChessBoard();
            foreach (ChessPiece piece in board.Pieces)
            {
                if (!(piece is King) && !(piece is Queen))
                {
                    piece.IsCaptured = true;
                }
            }
            PrintBoard(board);

            King wKing = board.GetPositionOccupier('e', 1) as King;
            King bKing = board.GetPositionOccupier('e', 8) as King;
            Queen bQueen = board.GetPositionOccupier('d', 8) as Queen;

            board.MovePiece(wKing, 'f', 1);
            board.MovePiece(bKing, 'f', 7);
            PrintBoard(board);
            board.MovePiece(wKing, 'g', 1);
            board.MovePiece(bKing, 'g', 6);
            PrintBoard(board);
            board.MovePiece(wKing, 'h', 1);
            board.MovePiece(bKing, 'g', 5);
            PrintBoard(board);
            board.MovePiece(wKing, 'g', 1);
            board.MovePiece(bKing, 'h', 4);
            PrintBoard(board);
            board.MovePiece(wKing, 'h', 1);
            board.MovePiece(bKing, 'h', 3);
            PrintBoard(board);
            board.MovePiece(wKing, 'g', 1);
            board.MovePiece(bQueen, 'd', 2);
            PrintBoard(board);
            board.MovePiece(wKing, 'h', 1);
            board.MovePiece(bQueen, 'h', 2);
            PrintBoard(board);

            while (board.Undo())
            {
                PrintBoard(board);
            }

            PrintBoard(board);

            while (board.Redo())
            {
                PrintBoard(board);
            }

            PrintBoard(board);
            Assert.IsTrue(board.Ended);
            Assert.AreEqual(ChessPlayer.Black, board.Winner);
            Assert.IsTrue(board.MovesHistory.Last.Previous.Value.Symbols.EndsWith("#"));
            Assert.AreEqual("0-1", board.MovesHistory.Last.Value.Symbols);

            board.Undo();
            board.Undo();
            board.Undo();
            board.Redo();
            board.MovePiece('d', 1, 'd', 2);
            PrintBoard(board);
            Assert.IsTrue(bQueen.IsCaptured);
            Assert.AreEqual(bQueen, board.MovesHistory.Last.Value.CapturedPiece);
        }

        [TestMethod()]
        public void PawnPromotionTest()
        {
            ChessBoard board = new ChessBoard();
            Pawn waPawn = board.GetPositionOccupier('a', 2) as Pawn;
            King bKing = board.GetPositionOccupier('e', 8) as King;

            foreach (ChessPiece piece in board.Pieces)
            {
                if (!(piece is King))
                {
                    piece.IsCaptured = true;
                }
            }
            waPawn.IsCaptured = false;
            PrintBoard(board);

            board.MovePiece(waPawn, 'a', 4);
            board.MovePiece(bKing, 'f', 8);
            board.MovePiece(waPawn, 'a', 5);
            board.MovePiece(bKing, 'g', 8);
            board.MovePiece(waPawn, 'a', 6);
            board.MovePiece(bKing, 'h', 8);
            board.MovePiece(waPawn, 'a', 7);
            board.MovePiece(bKing, 'g', 8);

            Queen wQueen2 = new Queen(ChessPlayer.White, new ChessPosition(1, 1));
            Queen bQueen2 = new Queen(ChessPlayer.Black, new ChessPosition(1, 1));
            King testwKing = new King(ChessPlayer.White, new ChessPosition(1, 1));
            Assert.ThrowsException<ArgumentException>(() => board.PromotePawn(waPawn, wQueen2));

            board.MovePiece(waPawn, 'a', 8);
            PrintBoard(board);
            Assert.ThrowsException<ArgumentException>(() => board.PromotePawn(waPawn, testwKing));
            Assert.ThrowsException<ArgumentException>(() => board.PromotePawn(waPawn, bQueen2));
            board.PromotePawn(waPawn, wQueen2);
            PrintBoard(board);
            Assert.AreEqual(wQueen2, board.GetPositionOccupier('a', 8));
            Assert.IsTrue(board.LastMoveNode.Value.Symbols == "=" + wQueen2.Letter + "+");

            board.MovePiece(bKing, 'h', 7);
            PrintBoard(board);
            board.Undo();
            board.Undo();
            PrintBoard(board);
            Assert.AreEqual(waPawn, board.GetPositionOccupier('a', 7));
            Assert.IsTrue(wQueen2.IsCaptured);

            board.Redo();
            PrintBoard(board);
            Assert.AreEqual(wQueen2, board.GetPositionOccupier('a', 8));
            Assert.IsTrue(board.LastMoveNode.Value.Symbols == "=" + wQueen2.Letter + "+");
            board.Redo();
            PrintBoard(board);
        }
    }
}