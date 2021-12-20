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
        /// Collection of all board pieces.
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

        /// <summary>
        /// Gets the piece which occupies a position.
        /// </summary>
        /// <param name="position">The position to be checked.</param>
        /// <returns>
        /// The piece in <paramref name="position"/>.
        /// <c>null</c> if there is no piece in <paramref name="position"/>.
        /// </returns>
        public ChessPiece GetPositionOccupier(ChessPosition position)
        {
            ChessPiece piece = null;

            foreach (ChessPiece p in Pieces)
            {
                if (!p.IsCaptured && p.Position == position)
                {
                    piece = p;
                    break;
                }
            }

            return piece;
        }

        /// <summary>
        /// Gets the available moves for a piece.
        /// </summary>
        /// <param name="piece">The piece to be checked.</param>
        /// <returns>
        /// A list of <c>ChessPosition</c>s containing available moves for <paramref name="piece"/>.
        /// </returns>
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
                            .Where(m => IsAfterEnd(pcc, occ, m.Column)).ToList()
                            .ForEach(m => moves.Remove(m));
                    }
                    else if (pcc == occ)
                    {
                        // Check vertically
                        moves
                            .Where(m => IsAfterEnd(pcr, ocr, m.Row)).ToList().
                            ForEach(m => moves.Remove(m));
                    }
                }
                if (piece is Bishop || piece is Queen)
                {
                    // Check diagonally
                    if (pcr - pcc == ocr - occ || pcr + pcc == ocr + occ)
                    {
                        moves
                            .Where(m => IsAfterEnd(pcr, ocr, m.Row) && IsAfterEnd(pcc, occ, m.Column)).ToList().
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

        /// <summary>
        /// Checks if a number is after two numbers (ray line form).
        /// </summary>
        /// <param name="start">Start number.</param>
        /// <param name="end">End number.</param>
        /// <param name="check">The number to be checked.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="check"/> is after both <paramref name="start"/> and <paramref name="end"/>
        /// <c>false</c> otherwise.
        /// </returns>
        private bool IsAfterEnd(int start, int end, int check)
        {
            if (start < end) { return check > end; }
            else if (start > end) { return check < end; }
            else { return false; }
        }

        /// <summary>
        /// Checks if <paramref name="player"/>'s king is in check.
        /// </summary>
        /// <param name="player">The player to be checked.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="player"/>'s king is in check. <c>false</c> otherwise.
        /// </returns>
        public bool InCheck(ChessPlayer player)
        {
            King king = Pieces.Where(p => p is King && p.Player == player).Single() as King;

            List<ChessPiece> opponetPieces = Pieces.Where(p => !p.IsCaptured && p.Player != player).ToList();
            foreach (ChessPiece piece in opponetPieces)
            {
                List<ChessPosition> moves = AvailableMoves(piece);
                foreach (ChessPosition move in moves)
                {
                    if (move == king.Position) { return true; }
                }
            }
            return false;
        }
    }
}
