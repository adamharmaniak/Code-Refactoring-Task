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
            var renderer = new ConsoleGameRenderer(settings);
            var input = new ConsoleInputReader();

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

    internal sealed class ConsoleGameRenderer
    {
        private const char Block = '■';
        private readonly GameSettings settings;

        public ConsoleGameRenderer(GameSettings settings)
        {
            this.settings = settings;
        }

        public void Render(SnakeGame game)
        {
            Console.Clear();

            DrawBorder();
            DrawFood(game.Food);
            DrawSnake(game);
        }

        public void RenderGameOver(int score)
        {
            Console.Clear();

            string message = "Game over, Score: " + score;
            int x = Math.Max(0, (settings.Width - message.Length) / 2);
            int y = settings.Height / 2;

            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(x, y);
            Console.Write(message);
            Console.ResetColor();

            Console.SetCursorPosition(0, settings.Height - 1);
        }

        private void DrawBorder()
        {
            for (int x = 0; x < settings.Width; x++)
            {
                DrawCell(new Cell(x, 0), ConsoleColor.White);
                DrawCell(new Cell(x, settings.Height - 1), ConsoleColor.White);
            }

            for (int y = 0; y < settings.Height; y++)
            {
                DrawCell(new Cell(0, y), ConsoleColor.White);
                DrawCell(new Cell(settings.Width - 1, y), ConsoleColor.White);
            }
        }

        private void DrawFood(Cell food)
        {
            DrawCell(food, ConsoleColor.Cyan);
        }

        private void DrawSnake(SnakeGame game)
        {
            foreach (Cell bodyPart in game.SnakeCells.Skip(1))
            {
                DrawCell(bodyPart, ConsoleColor.Green);
            }

            DrawCell(game.Head, ConsoleColor.Red);
        }

        private void DrawCell(Cell cell, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.SetCursorPosition(cell.X, cell.Y);
            Console.Write(Block);
            Console.ResetColor();
        }
    }

    internal sealed class ConsoleInputReader
    {
        public Direction ReadDirectionFor(int milliseconds, Direction currentDirection)
        {
            Direction selectedDirection = currentDirection;
            bool directionChanged = false;
            DateTime endTime = DateTime.Now.AddMilliseconds(milliseconds);

            while (DateTime.Now < endTime)
            {
                if (!Console.KeyAvailable)
                {
                    Thread.Sleep(10);
                    continue;
                }

                ConsoleKey key = Console.ReadKey(true).Key;

                if (directionChanged)
                {
                    continue;
                }

                if (TryMapKeyToDirection(key, out Direction newDirection)
                    && !DirectionHelper.AreOpposite(currentDirection, newDirection))
                {
                    selectedDirection = newDirection;
                    directionChanged = true;
                }
            }

            return selectedDirection;
        }

        private static bool TryMapKeyToDirection(ConsoleKey key, out Direction direction)
        {
            switch (key)
            {
                case ConsoleKey.UpArrow:
                    direction = Direction.Up;
                    return true;

                case ConsoleKey.DownArrow:
                    direction = Direction.Down;
                    return true;

                case ConsoleKey.LeftArrow:
                    direction = Direction.Left;
                    return true;

                case ConsoleKey.RightArrow:
                    direction = Direction.Right;
                    return true;

                default:
                    direction = Direction.Right;
                    return false;
            }
        }
    }
}
//¦
