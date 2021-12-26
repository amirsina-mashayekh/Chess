using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Chess.ChessPieces
{
    /// <summary>
    /// Specifies the player which the piece belongs to.
    /// </summary>
    public enum ChessPlayer
    {
        White,
        Black
    }

    /// <summary>
    /// Base class for chess pieces.
    /// </summary>
    public abstract class ChessPiece
    {
        public static Dictionary<Type, char> ChessPieceLetters = new Dictionary<Type, char>()
        {
            {typeof(King), 'K'},
            {typeof(Queen), 'Q'},
            {typeof(Rook), 'R'},
            {typeof(Bishop), 'B'},
            {typeof(Knight), 'N'},
            {typeof(Pawn), 'P'},
        };

        /// <summary>
        /// Gets the player which the piece belongs to.
        /// </summary>
        public ChessPlayer Player { get; }
        
        /// <summary>
        /// Gets the position of piece.
        /// </summary>
        public ChessPosition Position { get; }

        /// <summary>
        /// Gets the value of current piece.
        /// </summary>
        public abstract int ValuePoints { get; }

        /// <summary>
        /// Gets or sets whether the piece is captured.
        /// </summary>
        public virtual bool IsCaptured { get; set; }

        /// <summary>
        /// Gets the full name of piece.
        /// </summary>
        public string FullName => GetType().Name;

        /// <summary>
        /// Gets letter representation of piece.
        /// </summary>
        public char Letter => ChessPieceLetters[GetType()];

        /// <summary>
        /// Initializes a new instance of the <c>ChessPiece</c> class
        /// for <paramref name="player"/> in <paramref name="position"/>.
        /// </summary>
        /// <param name="player">The player which the piece belongs to.</param>
        /// <param name="position">The position of piece.</param>
        public ChessPiece(ChessPlayer player, ChessPosition position)
        {
            Player = player;
            Position = position;
            IsCaptured = false;
        }

        /// <summary>
        /// Returns the string representation of current piece.
        /// </summary>
        /// <returns>The string representation of current piece.</returns>
        public override string ToString()
        {
            return
                FullName + ", " +
                Player + ", " +
                (IsCaptured ? "Captured" : Position.ToString());
        }

        /// <summary>
        /// The short string representation of current piece including its letter and position.
        /// </summary>
        public string ShortString => Letter + Position.ToString();

        /// <summary>
        /// Returns the available moves for current piece.
        /// </summary>
        /// <returns>The available moves for current piece.</returns>
        public abstract List<ChessPosition> GetMoves();
    }
}
