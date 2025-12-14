namespace MazeLib
{
    public enum CellType
    {
        Empty,
        Wall,
        Start,
        Exit,
        Obstacle
    }

    public class Cell
    {
        public int X { get; set; }
        public int Y { get; set; }
        public CellType Type { get; set; }
        public Obstacle Obstacle { get; set; }
        public int Cost
        {
            get { return Obstacle != null ? Obstacle.Cost : 1; }
        }

        public Cell(int x, int y, CellType type = CellType.Empty)
        {
            X = x;
            Y = y;
            Type = type;
        }
    }
}
