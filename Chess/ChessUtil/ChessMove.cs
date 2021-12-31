using Chess.ChessUtil.ChessPieces;
using System;
using System.Linq;
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
        public ChessPosition Source => _source is null ? null : _source.Copy();

        /// <summary>
        /// Destination of move.
        /// </summary>
        public ChessPosition Destination => _destination is null ? null : _destination.Copy();

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
        public ChessPlayer? Player => MovedPiece is null ? null as ChessPlayer? : MovedPiece.Player;

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
            Symbols = "";
        }

        /// <summary>
        /// Initializes an empty instance of the <c>ChessMove</c> class.
        /// </summary>
        public ChessMove(King winner, string symbols)
        {
            _source = null;
            MovedPiece = winner;
            _destination = null;
            CapturedPiece = null;
            Symbols = symbols;
        }

        public override string ToString()
        {
            StringBuilder str = new StringBuilder();
            str.Append(Player.ToString()[0])
                .Append(MovedPiece.Letter)
                .Append(Source);

            if (CapturedPiece != null)
            {
                str.Append('x').Append(CapturedPiece.Letter);
            }
            str.Append(Destination).Append(Symbols);

            return str.ToString();
        }

        public string ToLAN()
        {
            if (_destination is null 
                || (MovedPiece is King && Math.Abs(_source.Column - _destination.Column) == 2))
            {
                return Symbols;
            }

            StringBuilder str = new StringBuilder();

            if (!(MovedPiece is Pawn)) str.Append(MovedPiece.Letter);
            str.Append(_source);

            if (CapturedPiece != null)
            {
                str.Append('x');
                if (!(CapturedPiece is Pawn)) str.Append(CapturedPiece.Letter);
            }
            str.Append(_destination).Append(Symbols);

            return str.ToString();
        }

        public string ToFAN()
        {
            StringBuilder str = new StringBuilder(ToLAN());
            foreach (string info in ChessPiece.ChessPiecesInfo.Values)
            {
                str.Replace(info[(int)ChessPieceInfo.Letter], info[(int)ChessPieceInfo.Symbol]);
            }
            return str.ToString();
        }
    }
}