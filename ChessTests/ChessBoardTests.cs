using Microsoft.VisualStudio.TestTools.UnitTesting;
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
            Queen wQueen = board.Pieces.Where(p => p is Queen && p.Player == ChessPlayer.White)
                .Single() as Queen;
            Queen bQueen = board.Pieces.Where(p => p is Queen && p.Player == ChessPlayer.Black)
                .Single() as Queen;
            Assert.AreEqual(0, board.AvailableMoves(wQueen).Count);

            wQueen.Position.File = 'e';
            wQueen.Position.Rank = 4;
            bQueen.Position.File = 'f';
            bQueen.Position.Rank = 5;
            PrintBoard(board);
            Assert.AreEqual(17, board.AvailableMoves(wQueen).Count);

            Pawn wPawnE = board.GetPositionOccupier(new ChessPosition('e', 2)) as Pawn;
            Assert.AreEqual(3, board.AvailableMoves(wPawnE).Count);
            Pawn wPawnF = board.GetPositionOccupier(new ChessPosition('f', 2)) as Pawn;
            Assert.AreEqual(4, board.AvailableMoves(wPawnF).Count);
        }

        [TestMethod()]
        public void InCheckTest()
        {
            ChessBoard board = new ChessBoard();
            King wKing = board.Pieces.Where(p => p is King && p.Player == ChessPlayer.White)
                .Single() as King;
            Assert.IsFalse(board.InCheck(ChessPlayer.White));

            wKing.Position.Rank = 4;
            PrintBoard(board);
            Assert.IsFalse(board.InCheck(ChessPlayer.White));

            wKing.Position.Rank = 6;
            PrintBoard(board);
            Assert.IsTrue(board.InCheck(ChessPlayer.White));

            board.GetPositionOccupier(new ChessPosition('d', 7)).IsCaptured = true;
            board.GetPositionOccupier(new ChessPosition('e', 7)).IsCaptured = true;
            board.GetPositionOccupier(new ChessPosition('f', 7)).IsCaptured = true;
            PrintBoard(board);
            Assert.IsTrue(board.InCheck(ChessPlayer.White));
        }
    }
}