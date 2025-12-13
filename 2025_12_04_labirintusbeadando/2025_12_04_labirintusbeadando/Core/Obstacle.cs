namespace _2025_12_04_labirintusbeadando.Core
{
    public class Obstacle
    {
        public CellType Type { get; }
        public string Name { get; }
        public int Cost { get; set; }
        public char Character => (char)Type;

        public Obstacle(CellType type, string name, int defaultCost)
        {
            Type = type;
            Name = name;
            Cost = defaultCost;
        }

        public static readonly Obstacle Mud = new Obstacle(CellType.Obstacle1, "Sár", 2);
        public static readonly Obstacle Water = new Obstacle(CellType.Obstacle2, "Víz", 3);
        public static readonly Obstacle Rocks = new Obstacle(CellType.Obstacle3, "Sziklák", 5);

        public static Obstacle[] AllObstacles => new[] { Mud, Water, Rocks };
    }
}