using SudokuModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Sudosuduko
{
    public partial class MainWindow : Window
    {
        public Button selectedButton = null;
        public SudokuBoard rootBoard;
        public SudokuBoard currentBoard;
        public int depth = 1;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void newGame(object sender, RoutedEventArgs e)
        {
            mainPanel.Children.Remove(startButton);
            rootBoard = new SudokuBoard(null, Difficulty.ROOT);
            currentBoard = rootBoard;
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    Button button = new Button();
                    button.Width = 313 / 9 - 2;
                    button.Height = 313 / 9 - 2;
                    button.Tag = new Point(i, j);
                    button.Margin = new Thickness(i * 313 / 9 + 1, j * 313 / 9 + 1, 0, 0);
                    button.KeyDown += delegate(object btsender, KeyEventArgs bte)
                    {
                        var keyString = bte.Key.ToString();
                        if (bte.Key == Key.Back)
                        {
                            button.Content = "";
                            currentBoard.sudo.Data[(int)((Point)button.Tag).X, (int)((Point)button.Tag).Y] = 0;
                        }
                        else if (keyString.Length == 2 && keyString[0] == 'D')
                        {
                            var keyInt = Int32.Parse("" + keyString[1]);
                            if (keyInt != 0)
                            {
                                if (button.Content != "#")
                                {
                                    button.Content = "" + keyInt;
                                    currentBoard.answers[(int)((Point)button.Tag).X, (int)((Point)button.Tag).Y] = (byte)keyInt;
                                }
                            }
                        }
                        checkForWin();
                    };
                    button.Click += delegate(object btsender, RoutedEventArgs bte)
                    {
                        if (selectedButton != null) {
                            selectedButton.Effect = null;
                        }
                        var effect = new DropShadowEffect();
                        effect.ShadowDepth = 0;
                        effect.BlurRadius = 4;
                        effect.Color = Color.FromRgb(180, 180, 255);
                        button.Effect = effect;
                        selectedButton = button;
                        if (depth != 1) {
                            upButton.IsEnabled = true;
                        }
                        if (depth != 5) {
                            downButton.IsEnabled = true;
                        }
                        setNumButtonsEnabled(button.Content != "#");
                    };
                    mainPanel.Children.Add(button);
                }
            }
            mainPanel.Children.Remove(startButton);
        }

        private void checkForWin() {
            bool good = true;
            for (int i = 0; i < 9; i++) {
                for (int j = 0; j < 9; j++) {
                    if (currentBoard.sudo.Data[i, j] != currentBoard.solvedBoard[i, j] && currentBoard.answers[i, j] != currentBoard.solvedBoard[i, j]) {
                        good = false;
                        break;
                    }
                }
                if (good == false) {
                    break;
                }
            }
            if (good) {
                solvedLabel.Text = "Solved!";
                if (currentBoard.parent != null) {
                    for (int i = 0; i < 9; i++) {
                        for (int j = 0; j < 9; j++) {
                            if (currentBoard.parent.boards[i, j] == currentBoard) {
                                var newDat = new byte[9, 9];
                                for (int x = 0; x < 9; x++) {
                                    for (int y = 0; y < 9; y++) {
                                        newDat[x, y] = currentBoard.parent.sudo.Data[x, y];
                                        if (x == i && y == j) {
                                            newDat[x, y] = currentBoard.parent.answers[i, j];
                                        }
                                    }
                                }
                                currentBoard.parent.answers[i, j] = currentBoard.parent.solvedBoard[i, j];
                                currentBoard.parent.sudo.Data = newDat;
                                currentBoard.parent.boards[i, j] = null;
                            }
                        }
                    }
                } else {
                    solvedLabel.Text += " Holy crap, you did it!";
                }
            }
        }

        private void buttonSudokuSetNumberClick(object sender, RoutedEventArgs e)
        {
            if(selectedButton != null && selectedButton.Content != "#") {
                Button button = (Button)sender;
                selectedButton.Content = button.Content;
                currentBoard.answers[(int)((Point)selectedButton.Tag).X, (int)((Point)selectedButton.Tag).Y] = Byte.Parse("" + button.Content);
            }
            checkForWin();
        }

        private void downButtonClick(object sender, RoutedEventArgs e)
        {
            if (selectedButton != null && depth != 5)
            {
                depth++;
                depthLabel.Content = "Depth: " + depth + (depth == 5 ? " (MAX)" : "");
                Point point = (Point)selectedButton.Tag;
                currentBoard = currentBoard.getBoard(point);
                buildSudokuBoard(currentBoard);
                selectedButton.Effect = null;
                selectedButton = null;
                upButton.IsEnabled = true;
                downButton.IsEnabled = false;
                setNumButtonsEnabled(false);
                updateRectangles();
                solvedLabel.Text = "";
            }
        }

        private void updateRectangles()
        {
            var rgb = (byte)(255 - (255 / depth));
            rect1.Stroke = new SolidColorBrush(Color.FromRgb(rgb, rgb, rgb));
            rect2.Stroke = new SolidColorBrush(Color.FromRgb(rgb, rgb, rgb));
            rect3.Stroke = new SolidColorBrush(Color.FromRgb(rgb, rgb, rgb));
            rect4.Stroke = new SolidColorBrush(Color.FromRgb(rgb, rgb, rgb));
            rect5.Stroke = new SolidColorBrush(Color.FromRgb(rgb, rgb, rgb));
            rect6.Stroke = new SolidColorBrush(Color.FromRgb(rgb, rgb, rgb));
            rect7.Stroke = new SolidColorBrush(Color.FromRgb(rgb, rgb, rgb));
            rect8.Stroke = new SolidColorBrush(Color.FromRgb(rgb, rgb, rgb));
            rect9.Stroke = new SolidColorBrush(Color.FromRgb(rgb, rgb, rgb));
        }

        public void buildSudokuBoard(SudokuBoard board)
        {
            for (int i = 0; i < mainPanel.Children.Count; i++)
            {
                if (mainPanel.Children[i] is Button)
                {
                    Button b = (Button)mainPanel.Children[i];
                    if (b.Tag is Point)
                    {
                        var point = (Point)b.Tag;
                        if (board.boards[(int)point.X, (int)point.Y] != null)
                        {
                            b.Content = "#";
                            b.IsEnabled = true;
                        }
                        else
                        {
                            b.Content = "" + board.sudo.Data[(int)point.X, (int)point.Y];
                            if (board.sudo.Data[(int)point.X, (int)point.Y] == 0)
                            {
                                b.Content = "";
                                b.IsEnabled = true;
                            }
                            else
                            {
                                b.IsEnabled = false;
                            }
                            if (board.answers[(int)point.X, (int)point.Y] != 0)
                            {
                                b.Content = "" + board.answers[(int)point.X, (int)point.Y];
                            }
                        }
                    }
                }
            }
        }

        private void upButtonClick(object sender, RoutedEventArgs e)
        {
            if (depth != 1)
            {
                depth--;
                depthLabel.Content = "Depth: " + depth + (depth == 1 ? " (ROOT)" : "");
                currentBoard = currentBoard.parent;
                buildSudokuBoard(currentBoard);
                if (selectedButton != null)
                {
                    selectedButton.Effect = null;
                    selectedButton = null;
                }
                upButton.IsEnabled = false;
                if (depth != 1)
                {
                    upButton.IsEnabled = true;
                }
                downButton.IsEnabled = false;
                setNumButtonsEnabled(false);
                updateRectangles();
                solvedLabel.Text = "";
            }
        }
        private void setNumButtonsEnabled(bool enabled) {
            numButton1.IsEnabled = enabled;
            numButton2.IsEnabled = enabled;
            numButton3.IsEnabled = enabled;
            numButton4.IsEnabled = enabled;
            numButton5.IsEnabled = enabled;
            numButton6.IsEnabled = enabled;
            numButton7.IsEnabled = enabled;
            numButton8.IsEnabled = enabled;
            numButton9.IsEnabled = enabled; 
        }

        private void cheatSolve(object sender, RoutedEventArgs e) {
            for (int i = 0; i < 9; i++) {
                for (int j = 0; j < 9; j++) {
                    currentBoard.answers[i, j] = currentBoard.solvedBoard[i, j];
                    for (int k = 0; k < mainPanel.Children.Count; k++) {
                        if(mainPanel.Children[k] is Button) {
                            var button = (Button)mainPanel.Children[k];
                            if (button.Tag is Point) {
                                var point = (Point)button.Tag;
                                if (point.X == i && point.Y == j) {
                                    button.Content = currentBoard.answers[i, j];
                                }
                            }
                        }
                    }
                }
            }
            checkForWin();
        }

        private void cheat(object sender, KeyEventArgs e) {
            if (e.Key == Key.F1) {
                cheatSolve(null, null);
            }
        }
    }
}
