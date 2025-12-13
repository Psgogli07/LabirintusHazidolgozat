using System;
using System.Collections.Generic;
using System.Linq;

namespace _2025_12_04_labirintusbeadando.Core
{
    public class Maze
    {
        private readonly CellType[,] grid;
        private readonly Dictionary<CellType, int> obstacleCosts;
        private Position start;
        private readonly List<Position> exits;

        public int Size { get; }
        public CellType[,] Grid => grid;
        public Position Start => start;
        public IReadOnlyList<Position> Exits => exits;
        public IReadOnlyDictionary<CellType, int> ObstacleCosts => obstacleCosts;
        public Position SelectedExit { get; private set; }

        public Maze(int size)
        {
            if (size <= 0)
                throw new ArgumentException("Size must be positive", nameof(size));

            Size = size;
            grid = new CellType[size, size];
            exits = new List<Position>();

            // Initialize obstacle costs
            obstacleCosts = new Dictionary<CellType, int>();
            foreach (var obstacle in Obstacle.AllObstacles)
            {
                obstacleCosts[obstacle.Type] = obstacle.Cost;
            }

            // Initialize all cells as walls
            for (int y = 0; y < size; y++)
                for (int x = 0; x < size; x++)
                    grid[y, x] = CellType.Wall;
        }

        public void SetCell(int x, int y, CellType type)
        {
            ValidatePosition(x, y);

            grid[y, x] = type;

            switch (type)
            {
                case CellType.Start:
                    start = new Position(x, y);
                    break;
                case CellType.Exit:
                    var exitPos = new Position(x, y);
                    if (!exits.Contains(exitPos))
                        exits.Add(exitPos);
                    break;
            }
        }

        public CellType GetCell(int x, int y)
        {
            ValidatePosition(x, y);
            return grid[y, x];
        }

        public bool IsValidPosition(int x, int y)
        {
            return x >= 0 && x < Size && y >= 0 && y < Size;
        }

        public bool IsWalkable(int x, int y)
        {
            if (!IsValidPosition(x, y))
                return false;

            var cell = grid[y, x];
            return cell != CellType.Wall;
        }

        public int GetCellCost(int x, int y)
        {
            var cellType = GetCell(x, y);

            if (cellType == CellType.Start || cellType == CellType.Exit || cellType == CellType.Path)
                return 1;

            if (obstacleCosts.TryGetValue(cellType, out int cost))
                return cost;

            return 1; // Default cost for unknown cell types
        }

        public void SelectExit(Position exit)
        {
            if (!exits.Contains(exit))
                throw new ArgumentException("Position is not an exit");

            SelectedExit = exit;
        }

        public void ClearSelectedExit()
        {
            SelectedExit = null;
        }

        public bool CanPlaceObstacle(int x, int y)
        {
            if (!IsValidPosition(x, y))
                return false;

            var cellType = grid[y, x];
            return cellType == CellType.Path ||
                   cellType == CellType.Obstacle1 ||
                   cellType == CellType.Obstacle2 ||
                   cellType == CellType.Obstacle3;
        }

        public void PlaceObstacle(int x, int y, CellType obstacleType)
        {
            if (!CanPlaceObstacle(x, y))
                throw new InvalidOperationException($"Cannot place obstacle at ({x}, {y})");

            if (!IsObstacleType(obstacleType))
                throw new ArgumentException("Invalid obstacle type");

            SetCell(x, y, obstacleType);
        }

        public void RemoveObstacle(int x, int y)
        {
            if (!IsValidPosition(x, y))
                return;

            var cellType = grid[y, x];
            if (IsObstacleType(cellType))
            {
                SetCell(x, y, CellType.Path);
            }
        }

        public static bool IsObstacleType(CellType type)
        {
            return type == CellType.Obstacle1 ||
                   type == CellType.Obstacle2 ||
                   type == CellType.Obstacle3;
        }

        public void SetObstacleCost(CellType obstacleType, int cost)
        {
            if (!IsObstacleType(obstacleType))
                throw new ArgumentException("Invalid obstacle type");

            if (cost < 1)
                throw new ArgumentException("Cost must be at least 1");

            obstacleCosts[obstacleType] = cost;
        }

        public char[,] GetCharGrid()
        {
            char[,] charGrid = new char[Size, Size];
            for (int y = 0; y < Size; y++)
                for (int x = 0; x < Size; x++)
                    charGrid[y, x] = (char)grid[y, x];

            return charGrid;
        }

        private void ValidatePosition(int x, int y)
        {
            if (!IsValidPosition(x, y))
                throw new ArgumentOutOfRangeException($"Position ({x}, {y}) is out of bounds");
        }

        public override string ToString()
        {
            var result = new System.Text.StringBuilder();
            for (int y = 0; y < Size; y++)
            {
                for (int x = 0; x < Size; x++)
                    result.Append((char)grid[y, x]);
                result.AppendLine();
            }
            return result.ToString();
        }
    }
}