using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
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

        private static readonly Brush whiteSquaresBrush = new SolidColorBrush(Color.FromArgb(255, 238, 238, 210));

        private static readonly Brush blackSquaresBrush = new SolidColorBrush(Color.FromArgb(255, 118, 150, 86));

        private static readonly Brush activeSquareBrush = new SolidColorBrush(Color.FromArgb(128, 0, 127, 255));

        private static readonly Brush highlightSquareBrush = new SolidColorBrush(Color.FromArgb(64, 255, 255, 0));

        private readonly ChessBoard board = new ChessBoard();

        private readonly Dictionary<ChessPiece, Viewbox> pieces = new Dictionary<ChessPiece, Viewbox>();

        private ChessPiece activePiece;

        private Rectangle[,] squares;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        public Rectangle ChessPositionToSquare(ChessPosition position)
        {
            return squares[position.Column - 1, position.Row - 1];
        }

        public void MoveToPosition(UIElement element, ChessPosition position)
        {
            Rectangle square = ChessPositionToSquare(position);
            Canvas.SetBottom(element, Canvas.GetBottom(square));
            Canvas.SetLeft(element, Canvas.GetLeft(square));
        }

        public void ClearHighlights()
        {
            for (int i = 0; i < BoardCanvas.Children.Count; i++)
            {
                if (!(BoardCanvas.Children[i] is Rectangle rect)) { continue; }

                if (rect.Fill == activeSquareBrush || rect.Fill == highlightSquareBrush)
                {
                    BoardCanvas.Children.Remove(rect);
                    i--;
                }
            }
        }

        public void UpdateBoard()
        {
            foreach (ChessPiece piece in board.Pieces)
            {
                Viewbox pieceBox = pieces[piece];

                if (piece.IsCaptured)
                {
                    BoardCanvas.Children.Remove(pieceBox);
                    continue;
                }

                if (!BoardCanvas.Children.Contains(pieceBox))
                {
                    BoardCanvas.Children.Add(pieceBox);
                    Panel.SetZIndex(pieceBox, 2);
                }

                MoveToPosition(pieceBox, piece.Position);
            }
        }

        private void BoardCanvas_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ClearHighlights();
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
                }
            }

            foreach (ChessPiece piece in board.Pieces)
            {
                TextBlock symbol = new TextBlock()
                {
                    Text = piece.Symbol.ToString(),
                    FontFamily = new FontFamily("Segoe UI Symbol")
                };

                Viewbox box = new Viewbox()
                {
                    Child = symbol,
                    Stretch = Stretch.Uniform,
                    Width = squareSize,
                    Height = squareSize
                };
                box.MouseLeftButtonDown += Piece_MouseLeftButtonDown;

                pieces.Add(piece, box);
            }

            UpdateBoard();
        }

        private void Piece_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ChessPiece piece = pieces.Single(p => p.Value == sender).Key;
            if (piece.Player != board.Turn) { return; }
            activePiece = piece;
            Rectangle highlightActive = new Rectangle()
            {
                Width = squareSize,
                Height = squareSize,
                Fill = activeSquareBrush
            };
            BoardCanvas.Children.Add(highlightActive);
            Panel.SetZIndex(highlightActive, 1);
            MoveToPosition(highlightActive, piece.Position);

            List<ChessPosition> validMoves = board.ValidMoves(piece);
            foreach (ChessPosition move in validMoves)
            {
                Rectangle avail = new Rectangle()
                {
                    Width = squareSize,
                    Height = squareSize,
                    Fill = highlightSquareBrush
                };
                BoardCanvas.Children.Add(avail);
                Panel.SetZIndex(avail, 3);
                MoveToPosition(avail, move);
                avail.MouseLeftButtonDown += AvailableMove_MouseLeftButtonDown;
            }
        }

        private void AvailableMove_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Rectangle move = sender as Rectangle;
            int col = (int)(Canvas.GetLeft(move) / squareSize) + 1;
            int row = (int)(Canvas.GetBottom(move) / squareSize) + 1;
            board.MovePiece(activePiece, col, row);
            UpdateBoard();
            ClearHighlights();
        }
    }
}