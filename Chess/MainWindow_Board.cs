using Chess.ChessUtil;
using Chess.ChessUtil.ChessPieces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Chess
{
    /// <summary>
    /// Methods and fields related to chess board.
    /// </summary>
    public partial class MainWindow
    {
        /// <summary>
        /// Size of chessboard squares (width and height).
        /// </summary>
        private const double squareSize = 100;

        /// <summary>
        /// Brush of white squares on chessboard.
        /// </summary>
        public static readonly Brush whiteSquaresBrush = new SolidColorBrush(Color.FromArgb(255, 238, 238, 210));

        /// <summary>
        /// Brush of black squares on chessboard.
        /// </summary>
        public static readonly Brush blackSquaresBrush = new SolidColorBrush(Color.FromArgb(255, 118, 150, 86));

        /// <summary>
        /// Brush of <c>SelectedPiece</c>'s square.
        /// </summary>
        public static readonly Brush selectedSquareBrush = new SolidColorBrush(Color.FromArgb(128, 0, 127, 255));

        /// <summary>
        /// Brush of highlighted squares (available moves for <c>SelectedPiece</c>).
        /// </summary>
        public static readonly Brush availableSquareBrush = new SolidColorBrush(Color.FromArgb(64, 255, 255, 0));

        /// <summary>
        /// Brush of in check square.
        /// </summary>
        public static readonly Brush inCheckSquareBrush = new SolidColorBrush(Color.FromArgb(128, 255, 0, 0));

        /// <summary>
        /// <c>ChessBoard</c> object which controls game logics and saves states.
        /// </summary>
        private readonly ChessBoard board = new ChessBoard();

        /// <summary>
        /// A dictionary containing all pieces and corresponding graphical representations.
        /// </summary>
        private readonly Dictionary<ChessPiece, Viewbox> pieces = new Dictionary<ChessPiece, Viewbox>();

        private ChessPiece _selectedPiece;

        /// <summary>
        /// The <c>ChessPiece</c> which is selected by a player.
        /// </summary>
        private ChessPiece SelectedPiece
        {
            get => _selectedPiece;
            set
            {
                if (value is null)
                {
                    for (int i = 0; i < BoardCanvas.Children.Count; i++)
                    {
                        if (!(BoardCanvas.Children[i] is Rectangle rect)) continue;

                        if (rect.Fill == selectedSquareBrush || rect.Fill == availableSquareBrush)
                        {
                            BoardCanvas.Children.Remove(rect);
                            i--;
                        }
                    }
                    return;
                }

                _selectedPiece = value;

                Rectangle highlightActive = new Rectangle()
                {
                    Width = squareSize,
                    Height = squareSize,
                    Fill = selectedSquareBrush
                };
                BoardCanvas.Children.Add(highlightActive);
                Panel.SetZIndex(highlightActive, 2);
                SetPosition(highlightActive, value.Position);

                List<ChessPosition> validMoves = board.ValidMoves(value);
                foreach (ChessPosition move in validMoves)
                {
                    Rectangle avail = new Rectangle()
                    {
                        Width = squareSize,
                        Height = squareSize,
                        Fill = availableSquareBrush
                    };
                    BoardCanvas.Children.Add(avail);
                    Panel.SetZIndex(avail, 4);
                    SetPosition(avail, move);
                    avail.MouseLeftButtonDown += AvailableMove_MouseLeftButtonDown;
                }
            }
        }

        /// <summary>
        /// Array of all squares on chessboard.
        /// </summary>
        private Rectangle[,] squares;

        /// <summary>
        /// Indicates whether an animation is running on chessboard.
        /// </summary>
        private bool animationRunning;

        /// <summary>
        /// Convers a <c>ChessPosition</c> to corresponding square on chessboard.
        /// </summary>
        /// <param name="position">The position to be converted.</param>
        /// <returns>The square on chessboard which is in <paramref name="position"/>.</returns>
        public Rectangle ChessPositionToSquare(ChessPosition position)
        {
            return squares[position.Column - 1, position.Row - 1];
        }

        /// <summary>
        /// Sets position of a <c>UIElement</c> to a position on chessboard.
        /// </summary>
        /// <param name="element">The element to be moved.</param>
        /// <param name="position">The position where <paramref name="element"/> should be moved to.</param>
        public void SetPosition(UIElement element, ChessPosition position)
        {
            Rectangle square = ChessPositionToSquare(position);
            Canvas.SetBottom(element, Canvas.GetBottom(square));
            Canvas.SetLeft(element, Canvas.GetLeft(square));
        }

        /// <summary>
        /// Moves a chess piece to a position animatedly.
        /// </summary>
        /// <param name="piece">Graphical representation of the piece to be moved.</param>
        /// <param name="position">The position where <paramref name="piece"/> should be moved to.</param>
        /// <returns>A <c>Task</c> object.</returns>
        public async Task MovePieceToPosition(Viewbox piece, ChessPosition position)
        {
            Rectangle square = ChessPositionToSquare(position);
            int z = Panel.GetZIndex(square);
            Panel.SetZIndex(square, z + 1);
            await Task.WhenAll(new Task[]
            {
                AnimateAsync(Canvas.GetBottom(square), piece, Canvas.BottomProperty, TimeSpan.FromMilliseconds(250)),
                AnimateAsync(Canvas.GetLeft(square), piece, Canvas.LeftProperty, TimeSpan.FromMilliseconds(250))
            });
            Panel.SetZIndex(square, z);
        }

        /// <summary>
        /// Changes a double-typed property of an element animatedly.
        /// </summary>
        /// <param name="end">The final value of property.</param>
        /// <param name="element">The element which should be animated.</param>
        /// <param name="property">The property which should be animated. Must use double value.</param>
        /// <param name="time">The time animation should take in milliseconds.</param>
        /// <returns>A <c>Task</c> object.</returns>
        /// <exception cref="ArgumentOutOfRangeException">When <c>time</c> is less than or equal to 0.</exception>
        /// <exception cref="ArgumentException">When <c>property.PropertyType</c> isn't double.</exception>
        private async Task AnimateAsync(double end, UIElement element, DependencyProperty property, TimeSpan time)
        {
            if (time.Ticks <= 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            if (property.PropertyType.FullName != "System.Double")
            {
                throw new ArgumentException();
            }

            animationRunning = true;
            TaskCompletionSource<bool> acs = new TaskCompletionSource<bool>();

            Storyboard storyboard = new Storyboard()
            {
                FillBehavior = FillBehavior.Stop
            };

            DoubleAnimationUsingKeyFrames animation = new DoubleAnimationUsingKeyFrames();
            _ = animation.KeyFrames.Add(new EasingDoubleKeyFrame(end, KeyTime.FromTimeSpan(time), new SineEase()));
            storyboard.Children.Add(animation);

            Storyboard.SetTarget(storyboard, element);
            Storyboard.SetTargetProperty(storyboard, new PropertyPath(property));
            storyboard.Completed += (sender, e) => acs.SetResult(true);

            storyboard.Begin();

            await acs.Task;

            element.SetValue(property, end);
            animationRunning = false;
        }

        /// <summary>
        /// Updates graphical representation of chessboard based on <c>board</c> object status.
        /// </summary>
        /// <param name="animate">Indicated if pieces should be moved animatedly.</param>
        /// <returns>A <c>Task</c> object.</returns>
        public async Task UpdateBoard(bool animate)
        {
            PrevMoveButton.IsEnabled = board.LastMoveNode.Previous != null;
            NextMoveButton.IsEnabled = board.LastMoveNode.Next != null;

            SelectedPiece = null;
            // Clear previous in check squares
            for (int i = 0; i < BoardCanvas.Children.Count; i++)
            {
                if (!(BoardCanvas.Children[i] is Rectangle rect)) continue;

                if (rect.Fill == inCheckSquareBrush)
                {
                    BoardCanvas.Children.Remove(rect);
                    i--;
                }
            }

            // Perform moves and add new pieces
            foreach (ChessPiece piece in board.Pieces)
            {
                Viewbox pieceBox = pieces[piece];

                if (piece.IsCaptured) continue;

                if (!BoardCanvas.Children.Contains(pieceBox))
                {
                    BoardCanvas.Children.Add(pieceBox);
                    Panel.SetZIndex(pieceBox, 3);
                }

                Rectangle currentSQ = ChessPositionToSquare(piece.Position);
                if (Canvas.GetBottom(currentSQ) != Canvas.GetBottom(pieceBox)
                    || Canvas.GetLeft(currentSQ) != Canvas.GetLeft(pieceBox))
                {
                    if (animate)
                    {
                        await MovePieceToPosition(pieceBox, piece.Position);
                    }
                    else
                    {
                        SetPosition(pieceBox, piece.Position);
                    }
                }
            }

            // Remove captured pieces
            // Created separate loop so that pieces are removed after loop
            foreach (ChessPiece piece in board.Pieces)
            {
                Viewbox pieceBox = pieces[piece];

                if (piece.IsCaptured)
                {
                    BoardCanvas.Children.Remove(pieceBox);
                }
            }

            // Warn in check king
            King inCheck = null;
            if (board.InCheck(ChessPlayer.White))
            {
                inCheck = board.Pieces
                    .Single(p => p is King && p.Player == ChessPlayer.White) as King;
            }
            else if (board.InCheck(ChessPlayer.Black))
            {
                inCheck = board.Pieces
                    .Single(p => p is King && p.Player == ChessPlayer.Black) as King;
            }

            if (inCheck != null)
            {
                Rectangle inCheckSQ = new Rectangle()
                {
                    Width = squareSize,
                    Height = squareSize,
                    Fill = inCheckSquareBrush
                };
                BoardCanvas.Children.Add(inCheckSQ);
                SetPosition(inCheckSQ, inCheck.Position);
            }
        }

        /// <summary>
        /// Adds the last move to <c>MovesHistory</c>.
        /// </summary>
        public void AddLastMoveToHistory()
        {
            int index = MovesHistory.SelectedIndex + 1;
            DockPanel content = new DockPanel();
            TextBlock moveNumber = new TextBlock()
            {
                Text = Math.Ceiling((float)(index + 1) / 2).ToString() + '.',
                TextAlignment = TextAlignment.Left,
                Width = 50
            };
            content.Children.Add(moveNumber);
            DockPanel.SetDock(moveNumber, Dock.Left);
            TextBlock moveText = new TextBlock()
            {
                Text = board.LastMoveNode.Value.ToFAN(),
                TextAlignment = TextAlignment.Center,
                Margin = new Thickness(-moveNumber.Width, 0, 0, 0)
            };
            content.Children.Add(moveText);
            DockPanel.SetDock(moveText, Dock.Right);
            ListViewItem item = new ListViewItem
            {
                Content = content,
                Tag = board.LastMoveNode
            };

            MovesHistory.Items.Insert(index, item);
            MovesHistory.SelectedIndex++;
            index++;
            while (MovesHistory.Items.Count > index)
            {
                MovesHistory.Items.RemoveAt(index);
            }
            MovesHistory.ScrollIntoView(item);
        }
    }
}
