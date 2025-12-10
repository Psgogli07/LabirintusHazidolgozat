using _2025_12_04_labirintusbeadando.Core;
namespace _2025_12_04_labirintusbeadando
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Add meg a labirintus méretét (N): ");
            int size = int.Parse(Console.ReadLine()!);

            Console.WriteLine("Hány kijárat legyen? (1-3): ");
            int exits = int.Parse(Console.ReadLine()!);

            var generator = new MazeGenerator();
            Maze maze = generator.Generate(size, exits);

            Console.WriteLine("\n--- Generált Labirintus ---\n");
            PrintMaze(maze);

            Console.WriteLine("\nKész! Nyomj Enter-t a kilépéshez...");
            Console.ReadLine();
        }

        static void PrintMaze(Maze maze)
        {
            for (int y = 0; y < maze.Size; y++)
            {
                for (int x = 0; x < maze.Size; x++)
                {
                    Console.Write(maze.Grid[y, x]);
                }
                Console.WriteLine();
            }
        }
    }
}
