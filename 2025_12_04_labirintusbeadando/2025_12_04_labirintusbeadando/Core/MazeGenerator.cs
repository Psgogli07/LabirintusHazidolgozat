using System;
using System.Collections.Generic;

namespace _2025_12_04_labirintusbeadando.Core
{
    public class MazeGenerator
    {
        private Random rnd = new Random();
        private bool[,] visited;

        public Maze Generate(int size, int exitCount)
        {
            Maze maze = new Maze(size);

            // Alap falak
            for (int y = 0; y < size; y++)
                for (int x = 0; x < size; x++)
                    maze.SetCell(x, y, '#');

            visited = new bool[size, size];

            // Start bal oldalon
            int startX = 0;
            int startY = rnd.Next(1, size - 1);
            maze.SetCell(startX, startY, 'S');

            // DFS a fal mögötti cellától
            GenerateDFSIterative(maze, startX + 1, startY);

            // Kijáratok
            AddExits(maze, exitCount);

            return maze;
        }

        private void GenerateDFSIterative(Maze maze, int startX, int startY)
        {
            visited = new bool[maze.Size, maze.Size];

            Stack<(int x, int y)> stack = new Stack<(int x, int y)>();
            stack.Push((startX, startY));
            maze.SetCell(startX, startY, ' ');
            visited[startY, startX] = true;

            var directions = new List<(int dx, int dy)>
            {
                (2,0), (-2,0), (0,2), (0,-2)
            };

            while (stack.Count > 0)
            {
                var (x, y) = stack.Pop();

                var shuffledDirs = new List<(int dx, int dy)>(directions);
                Shuffle(shuffledDirs);

                foreach (var (dx, dy) in shuffledDirs)
                {
                    int nx = x + dx;
                    int ny = y + dy;

                    if (!IsInside(maze, nx, ny)) continue;
                    if (visited[ny, nx]) continue;

                    maze.SetCell(x + dx / 2, y + dy / 2, ' '); // fal eltávolítása
                    maze.SetCell(nx, ny, ' ');

                    visited[ny, nx] = true;
                    stack.Push((nx, ny));
                }
            }
        }

        private bool IsInside(Maze maze, int x, int y)
        {
            return x > 0 && y > 0 && x < maze.Size - 1 && y < maze.Size - 1;
        }

        private void Shuffle(List<(int X, int Y)> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = rnd.Next(i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }

        private void AddExits(Maze maze, int exitCount)
        {
            int placed = 0;
            while (placed < exitCount)
            {
                int side = rnd.Next(4);
                int pos = rnd.Next(1, maze.Size - 1);
                int x = 0, y = 0;

                switch (side)
                {
                    case 0: x = maze.Size - 1; y = pos; break;
                    case 1: x = pos; y = 0; break;
                    case 2: x = 0; y = pos; break;
                    case 3: x = pos; y = maze.Size - 1; break;
                }

                int bx = x == 0 ? 1 : (x == maze.Size - 1 ? x - 1 : x);
                int by = y == 0 ? 1 : (y == maze.Size - 1 ? y - 1 : y);

                if (maze.Grid[by, bx] == ' ')
                {
                    maze.SetCell(x, y, 'E');
                    placed++;
                }
            }
        }

        // ===== BFS: legrövidebb út a legközelebbi kijárathoz =====
        public List<Position> FindShortestPath(Maze maze)
        {
            int size = maze.Size;
            bool[,] visited = new bool[size, size];
            Position[,] parent = new Position[size, size];
            Queue<Position> queue = new Queue<Position>();

            Position start = new Position(0, 0);
            List<Position> exits = new List<Position>();

            // Start és exit pozíciók
            for (int y = 0; y < size; y++)
                for (int x = 0; x < size; x++)
                {
                    if (maze.Grid[y, x] == 'S') start = new Position(x, y);
                    if (maze.Grid[y, x] == 'E') exits.Add(new Position(x, y));
                }

            // Válasszuk ki a legközelebbi exitet
            Position end = exits[0];
            int minDist = Math.Abs(end.X - start.X) + Math.Abs(end.Y - start.Y);
            foreach (var e in exits)
            {
                int dist = Math.Abs(e.X - start.X) + Math.Abs(e.Y - start.Y);
                if (dist < minDist)
                {
                    minDist = dist;
                    end = e;
                }
            }

            queue.Enqueue(start);
            visited[start.Y, start.X] = true;

            int[] dx = { 1, -1, 0, 0 };
            int[] dy = { 0, 0, 1, -1 };

            while (queue.Count > 0)
            {
                var pos = queue.Dequeue();

                if (pos.X == end.X && pos.Y == end.Y) break;

                for (int i = 0; i < 4; i++)
                {
                    int nx = pos.X + dx[i];
                    int ny = pos.Y + dy[i];

                    if (nx >= 0 && ny >= 0 && nx < size && ny < size &&
                        !visited[ny, nx] && maze.Grid[ny, nx] != '#')
                    {
                        queue.Enqueue(new Position(nx, ny));
                        visited[ny, nx] = true;
                        parent[ny, nx] = pos;
                    }
                }
            }

            // Útvonal visszafejtése
            List<Position> path = new List<Position>();
            Position p = end;
            while (!(p.X == start.X && p.Y == start.Y))
            {
                path.Add(p);
                p = parent[p.Y, p.X];
            }
            path.Add(start);
            path.Reverse();

            return path;
        }
    }
}
