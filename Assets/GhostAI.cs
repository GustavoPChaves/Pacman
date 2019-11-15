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

    bool CellIsValid(MazeCell cell)
    {
        return cell != null && !cell.occupied;
    }
    private void ChooseDirection()
    {
        float distanceUp, distanceDown, distanceLeft, distanceRight;
        distanceUp = distanceDown = distanceLeft = distanceRight = float.MaxValue;
        CalculateDistancesFromCellNeighborToTarget(ref distanceUp, ref distanceDown, ref distanceLeft, ref distanceRight);
        ChooseMinimumDistance(distanceUp, distanceDown, distanceLeft, distanceRight);
    }

    private void ChooseMinimumDistance(float dist1, float dist2, float dist3, float dist4)
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

    private void CalculateDistancesFromCellNeighborToTarget(ref float dist1, ref float dist2, ref float dist3, ref float dist4)
    {
        if (CellIsValid(_currentCell.up) && !(_ghostMovement.Direction.y < 0))
        {
            dist1 = MazeManager.Instance.distance(_currentCell.up, _targetCell);
        }

        if (CellIsValid(_currentCell.down) && !(_ghostMovement.Direction.y > 0))
        {
            dist2 = MazeManager.Instance.distance(_currentCell.down, _targetCell);
        }

        if (CellIsValid(_currentCell.left) && !(_ghostMovement.Direction.x > 0))
        {
            dist3 = MazeManager.Instance.distance(_currentCell.left, _targetCell);
        }

        if (CellIsValid(_currentCell.right) && !(_ghostMovement.Direction.x < 0))
        {
            dist4 = MazeManager.Instance.distance(_currentCell.right, _targetCell);
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


