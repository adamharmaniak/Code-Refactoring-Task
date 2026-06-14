// Source - https://codereview.stackexchange.com/q/127515
// Posted by Wagacca, modified by community. See post 'Timeline' for change history
// Retrieved 2026-06-14, License - CC BY-SA 3.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
///█ ■
////https://www.youtube.com/watch?v=SGZgvMwjq2U
namespace Snake
{
    static class Program
    {
        private static void Main()
        {
            
            var settings = new GameSettings(32, 16, 500, 5);

            ConfigureConsole(settings);

            var game = new SnakeGame(settings, new Random());

            while (!game.IsGameOver)
            {
                
            }
            Console.SetCursorPosition(screenwidth / 5, screenheight / 2);
            Console.WriteLine("Game over, Score: "+ score);
            Console.SetCursorPosition(screenwidth / 5, screenheight / 2 +1);
        }

        private static void ConfigureConsole(GameSettings settings)
        {
            Console.WindowWidth = settings.Width;
            Console.WindowHeight = settings.Height;
            Console.CursorVisible = false;
        }

        class Cell
        {
            public int xpos { get; set; }
            public int ypos { get; set; }
            public ConsoleColor schermkleur { get; set; }
        }
    }

    internal sealed class GameSettings
        {
            public GameSettings(int width, int height, int frameDelayMilliseconds, int initialScore)
            {
                Width = width;
                Height = height;
                FrameDelayMilliseconds = frameDelayMilliseconds;
                InitialScore = initialScore;
            }

            public int Width { get; }
            public int Height { get; }
            public int FrameDelayMilliseconds { get; }
            public int InitialScore { get; }
        }

    internal sealed class SnakeGame
    {
        private readonly GameSettings settings;
        private readonly Random random;
        private readonly LinkedList<Cell> snake = new LinkedList<Cell>();

        public SnakeGame(GameSettings settings, Random random)
        {
            this.settings = settings;
            this.random = random;

            Score = settings.InitialScore;
            Direction = Direction.Right;

            snake.AddFirst(new Cell(settings.Width / 2, settings.Height / 2));
            Food = CreateFood();
        }

        public int Score { get; private set; }
        public bool IsGameOver { get; private set; }
        public Direction Direction { get; private set; }
        public Cell Food { get; private set; }
        public Cell Head => snake.First.Value;
        public IEnumerable<Cell> SnakeCells => snake;

        public void ChangeDirection(Direction newDirection)
        {
            if (!DirectionHelper.AreOpposite(Direction, newDirection))
            {
                Direction = newDirection;
            }
        }

        public void Update()
        {
            if (IsGameOver)
            {
                return;
            }

            Cell nextHead = GetNextHead();
            bool ateFood = nextHead.Equals(Food);

            snake.AddFirst(nextHead);

            if (ateFood)
            {
                Score++;
                Food = CreateFood();
            }
            else
            {
                TrimSnake();
            }

            IsGameOver = HitsWall(nextHead) || HitsItself(nextHead);
        }

        private Cell GetNextHead()
        {
            switch (Direction)
            {
                case Direction.Up:
                    return new Cell(Head.X, Head.Y - 1);

                case Direction.Down:
                    return new Cell(Head.X, Head.Y + 1);

                case Direction.Left:
                    return new Cell(Head.X - 1, Head.Y);

                case Direction.Right:
                    return new Cell(Head.X + 1, Head.Y);

                default:
                    throw new InvalidOperationException("Unknown direction.");
            }
        }

        private void TrimSnake()
        {
            while (snake.Count > Score)
            {
                snake.RemoveLast();
            }
        }

        private bool HitsWall(Cell cell)
        {
            return cell.X == 0
                || cell.Y == 0
                || cell.X == settings.Width - 1
                || cell.Y == settings.Height - 1;
        }

        private bool HitsItself(Cell head)
        {
            return snake.Skip(1).Contains(head);
        }

        private Cell CreateFood()
        {
            Cell food;

            do
            {
                food = new Cell(
                    random.Next(1, settings.Width - 1),
                    random.Next(1, settings.Height - 1)
                );
            }
            while (snake.Contains(food));

            return food;
        }
    }
}
//¦
