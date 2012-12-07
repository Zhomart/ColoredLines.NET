using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Drawing;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Controls;
using System.Timers;
using System.Windows.Media.Animation;
using System.Windows;

namespace ColouredLines.net
{
    class Ball
    {
        public BallColor color;
        //Image image;

        DoubleAnimation da = new DoubleAnimation();
        TranslateTransform tt = new TranslateTransform();

        public Point location;

        AnimatingState animatingState = AnimatingState.Stopped;

        Game game;

        int width;
        int height;

        List<Point> path;
        Point last_location; // helper

        public Ellipse ellipse;

        public Ball(Game game, BallColor color, Point location) : this(game, color)
        {
            this.location = location;
        }

        public Ball(Game game, BallColor color)
        {
            this.game = game;
            this.color = color;
            //image = Image.FromFile(@"assets/red.png");

            da.Completed += d_Completed;
        }

        Color convertedColor() 
        {
            if (color == BallColor.Blue) return Colors.Blue;
            if (color == BallColor.Green) return Colors.Green;
            if (color == BallColor.LightBlue) return Colors.CadetBlue;
            if (color == BallColor.Pink) return Colors.Salmon;
            if (color == BallColor.Purple) return Colors.RosyBrown;
            if (color == BallColor.Red) return Colors.Red;
            if (color == BallColor.Yellow) return Colors.Yellow;
            return Colors.Black;
        }

        int canvasLeft()
        {
            double step = ((game.canvas.Width - 5) / 9);
            return (int)(step * location.x + 6);
        }

        int canvasTop()
        {
            double step = ((game.canvas.Height - 5) / 9);
            return (int)(step * location.y + 5.5);
        }

        public void initOnNewPanel(int index) 
        {
            double scale = 0.8;
            width = (int)(scale * (game.newBallCanvas.Width / 3));
            height = (int)(scale * (game.newBallCanvas.Height));

            ellipse = new Ellipse();
            ellipse.Width = width;
            ellipse.Height = height;
            ellipse.Fill = new SolidColorBrush(convertedColor());
            Canvas.SetLeft(ellipse, index * (width * 1.25) + 3);
            Canvas.SetTop(ellipse, 3);
            game.newBallCanvas.Children.Add(ellipse);
        }

        public void initOnBoard()
        {
            double scale = 0.8;
            width = (int)(scale * (game.canvas.Width / 9));
            height = (int)(scale * (game.canvas.Height / 9));

            ellipse = new Ellipse();
            ellipse.Width = width;
            ellipse.Height = height;
            ellipse.Fill = new SolidColorBrush(convertedColor());
            game.canvas.Children.Add(ellipse);

            relocate();

            initHandlers();
        }

        public void initHandlers()
        {
            ellipse.MouseDown += ellipse_MouseDown;
        }

        void ellipse_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (animatingState == AnimatingState.Stopped && !game.ballMoving)
                startAnimation();
            else if (animatingState == AnimatingState.Jumping)
                stopAnimation();
        }

        void relocate()
        {
            // game.ShowMessage(location.s);
            Canvas.SetLeft(ellipse, canvasLeft());
            Canvas.SetTop(ellipse, canvasTop());
        }

        public void startAnimation()
        {
            da.From = 0;
            da.To = -5;
            da.Duration = new Duration(TimeSpan.FromSeconds(0.3));
            da.RepeatBehavior = RepeatBehavior.Forever;
            da.AutoReverse = true;

            ellipse.RenderTransform = tt;
            tt.BeginAnimation(TranslateTransform.YProperty, da);

            animatingState = AnimatingState.Jumping;
        }

        public void stopAnimation()
        {
            tt.BeginAnimation(TranslateTransform.YProperty, null);
            animatingState = AnimatingState.Stopped;
        }

        public void moveTo(int x, int y)
        {
            var newloc = new Point(x, y);

            path = game.findPath(location, newloc);

            string s = "";
            foreach (var a in path) s += ", " + "[" + a.x + "," + a.y + "]";
            // game.ShowMessage(s);

            if (path != null && path.Count > 0)
            {
                path.RemoveAt(0);

                animatingState = AnimatingState.Moving;

                translateByLineTo(path[0]);
                last_location = path[0];
                path.RemoveAt(0);
            }
        }

        void translateByLineTo(Point p)
        {
            // game.ShowMessage(location.s+" -> " +p.s);

            DoubleAnimation d = new DoubleAnimation();

            d.From = 0;

            double scale = width + 7;

            if (p.x == location.x)
                d.To = p.y - location.y;
            else
                d.To = p.x - location.x;

            d.To *= scale;

            d.Duration = new Duration(TimeSpan.FromSeconds(0.5));
            d.RepeatBehavior = new RepeatBehavior(1);
            d.AutoReverse = false;
            d.Completed += d_Completed;

            TranslateTransform t = new TranslateTransform();

            ellipse.RenderTransform = t;

            if (location.x == p.x)
                t.BeginAnimation(TranslateTransform.YProperty, d);
            else
                t.BeginAnimation(TranslateTransform.XProperty, d);
        }

        void d_Completed(object sender, EventArgs e)
        {
            if (animatingState == AnimatingState.Moving)
            {
                if (path.Count > 0)
                {
                    location = last_location;
                    relocate();

                    translateByLineTo(path[0]);
                    last_location = path[0];
                    path.RemoveAt(0);
                    return;
                }
                else
                {
                    ellipse.RenderTransform = null;
                    location = last_location;
                    relocate();

                    ballMovingCompleted();
                }
            }
            animatingState = AnimatingState.Stopped;
        }

        void ballMovingCompleted()
        {
            game.ballMoving = false;
            game.nextAction();
        }

        enum AnimatingState { Stopped, Jumping, Moving }
    }

    enum BallColor { Green = 0, Red = 1, Yellow = 2, Blue = 3, Pink = 4, Purple = 5, LightBlue = 6 }
}
