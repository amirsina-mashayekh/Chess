﻿using Chess.ChessUtil.ChessPieces;
using System;
using System.Text;

namespace Chess.ChessUtil
{
    /// <summary>
    /// Represents a move in chess board.
    /// </summary>
    public class ChessMove
    {
        private readonly ChessPosition _source;

        private readonly ChessPosition _destination;

        /// <summary>
        /// Source of move.
        /// </summary>
        public ChessPosition Source => _source.Copy();

        /// <summary>
        /// Destination of move.
        /// </summary>
        public ChessPosition Destination => _destination.Copy();

        /// <summary>
        /// The piece which is moved.
        /// </summary>
        public ChessPiece MovedPiece { get; }

        /// <summary>
        /// The piece which is captured while this move.
        /// <c>null</c> if no piece is captured.
        /// </summary>
        public ChessPiece CapturedPiece { get; }

        /// <summary>
        /// Extra information about move.
        /// </summary>
        public string Symbols { get; set; }

        /// <summary>
        /// The player who performed the move.
        /// </summary>
        public ChessPlayer Player => MovedPiece.Player;

        /// <summary>
        /// Initializes a new instance of the <c>ChessMove</c> class.
        /// </summary>
        /// <param name="source">The source of move.</param>
        /// <param name="movedPiece">The piece which is moved.</param>
        /// <param name="capturedPiece">
        /// The piece which is captured while this move.
        /// Pass <c>null</c> if no piece is captured.
        /// </param>
        public ChessMove(ChessPosition source, ChessPiece movedPiece, ChessPiece capturedPiece)
        {
            _source = source.Copy();
            MovedPiece = movedPiece;
            _destination = movedPiece.Position.Copy();
            CapturedPiece = capturedPiece;
        }

        public override string ToString()
        {
            StringBuilder str = new StringBuilder();
            str.Append(Player.ToString()[0]);
            str.Append(MovedPiece.Letter).Append(Source);

            if (CapturedPiece != null)
            {
                str.Append('x').Append(CapturedPiece);
            }
            str.Append(Destination);

            str.Append(Symbols);

            return str.ToString();
        }

        public string ToSAN()
        {
            if (MovedPiece is King && Math.Abs(Source.Column - Destination.Column) == 2)
            {
                return Symbols;
            }

            StringBuilder str = new StringBuilder();

            if (!(MovedPiece is Pawn)) str.Append(MovedPiece.Letter);
            str.Append(Source);

            if (CapturedPiece != null)
            {
                str.Append('x');
                if (!(CapturedPiece is Pawn)) str.Append(CapturedPiece.Letter);
            }
            str.Append(Destination);

            str.Append(Symbols);

            return str.ToString();
        }

        public string ToFAN()
        {
            string str = ToSAN().Replace(MovedPiece.Letter, MovedPiece.Symbol);
            if (CapturedPiece != null)
            {
                return str.Replace(CapturedPiece.Letter, CapturedPiece.Symbol);
            }
            return str;
        }
    }
}