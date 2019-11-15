using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MazeManager : GenericSingletonClass<MazeManager>
{

    public List<MazeCell> mazeCells;

    string _data;
    string _path = Path.Combine("Assets/Resources/OriginalMaze.txt");

    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();

        _data = File.ReadAllText(_path);
        mazeCells = GenerateMazeFromData(_data);

    }

    // Update is called once per frame
    void Update()
    {
        DrawNeighbors();
    }

    List<MazeCell> GenerateMazeFromData(string data)
    {
        int Y = 31;
        var mazeCells = new List<MazeCell>();

        using (StringReader reader = new StringReader(data))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {

                 // for every line
                MazeCellFromLineData(Y, mazeCells, line, transform.position);

                Y--;
            }
        }

        // after reading all MazeCells, determine the intersection MazeCells
        DetermineIntersectionsOnMaze(mazeCells);

        return mazeCells;

    }

    private static int MazeCellFromLineData(int Y, List<MazeCell> mazeCells, string line, Vector2 parentPosition)
    {
        int X = 1; 
        for (int i = 0; i < line.Length; ++i)
        {
            MazeCell newMazeCell = new MazeCell(X + parentPosition.x, Y + parentPosition.y);

            // if the MazeCell we read is a valid MazeCell (movable)
            if (line[i] == '1')
            {
                SetupNeighborhoodOfMazeCell(Y, mazeCells, line, i, newMazeCell);
            }
            else
            {
                // if the current MazeCell is not movable
                newMazeCell.occupied = true;
            }

            mazeCells.Add(newMazeCell);
            X++;
        }

        return X;
    }

    private static void SetupNeighborhoodOfMazeCell(int Y, List<MazeCell> mazeCells, string line, int i, MazeCell newMazeCell)
    {
        SetupRightAndLeftNeighborOfMazeCell(mazeCells, line, i, newMazeCell);
        // check for up-down neighbor, starting from second row (Y<30)
        int upNeighbor = mazeCells.Count - line.Length; // up neighbor index
        SetupUpAndDownNeighborhoodOfMazeCell(Y, mazeCells, newMazeCell, upNeighbor);
    }

    private static void SetupUpAndDownNeighborhoodOfMazeCell(int Y, List<MazeCell> mazeCells, MazeCell newMazeCell, int upNeighbor)
    {
        if (HasUpNeighborOnPosition(Y, mazeCells, newMazeCell, upNeighbor))
        {
            mazeCells[upNeighbor].down = newMazeCell;
            newMazeCell.up = mazeCells[upNeighbor];

            // adjust adjcent MazeCell counts of each MazeCell
            newMazeCell.adjacentCount++;
            mazeCells[upNeighbor].adjacentCount++;
        }
    }

    private static void SetupRightAndLeftNeighborOfMazeCell(List<MazeCell> mazeCells, string line, int i, MazeCell newMazeCell)
    {
        // check for left-right neighbor
        if (HasLeftNeighborOnPosition(line, i))
        {
            // assign each MazeCell to the corresponding side of other MazeCell
            newMazeCell.left = mazeCells[mazeCells.Count - 1];
            mazeCells[mazeCells.Count - 1].right = newMazeCell;

            // adjust adjcent MazeCell counts of each MazeCell
            newMazeCell.adjacentCount++;
            mazeCells[mazeCells.Count - 1].adjacentCount++;
        }
    }

    private static bool HasUpNeighborOnPosition(int Y, List<MazeCell> mazeCells, MazeCell newMazeCell, int upNeighbor)
    {
        return Y < 30 && !newMazeCell.occupied && !mazeCells[upNeighbor].occupied;
    }

    private static bool HasLeftNeighborOnPosition(string line, int i)
    {
        return i != 0 && line[i - 1] == '1';
    }

    void DetermineIntersectionsOnMaze(List<MazeCell> mazeCells)
    {
        foreach (MazeCell MazeCell in mazeCells)
        {
            if (MazeCell.adjacentCount > 2)
                MazeCell.isIntersection = true;
        }
    }

    //-----------------------------------------------------------------------
    // Draw lines between neighbor tiles (debug)
    void DrawNeighbors()
    {
        foreach (MazeCell cell in mazeCells)
        {
            Vector3 pos = new Vector3(cell.x, cell.y, 0);
            Vector3 up = new Vector3(cell.x + 0.1f, cell.y + 1, 0);
            Vector3 down = new Vector3(cell.x - 0.1f, cell.y - 1, 0);
            Vector3 left = new Vector3(cell.x - 1, cell.y + 0.1f, 0);
            Vector3 right = new Vector3(cell.x + 1, cell.y - 0.1f, 0);

            if (cell.up != null) Debug.DrawLine(pos, up, Color.white);
            if (cell.down != null) Debug.DrawLine(pos, down, Color.white);
            if (cell.left != null) Debug.DrawLine(pos, left, Color.white);
            if (cell.right != null) Debug.DrawLine(pos, right, Color.white);
        }

    }

    //----------------------------------------------------------------------
    // returns the index in the tiles list of a given tile's coordinates
    public int Index(int X, int Y)
    {
        X -= (int)transform.position.x;
        Y -= (int)transform.position.y;
        // if the requsted index is in bounds
        if (X >= 1 && X <= 28 && Y <= 31 && Y >= 1)
            return (31 - Y) * 28 + X - 1;

        // else, if the requested index is out of bounds
        // return closest in-bounds tile's index 
        if (X < 1) X = 1;
        if (X > 28) X = 28;
        if (Y < 1) Y = 1;
        if (Y > 31) Y = 31;

        return (31 - Y) * 28 + X - 1;
    }

    public int Index(MazeCell cell)
    {

        return (int)((31 - cell.y) * 28 + cell.x - 1);
    }

    //----------------------------------------------------------------------
    // returns the distance between two tiles
    public float distance(MazeCell cell1, MazeCell cell2)
    {
        return Mathf.Sqrt(Mathf.Pow(cell1.x - cell2.x, 2) + Mathf.Pow(cell1.y - cell2.y, 2));
    }


}

public class MazeCell
{
    public float x { get; set; }
    public float y { get; set; }
    public bool occupied { get; set; }
    public int adjacentCount { get; set; }
    public bool isIntersection { get; set; }

    public MazeCell left, right, up, down;

    public MazeCell(float x, float y)
    {
        this.x = x;
        this.y = y;
        occupied = false;
        left = right = up = down = null;
    }


};
