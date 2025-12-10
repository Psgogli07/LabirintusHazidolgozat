using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2025_12_04_labirintusbeadando.Core
{
    public class Maze
    {
        public int Size { get; }
        public char[,] Grid { get; }

        public Maze(int size)
        {
            Size = size;
            Grid = new char[size, size];
        }

        public void SetCell(int x, int y, char value)
        {
            Grid[y, x] = value;
        }
    }
}
