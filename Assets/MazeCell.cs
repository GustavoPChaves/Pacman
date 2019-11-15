
public class MazeCell
{
    public float positionX { get; set; }
    public float positionY { get; set; }
    public bool occupied { get; set; }
    public int adjacentCount { get; set; }
    public bool isIntersection { get; set; }

    public MazeCell left, right, up, down;

    public MazeCell(float x, float y)
    {
        this.positionX = x;
        this.positionY = y;
        occupied = false;
        left = right = up = down = null;
    }

    public MazeCell(MazeCell mazeCell)
    {
        positionX = mazeCell.positionX;
        positionY = mazeCell.positionY;
        occupied = mazeCell.occupied;
        left = mazeCell.left;
        right = mazeCell.right;
        down = mazeCell.down;
        up = mazeCell.up;
        isIntersection = mazeCell.isIntersection;
        adjacentCount = mazeCell.adjacentCount;
    }
}