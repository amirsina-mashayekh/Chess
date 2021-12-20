using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chess.ChessPieces;

namespace Chess
{
    public class ChessBoard
    {
        /// <summary>
        /// Collection of all game pieces.
        /// </summary>
        public readonly ChessPiece[] Pieces = new ChessPiece[]
        {
            new Rook(ChessPlayer.White, new ChessPosition('a', 1)),
            new Knight(ChessPlayer.White, new ChessPosition('b', 1)),
            new Bishop(ChessPlayer.White, new ChessPosition('c', 1)),
            new Queen(ChessPlayer.White, new ChessPosition('d', 1)),
            new King(ChessPlayer.White, new ChessPosition('e', 1)),
            new Bishop(ChessPlayer.White, new ChessPosition('f', 1)),
            new Knight(ChessPlayer.White, new ChessPosition('g', 1)),
            new Rook(ChessPlayer.White, new ChessPosition('h', 1)),
            new Pawn(ChessPlayer.White, new ChessPosition('a', 2)),
            new Pawn(ChessPlayer.White, new ChessPosition('b', 2)),
            new Pawn(ChessPlayer.White, new ChessPosition('c', 2)),
            new Pawn(ChessPlayer.White, new ChessPosition('d', 2)),
            new Pawn(ChessPlayer.White, new ChessPosition('e', 2)),
            new Pawn(ChessPlayer.White, new ChessPosition('f', 2)),
            new Pawn(ChessPlayer.White, new ChessPosition('g', 2)),
            new Pawn(ChessPlayer.White, new ChessPosition('h', 2)),
            new Rook(ChessPlayer.Black, new ChessPosition('a', 8)),
            new Knight(ChessPlayer.Black, new ChessPosition('b', 8)),
            new Bishop(ChessPlayer.Black, new ChessPosition('c', 8)),
            new Queen(ChessPlayer.Black, new ChessPosition('d', 8)),
            new King(ChessPlayer.Black, new ChessPosition('e', 8)),
            new Bishop(ChessPlayer.Black, new ChessPosition('f', 8)),
            new Knight(ChessPlayer.Black, new ChessPosition('g', 8)),
            new Rook(ChessPlayer.Black, new ChessPosition('h', 8)),
            new Pawn(ChessPlayer.Black, new ChessPosition('a', 7)),
            new Pawn(ChessPlayer.Black, new ChessPosition('b', 7)),
            new Pawn(ChessPlayer.Black, new ChessPosition('c', 7)),
            new Pawn(ChessPlayer.Black, new ChessPosition('d', 7)),
            new Pawn(ChessPlayer.Black, new ChessPosition('e', 7)),
            new Pawn(ChessPlayer.Black, new ChessPosition('f', 7)),
            new Pawn(ChessPlayer.Black, new ChessPosition('g', 7)),
            new Pawn(ChessPlayer.Black, new ChessPosition('h', 7))
        };

        public bool InCheck(ChessPlayer player)
        {
            King king = null;

            foreach (King k in Pieces)
            {
                if (k.Player == player)
                {
                    king = k;
                    break;
                }
            }

            List<ChessPiece> opponetPieces = Pieces.Where(piece => piece.Player != player).ToList();
            foreach (ChessPiece piece in opponetPieces)
            {
                List<ChessPosition> moves = AvailableMoves(piece);
                foreach (ChessPosition move in moves)
                {
                    if (move == king.Position)
                    {
                        if (piece is Knight) { return true; }
                    }
                }
            }
            return false;
        }

        public ChessPiece GetPositionOccupier(ChessPosition position)
        {
            ChessPiece piece = null;

            foreach (ChessPiece p in Pieces)
            {
                if (p.Position == position)
                {
                    piece = p;
                    break;
                }
            }

            return piece;
        }

        public List<ChessPosition> AvailableMoves(ChessPiece piece)
        {
            List<ChessPosition> moves = piece.GetMoves();

            for (int i = 0; i < moves.Count; i++)
            {
                ChessPosition move = moves[i];
                ChessPiece occupier = GetPositionOccupier(move);

                if (occupier is null) { continue; }

                if (occupier.Player == piece.Player)
                {
                    moves.Remove(move);
                    i--;
                }

                int pcr = piece.Position.Row;
                int pcc = piece.Position.Column;
                int ocr = occupier.Position.Row;
                int occ = occupier.Position.Column;

                if (piece is Rook || piece is Queen)
                {
                    if (pcr == ocr)
                    {
                        // Check horizontally
                        moves
                            .Where(m => !IsBetween(pcc, occ, m.Column)).ToList()
                            .ForEach(m => moves.Remove(m));
                    }
                    else if (pcc == occ)
                    {
                        // Check vertically
                        moves
                            .Where(m => !IsBetween(pcr, ocr, m.Row)).ToList().
                            ForEach(m => moves.Remove(m));
                    }
                }
                if (piece is Bishop || piece is Queen)
                {
                    // Check diagonally
                    if (pcr - pcc == ocr - occ || pcr + pcc == ocr + occ)
                    {
                        moves
                            .Where(m => !IsBetween(pcr, ocr, m.Row)).ToList().
                            ForEach(m => moves.Remove(m));
                    }
                }
                if (piece is Pawn && pcc == occ && occupier.Player != piece.Player)
                {
                    moves.Remove(move);
                    i--;
                }
            }

            return moves;
        }

        public bool IsBetween(int num1, int num2, int check)
        {
            return num1 != check
                && num2 != check
                && Math.Abs(num1 - check) + Math.Abs(num2 - check) == Math.Abs(num1 - num2);
        }
    }
}
