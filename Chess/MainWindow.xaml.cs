using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Chess.ChessUtil;
using Chess.ChessUtil.ChessPieces;

namespace Chess
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const double squareSize = 100;

        public static readonly Brush whiteSquaresBrush = new SolidColorBrush(Color.FromArgb(255, 238, 238, 210));

        public static readonly Brush blackSquaresBrush = new SolidColorBrush(Color.FromArgb(255, 118, 150, 86));

        public static readonly Brush activeSquareBrush = new SolidColorBrush(Color.FromArgb(128, 0, 127, 255));

        public static readonly Brush availableSquareBrush = new SolidColorBrush(Color.FromArgb(64, 255, 255, 0));

        public static readonly Brush inCheckSquareBrush = new SolidColorBrush(Color.FromArgb(128, 255, 0, 0));

        private readonly ChessBoard board = new ChessBoard();

        private readonly Dictionary<ChessPiece, Viewbox> pieces = new Dictionary<ChessPiece, Viewbox>();

        private ChessPiece activePiece;

        private Rectangle[,] squares;

        private bool animationRunning;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        public Rectangle ChessPositionToSquare(ChessPosition position)
        {
            return squares[position.Column - 1, position.Row - 1];
        }

        public void SetPosition(UIElement element, ChessPosition position)
        {
            Rectangle square = ChessPositionToSquare(position);
            Canvas.SetBottom(element, Canvas.GetBottom(square));
            Canvas.SetLeft(element, Canvas.GetLeft(square));
        }

        public async Task MoveToPosition(Viewbox piece, ChessPosition position)
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

        public void ClearHighlights()
        {
            for (int i = 0; i < BoardCanvas.Children.Count; i++)
            {
                if (!(BoardCanvas.Children[i] is Rectangle rect)) continue;

                if (rect.Fill == activeSquareBrush || rect.Fill == availableSquareBrush)
                {
                    BoardCanvas.Children.Remove(rect);
                    i--;
                }
            }
        }

        public async Task UpdateBoard(bool animate)
        {
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
                        await MoveToPosition(pieceBox, piece.Position);
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

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            int cols = ChessPosition.MaxColumn - ChessPosition.MinColumn + 1;
            int rows = ChessPosition.MaxRow - ChessPosition.MinRow + 1;
            BoardCanvas.Width = squareSize * cols;
            BoardCanvas.Height = squareSize * rows;

            squares = new Rectangle[cols, rows];

            for (int i = 0; i < cols; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    Rectangle rect = new Rectangle()
                    {
                        Fill = (i + j) % 2 == 0 ? blackSquaresBrush : whiteSquaresBrush,
                        Width = squareSize,
                        Height = squareSize
                    };
                    BoardCanvas.Children.Add(rect);
                    Canvas.SetLeft(rect, i * squareSize);
                    Canvas.SetBottom(rect, j * squareSize);
                    Panel.SetZIndex(rect, 0);
                    squares[i, j] = rect;

                    if (j + 1 == ChessPosition.MinRow)
                    {
                        // Add file letters
                        TextBlock file = new TextBlock()
                        {
                            Text = ChessPosition.ColumnToFile(i + 1).ToString(),
                            FontSize = 18,
                            FontWeight = FontWeights.SemiBold,
                            TextAlignment = TextAlignment.Right,
                            Foreground = (i + j) % 2 == 0 ? whiteSquaresBrush : blackSquaresBrush,
                            Width = squareSize * 0.95
                        };
                        BoardCanvas.Children.Add(file);
                        Canvas.SetLeft(file, i * squareSize);
                        Canvas.SetBottom(file, j * squareSize + squareSize * 0.02);
                        Panel.SetZIndex(file, 1);
                    }

                    if (i + 1 == ChessPosition.MinRow)
                    {
                        // Add rank letters
                        TextBlock file = new TextBlock()
                        {
                            Text = (j + 1).ToString(),
                            FontSize = 18,
                            FontWeight = FontWeights.SemiBold,
                            TextAlignment = TextAlignment.Left,
                            Foreground = (i + j) % 2 == 0 ? whiteSquaresBrush : blackSquaresBrush,
                            Height = squareSize
                        };
                        BoardCanvas.Children.Add(file);
                        Canvas.SetLeft(file, i * squareSize + squareSize * 0.05);
                        Canvas.SetBottom(file, j * squareSize);
                        Panel.SetZIndex(file, 1);
                    }
                }
            }

            foreach (ChessPiece piece in board.Pieces)
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

                pieces.Add(piece, box);
            }

            UpdateBoard(false).Wait();
        }

        private void BoardCanvas_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (board.Ended || animationRunning)
            {
                e.Handled = true;
                return;
            }
            ClearHighlights();
        }

        private void Piece_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ChessPiece piece = pieces.Single(p => p.Value == sender).Key;
            if (piece.Player != board.Turn) return;
            activePiece = piece;
            Rectangle highlightActive = new Rectangle()
            {
                Width = squareSize,
                Height = squareSize,
                Fill = activeSquareBrush
            };
            BoardCanvas.Children.Add(highlightActive);
            Panel.SetZIndex(highlightActive, 2);
            SetPosition(highlightActive, piece.Position);

            List<ChessPosition> validMoves = board.ValidMoves(piece);
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

        private async void AvailableMove_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Rectangle move = sender as Rectangle;
            int col = (int)(Canvas.GetLeft(move) / squareSize) + 1;
            int row = (int)(Canvas.GetBottom(move) / squareSize) + 1;
            ClearHighlights();

            board.MovePiece(activePiece, col, row);

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
                Content =  content,
                Tag = board.LastMoveNode
            };
            item.Selected += HistoryItem_Selected;

            MovesHistory.Items.Insert(index, item);
            MovesHistory.SelectedIndex++;
            index++;
            while (MovesHistory.Items.Count > index)
            {
                MovesHistory.Items.RemoveAt(index);
            }
            MovesHistory.ScrollIntoView(item);

            await UpdateBoard(true);
        }

        private async void HistoryItem_Selected(object sender, RoutedEventArgs e)
        {
            ListViewItem item = sender as ListViewItem;
            LinkedListNode<ChessMove> moveNode = item.Tag as LinkedListNode<ChessMove>;
            LinkedListNode<ChessMove> currentMoveNode = board.LastMoveNode;

            bool undo = true;
            // Check if selected node is before or after current node
            while (currentMoveNode != moveNode)
            {
                currentMoveNode = currentMoveNode.Previous;
                if (currentMoveNode.Previous == null)
                {
                    undo = false;
                    break;
                }
            }
            
            while (board.LastMoveNode != moveNode)
            {
                if (undo) board.Undo();
                else board.Redo();
                await UpdateBoard(false);
            }
        }
    }
}