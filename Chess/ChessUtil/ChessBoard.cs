﻿using Chess.ChessUtil.ChessPieces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chess.ChessUtil
{
    /// <summary>
    /// Represents a chess board.
    /// Provides methods to manage chess board.
    /// </summary>
    public partial class ChessBoard
    {
        /// <summary>
        /// Collection of all board pieces.
        /// </summary>
        public readonly List<ChessPiece> Pieces;

        /// <summary>
        /// Gets the start time of game (time when this instance was created).
        /// </summary>
        public DateTime StartTime { get; }

        /// <summary>
        /// The player whose turn it is to move.
        /// </summary>
        public ChessPlayer Turn { get; private set; }

        /// <summary>
        /// Toggles player turn.
        /// </summary>
        private void ToggleTurn()
        {
            if (Turn == ChessPlayer.White)
            {
                Turn = ChessPlayer.Black;
            }
            else
            {
                Turn = ChessPlayer.White;
            }
        }

        private bool _ended;

        /// <summary>
        /// Gets or sets whether game is finished.
        /// </summary>
        public bool Ended
        {
            get => _ended;
            set
            {
                _ended = value;
                if (value)
                {
                    if (MovesHistory.Last.Value is null || MovesHistory.Last.Value.Destination != null)
                    {
                        King winner = Pieces.SingleOrDefault(p => p is King && p.Player == Winner) as King;
                        ChessMove result = new ChessMove(winner, GetResultNotation());
                        MovesHistory.AddLast(result);
                    }
                    LastMoveNode = MovesHistory.Last.Previous;
                }
            }
        }

        private ChessPlayer? _winner;

        /// <summary>
        /// Gets or sets the winner of game. Returns <c>null</c> if game is not finished or drawn.
        /// </summary>
        public ChessPlayer? Winner
        {
            get => _winner;
            set
            {
                _winner = value;
                Ended = value != null;
            }
        }

        /// <summary>
        /// History of moves.
        /// </summary>
        public readonly LinkedList<ChessMove> MovesHistory;

        /// <summary>
        /// Indicates the last move in <c>MovesHistory</c>.
        /// </summary>
        public LinkedListNode<ChessMove> LastMoveNode { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <c>ChessBoard</c> class.
        /// </summary>
        public ChessBoard()
        {
            Pieces = new List<ChessPiece>
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
            Winner = null;
            Turn = ChessPlayer.White;
            MovesHistory = new LinkedList<ChessMove>();
            MovesHistory.AddFirst(null as ChessMove);
            LastMoveNode = MovesHistory.First;
            StartTime = DateTime.Now;
        }

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
                if (!moves.Contains(move))
                {
                    continue;
                }

                ChessPiece occupier = GetPositionOccupier(move);

                if (occupier is null)
                {
                    continue;
                }

                if (occupier.Player == piece.Player)
                {
                    moves.Remove(move);
                }

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
                if (piece is Pawn && !piece.Position.IsMoved)
                {
                    moves
                        .Where(m => m.Column == occ && IsAfterEnd(pcr, ocr, m.Row)).ToList()
                        .ForEach(m => moves.Remove(m));
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
            if (start < end)
            {
                return check > end;
            }
            else if (start > end)
            {
                return check < end;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if <paramref name="player"/>'s king is in check.
        /// </summary>
        /// <param name="player">The player to be checked.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="player"/>'s king is in check.
        /// <c>false</c> otherwise.
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
                    if (move == king.Position)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Checks is <paramref name="player"/> has any valid moves.
        /// </summary>
        /// <param name="player">The player to be checked.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="player"/> has any valid moves.
        /// <c>false</c> otherwise.
        /// </returns>
        public bool HasValidMoves(ChessPlayer player)
        {
            return Pieces.Any(p => !p.IsCaptured && p.Player == player && ValidMoves(p).Count > 0);
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
                if (occ != null)
                {
                    occ.IsCaptured = true;
                }

                piece.Position.Row = move.Row;
                piece.Position.Column = move.Column;
                if (InCheck(piece.Player))
                {
                    moves.RemoveAt(i);
                    i--;
                }
                if (occ != null)
                {
                    occ.IsCaptured = false;
                }
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
                    if (rook.Position.IsMoved)
                    {
                        continue;
                    }

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
                ChessPosition epPos = null;
                // Check for En passant
                ChessMove lastMove = LastMoveNode.Value;
                if (lastMove != null)
                {
                    if (lastMove.CapturedPiece is null)
                    {
                        // Last move was a normal (no capture) pawn move
                        if (Math.Abs(lastMove.Source.Row - lastMove.Destination.Row) == 2)
                        {
                            // Pawn moved 2 rows in last move
                            epPos = new ChessPosition(lastMove.Destination.Column, lastMove.Destination.Row);
                        }
                    }
                }

                moves
                    .Where(p => p.Column != c && GetPositionOccupier(p) is null)
                    .ToList()
                    .ForEach(m =>
                    {
                        if (!(epPos != null && epPos.Row == r && epPos.Column == m.Column))
                        {
                            // No En passant so delete move
                            moves.Remove(m);
                        }
                    });
            }

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

            ChessPosition src = piece.Position.Copy();
            ChessPiece occupier = GetPositionOccupier(new ChessPosition(column, row));
            StringBuilder symbols = new StringBuilder();

            if (occupier != null)
            {
                // Capture piece in destination position
                occupier.IsCaptured = true;
            }
            else if (piece is Pawn && column != piece.Position.Column)
            {
                // Pawn is moving diagonally to an empty position
                // which means En passant is being performed.
                Pawn captured = GetPositionOccupier(new ChessPosition(column, piece.Position.Row)) as Pawn;
                captured.IsCaptured = true;
                occupier = captured;
            }

            // Check for castling
            if (piece is King && Math.Abs(src.Column - column) == 2)
            {
                if (src.Column < column)
                {
                    // Kingside
                    GetPositionOccupier(new ChessPosition(ChessPosition.MaxColumn, row))
                        .Position.Column = column - 1;
                    symbols.Append("0-0");
                }
                else
                {
                    // Queenside
                    GetPositionOccupier(new ChessPosition(ChessPosition.MinColumn, row))
                        .Position.Column = column + 1;
                    symbols.Append("0-0-0");
                }
            }

            // Change piece position
            piece.Position.Column = column;
            piece.Position.Row = row;

            ToggleTurn();
            // Check status
            if (MovesHistory.Last.Value != null && MovesHistory.Last.Value.Destination is null)
            {
                MovesHistory.RemoveLast();
            }
            LinkedListNode<ChessMove> move = new LinkedListNode<ChessMove>(new ChessMove(src, piece, occupier));
            if (LastMoveNode != null)
            {
                while (LastMoveNode.Next != null)
                {
                    // Moving after undo, remove extra moves
                    MovesHistory.RemoveLast();
                }
            }
            MovesHistory.AddLast(move);
            LastMoveNode = MovesHistory.Last;

            bool inCheck = InCheck(Turn);
            if (!HasValidMoves(Turn))
            {
                // Either Stalemate or Checkmate
                if (inCheck)
                {
                    symbols.Append('#');
                    ToggleTurn();
                    Winner = Turn;
                    ToggleTurn();
                }
                Ended = true;
            }
            else if (inCheck)
            {
                symbols.Append('+');
            }

            move.Value.Symbols = symbols.ToString();
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

        /// <summary>
        /// Reverses last move.
        /// </summary>
        /// <returns>Wether any more undo is available.</returns>
        /// <exception cref="InvalidOperationException">There is no move to undo.</exception>
        public bool Undo()
        {
            if (LastMoveNode == MovesHistory.First)
            {
                throw new InvalidOperationException("There is no move to undo.");
            }

            ChessMove lastMove = LastMoveNode.Value;
            ChessPosition src = lastMove.Source;
            ChessPosition dst = lastMove.Destination;
            ChessPiece moved = lastMove.MovedPiece;
            LastMoveNode = LastMoveNode.Previous;
            Winner = null;
            ToggleTurn();

            if (dst is null)
            {
                return true;
            }

            if (!string.IsNullOrEmpty(lastMove.Symbols) && lastMove.Symbols[0] == '=')
            {
                // Undo pawn promotion
                ChessPiece promotion = GetPositionOccupier(dst);
                moved.IsCaptured = false;
                promotion.IsCaptured = true;
            }

            if (moved is King && Math.Abs(dst.Column - src.Column) == 2)
            {
                // Undo castling
                int dir = src.Column < dst.Column ? 1 : -1;     // 1: Kingside, -1: Queenside
                Rook r = GetPositionOccupier(new ChessPosition(src.Column + dir, dst.Row)) as Rook;
                r.Position.Column = dir == 1 ? ChessPosition.MaxColumn : ChessPosition.MinColumn;
                r.Position.IsMoved = false;
            }

            moved.Position.Column = src.Column;
            moved.Position.Row = src.Row;
            moved.Position.IsMoved = src.IsMoved;

            if (lastMove.CapturedPiece != null)
            {
                lastMove.CapturedPiece.IsCaptured = false;
            }

            return LastMoveNode != MovesHistory.First;
        }

        /// <summary>
        /// Redo last undone move.
        /// </summary>
        /// <returns>Wether any more redo is available.</returns>
        /// <exception cref="InvalidOperationException">There is no move to redo.</exception>
        public bool Redo()
        {
            if (LastMoveNode.Next is null)
            {
                throw new InvalidOperationException("There is no move to redo.");
            }
            LastMoveNode = LastMoveNode.Next;
            ChessMove lastMove = LastMoveNode.Value;
            ChessPosition src = lastMove.Source;
            ChessPosition dst = lastMove.Destination;
            ChessPiece moved = lastMove.MovedPiece;

            if (dst is null)
            {
                Winner = lastMove.Player;
                Ended = true;
                return false;
            }

            ToggleTurn();
            moved.Position.Column = dst.Column;
            moved.Position.Row = dst.Row;
            if (lastMove.CapturedPiece != null)
            {
                lastMove.CapturedPiece.IsCaptured = true;
            }

            if (!string.IsNullOrEmpty(lastMove.Symbols) && lastMove.Symbols[0] == '=')
            {
                // Redo pawn promotion
                char letter = lastMove.Symbols[1];
                ChessPiece promotion = Pieces
                    .First(p => p.Player == moved.Player && p.Letter == letter && p.Position == dst);

                promotion.IsCaptured = false;
                moved.IsCaptured = true;
            }

            if (moved is King && Math.Abs(dst.Column - src.Column) == 2)
            {
                // Redo castling
                if (src.Column < dst.Column)
                {
                    // Kingside
                    GetPositionOccupier(new ChessPosition(ChessPosition.MaxColumn, dst.Row))
                        .Position.Column = dst.Column - 1;
                }
                else
                {
                    // Queenside
                    GetPositionOccupier(new ChessPosition(ChessPosition.MinColumn, dst.Row))
                        .Position.Column = dst.Column + 1;
                }
            }

            Ended = !HasValidMoves(Turn);
            if (lastMove.Symbols.IndexOf('#') > -1)
            {
                Winner = lastMove.Player;
            }
            return LastMoveNode.Next != null;
        }

        /// <summary>
        /// Promote pawn to another piece.
        /// </summary>
        /// <param name="pawn">The pawn to be promoted.</param>
        /// <param name="promotion">The piece to replace <paramref name="pawn"/>.</param>
        /// <exception cref="ArgumentException">
        /// The <paramref name="promotion"/> is king or pawn.
        /// The <paramref name="pawn"/> is not at end of board.
        /// </exception>
        public void PromotePawn(Pawn pawn, ChessPiece promotion)
        {
            if (promotion is King || promotion is Pawn)
            {
                throw new ArgumentException("Cannot promote pawn to king or pawn.", nameof(promotion));
            }

            if (promotion.Player != pawn.Player)
            {
                throw new ArgumentException("Promotion piece has wrong player.", nameof(promotion));
            }

            if (!pawn.ShouldPromote())
            {
                throw new ArgumentException("Cannot promote pawn which is not at end of board.", nameof(pawn));
            }

            promotion.Position.Column = pawn.Position.Column;
            promotion.Position.Row = pawn.Position.Row;
            promotion.Position.IsMoved = true;

            Pieces.Add(promotion);
            pawn.IsCaptured = true;

            StringBuilder symbols = new StringBuilder();
            symbols.Append('=').Append(promotion.Letter);
            ChessPlayer opp = pawn.Player == ChessPlayer.White ? ChessPlayer.Black : ChessPlayer.White;
            bool inCheck = InCheck(opp);
            if (!HasValidMoves(opp))
            {
                // Either Stalemate or Checkmate
                if (inCheck)
                {
                    symbols.Append('#');
                    Winner = pawn.Player;
                }
                Ended = true;
            }
            else if (inCheck)
            {
                symbols.Append('+');
            }

            LastMoveNode.Value.Symbols = symbols.ToString() + LastMoveNode.Value.Symbols;
        }

        /// <summary>
        /// Returns notation for result of game.
        /// </summary>
        /// <returns>Notation for result of game.</returns>
        public string GetResultNotation()
        {
            if (!Ended)
            {
                return "*";
            }

            StringBuilder resultNotation = new StringBuilder();

            if (Winner is null)
            {
                resultNotation.Append("1/2-1/2");
            }
            else
            {
                resultNotation.Append(Winner == ChessPlayer.White ? 1 : 0);
                resultNotation.Append('-');
                resultNotation.Append(Winner == ChessPlayer.Black ? 1 : 0);
            }

            return resultNotation.ToString();
        }
    }
}