using System;
using System.Collections.Generic;

namespace MazeLib
{
    public class Maze
    {
        public int Width { get; }
        public int Height { get; }
        public Cell[,] Grid { get; }
        public Cell Start { get; set; }
        public List<Cell> Exits { get; } = new List<Cell>();
        private Random rand = new Random();

        public Maze(int width, int height)
        {
            Width = width;
            Height = height;
            Grid = new Cell[width, height];
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    Grid[x, y] = new Cell(x, y);
        }

        public void Generate()
        {
            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                    Grid[x, y].Type = CellType.Wall;

            int startX = 1;
            int startY = 1;

            Start = Grid[startX, startY];
            Start.Type = CellType.Start;

            Carve(startX, startY);
            CreateExitOnBorder();
        }

        private void Carve(int x, int y)
        {
            Grid[x, y].Type = CellType.Empty;

            var directions = new (int dx, int dy)[]
            {
                (0,-2),(2,0),(0,2),(-2,0)
            };
            Shuffle(directions);

            for (int i = 0; i < directions.Length; i++)
            {
                int dx = directions[i].dx;
                int dy = directions[i].dy;
                int nx = x + dx;
                int ny = y + dy;

                if (IsInside(nx, ny) && Grid[nx, ny].Type == CellType.Wall)
                {
                    Grid[x + dx / 2, y + dy / 2].Type = CellType.Empty;
                    Carve(nx, ny);
                }
            }
        }

        private bool IsInside(int x, int y)
        {
            return x > 0 && y > 0 && x < Width - 1 && y < Height - 1;
        }

        private void Shuffle((int dx, int dy)[] array)
        {
            for (int i = array.Length - 1; i > 0; i--)
            {
                int j = rand.Next(i + 1);
                var temp = array[i];
                array[i] = array[j];
                array[j] = temp;
            }
        }

        private void CreateExitOnBorder()
        {
            var possibleExits = new List<Cell>();

            for (int x = 1; x < Width - 1; x++)
            {
                if (Grid[x, 1].Type == CellType.Empty) possibleExits.Add(Grid[x, 0]);
                if (Grid[x, Height - 2].Type == CellType.Empty) possibleExits.Add(Grid[x, Height - 1]);
            }

            for (int y = 1; y < Height - 1; y++)
            {
                if (Grid[1, y].Type == CellType.Empty) possibleExits.Add(Grid[0, y]);
                if (Grid[Width - 2, y].Type == CellType.Empty) possibleExits.Add(Grid[Width - 1, y]);
            }

            if (possibleExits.Count == 0) return;

            var exit = possibleExits[rand.Next(possibleExits.Count)];
            exit.Type = CellType.Exit;
            Exits.Add(exit);
        }

        public void AddObstacle(int x, int y, Obstacle obstacle)
        {
            var cell = Grid[x, y];
            if (cell.Type == CellType.Empty)
            {
                cell.Type = CellType.Obstacle;
                cell.Obstacle = obstacle;
            }
        }

        public void RemoveRandomWalls(int count)
        {
            var wallCells = new List<Cell>();
            for (int x = 1; x < Width - 1; x++)
                for (int y = 1; y < Height - 1; y++)
                    if (Grid[x, y].Type == CellType.Wall)
                        wallCells.Add(Grid[x, y]);

            int toRemove = Math.Min(count, wallCells.Count);

            for (int i = 0; i < wallCells.Count; i++)
            {
                int j = rand.Next(i, wallCells.Count);
                var temp = wallCells[i];
                wallCells[i] = wallCells[j];
                wallCells[j] = temp;
            }

            for (int i = 0; i < toRemove; i++)
                wallCells[i].Type = CellType.Empty;
        }
    }
}
