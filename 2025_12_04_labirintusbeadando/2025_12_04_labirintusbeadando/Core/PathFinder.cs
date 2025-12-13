using System;
using System.Collections.Generic;
using System.Linq;

namespace _2025_12_04_labirintusbeadando.Core
{
    public class PathFinder
    {
        public class PathResult
        {
            public List<Position> Path { get; set; }
            public int TotalCost { get; set; }
            public int PathLength => Path?.Count ?? 0;

            public PathResult(List<Position> path, int totalCost)
            {
                Path = path ?? new List<Position>();
                TotalCost = totalCost;
            }
        }

        public PathResult FindShortestPath(Maze maze, Position targetExit = null)
        {
            if (maze == null)
                return new PathResult(new List<Position>(), 0);

            var exit = targetExit ?? maze.SelectedExit;
            if (exit == null && maze.Exits.Count > 0)
                exit = maze.Exits[0];

            if (exit == null)
                return new PathResult(new List<Position>(), 0);

            return FindPathAStar(maze, maze.Start, exit);
        }

        public List<PathResult> FindAlternativePaths(Maze maze, int count, Position targetExit = null)
        {
            var results = new List<PathResult>();
            if (maze == null || count < 1) return results;

            var exit = targetExit ?? maze.SelectedExit;
            if (exit == null && maze.Exits.Count > 0)
                exit = maze.Exits[0];

            if (exit == null) return results;

            // Find shortest path first
            var shortestPath = FindShortestPath(maze, exit);
            if (shortestPath.Path.Count > 0)
                results.Add(shortestPath);

            // Find alternative paths using Yen's algorithm
            for (int i = 1; i < count && i < 5; i++)
            {
                var altPath = FindKthShortestPath(maze, exit, i);
                if (altPath.Path.Count > 0)
                    results.Add(altPath);
            }

            return results;
        }

        private PathResult FindPathAStar(Maze maze, Position start, Position goal)
        {
            var openSet = new HashSet<Position> { start };
            var cameFrom = new Dictionary<Position, Position>();

            var gScore = new Dictionary<Position, int> { { start, 0 } };
            var fScore = new Dictionary<Position, int> { { start, Heuristic(start, goal) } };

            while (openSet.Count > 0)
            {
                var current = openSet.OrderBy(pos => fScore.ContainsKey(pos) ? fScore[pos] : int.MaxValue).First();

                if (current.Equals(goal))
                {
                    var path = ReconstructPath(cameFrom, current);
                    return new PathResult(path, gScore[current]);
                }

                openSet.Remove(current);

                foreach (var neighbor in GetNeighbors(maze, current))
                {
                    int tentativeGScore = gScore[current] + maze.GetCellCost(neighbor.X, neighbor.Y);

                    if (!gScore.ContainsKey(neighbor) || tentativeGScore < gScore[neighbor])
                    {
                        cameFrom[neighbor] = current;
                        gScore[neighbor] = tentativeGScore;
                        fScore[neighbor] = gScore[neighbor] + Heuristic(neighbor, goal);

                        if (!openSet.Contains(neighbor))
                            openSet.Add(neighbor);
                    }
                }
            }

            return new PathResult(new List<Position>(), 0);
        }

        private PathResult FindKthShortestPath(Maze maze, Position goal, int k)
        {
            // Simple alternative: find path avoiding some cells from the shortest path
            var shortestPath = FindShortestPath(maze, goal);
            if (shortestPath.Path.Count == 0 || k >= shortestPath.Path.Count)
                return new PathResult(new List<Position>(), 0);

            // Temporarily block a random cell from the shortest path (not start or goal)
            var tempMaze = CloneMaze(maze);
            int blockIndex = new Random().Next(1, Math.Min(shortestPath.Path.Count - 1, 5));
            var blockPos = shortestPath.Path[blockIndex];

            if (tempMaze.IsWalkable(blockPos.X, blockPos.Y))
            {
                tempMaze.SetCell(blockPos.X, blockPos.Y, CellType.Wall);
                return FindShortestPath(tempMaze, goal);
            }

            return new PathResult(new List<Position>(), 0);
        }

        private Maze CloneMaze(Maze original)
        {
            var clone = new Maze(original.Size);

            for (int y = 0; y < original.Size; y++)
                for (int x = 0; x < original.Size; x++)
                    clone.SetCell(x, y, original.Grid[y, x]);

            foreach (var exit in original.Exits)
                clone.SelectExit(exit);

            foreach (var kvp in original.ObstacleCosts)
                clone.SetObstacleCost(kvp.Key, kvp.Value);

            return clone;
        }

        private List<Position> GetNeighbors(Maze maze, Position position)
        {
            var neighbors = new List<Position>();
            var directions = new List<Position>
            {
                new Position(1, 0),
                new Position(-1, 0),
                new Position(0, 1),
                new Position(0, -1)
            };

            foreach (var dir in directions)
            {
                int newX = position.X + dir.X;
                int newY = position.Y + dir.Y;

                if (maze.IsValidPosition(newX, newY) && maze.IsWalkable(newX, newY))
                    neighbors.Add(new Position(newX, newY));
            }

            return neighbors;
        }

        private int Heuristic(Position a, Position b)
        {
            // Manhattan distance
            return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
        }

        private List<Position> ReconstructPath(Dictionary<Position, Position> cameFrom, Position current)
        {
            var path = new List<Position> { current };

            while (cameFrom.ContainsKey(current))
            {
                current = cameFrom[current];
                path.Add(current);
            }

            path.Reverse();
            return path;
        }
    }
}