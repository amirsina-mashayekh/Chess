using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Chess.ChessUtil.ChessPieces
{
    /// <summary>
    /// Specifies the player which the piece belongs to.
    /// </summary>
    public enum ChessPlayer
    {
        White, Black
    }

    public enum ChessPieceInfo
    {
        Letter = 0, Symbol = 1, Value = 2
    }

    /// <summary>
    /// Base class for chess pieces.
    /// </summary>
    public abstract class ChessPiece
    {
        public static ReadOnlyDictionary<Type, string> ChessPiecesInfo = new ReadOnlyDictionary<Type, string>(new Dictionary<Type, string>()
        {
            {typeof(King), "K\u2654" + (char)0},
            {typeof(Queen), "Q\u2655" + (char)9},
            {typeof(Rook), "R\u2656" + (char)5},
            {typeof(Bishop), "B\u2657" + (char)3},
            {typeof(Knight), "N\u2658" + (char)3},
            {typeof(Pawn), "P\u2659" + (char)1}
        });

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
        public int ValuePoints => ChessPiecesInfo[GetType()][(int)ChessPieceInfo.Value];

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
        public char Letter => ChessPiecesInfo[GetType()][(int)ChessPieceInfo.Letter];

        /// <summary>
        /// Gets symbol character of white colored piece.
        /// </summary>
        private char WhiteSymbol => ChessPiecesInfo[GetType()][(int)ChessPieceInfo.Symbol];

        /// <summary>
        /// Gets symbol character of piece.
        /// </summary>
        public char Symbol => Player == ChessPlayer.White ? WhiteSymbol : (char)(WhiteSymbol + 6);

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