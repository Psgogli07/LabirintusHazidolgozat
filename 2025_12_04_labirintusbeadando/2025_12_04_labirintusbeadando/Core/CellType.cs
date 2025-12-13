namespace _2025_12_04_labirintusbeadando.Core
{
    public enum CellType
    {
        Wall = '#',
        Path = ' ',
        Start = 'S',
        Exit = 'E',
        Obstacle1 = '1',  // Low cost obstacle
        Obstacle2 = '2',  // Medium cost obstacle
        Obstacle3 = '3'   // High cost obstacle
    }
}