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
    /// Represents the position of piece in chess board.
    /// </summary>
    public class ChessPosition : IEquatable<ChessPosition>
    {
        /// <summary>
        /// Minimum value of column.
        /// </summary>
        public const int MinColumn = 1;

        /// <summary>
        /// Maximum value of column.
        /// </summary>
        public const int MaxColumn = 8;

        private int _column;

        /// <summary>
        /// Gets or sets the column value of this <c>ChessPosition</c>.
        /// </summary>
        public int Column
        {
            get { return _column; }
            set
            {
                if (value < MinColumn || value > MaxColumn)
                {
                    throw new ArgumentOutOfRangeException(nameof(value),
                        $"Chess position column must be between {MinColumn} and {MaxColumn}.");
                }
                _column = value;
                IsMoved = true;
            }
        }

        /// <summary>
        /// Gets or sets the file of this <c>ChessPosition</c>.
        /// </summary>
        public char File
        {
            get { return ColumnToFile(_column); }
            set
            {
                Column = FileToColumn(value);
            }
        }

        /// <summary>
        /// Minimum value of row.
        /// </summary>
        public const int MinRow = 1;

        /// <summary>
        /// Maximum value of row.
        /// </summary>
        public const int MaxRow = 8;

        private int _row;

        /// <summary>
        /// Gets or sets the row value of this <c>Position</c>.
        /// </summary>
        public int Row
        {
            get { return _row; }
            set
            {
                if (value < MinRow || value > MaxRow)
                {
                    throw new ArgumentOutOfRangeException(nameof(value),
                        $"Chess piece position row must be between {MinColumn} and {MaxColumn}.");
                }
                _row = value;
                IsMoved = true;
            }
        }

        /// <summary>
        /// Gets or sets the rank of this <c>Position</c>.
        /// </summary>
        public int Rank
        {
            get { return _row; }
            set { Row = value; }
        }

        /// <summary>
        /// Gets or sets whether the position is changed from initial value.
        /// </summary>
        public bool IsMoved { get; set; }

        /// <summary>
        /// Initializes a new instance of the <c>Position</c> class by column and row number.
        /// </summary>
        /// <param name="column">The column of where piece is located.</param>
        /// <param name="row">The row of where piece is located.</param>
        public ChessPosition(int column, int row)
        {
            Column = column;
            Row = row;
            IsMoved = false;
        }

        /// <summary>
        /// Initializes a new instance of the <c>Position</c> class by file and rank.
        /// </summary>
        /// <param name="file">The file (column) of where piece is located.</param>
        /// <param name="rank">The rank (row) of where piece is located.</param>
        public ChessPosition(char file, int rank)
        {
            File = file;
            Rank = rank;
            IsMoved = false;
        }

        /// <summary>
        /// Returns the string representation of position.
        /// </summary>
        /// <returns>The string representation of position.</returns>
        public override string ToString()
        {
            return (char)(Column + 'a' - 1) + Row.ToString();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ChessPosition);
        }

        public bool Equals(ChessPosition other)
        {
            return other != null &&
                   Column == other.Column &&
                   Row == other.Row;
        }

        public override int GetHashCode()
        {
            int hashCode = 656739706;
            hashCode = hashCode * -1521134295 + Column.GetHashCode();
            hashCode = hashCode * -1521134295 + Row.GetHashCode();
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

        /// <summary>
        /// Converts a column to corresponding file.
        /// </summary>
        /// <param name="column">The number of column.</param>
        /// <returns>The file letter of <paramref name="column"/>.</returns>
        public static char ColumnToFile(int column)
        {
            return (char)('a' + column - 1);
        }

        /// <summary>
        /// Converts a file to corresponding column.
        /// </summary>
        /// <param name="file">The letter of file.</param>
        /// <returns>The column number of <paramref name="file"/>.</returns>
        public static int FileToColumn(char file)
        {
            int column = file - 'a' + 1;
            return column;
        }
    }

    /// <summary>
    /// Base class for chess pieces.
    /// </summary>
    public abstract class ChessPiece
    {
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
        /// Returns the available moves for current piece.
        /// </summary>
        /// <returns>The available moves for current piece.</returns>
        public abstract List<ChessPosition> GetMoves();
    }
}
