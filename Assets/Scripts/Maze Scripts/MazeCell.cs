public class MazeCell
{
    public int AdjacentCount { get; set; }
    public float PositionX { get; set; }
    public float PositionY { get; set; }
    public bool Occupied { get; set; }
    public bool IsIntersection { get; set; }

    public MazeCell left, right, up, down;

    public MazeCell(float x, float y)
    {
        PositionX = x;
        PositionY = y;
        Occupied = false;
        left = right = up = down = null;
    }
}