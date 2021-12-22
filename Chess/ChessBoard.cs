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
        /// The player whose turn it is to move.
        /// </summary>
        public ChessPlayer Turn { get; private set; } = ChessPlayer.White;

        /// <summary>
        /// Toggles player turn.
        /// </summary>
        public void ToggleTurn()
        {
            if (Turn == ChessPlayer.White) { Turn = ChessPlayer.Black; }
            else { Turn = ChessPlayer.White; }
        }

        /// <summary>
        /// Gets whether game is finished.
        /// </summary>
        public bool Ended { get; private set; } = false;

        /// <summary>
        /// History of moves.
        /// </summary>
        public readonly LinkedList<string> MovesHistory = new LinkedList<string>();

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
            return Pieces.SingleOrDefault(p => !p.IsCaptured && p.Position == position);
        }

        /// <summary>
        /// Gets the piece which occupies a position with specified file and rank.
        /// </summary>
        /// <param name="file">The file of position.</param>
        /// <param name="rank">The rank of position.</param>
        /// <returns>
        /// The piece in <paramref name="file"/> and <paramref name="rank"/>.
        /// <c>null</c> if there is no piece in position.
        /// </returns>
        public ChessPiece GetPositionOccupier(char file, int rank)
        {
            return GetPositionOccupier(new ChessPosition(file, rank));
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
            int pcr = piece.Position.Row;
            int pcc = piece.Position.Column;

            foreach (ChessPosition move in moves.ToList())
            {
                if (!moves.Contains(move)) { continue; }
                ChessPiece occupier = GetPositionOccupier(move);

                if (occupier is null) { continue; }

                if (occupier.Player == piece.Player) { moves.Remove(move); }

                int ocr = move.Row;
                int occ = move.Column;
                if (piece is Rook || piece is Queen)
                {
                    if (pcr == ocr)
                    {
                        // Check horizontally
                        moves
                            .Where(m => m.Row == pcr && IsAfterEnd(pcc, occ, m.Column)).ToList()
                            .ForEach(m => moves.Remove(m));
                    }
                    else if (pcc == occ)
                    {
                        // Check vertically
                        moves
                            .Where(m => m.Column == pcc && IsAfterEnd(pcr, ocr, m.Row)).ToList()
                            .ForEach(m => moves.Remove(m));
                    }
                }
                if (piece is Bishop || piece is Queen)
                {
                    // Check diagonally
                    if (pcr - pcc == ocr - occ || pcr + pcc == ocr + occ)
                    {
                        moves
                            .Where(m => IsAfterEnd(pcr, ocr, m.Row) && IsAfterEnd(pcc, occ, m.Column))
                            .ToList()
                            .ForEach(m => moves.Remove(m));
                    }
                }
                if (piece is Pawn && pcc == occ && occupier.Player != piece.Player)
                {
                    // Pawn cannot attack vertically
                    moves.Remove(move);
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
            King king = Pieces.Single(p => p is King && p.Player == player) as King;

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

        /// <summary>
        /// Gets the valid moves from available moves of a piece.
        /// </summary>
        /// <param name="piece">The piece to be checked.</param>
        /// <returns>
        /// A list of <c>ChessPosition</c>s containing valid moves for <paramref name="piece"/>.
        /// </returns>
        public List<ChessPosition> ValidMoves(ChessPiece piece)
        {
            List<ChessPosition> moves = AvailableMoves(piece);
            bool moved = piece.Position.IsMoved;
            int r = piece.Position.Row;
            int c = piece.Position.Column;

            // Check for check! (=
            for (int i = 0; i < moves.Count; i++)
            {
                ChessPosition move = moves[i];
                ChessPiece occ = GetPositionOccupier(move);
                if (occ != null) { occ.IsCaptured = true; }

                piece.Position.Row = move.Row;
                piece.Position.Column = move.Column;
                if (InCheck(piece.Player))
                {
                    moves.RemoveAt(i);
                    i--;
                }
                if (occ != null) { occ.IsCaptured = false; }
            }

            // Reset piece position
            piece.Position.Row = r;
            piece.Position.Column = c;
            piece.Position.IsMoved = moved;
            // Check for castling
            if (piece is King && !piece.Position.IsMoved && !InCheck(piece.Player))
            {
                List<ChessPiece> rooks = Pieces
                    .Where(p => p is Rook
                                && p.Player == piece.Player
                                && !piece.IsCaptured)
                    .ToList();

                foreach (ChessPiece rook in rooks)
                {
                    if (rook.Position.IsMoved) { continue; }
                    int dir = rook.Position.Column > c ? 1 : -1;
                    if (moves.Exists(p => p.Column == c + dir && p.Row == r))
                    {
                        ChessPosition lc = new ChessPosition(c + dir * 2, r);
                        if (GetPositionOccupier(lc) is null)
                        {
                            piece.Position.Column = lc.Column;
                            if (!InCheck(piece.Player))
                            {
                                moves.Add(lc);
                            }
                        }
                    }
                }
            }

            // Check for pawn moves
            if (piece is Pawn)
            {
                moves
                    .Where(p => p.Column != c && GetPositionOccupier(p) is null)
                    .ToList()
                    .ForEach(m => moves.Remove(m));
            }

            // Check for En passant
            // TODO: Implement

            // Reset piece position
            piece.Position.Row = r;
            piece.Position.Column = c;
            piece.Position.IsMoved = moved;
            return moves;
        }

        /// <summary>
        /// Moves a piece to a position.
        /// </summary>
        /// <param name="piece">The piece to be moved.</param>
        /// <param name="column">The column of destination position.</param>
        /// <param name="row">The row of destination position.</param>
        public void MovePiece(ChessPiece piece, int column, int row)
        {
            if (Ended)
            {
                throw new InvalidOperationException("Cannot move pieces after the end of game.");
            }

            if (piece.Player != Turn)
            {
                throw new InvalidOperationException("Tried to move piece out of its turn.");
            }

            if (!ValidMoves(piece).Any(p => p.Column == column && p.Row == row))
            {
                throw new ArgumentException("Piece must be moved to a valid position.");
            }

            StringBuilder moveStr = new StringBuilder();
            moveStr.Append(piece.ShortString);
            ChessPiece occupier = GetPositionOccupier(new ChessPosition(column, row));
            if (occupier != null)
            {
                // Capture piece in destination position
                occupier.IsCaptured = true;
                moveStr.Append('x').Append(occupier.ShortString);
            }
            else if (piece is Pawn && column != piece.Position.Column)
            {
                // Pawn is moving diagonally to an empty position
                // which means En passant is being performed.
                Pawn captured = GetPositionOccupier(new ChessPosition(column, piece.Position.Row)) as Pawn;
                captured.IsCaptured = true;
                moveStr.Append('x').Append(captured.ShortString);
            }
            else
            {
                // Normal move
                moveStr.Append(ChessPosition.ColumnToFile(column)).Append(row);
            }

            // Change piece position
            piece.Position.Column = column;
            piece.Position.Row = row;

            ToggleTurn();
            // Check status
            bool inCheck = InCheck(Turn);
            if (!Pieces.Any(p => !p.IsCaptured && p.Player == Turn && ValidMoves(p).Count > 0))
            {
                // Either Stalemate or Checkmate
                if (inCheck) { moveStr.Append('#'); }
                Ended = true;
            }
            else if (inCheck) { moveStr.Append('+'); }

            MovesHistory.AddLast(moveStr.ToString());
        }

        /// <summary>
        /// Moves a piece to a position.
        /// </summary>
        /// <param name="piece">The piece to be moved.</param>
        /// <param name="file">The file of destination position.</param>
        /// <param name="rank">The rank of destination position.</param>
        public void MovePiece(ChessPiece piece, char file, int rank)
        {
            MovePiece(piece, ChessPosition.FileToColumn(file), rank);
        }

        /// <summary>
        /// Moves a piece from a specified position to another position.
        /// </summary>
        /// <param name="srcFile">The file of source position.</param>
        /// <param name="srcRank">The rank of source position.</param>
        /// <param name="dstFile">The file of destination position.</param>
        /// <param name="dstRank">The rank of destination position.</param>
        /// <exception cref="ArgumentException">There is no piece at source position.</exception>
        public void MovePiece(char srcFile, int srcRank, char dstFile, int dstRank)
        {
            ChessPiece piece = GetPositionOccupier(srcFile, srcRank);
            if (piece is null)
            {
                throw new ArgumentException(
                    $"There is no piece at {srcFile}{srcRank}.",
                    nameof(srcFile) + ", " + nameof(srcRank));
            }
            MovePiece(piece, ChessPosition.FileToColumn(dstFile), dstRank);
        }
    }
}
