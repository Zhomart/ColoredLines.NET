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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ColouredLines.net
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Game game;

        public MainWindow()
        {
            InitializeComponent();
            rulesGrid.Visibility = Visibility.Hidden;
            gameGrid.Visibility = Visibility.Hidden;
        }

        private void rulesButton_Click(object sender, RoutedEventArgs e)
        {
            rulesGrid.Visibility = Visibility.Visible;
            mainMenuGrid.Visibility = Visibility.Hidden;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            mainMenuGrid.Visibility = Visibility.Visible;
            rulesGrid.Visibility = Visibility.Hidden;
        }

        private void finishGameButton_Click(object sender, RoutedEventArgs e)
        {
            mainMenuGrid.Visibility = Visibility.Visible;
            gameGrid.Visibility = Visibility.Hidden;
        }

        public void FinishGameToMainWindow()
        {
            finishGameButton_Click(null, null);
        }

        private void startGameButton_Click(object sender, RoutedEventArgs e)
        {
            gameGrid.Visibility = Visibility.Visible;
            mainMenuGrid.Visibility = Visibility.Hidden;

            game = new Game(this, GameCanvas, NewPanelCanvas);
        }

        private void Canvas_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Point p = e.GetPosition(GameCanvas);
            double width = GameCanvas.Width / 9.0;
            double height = GameCanvas.Height / 9.0;
            
            int x = (int)(p.X / width);
            int y = (int)(p.Y / width);

            game.clickedTo(x, y);
        }

        public void ShowMessage(string s)
        {
            MessageBox.Show(s);
        }

        public void updateScore()
        {
            TheScore.Content = game.score + "";
        }
    }
}
