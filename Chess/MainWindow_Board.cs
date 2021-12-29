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
        private static readonly Brush selectedSquareBrush = new SolidColorBrush(Color.FromArgb(128, 0, 127, 255));

        /// <summary>
        /// Brush of highlighted squares (available moves for <c>SelectedPiece</c>).
        /// </summary>
        private static readonly Brush availableSquareBrush = new SolidColorBrush(Color.FromArgb(64, 255, 255, 0));

        /// <summary>
        /// Brush of in check square.
        /// </summary>
        private static readonly Brush inCheckSquareBrush = new SolidColorBrush(Color.FromArgb(128, 255, 0, 0));

        /// <summary>
        /// <c>ChessBoard</c> object which controls game logics and saves states.
        /// </summary>
        private ChessBoard board = new ChessBoard();

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
        /// Piece to replace pawn in next promotion.
        /// </summary>
        private ChessPiece promotionPiece = null;

        /// <summary>
        /// Initializes a new game.
        /// </summary>
        private void InitNewGame()
        {
            board = new ChessBoard();
            foreach (Viewbox piece in pieces.Values)
            {
                BoardCanvas.Children.Remove(piece);
            }
            pieces.Clear();
            MovesHistory.Items.Clear();

            // Draw pieces
            foreach (ChessPiece piece in board.Pieces)
            {
                pieces.Add(piece, GetGraphicalChessPiece(piece));
            }

            UpdateBoard().Wait();
        }

        /// <summary>
        /// Converts a chess piece to graphical form.
        /// </summary>
        /// <param name="piece">The piece to be converted.</param>
        /// <returns>A <c>Viewbox</c> which represents <paramref name="piece"/> graphically.</returns>
        private Viewbox GetGraphicalChessPiece(ChessPiece piece)
        {
            TextBlock symbol = new TextBlock()
            {
                Text = piece.Symbol.ToString(),
                FontFamily = new FontFamily("Segoe UI Symbol"),
                FontSize = 24,
                TextAlignment = TextAlignment.Center,
                Width = 24,
                Height = 29
            };
            symbol.Margin = new Thickness(0, symbol.Width - symbol.Height, 0, 0);

            Viewbox box = new Viewbox()
            {
                Child = symbol,
                Stretch = Stretch.Uniform,
                Width = squareSize,
                Height = squareSize,
                ClipToBounds = true
            };
            box.MouseLeftButtonDown += Piece_MouseLeftButtonDown;

            return box;
        }

        /// <summary>
        /// Convers a <c>ChessPosition</c> to corresponding square on chessboard.
        /// </summary>
        /// <param name="position">The position to be converted.</param>
        /// <returns>The square on chessboard which is in <paramref name="position"/>.</returns>
        private Rectangle ChessPositionToSquare(ChessPosition position)
        {
            return squares[position.Column - 1, position.Row - 1];
        }

        /// <summary>
        /// Sets position of a <c>UIElement</c> to a position on chessboard.
        /// </summary>
        /// <param name="element">The element to be moved.</param>
        /// <param name="position">The position where <paramref name="element"/> should be moved to.</param>
        private void SetPosition(UIElement element, ChessPosition position)
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
        private async Task MovePieceToPosition(Viewbox piece, ChessPosition position)
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
        private async Task UpdateBoard(bool animate = false)
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

            // Add new pieces
            foreach (KeyValuePair<ChessPiece, Viewbox> piece in pieces)
            {
                if (!piece.Key.IsCaptured && !BoardCanvas.Children.Contains(piece.Value))
                {
                    BoardCanvas.Children.Add(piece.Value);
                    Panel.SetZIndex(piece.Value, 3);
                }
            }

            // Perform moves
            foreach (ChessPiece piece in board.Pieces)
            {
                Viewbox pieceBox = pieces[piece];

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

            // Check for pawn promotions
            foreach (ChessPiece piece in board.Pieces)
            {
                if (piece is Pawn p && !p.IsCaptured && p.ShouldPromote()
                    && !board.LastMoveNode.Value.Symbols.StartsWith("="))    // If there is "=" in history, last move was a redo
                {
                    if (promotionPiece is null)
                    {
                        ChessPosition pos = p.Position.Copy();
                        ChessPlayer pl = p.Player;
                        ChessPiece[] options = new ChessPiece[]
                        {
                            new Queen(pl, pos),
                            new Rook(pl, pos),
                            new Bishop(pl, pos),
                            new Knight(pl, pos),
                        };
                        PawnPromotionGrid.Visibility = Visibility.Visible;
                        for (int i = 0; i < options.Length; i++)
                        {
                            PromotionOptionsGrid.ColumnDefinitions.Add(new ColumnDefinition());
                            Button option = new Button()
                            {
                                Content = options[i].Symbol,
                                FontSize = 48,
                                Tag = options[i]
                            };
                            option.Click += PromotionOption_Click;
                            PromotionOptionsGrid.Children.Add(option);
                            Grid.SetColumn(option, i);
                        }
                        return;
                    }

                    Viewbox pg = GetGraphicalChessPiece(promotionPiece);
                    pieces.Add(promotionPiece, pg);
                    board.PromotePawn(p, promotionPiece);
                    BoardCanvas.Children.Add(pg);
                    SetPosition(pg, promotionPiece.Position);
                    Panel.SetZIndex(pg, 3);
                    promotionPiece = null;
                    break;
                }
            }

            // Remove captured pieces
            // Created separate loop so that pieces are removed after animation
            foreach (ChessPiece piece in board.Pieces)
            {
                if (piece.IsCaptured)
                {
                    BoardCanvas.Children.Remove(pieces[piece]);
                }
            }

            // Warn in check king
            King inCheck = null;
            ChessMove lastMove = board.LastMoveNode.Value;
            if (lastMove != null && lastMove.Symbols.IndexOfAny(new char[] {'+', '#'}) > 0)
            {
                inCheck = board.Pieces
                    .Single(p => p is King && p.Player == board.Turn) as King;
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

        private void PromotionOption_Click(object sender, RoutedEventArgs e)
        {
            promotionPiece = (sender as Button).Tag as ChessPiece;
            PawnPromotionGrid.Visibility = Visibility.Collapsed;
            UpdateBoard().Wait();
        }

        /// <summary>
        /// Adds the last move to <c>MovesHistory</c>.
        /// </summary>
        private void AddLastMoveToHistory()
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
