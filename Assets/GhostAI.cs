using System.Collections.Generic;
using UnityEngine;

public class GhostAI : MonoBehaviour
{

    GhostMovement _ghostMovement;

    [SerializeField]
    Transform target;
    [SerializeField]
    List<MazeCell> maze;

    MazeCell currentCell, targetCell, nextCell;

    Vector2Int positionInMaze = new Vector2Int(15, 20);

    // Start is called before the first frame update
    void Start()
    {
        _ghostMovement = GetComponent<GhostMovement>();
        maze = MazeManager.Instance.mazeCells;
        InitializeGhost();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        ChaseAI();
    }
    public void InitializeGhost()
    {
        SetGhostDirection(Vector2.right);
    }

    void ChaseAI()
    {
        Vector2 currentPos = transform.position;
        if (Vector3.Distance(currentPos, _ghostMovement.targetPosition) > 0.01)
        {
            Vector2 p = Vector2.MoveTowards(currentPos, _ghostMovement.targetPosition, 0.1f);
            GetComponent<Rigidbody2D>().MovePosition(p);
            return;
        }
        currentCell = MazeManager.Instance.CellFromPosition(positionInMaze);
        targetCell = GetTargetCell();
        nextCell = GetNextCell(currentCell);

        ChooseDirection();
    }

    private void ChooseDirection()
    {
        //---------------------
        // IF WE BUMP INTO WALL
        if (nextCell == null || nextCell.occupied && !currentCell.isIntersection)
        {
            // if _ghostMovement moves to right or left and there is wall next tile
            if (_ghostMovement.Direction.x != 0)
            {
                if (currentCell.down == null)
                {
                    SetGhostDirection(Vector2.up);
                }
                else
                {
                    SetGhostDirection(Vector2.down);
                }

            }

            // if _ghostMovement moves to up or down and there is wall next tile
            else if (_ghostMovement.Direction.y != 0)
            {
                if (currentCell.left == null)
                {
                    SetGhostDirection(Vector2.right);
                }
                else
                {
                    SetGhostDirection(Vector2.left);
                }
            }

        }

        //---------------------------------------------------------------------------------------
        // IF WE ARE AT INTERSECTION
        // calculate the distance to target from each available tile and choose the shortest one
        else if (!nextCell.occupied && currentCell.isIntersection)
        {

            float dist1, dist2, dist3, dist4;
            dist1 = dist2 = dist3 = dist4 = float.MaxValue;
            if (currentCell.up != null && !currentCell.up.occupied && !(_ghostMovement.Direction.y < 0))
            {
                dist1 = MazeManager.Instance.distance(currentCell.up, targetCell);
            }

            if (currentCell.down != null && !currentCell.down.occupied && !(_ghostMovement.Direction.y > 0))
            {
                dist2 = MazeManager.Instance.distance(currentCell.down, targetCell);
            }

            if (currentCell.left != null && !currentCell.left.occupied && !(_ghostMovement.Direction.x > 0))
            {
                dist3 = MazeManager.Instance.distance(currentCell.left, targetCell);
            }

            if (currentCell.right != null && !currentCell.right.occupied && !(_ghostMovement.Direction.x < 0))
            {
                dist4 = MazeManager.Instance.distance(currentCell.right, targetCell);
            }

            float min = Mathf.Min(dist1, dist2, dist3, dist4);
            if (min == dist1)
            {
                SetGhostDirection(Vector2.up);
            }

            if (min == dist2)
            {
                SetGhostDirection(Vector2.down);
            }

            if (min == dist3)
            {
                SetGhostDirection(Vector2.left);
            }

            if (min == dist4)
            {
                SetGhostDirection(Vector2.right);
            }
        }



        // if there is no decision to be made, designate next waypoint for the _ghostMovement
        else
        {
            SetGhostDirection(_ghostMovement.Direction);  // setter updates the waypoint
        }
    }

    private MazeCell GetCellFromPosition(Vector2 currentPos)
    {
        Debug.DrawLine(currentPos, currentPos * 0.99f, Color.green, 3);
        return maze[MazeManager.Instance.Index((int)Mathf.FloorToInt(currentPos.x), (int)Mathf.FloorToInt(currentPos.y))];
    }

    private MazeCell GetNextCell(Vector2 currentPos)
    {
        // get the next tile according to Direction
        if (_ghostMovement.Direction.IsVectorRight())
        {
            return maze[MazeManager.Instance.Index((int)(currentPos.x + 1), (int)currentPos.y)];
        }

        if (_ghostMovement.Direction.IsVectorLeft())
        {
            return maze[MazeManager.Instance.Index((int)(currentPos.x - 1), (int)currentPos.y)];
        }

        if (_ghostMovement.Direction.IsVectorUp())
        {
            return maze[MazeManager.Instance.Index((int)currentPos.x, (int)(currentPos.y + 1))];
        }

        if (_ghostMovement.Direction.IsVectorDown())
        {
            return maze[MazeManager.Instance.Index((int)currentPos.x, (int)(currentPos.y - 1))];
        }

        return null;
    }
    private MazeCell GetNextCell(MazeCell currentCell)
    {
        // get the next tile according to Direction
        if (_ghostMovement.Direction.IsVectorRight())
        {
            return currentCell.right;
        }

        if (_ghostMovement.Direction.IsVectorLeft())
        {
            return currentCell.left;
        }

        if (_ghostMovement.Direction.IsVectorUp())
        {
            return currentCell.up;
        }

        if (_ghostMovement.Direction.IsVectorDown())
        {
            return currentCell.down;
        }

        return null;
    }
    void SetGhostDirection(Vector2 direction)
    {

        print(positionInMaze);
        _ghostMovement.Direction = direction;
        positionInMaze.x += (int)direction.x;
        positionInMaze.y += (int)direction.y;
 
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.position = new Vector3( positionInMaze.x + 1, positionInMaze.y + 1);
        _ghostMovement.targetPosition = positionInMaze + Vector2.one;
        //_ghostMovement.GoToPosition(positionInMaze);
    }

    private MazeCell GetTargetCell()
    {

        Vector2 targetPos;
        targetPos = new Vector2(target.position.x, target.position.y);
        targetCell = maze[MazeManager.Instance.Index((int)targetPos.x, (int)targetPos.y)];

        return targetCell;
    }
}
