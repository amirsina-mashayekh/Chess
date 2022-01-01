using Chess.ChessUtil.ChessPieces;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Chess.ChessUtil
{
    public partial class ChessBoard
    {
        /// <summary>
        /// Converts board info to Portable Game Notation(PGN) format.
        /// </summary>
        /// <param name="matchEvent">Name of the tournament or match event.</param>
        /// <param name="matchSite">Location of the event.</param>
        /// <param name="round">Playing round ordinal of the game within the event.</param>
        /// <param name="wLName">Last name of player of the white pieces.</param>
        /// <param name="wFName">First name of player of the white pieces.</param>
        /// <param name="bLName">Last name of player of the black pieces.</param>
        /// <param name="bFName">First name of player of the black pieces.</param>
        /// <returns>An array of strings containing lines of PGN.</returns>
        public string[] ToPGN(string matchEvent, string matchSite, string round,
            string wLName, string wFName, string bLName, string bFName)
        {
            ChessBoard board = MemberwiseClone() as ChessBoard;
            string[] PGN = new string[9]
            {
                matchEvent, matchSite, ".", round, wLName, wFName, bLName, bFName, "."
            };
            string result = board.GetResultNotation();

            for (int i = 0; i < 8; i++)
            {
                if (string.IsNullOrWhiteSpace(PGN[i]))
                {
                    PGN[i] = "??";
                }
            }

            PGN[0] = $@"[Event ""{PGN[0]}""]";
            PGN[1] = $@"[Site ""{PGN[1]}""]";
            PGN[2] = $@"[Date ""{StartTime.ToString("yyyy.MM.dd", CultureInfo.InvariantCulture)}""]";
            PGN[3] = $@"[Round ""{PGN[3]}""]";
            PGN[4] = $@"[White ""{PGN[4]}, {PGN[5]}""]";
            PGN[5] = $@"[Black ""{PGN[6]}, {PGN[7]}""]";
            PGN[6] = $@"[Result ""{result}""]";
            PGN[7] = "";

            StringBuilder moves = new StringBuilder();
            int moveCounter = 1;
            if (board.MovesHistory.First.Next != null)
            {
                while (board.Undo())
                {
                    ;
                }

                LinkedListNode<ChessMove> last = board.MovesHistory.Last;
                if (last.Value.Destination is null)
                {
                    last = last.Previous;
                }

                while (board.LastMoveNode != last)
                {
                    board.Redo();
                    ChessMove lastMove = board.LastMoveNode.Value;
                    ChessPosition src = lastMove.Source;
                    ChessPosition dst = lastMove.Destination;
                    ChessPiece moved = lastMove.MovedPiece;

                    if (lastMove.Player == ChessPlayer.White)
                    {
                        moves.Append($"{moveCounter}. ");
                        moveCounter++;
                    }
                    string notation = lastMove.ToAN();

                    board.Undo();
                    ChessPiece[] ambiguousPieces =
                        board.Pieces.Where(p => p != moved
                                          && p.GetType() == moved.GetType()
                                          && p.Player == moved.Player
                                          && ValidMoves(p).Any(m => m == dst)).ToArray();
                    if (ambiguousPieces.Length > 0)
                    {
                        if (!ambiguousPieces.Any(p => p.Position.Column == src.Column))
                        {
                            if (moved is Pawn)
                            {
                                moves.Append(notation);
                            }
                            else
                            {
                                moves.Append(notation.Insert(1, src.File.ToString()));
                            }
                        }
                        else if (!ambiguousPieces.Any(p => p.Position.Row == src.Row))
                        {
                            moves.Append(notation.Insert(1, src.Rank.ToString()));
                        }
                        else
                        {
                            moves.Append(notation.Insert(1, src.File.ToString()));
                            moves.Append(notation.Insert(2, src.Rank.ToString()));
                        }
                    }
                    else
                    {
                        moves.Append(notation);
                    }

                    board.Redo();
                    moves.Append(' ');
                }
            }

            moves.Append(result);
            PGN[8] = moves.ToString();

            return PGN;
        }
    }
}