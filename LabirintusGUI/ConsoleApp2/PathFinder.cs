using System.Collections.Generic;

namespace MazeLib
{
    public class PathFinder
    {
        private Maze maze;
        public PathFinder(Maze maze) { this.maze = maze; }

        public List<Cell> FindShortestPath(Cell target)
        {
            var queue = new Queue<(Cell cell, List<Cell> path)>();
            queue.Enqueue((maze.Start, new List<Cell> { maze.Start }));
            var visited = new HashSet<Cell>();

            while (queue.Count > 0)
            {
                var tuple = queue.Dequeue();
                var current = tuple.cell;
                var path = tuple.path;

                if (current == target) return path;

                visited.Add(current);

                foreach (var neighbor in GetNeighbors(current))
                {
                    if (!visited.Contains(neighbor) && neighbor.Type != CellType.Wall && neighbor.Type != CellType.Obstacle)
                    {
                        var newPath = new List<Cell>(path);
                        newPath.Add(neighbor);
                        queue.Enqueue((neighbor, newPath));
                    }
                }
            }
            return null;
        }

        private IEnumerable<Cell> GetNeighbors(Cell cell)
        {
            int x = cell.X;
            int y = cell.Y;
            var deltas = new (int dx, int dy)[] { (0, 1), (1, 0), (0, -1), (-1, 0) };
            foreach (var delta in deltas)
            {
                int nx = x + delta.dx;
                int ny = y + delta.dy;
                if (nx >= 0 && nx < maze.Width && ny >= 0 && ny < maze.Height)
                    yield return maze.Grid[nx, ny];
            }
        }

        public List<List<Cell>> FindAlternativePaths(Cell target, int maxAlternatives = 3)
        {
            var results = new List<List<Cell>>();
            var queue = new Queue<(Cell cell, List<Cell> path)>();
            queue.Enqueue((maze.Start, new List<Cell> { maze.Start }));

            while (queue.Count > 0 && results.Count < maxAlternatives)
            {
                var tuple = queue.Dequeue();
                var current = tuple.cell;
                var path = tuple.path;

                if (current == target)
                {
                    results.Add(path);
                    continue;
                }

                foreach (var neighbor in GetNeighbors(current))
                {
                    if (!path.Contains(neighbor) && neighbor.Type != CellType.Wall && neighbor.Type != CellType.Obstacle)
                    {
                        var newPath = new List<Cell>(path);
                        newPath.Add(neighbor);
                        queue.Enqueue((neighbor, newPath));
                    }
                }
            }
            return results;
        }
    }
}
