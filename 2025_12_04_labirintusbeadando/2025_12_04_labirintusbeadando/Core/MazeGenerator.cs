using System;
using System.Collections.Generic;
using System.Linq;

namespace _2025_12_04_labirintusbeadando.Core
{
    public class MazeGenerator
    {
        private readonly Random random;
        private const int STEP_SIZE = 2;

        public MazeGenerator(int? seed = null)
        {
            random = seed.HasValue ? new Random(seed.Value) : new Random();
        }

        public Maze Generate(int size, int exitCount)
        {
            ValidateParameters(size, exitCount);

            Maze maze = new Maze(size);

            // Generate start position on left side
            Position start = GenerateStartPosition(size);
            maze.SetCell(start.X, start.Y, CellType.Start);

            // Generate maze paths using DFS
            GeneratePaths(maze, start.X + 1, start.Y);

            // Add exits
            AddExits(maze, exitCount);

            // Select first exit by default
            if (maze.Exits.Count > 0)
                maze.SelectExit(maze.Exits[0]);

            return maze;
        }

        private void ValidateParameters(int size, int exitCount)
        {
            if (size < 5)
                throw new ArgumentException("Size must be at least 5");

            int maxExits = (int)Math.Floor(size * 0.2);
            if (exitCount < 1)
                throw new ArgumentException("Exit count must be at least 1");

            if (exitCount > maxExits)
                throw new ArgumentException($"Exit count cannot exceed {maxExits} for size {size}");
        }

        private Position GenerateStartPosition(int size)
        {
            // Generate odd Y position for proper path generation
            int y = random.Next(1, size - 2);
            if (y % 2 == 0) y--;
            return new Position(0, y);
        }

        private void GeneratePaths(Maze maze, int startX, int startY)
        {
            if (startX % 2 == 0) startX--;
            if (startY % 2 == 0) startY--;

            bool[,] visited = new bool[maze.Size, maze.Size];
            Stack<Position> stack = new Stack<Position>();

            Position startPos = new Position(startX, startY);
            stack.Push(startPos);
            maze.SetCell(startX, startY, CellType.Path);
            visited[startY, startX] = true;

            var directions = new List<Position>
            {
                new Position(STEP_SIZE, 0),
                new Position(-STEP_SIZE, 0),
                new Position(0, STEP_SIZE),
                new Position(0, -STEP_SIZE)
            };

            while (stack.Count > 0)
            {
                var current = stack.Pop();

                var shuffledDirs = Shuffle(directions);
                foreach (var dir in shuffledDirs)
                {
                    int newX = current.X + dir.X;
                    int newY = current.Y + dir.Y;

                    if (!IsValidPositionForPath(maze, newX, newY) || visited[newY, newX])
                        continue;

                    // Remove wall between current and new position
                    int wallX = current.X + dir.X / 2;
                    int wallY = current.Y + dir.Y / 2;
                    maze.SetCell(wallX, wallY, CellType.Path);

                    maze.SetCell(newX, newY, CellType.Path);
                    visited[newY, newX] = true;

                    stack.Push(new Position(newX, newY));
                }
            }
        }

        private bool IsValidPositionForPath(Maze maze, int x, int y)
        {
            return x > 0 && y > 0 && x < maze.Size - 1 && y < maze.Size - 1;
        }

        private List<Position> Shuffle(List<Position> list)
        {
            var shuffled = new List<Position>(list);
            for (int i = shuffled.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                (shuffled[i], shuffled[j]) = (shuffled[j], shuffled[i]);
            }
            return shuffled;
        }

        private void AddExits(Maze maze, int exitCount)
        {
            int placed = 0;
            List<Position> possibleExitPositions = GetPossibleExitPositions(maze);
            ShufflePositions(possibleExitPositions);

            foreach (var exitPos in possibleExitPositions)
            {
                if (placed >= exitCount) break;

                if (IsAdjacentToPath(maze, exitPos))
                {
                    maze.SetCell(exitPos.X, exitPos.Y, CellType.Exit);
                    placed++;
                }
            }

            if (placed < exitCount)
                throw new InvalidOperationException("Could not place all exits");
        }

        private List<Position> GetPossibleExitPositions(Maze maze)
        {
            var positions = new List<Position>();

            // Top and bottom rows (excluding corners)
            for (int x = 1; x < maze.Size - 1; x++)
            {
                positions.Add(new Position(x, 0));
                positions.Add(new Position(x, maze.Size - 1));
            }

            // Left and right columns (excluding corners)
            for (int y = 1; y < maze.Size - 1; y++)
            {
                positions.Add(new Position(0, y));
                positions.Add(new Position(maze.Size - 1, y));
            }

            return positions;
        }

        private void ShufflePositions(List<Position> positions)
        {
            for (int i = positions.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                (positions[i], positions[j]) = (positions[j], positions[i]);
            }
        }

        private bool IsAdjacentToPath(Maze maze, Position pos)
        {
            var adjacentPositions = new List<Position>
            {
                new Position(pos.X + 1, pos.Y),
                new Position(pos.X - 1, pos.Y),
                new Position(pos.X, pos.Y + 1),
                new Position(pos.X, pos.Y - 1)
            };

            foreach (var adjacent in adjacentPositions)
            {
                if (maze.IsValidPosition(adjacent.X, adjacent.Y) &&
                    maze.GetCell(adjacent.X, adjacent.Y) == CellType.Path)
                {
                    return true;
                }
            }
            return false;
        }
    }
}