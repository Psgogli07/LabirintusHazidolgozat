using System;
using System.Collections.Generic;
using System.Linq;

namespace _2025_12_04_labirintusbeadando.Core
{
    public class MazeService
    {
        private MazeGenerator generator;
        private PathFinder pathFinder;
        private Maze currentMaze;
        private List<PathFinder.PathResult> currentPaths;
        private int selectedPathIndex;

        public Maze CurrentMaze => currentMaze;
        public List<PathFinder.PathResult> CurrentPaths => currentPaths;
        public int SelectedPathIndex => selectedPathIndex;
        public PathFinder.PathResult SelectedPath =>
            currentPaths != null && selectedPathIndex >= 0 && selectedPathIndex < currentPaths.Count
            ? currentPaths[selectedPathIndex]
            : null;

        public event EventHandler MazeChanged;
        public event EventHandler PathsChanged;

        public MazeService(int? seed = null)
        {
            generator = new MazeGenerator(seed);
            pathFinder = new PathFinder();
            currentPaths = new List<PathFinder.PathResult>();
            selectedPathIndex = -1;
        }

        public Maze GenerateMaze(int size, int exitCount)
        {
            try
            {
                currentMaze = generator.Generate(size, exitCount);
                currentPaths.Clear();
                selectedPathIndex = -1;

                MazeChanged?.Invoke(this, EventArgs.Empty);
                return currentMaze;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to generate maze: {ex.Message}", ex);
            }
        }

        public void SelectExit(Position exit)
        {
            if (currentMaze == null) return;

            currentMaze.SelectExit(exit);
            currentPaths.Clear();
            selectedPathIndex = -1;

            MazeChanged?.Invoke(this, EventArgs.Empty);
        }

        public void FindShortestPath(bool includeObstacleCosts = true)
        {
            if (currentMaze == null) return;

            var path = pathFinder.FindShortestPath(currentMaze);
            currentPaths = new List<PathFinder.PathResult> { path };
            selectedPathIndex = 0;

            PathsChanged?.Invoke(this, EventArgs.Empty);
        }

        public void FindAlternativePaths(int count)
        {
            if (currentMaze == null) return;

            currentPaths = pathFinder.FindAlternativePaths(currentMaze, count);
            selectedPathIndex = currentPaths.Count > 0 ? 0 : -1;

            PathsChanged?.Invoke(this, EventArgs.Empty);
        }

        public void SelectPath(int index)
        {
            if (currentPaths == null || index < 0 || index >= currentPaths.Count)
                return;

            selectedPathIndex = index;
            PathsChanged?.Invoke(this, EventArgs.Empty);
        }

        public void PlaceObstacle(int x, int y, CellType obstacleType)
        {
            if (currentMaze == null) return;

            currentMaze.PlaceObstacle(x, y, obstacleType);
            currentPaths.Clear();
            selectedPathIndex = -1;

            MazeChanged?.Invoke(this, EventArgs.Empty);
        }

        public void RemoveObstacle(int x, int y)
        {
            if (currentMaze == null) return;

            currentMaze.RemoveObstacle(x, y);
            currentPaths.Clear();
            selectedPathIndex = -1;

            MazeChanged?.Invoke(this, EventArgs.Empty);
        }

        public void SetObstacleCost(CellType obstacleType, int cost)
        {
            if (currentMaze == null) return;

            currentMaze.SetObstacleCost(obstacleType, cost);
            currentPaths.Clear();
            selectedPathIndex = -1;

            MazeChanged?.Invoke(this, EventArgs.Empty);
        }

        public int CalculateMaxExits(int mazeSize)
        {
            return (int)Math.Floor(mazeSize * 0.2);
        }

        public bool ValidateExitCount(int mazeSize, int exitCount)
        {
            int maxExits = CalculateMaxExits(mazeSize);
            return exitCount >= 1 && exitCount <= maxExits;
        }

        public void ClearPaths()
        {
            currentPaths.Clear();
            selectedPathIndex = -1;
            PathsChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}