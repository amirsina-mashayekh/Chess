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
    public enum PiecePlayer
    {
        White,
        Black
    }

    /// <summary>
    /// Represents the position of piece in board.
    /// </summary>
    public class ChessPosition : IEquatable<ChessPosition>
    {
        /// <summary>
        /// Minimum value of file (column).
        /// </summary>
        public const int MinFile = 1;

        /// <summary>
        /// Maximum value of file (column).
        /// </summary>
        public const int MaxFile = 8;

        /// <summary>
        /// Gets the file (column) value of this <c>ChessPosition</c>.
        /// </summary>
        public int File { get; }

        /// <summary>
        /// Minimum value of rank (row).
        /// </summary>
        public const int MinRank = 1;

        /// <summary>
        /// Maximum value of rank (row).
        /// </summary>
        public const int MaxRank = 8;

        /// <summary>
        /// Gets the rank (row) value of this <c>Position</c>.
        /// </summary>
        public int Rank { get; }

        /// <summary>
        /// Initializes a new instance of the <c>Position</c> class.
        /// </summary>
        /// <param name="file">The file (column) of where piece is located.</param>
        /// <param name="rank">The rank (row) of where piece is located.</param>
        public ChessPosition(int file, int rank)
        {
            if (file < MinFile || file > MaxFile)
            {
                throw new ArgumentOutOfRangeException(nameof(file),
                    $"Chess position file must be between {MinFile} and {MaxFile}.");
            }
            if (rank < MinRank || rank > MaxRank)
            {
                throw new ArgumentOutOfRangeException(nameof(rank),
                    $"Chess piece position rank must be between {MinFile} and {MaxFile}.");
            }

            File = file;
            Rank = rank;
        }

        /// <summary>
        /// Returns the string representation of position.
        /// </summary>
        /// <returns>The string representation of position.</returns>
        public override string ToString()
        {
            return (char)(File + 'a' - 1) + Rank.ToString();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ChessPosition);
        }

        public bool Equals(ChessPosition other)
        {
            return other != null &&
                   File == other.File &&
                   Rank == other.Rank;
        }

        public override int GetHashCode()
        {
            int hashCode = 656739706;
            hashCode = hashCode * -1521134295 + File.GetHashCode();
            hashCode = hashCode * -1521134295 + Rank.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(ChessPosition left, ChessPosition right)
        {
            return EqualityComparer<ChessPosition>.Default.Equals(left, right);
        }

        public static bool operator !=(ChessPosition left, ChessPosition right)
        {
            return !(left == right);
        }
    }

    /// <summary>
    /// Base class for chess pieces.
    /// </summary>
    internal abstract class ChessPiece
    {
        /// <summary>
        /// Gets the player which the piece belongs to.
        /// </summary>
        public PiecePlayer Player { get; }
        
        /// <summary>
        /// Gets or Sets the position of piece.
        /// </summary>
        public virtual ChessPosition Position { get; set; }

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
        public abstract string FullName { get; }

        /// <summary>
        /// Gets letter representation of piece.
        /// </summary>
        public abstract char Letter { get; }

        /// <summary>
        /// Initializes a new instance of the <c>ChessPiece</c> class.
        /// </summary>
        /// <param name="player">The player which the piece belongs to.</param>
        /// <param name="position">The position of piece.</param>
        public ChessPiece(PiecePlayer player, ChessPosition position)
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
        /// Returns the available moves for current piece.
        /// </summary>
        /// <returns>The available moves for current piece.</returns>
        public abstract List<ChessPosition> GetAvailableMoves();
    }
}
