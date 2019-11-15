using System.Collections.Generic;
using UnityEngine;

public class GhostAI : MonoBehaviour
{

    GhostMovement _ghostMovement;

    [SerializeField]
    Transform _target;

    MazeCell _currentCell, _targetCell, _nextCell;

    Vector2Int positionInMaze = new Vector2Int(15, 20);

    // Start is called before the first frame update
    void Start()
    {
        _ghostMovement = GetComponent<GhostMovement>();
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
        _currentCell = MazeManager.Instance.CellFromPosition(positionInMaze);
        _targetCell = GetTargetCell();
        _nextCell = GetNextCell(_currentCell);

        if (!_ghostMovement.HasReachTarget())
        {
            _ghostMovement.MoveToTargetPosition();
            return;
        }

        ChooseDirection();
    }


    bool NextCellIsValid()
    {
        return _nextCell != null && !_nextCell.occupied;
    }
    bool CellIsValid(MazeCell cell)
    {
        return cell != null && !cell.occupied;
    }
    private void ChooseDirection()
    {
        float dist1, dist2, dist3, dist4;
        dist1 = dist2 = dist3 = dist4 = float.MaxValue;

        if (_currentCell.up != null && !_currentCell.up.occupied && !(_ghostMovement.Direction.y < 0))
        {
            dist1 = MazeManager.Instance.distance(_currentCell.up, _targetCell);
        }

        if (_currentCell.down != null && !_currentCell.down.occupied && !(_ghostMovement.Direction.y > 0))
        {
            dist2 = MazeManager.Instance.distance(_currentCell.down, _targetCell);
        }

        if (_currentCell.left != null && !_currentCell.left.occupied && !(_ghostMovement.Direction.x > 0))
        {
            dist3 = MazeManager.Instance.distance(_currentCell.left, _targetCell);
        }

        if (_currentCell.right != null && !_currentCell.right.occupied && !(_ghostMovement.Direction.x < 0))
        {
            dist4 = MazeManager.Instance.distance(_currentCell.right, _targetCell);
        }

        if (!NextCellIsValid())
        {
            // if _ghostMovement moves to right or left and there is wall next tile
            if (_ghostMovement.Direction.x != 0)
            {
                var min = Mathf.Min(dist1, dist2);

                if (min == dist1)
                {
                    SetGhostDirection(Vector2.up);
                }

                else if (min == dist2)
                {
                    SetGhostDirection(Vector2.down);
                }

            }
            // if _ghostMovement moves to up or down and there is wall next tile
            else if (_ghostMovement.Direction.y != 0)
            {
                var min = Mathf.Min(dist3, dist4);
                if (min == dist3)
                {
                    SetGhostDirection(Vector2.left);
                }

                else if (min == dist4)
                {
                    SetGhostDirection(Vector2.right);
                }

            }

        }
        //---------------------------------------------------------------------------------------
        // IF WE ARE AT INTERSECTION
        // calculate the distance to target from each available tile and choose the shortest one
        else if (NextCellIsValid())
        {
            var min = Mathf.Min(dist1, dist2, dist3, dist4);

            if (min == dist1)
            {
                SetGhostDirection(Vector2.up);
            }

            else if (min == dist2)
            {
                SetGhostDirection(Vector2.down);
            }

            else if (min == dist3)
            {
                SetGhostDirection(Vector2.left);
            }

            else if (min == dist4)
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

        _ghostMovement.Direction = direction;
        positionInMaze.x += (int)direction.x;
        positionInMaze.y += (int)direction.y;
        _ghostMovement.TargetPosition = positionInMaze; 
    }

    private MazeCell GetTargetCell()
    {

        Vector2 targetPos = _target.position;
        _targetCell = MazeManager.Instance.CellFromPosition(targetPos);

        return _targetCell;
    }
}


//private void ChooseDirection()
//{
//    //---------------------
//    // IF WE BUMP INTO WALL
//    if (_nextCell == null || _nextCell.occupied && !_currentCell.isIntersection)
//    {
//        // if _ghostMovement moves to right or left and there is wall next tile
//        if (_ghostMovement.Direction.x != 0)
//        {
//            if (_currentCell.down == null)
//            {
//                SetGhostDirection(Vector2.up);
//            }
//            else
//            {
//                SetGhostDirection(Vector2.down);
//            }

//        }

//        // if _ghostMovement moves to up or down and there is wall next tile
//        else if (_ghostMovement.Direction.y != 0)
//        {
//            if (_currentCell.left == null)
//            {
//                SetGhostDirection(Vector2.right);
//            }
//            else
//            {
//                SetGhostDirection(Vector2.left);
//            }
//        }

//    }

//    //---------------------------------------------------------------------------------------
//    // IF WE ARE AT INTERSECTION
//    // calculate the distance to target from each available tile and choose the shortest one
//    else if (!_nextCell.occupied && _currentCell.isIntersection)
//    {

//        float dist1, dist2, dist3, dist4;
//        dist1 = dist2 = dist3 = dist4 = float.MaxValue;
//        if (_currentCell.up != null && !_currentCell.up.occupied && !(_ghostMovement.Direction.y < 0))
//        {
//            dist1 = MazeManager.Instance.distance(_currentCell.up, _targetCell);
//        }

//        if (_currentCell.down != null && !_currentCell.down.occupied && !(_ghostMovement.Direction.y > 0))
//        {
//            dist2 = MazeManager.Instance.distance(_currentCell.down, _targetCell);
//        }

//        if (_currentCell.left != null && !_currentCell.left.occupied && !(_ghostMovement.Direction.x > 0))
//        {
//            dist3 = MazeManager.Instance.distance(_currentCell.left, _targetCell);
//        }

//        if (_currentCell.right != null && !_currentCell.right.occupied && !(_ghostMovement.Direction.x < 0))
//        {
//            dist4 = MazeManager.Instance.distance(_currentCell.right, _targetCell);
//        }

//        float min = Mathf.Min(dist1, dist2, dist3, dist4);
//        if (min == dist1)
//        {
//            SetGhostDirection(Vector2.up);
//        }

//        if (min == dist2)
//        {
//            SetGhostDirection(Vector2.down);
//        }

//        if (min == dist3)
//        {
//            SetGhostDirection(Vector2.left);
//        }

//        if (min == dist4)
//        {
//            SetGhostDirection(Vector2.right);
//        }
//    }



//    // if there is no decision to be made, designate next waypoint for the _ghostMovement
//    else
//    {
//        SetGhostDirection(_ghostMovement.Direction);  // setter updates the waypoint
//    }
//}