using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;


namespace ColouredLines.net
{
    class Game
    {
        MainWindow mainWindow;

        public bool ballMoving = false;

        public List<Ball> balls;

        public List<Ball> new_balls;

        Point last_point;

        public Ball[,] board;

        public Random random;

        public int[] POINTS = new int[] { 0, 0, 0, 0, 0, 10, 12, 18, 28, 42};

        public int score = 0;

        public Canvas canvas;

        public Canvas newBallCanvas;

        List<Point> new_added_balls;

        public Ball selectedBall = null;

        List<Point> directions;

        public Game(MainWindow mw, Canvas canvas, Canvas newBallCanvas)
        {
            this.mainWindow = mw;
            this.canvas = canvas;
            this.newBallCanvas = newBallCanvas;

            canvas.Children.Clear();
            newBallCanvas.Children.Clear();

            random = new Random();

            directions = new List<Point>();
            directions.Add(new Point(1, 0));
            directions.Add(new Point(-1, 0));
            directions.Add(new Point(0, 1));
            directions.Add(new Point(0, -1));

            balls = new List<Ball>();
            new_balls = new List<Ball>();

            board = new Ball[9, 9];

            for (int i = 0; i < 5; ++i)
            {
                Ball ball = new Ball(this, randomColor(), randomLocation());
                addBall(ball);
            }

            generateNewBalls();

            drawBalls();

            drawNewBalls();
        }

        public void drawBalls()
        {
            foreach (var b in balls)
                b.initOnBoard();
        }

        public void drawNewBalls()
        {
            int index = 0;
            foreach (var b in new_balls)
            {
                b.initOnNewPanel(index);
                index++;
            }
        }

        public void clickedTo(int x, int y)
        {
            if (ballMoving) return;

            if (selectedBall != null) selectedBall.stopAnimation();

            if (board[x, y] != null && board[x, y] == selectedBall)
            {
                selectedBall = null;
                return;
            }

            if (board[x, y] != null)
            {
                selectedBall = board[x, y];
                return;
            }

            if (selectedBall != null)
            {
                board[selectedBall.location.x, selectedBall.location.y] = null;
                ballMoving = true;
                last_point = new Point(x, y);
                selectedBall.moveTo(x, y);
                board[x, y] = selectedBall;
                selectedBall = null;
            }
        }

        public void nextAction()
        {
            processNewPoint(last_point.x, last_point.y, true);
        }

        void processNewPoint(int x, int y, bool add_new_balls)
        {
            int new_score = 0;

            foreach (var pts in getPoints(x, y))
            {
                new_score += POINTS[pts.Count];

                if (POINTS[pts.Count] == 0) continue;

                string s = "";

                foreach (var p in pts)
                {
                    s += p.color + " " + p.location.s + " | ";
                    canvas.Children.Remove(p.ellipse);
                    balls.Remove(p);
                    board[p.location.x, p.location.y] = null;
                }

                // ShowMessage(s);
            }

            if (new_score > 0)
            {
                // ShowMessage("new_score: " + new_score);

                score += new_score;

                mainWindow.updateScore();
            }
            else
            {
                if (add_new_balls)
                {
                    addNewBallsToBoard();

                    foreach (var p in new_added_balls)
                    {
                        if (HasNewPoint(p.x, p.y))
                            processNewPoint(p.x, p.y, false);
                    }
                }
            }
        }

        bool HasNewPoint(int x, int y)
        {
            int new_score = 0;

            foreach (var pts in getPoints(x, y))
                new_score += POINTS[pts.Count];

            return new_score > 0;
        }

        public List<List<Ball>> getPoints(int x, int y)
        {
            List<List<Ball>> result = new List<List<Ball>>();
            if (board[x, y] == null) return result;

            var q1 = new List<Ball>();
            var q2 = new List<Ball>();
            var q3 = new List<Ball>();
            var q4 = new List<Ball>();

            int min_len = 3;

            q1.Add(board[x, y]);
            q2.Add(board[x, y]);
            q3.Add(board[x, y]);
            q4.Add(board[x, y]);

            for (int t = 1; t < 9; ++t)
                if (x + t >= 9 || board[x + t, y] == null || board[x, y].color != board[x + t, y].color) break; else q1.Add(board[x + t, y]);
            for (int t = 1; t < 9; ++t)
                if (x - t < 0 || board[x - t, y] == null || board[x, y].color != board[x - t, y].color) break; else q1.Add(board[x - t, y]);

            for (int t = 1; t < 9; ++t)
                if (y + t >= 9 || board[x, y + t] == null || board[x, y].color != board[x, y + t].color) break; else q2.Add(board[x, y + t]);
            for (int t = 1; t < 9; ++t)
                if (y - t < 0 || board[x, y - t] == null || board[x, y].color != board[x, y - t].color) break; else q2.Add(board[x, y - t]);

            for (int t = 1; t < 9; ++t)
                if (y + t >= 9 || x + t >= 9 || board[x + t, y + t] == null || board[x, y].color != board[x + t, y + t].color) break; else q3.Add(board[x + t, y + t]);
            for (int t = 1; t < 9; ++t)
                if (y - t < 0 || x - t < 0 || board[x - t, y - t] == null || board[x, y].color != board[x - t, y - t].color) break; else q3.Add(board[x - t, y - t]);

            for (int t = 1; t < 9; ++t)
                if (y + t >= 9 || x - t < 0 || board[x - t, y + t] == null || board[x, y].color != board[x - t, y + t].color) break; else q4.Add(board[x - t, y + t]);
            for (int t = 1; t < 9; ++t)
                if (y - t < 0 || x + t >= 9 || board[x + t, y - t] == null || board[x, y].color != board[x + t, y - t].color) break; else q4.Add(board[x + t, y - t]);

            if (q1.Count >= min_len) result.Add(q1);
            if (q2.Count >= min_len) result.Add(q2);
            if (q3.Count >= min_len) result.Add(q3);
            if (q4.Count >= min_len) result.Add(q4);

            return result;
        }

        public void finishGame()
        {
            ShowMessage("Game over, your score: " + score);
            mainWindow.FinishGameToMainWindow();
        }

        void addNewBallsToBoard()
        {
            int free_cells = 0;
            for (int i = 0; i < 9; ++i)
                for (int j = 0; j < 9; ++j)
                    if (board[i, j] == null) free_cells++;

            if (free_cells <= 3)
            {
                finishGame();
                return;
            }

            foreach (var b in new_balls)
                newBallCanvas.Children.Remove(b.ellipse);

            new_added_balls = new List<Point>();

            foreach (var b in new_balls)
            {
                b.location = randomLocation();
                addBall(b);
                b.initOnBoard();
                new_added_balls.Add(new Point(b.location));
            }

            generateNewBalls();
            drawNewBalls();
        }

        public List<Point> findPath(Point from, Point to)
        {
            int[,] a = new int[9, 9];

            for (int i = 0; i < 9; ++i)
                for (int j = 0; j < 9; ++j)
                    if (board[i, j] != null) a[i, j] = -1;
                    else a[i, j] = 0;

            List<Point> qu = new List<Point>();
            qu.Add(from);

            a[from.x, from.y] = 1;

            while (qu.Count > 0)
            {
                Point v = qu.First();
                qu.RemoveAt(0);

                foreach (var q in directions)
                {
                    int i = q.x;
                    int j = q.y;
                    if (v.x + i >= 0 && v.x + i < 9 && v.y + j >= 0 && v.y + j < 9)
                        if (a[v.x + i, v.y + j] == 0)
                        {
                            a[v.x + i, v.y + j] = a[v.x, v.y] + 1;
                            qu.Add(new Point(v.x + i, v.y + j));
                        }
                }
            }

            if (a[to.x, to.y] <= 0) return null;

            List<Point> result = new List<Point>();
            result.Add(to);

            Point w = to;
            while (w.x != from.x || w.y != from.y)
            {
                foreach (var q in directions)
                {
                    int i = q.x;
                    int j = q.y;

                    if (w.x + i >= 0 && w.x + i < 9 && w.y + j >= 0 && w.y + j < 9)
                        if (a[w.x + i, w.y + j] == a[w.x, w.y] - 1)
                        {
                            w = new Point(w.x + i, w.y + j);
                            result.Insert(0, w);
                        }
                }
            }

            for (int i = 1; i < result.Count - 1; ++i)
                if ((result[i].x == result[i - 1].x && result[i].x == result[i + 1].x) || (result[i].y == result[i - 1].y && result[i].y == result[i + 1].y))
                {
                    result.RemoveAt(i);
                    --i;
                }

            return result;
        }

        public void ShowMessage(string s)
        {
            mainWindow.ShowMessage(s);
        }

        // testing
        void drawBorders()
        {
            Rectangle r = new Rectangle();
            r.Width = canvas.Width - 4;
            r.Height = canvas.Height - 4;
            r.Stroke = new SolidColorBrush(Colors.Red);
            Canvas.SetLeft(r, 0);
            Canvas.SetTop(r, 0);
            canvas.Children.Add(r);
        }

        public void addBall(Ball ball)
        {
            balls.Add(ball);
            board[ball.location.x, ball.location.y] = ball;
        }

        public BallColor randomColor()
        {
            Array values = Enum.GetValues(typeof(BallColor));
            BallColor randomBar = (BallColor)values.GetValue(random.Next(values.Length));
            return randomBar;
        }

        public Point randomLocation()
        {
            while (true)
            {
                int x = random.Next(9);
                int y = random.Next(9);
                if (board[x, y] == null) {
                    return new Point(x, y);
                }
            }
        }

        public void generateNewBalls()
        {
            new_balls.Clear();
            for (int i = 0; i < 3; ++i) {
                Ball ball = new Ball(this, randomColor());
                new_balls.Add(ball);
            }
        }
    }

    class Point
    {
        public int x, y;

        public Point()
        {
            x = y = 0;
        }

        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public Point(Point p)
        {
            this.x = p.x;
            this.y = p.y;
        }

        public string s { get {
            return "[" + x + "," + y + "]";
        } }

    }
}
