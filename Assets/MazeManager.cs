using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MazeManager : GenericSingletonClass<MazeManager>
{
    private List<MazeCell> _mazeList;
    private MazeCell[,] _mazeArray = new MazeCell[28,31];
    string _data;
    string _path = Path.Combine("Assets/Resources/OriginalMaze.txt");

    public bool debugEnabled = false;

    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        _data = File.ReadAllText(_path);
        _mazeList = GenerateMazeFromData(_data);
    }
    private void Start()
    {
        if (debugEnabled)
        {
            DrawNeighbors();
        }
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
    private void MazeCellFromLineData(int Y, List<MazeCell> mazeCells, string line, Vector2 parentPosition)
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
            _mazeArray[X - 1, Y - 1] = newMazeCell;
            X++;
        }
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
        foreach (MazeCell cell in _mazeList)
        {
            Vector3 pos = new Vector3(cell.positionX, cell.positionY, 0);
            Vector3 up = new Vector3(cell.positionX + 0.1f, cell.positionY + 1, 0);
            Vector3 down = new Vector3(cell.positionX - 0.1f, cell.positionY - 1, 0);
            Vector3 left = new Vector3(cell.positionX - 1, cell.positionY + 0.1f, 0);
            Vector3 right = new Vector3(cell.positionX + 1, cell.positionY - 0.1f, 0);

            if (cell.up != null) Debug.DrawLine(pos, up, Color.white, float.MaxValue);
            if (cell.down != null) Debug.DrawLine(pos, down, Color.white, float.MaxValue);
            if (cell.left != null) Debug.DrawLine(pos, left, Color.white, float.MaxValue);
            if (cell.right != null) Debug.DrawLine(pos, right, Color.white, float.MaxValue);
        }
    }
    void OnDrawGizmos()
    {
        if (debugEnabled && _mazeList != null)
        {
            foreach (MazeCell cell in _mazeList)
            {
                Vector3 pos = new Vector2(cell.positionX, cell.positionY);
                if (!cell.occupied)
                {
                    Gizmos.color = Color.white;
                    Gizmos.DrawWireSphere(pos, 0.5f);
                }
                else
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawWireSphere(pos, 0.5f);
                }
            }
        }
    }
    public MazeCell CellFromPosition(Vector2 position)
    {
        position -= Vector2.one;
        position = new Vector2(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y));

        if (position.x < 0)
            position.x = 0;
        if (position.y < 0)
            position.y = 0;
        if (position.x > 27)
            position.x = 27;
        if (position.y > 30)
            position.y = 30;

        return _mazeArray[(int)position.x, (int)position.y];
    }
    //----------------------------------------------------------------------
    // returns the distance between two cells
    public float distance(MazeCell cell1, MazeCell cell2)
    {
        return Mathf.Sqrt(Mathf.Pow(cell1.positionX - cell2.positionX, 2) + Mathf.Pow(cell1.positionY - cell2.positionY, 2));
    }


}


