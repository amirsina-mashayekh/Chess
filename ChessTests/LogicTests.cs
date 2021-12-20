using Microsoft.VisualStudio.TestTools.UnitTesting;
using Chess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chess.ChessPieces;
using static Chess.ChessBoard;

namespace Chess.Tests
{
    [TestClass()]
    public class LogicTests
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
            ChessBoard testBoard = new ChessBoard();
            PrintBoard(testBoard);
            Queen wQueen = testBoard.Pieces
                .Where(p => p is Queen && p.Player == ChessPlayer.White).Single() as Queen;
            Queen bQueen = testBoard.Pieces
                .Where(p => p is Queen && p.Player == ChessPlayer.Black).Single() as Queen;
            Assert.AreEqual(0, testBoard.AvailableMoves(wQueen).Count);

            wQueen.Position.File = 'e';
            wQueen.Position.Rank = 4;
            bQueen.Position.File = 'f';
            bQueen.Position.Rank = 5;
            PrintBoard(testBoard);
            Assert.AreEqual(6, testBoard.AvailableMoves(wQueen).Count);

            Pawn wPawnE = testBoard.GetPositionOccupier(new ChessPosition('e', 2)) as Pawn;
            Assert.AreEqual(3, testBoard.AvailableMoves(wPawnE).Count);
            Pawn wPawnF = testBoard.GetPositionOccupier(new ChessPosition('f', 2)) as Pawn;
            Assert.AreEqual(4, testBoard.AvailableMoves(wPawnF).Count);
        }
    }
}