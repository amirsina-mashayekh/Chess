using System;
using System.Collections.Generic;

namespace Chess.ChessUtil
{
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
        /// Minimum value of file.
        /// </summary>
        public static char MinFile => ColumnToFile(MinColumn);

        /// <summary>
        /// Maximum value of column.
        /// </summary>
        public const int MaxColumn = 8;

        /// <summary>
        /// Maximum value of file.
        /// </summary>
        public static char MaxFile => ColumnToFile(MaxColumn);

        private int _column;

        /// <summary>
        /// Gets or sets the column value of this <c>ChessPosition</c>.
        /// </summary>
        public int Column
        {
            get => _column;
            set
            {
                if (!ColumnIsValid(value))
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
            get => ColumnToFile(_column);
            set => Column = FileToColumn(value);
        }

        /// <summary>
        /// Minimum value of row.
        /// </summary>
        public const int MinRow = 1;

        /// <summary>
        /// Minimum value of rank.
        /// </summary>
        public static int MinRank => MinRow;

        /// <summary>
        /// Maximum value of row.
        /// </summary>
        public const int MaxRow = 8;

        /// <summary>
        /// Maximum value of rank.
        /// </summary>
        public static int MaxRank => MaxRow;

        private int _row;

        /// <summary>
        /// Gets or sets the row value of this <c>Position</c>.
        /// </summary>
        public int Row
        {
            get => _row;
            set
            {
                if (!RowIsValid(value))
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
            get => _row;
            set => Row = value;
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
        /// Returns a copy of this <c>ChessPosition</c> instance.
        /// </summary>
        /// <returns>A new <c>ChessPosition</c> instance with same properties with this instance.</returns>
        public ChessPosition Copy()
        {
            return new ChessPosition(_column, _row);
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

        /// <summary>
        /// Determines if <paramref name="column"/> is in board range.
        /// </summary>
        /// <param name="column">The column value to be checked.</param>
        /// <returns></returns>
        public static bool ColumnIsValid(int column)
        {
            return !(column < MinColumn || column > MaxColumn);
        }

        /// <summary>
        /// Determines if <paramref name="row"/> is in board range.
        /// </summary>
        /// <param name="row">The row value to be checked.</param>
        /// <returns></returns>
        public static bool RowIsValid(int row)
        {
            return !(row < MinRow || row > MaxRow);
        }
    }
}