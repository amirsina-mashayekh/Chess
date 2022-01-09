using Chess.ChessUtil;
using Chess.ChessUtil.ChessPieces;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

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
                            Width = squareSize * 0.95,
                            RenderTransformOrigin = new Point(0.52, 0.5)
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
                            Height = squareSize,
                            Width = squareSize,
                            RenderTransformOrigin = new Point(0.45, 0.5)
                        };
                        BoardCanvas.Children.Add(file);
                        Canvas.SetLeft(file, i * squareSize + squareSize * 0.05);
                        Canvas.SetBottom(file, j * squareSize);
                        Panel.SetZIndex(file, 1);
                    }
                }
            }

            WhiteStatusPanel.Background = availableSquareBrush;
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
            if (piece.Player != board.Turn)
            {
                return;
            }

            SelectedPiece = piece;
        }

        private async void AvailableMove_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Rectangle move = sender as Rectangle;
            int col = (int)(Canvas.GetLeft(move) / squareSize) + 1;
            int row = (int)(Canvas.GetBottom(move) / squareSize) + 1;

            board.MovePiece(SelectedPiece, col, row);

            await UpdateBoard(true);

            PushMoveToHistory(board.LastMoveNode);

            if (board.Ended)
            {
                string msg;
                if (board.Winner is null)
                {
                    msg = $"{board.Turn} stalemated. Game is drawn.";
                }
                else
                {
                    msg = $"{board.Winner} won!";
                }
                MessageBox.Show(msg, "Game finished", MessageBoxButton.OK, MessageBoxImage.Information);

                PushMoveToHistory(board.MovesHistory.Last);
            }
        }

        private void PromotionOption_Click(object sender, RoutedEventArgs e)
        {
            promotionPiece = (sender as Button).Tag as ChessPiece;
            PawnPromotionGrid.Visibility = Visibility.Collapsed;
            UpdateBoard().Wait();
        }

        private void MovesHistory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LinkedListNode<ChessMove> moveNode;

            if (MovesHistory.SelectedItem is ListViewItem item)
            {
                moveNode = item.Tag as LinkedListNode<ChessMove>;
            }
            else
            {
                moveNode = board.MovesHistory.First;
            }

            ListViewItem prev = e.RemovedItems.Count > 0 ? e.RemovedItems[0] as ListViewItem : null;

            PrevMoveButton.IsEnabled = MovesHistory.SelectedIndex > -1;
            NextMoveButton.IsEnabled = MovesHistory.SelectedIndex < MovesHistory.Items.Count - 1;

            if (moveNode.Value != null && moveNode.Value.Destination is null)
            {
                ResignButton.IsEnabled = DrawOfferButton.IsEnabled = false;
                moveNode = moveNode.Previous;
            }
            else
            {
                ResignButton.IsEnabled = DrawOfferButton.IsEnabled = true;
            }

            // Undo or redo
            bool redo = prev is null || MovesHistory.Items.IndexOf(prev) < MovesHistory.SelectedIndex;

            while (board.LastMoveNode != moveNode)
            {
                if (redo)
                {
                    board.Redo();
                }
                else if (moveNode.Value is null || moveNode.Value.Destination != null)
                {
                    board.Undo();
                }
            }
            if (redo && !ResignButton.IsEnabled)
            {
                board.Redo();
            }

            UpdateBoard().Wait();

            if (board.Turn == ChessPlayer.White)
            {
                WhiteStatusPanel.Background =
                    board.Winner == ChessPlayer.Black ? inCheckSquareBrush : availableSquareBrush;
                BlackStatusPanel.Background = Brushes.Transparent;
            }
            else
            {
                BlackStatusPanel.Background =
                    board.Winner == ChessPlayer.White ? inCheckSquareBrush : availableSquareBrush;
                WhiteStatusPanel.Background = Brushes.Transparent;
            }
        }

        private async void PrevMoveButton_Click(object sender, RoutedEventArgs e)
        {
            if (animationRunning)
            {
                return;
            }

            if (((MovesHistory.SelectedItem as ListViewItem)
                .Tag as LinkedListNode<ChessMove>).Value.Destination != null)
            {
                board.Undo();
                await UpdateBoard(true);
            }
            MovesHistory.SelectedIndex--;
        }

        private async void NextMoveButton_Click(object sender, RoutedEventArgs e)
        {
            if (animationRunning)
            {
                return;
            }

            board.Redo();
            await UpdateBoard(true);
            MovesHistory.SelectedIndex++;
        }

        private void NewGameButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(
                "Are you sure you want to start a new game?\nAll data of current game will be lost.",
                "New Game", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);

            if (result == MessageBoxResult.Yes)
            {
                InitNewGame();
            }
        }

        private void FlipBoardCheckBox_CheckChanged(object sender, RoutedEventArgs e)
        {
            FlipBoard();
        }

        private void ResignButton_Click(object sender, RoutedEventArgs e)
        {
            ChessPlayer opp = board.Turn == ChessPlayer.White ? ChessPlayer.Black : ChessPlayer.White;
            MessageBoxResult ans = MessageBox.Show(
                $"Are you sure you want to resign\nand concede the game to {opp}?",
                $"{board.Turn} resigning", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);

            if (ans == MessageBoxResult.Yes)
            {
                EndGame(opp);
            }
        }

        private void DrawOfferButton_Click(object sender, RoutedEventArgs e)
        {
            ChessPlayer opp = board.Turn == ChessPlayer.White ? ChessPlayer.Black : ChessPlayer.White;
            MessageBoxResult ans = MessageBox.Show(
                $"{board.Turn} offered draw. Does {opp} accept?",
                $"Draw offer", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);

            if (ans == MessageBoxResult.Yes)
            {
                EndGame(null);
            }
        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            ExportGrid.Visibility = Visibility.Visible;
        }

        private void ExportPGNButton_Click(object sender, RoutedEventArgs e)
        {
            string[] pgn;

            try
            {
                pgn = board.ToPGN(MatchEvent.Text,
                                           MatchSite.Text,
                                           MatchRound.Text,
                                           WhiteLastName.Text,
                                           WhiteFirstName.Text,
                                           BlackLastName.Text,
                                           BlackFirstName.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error happened while exporting game: " + ex.Message,
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string wName = WhiteLastName.Text;
            if (string.IsNullOrWhiteSpace(wName))
            {
                wName = "Unknown";
            }

            string bName = BlackLastName.Text;
            if (string.IsNullOrWhiteSpace(bName))
            {
                bName = "Unknown";
            }

            SaveFileDialog sfd = new SaveFileDialog()
            {
                Title = "Save Game",
                FileName = $"{wName}_vs_{bName}_" +
                    $"{board.StartTime.ToString("yyyy.MM.dd", CultureInfo.InvariantCulture)}",
                DefaultExt = ".pgn",
                Filter = "Protable Game Notation (*.pgn)|*.pgn",
                AddExtension = true,
                CheckPathExists = true,
                OverwritePrompt = true
            };

            if (sfd.ShowDialog() == false)
            {
                return;
            }

            try
            {
                File.WriteAllLines(sfd.FileName, pgn);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error happened while writing game file: " + ex.Message,
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            MessageBox.Show("Game exported successfully!",
                "Done", MessageBoxButton.OK, MessageBoxImage.Information);

            ExportGrid.Visibility = Visibility.Collapsed;
        }
    }
}