namespace MazeLib
{
    public class Obstacle
    {
        public string Name { get; set; }
        public int Cost { get; set; }

        public Obstacle(string name, int cost)
        {
            Name = name;
            Cost = cost;
        }

        public static Obstacle Hole(int cost) => new Obstacle("Gödör", cost);
        public static Obstacle Trap(int cost) => new Obstacle("Csapda", cost);
        public static Obstacle LockedDoor(int cost) => new Obstacle("Zárt ajtó", cost);
    }
}
