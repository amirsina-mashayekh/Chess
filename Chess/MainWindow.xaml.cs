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
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            int cols = ChessPosition.MaxColumn - ChessPosition.MinColumn + 1;
            int rows = ChessPosition.MaxRow - ChessPosition.MinRow + 1;
            BoardCanvas.Width = squareSize * cols;
            BoardCanvas.Height = squareSize * rows;

            squares = new Rectangle[cols, rows];

            // Draw checkboard
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

            InitNewGame();
        }

        private void BoardCanvas_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (board.Ended || animationRunning)
            {
                e.Handled = true;
                return;
            }
            SelectedPiece = null;
        }

        private void Piece_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ChessPiece piece = pieces.Single(p => p.Value == sender).Key;
            if (piece.Player != board.Turn) return;
            SelectedPiece = piece;
        }

        private async void AvailableMove_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Rectangle move = sender as Rectangle;
            int col = (int)(Canvas.GetLeft(move) / squareSize) + 1;
            int row = (int)(Canvas.GetBottom(move) / squareSize) + 1;

            board.MovePiece(SelectedPiece, col, row);

            AddLastMoveToHistory();

            await UpdateBoard(true);
        }

        private async void PrevMoveButton_Click(object sender, RoutedEventArgs e)
        {
            if (animationRunning) return;
            board.Undo();
            await UpdateBoard(true);
            MovesHistory.SelectedIndex--;
        }

        private async void NextMoveButton_Click(object sender, RoutedEventArgs e)
        {
            if (animationRunning) return;
            board.Redo();
            await UpdateBoard(true);
            MovesHistory.SelectedIndex++;
        }

        private void MovesHistory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(MovesHistory.SelectedItem is ListViewItem item))
                return;

            ListViewItem prev = e.RemovedItems.Count > 0 ? e.RemovedItems[0] as ListViewItem : null;

            LinkedListNode<ChessMove> moveNode = item.Tag as LinkedListNode<ChessMove>;

            bool redo = prev is null || MovesHistory.Items.IndexOf(prev) < MovesHistory.SelectedIndex;

            while (board.LastMoveNode != moveNode)
            {
                if (redo) board.Redo();
                else board.Undo();
                UpdateBoard().Wait();
            }
        }

        private void NewGameButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(
                "Are you sure you want to start a new game?\nAll data of current game will be lost.",
                "New Game", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);
            
            if (result == MessageBoxResult.Yes)
                InitNewGame();
        }
    }
}