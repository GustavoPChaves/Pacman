using System;
using System.Collections;
using UnityEngine;

public class GhostAI : MonoBehaviour
{
    GhostMovement _ghostMovement;

    [SerializeField]
    Transform _target;

    Func<MazeCell> _targetFunction;

    [SerializeField]
    GhostType _ghostType;

    MazeCell _currentCell, _targetCell;

    Vector2Int positionInMaze = new Vector2Int(15, 20);

    GhostState _currentState = GhostState.Scatter;

    Coroutine changeStateCoroutine;

    bool _canReverse;

    const float _scatterTime = 7;
    const float _chaseTime = 20;
    const float _frightenedTime = 7;

    [SerializeField]
    bool _isActive;

    public void SetActive(bool option)
    {
        _isActive = option;
    }

    MazeCell ReturnToStartPosition()
    {
        
        return MazeManager.Instance.CellFromPosition(new Vector2Int(15, 20));

    }

    int _stateCycleCount;

    public GhostState CurrentState
    {
        get => _currentState;
        set
        {
            _currentState = value;
            _ghostMovement.Frightened(_currentState == GhostState.Frightened);
            print(_currentState == GhostState.Frightened);
            _ghostMovement.Dead(_currentState == GhostState.Dead);

            _targetFunction = GetTargetFunctionFromState(_currentState, _ghostType);
        }
    }

    public void Reset()
    {
        StopChangeStateCoroutine();
        CurrentState = GhostState.Scatter;
        SetActive(false);
        positionInMaze = new Vector2Int(15, 20);
        _ghostMovement.ResetPosition();
        _targetCell = MazeManager.Instance.CellFromPosition(positionInMaze);
        if(_ghostType == GhostType.Blinky)
        {
            SetActive(true);
        }
        ChangeState();

    }

    Func<MazeCell> GetTargetFunctionFromState(GhostState ghostState, GhostType ghostType)
    {
        switch (ghostState)
        {
            case GhostState.Chase:
                return GetTargetChaseFunctionWithGhostType(ghostType);

            case GhostState.Scatter:
                return () => GetGhostSpotWith(ghostType);

            case GhostState.Frightened:
                return GetTargetChaseFunctionWithGhostType(ghostType);

            case GhostState.Dead:
                return () => ReturnToStartPosition();


        }

        return GetTargetChaseFunctionWithGhostType(ghostType);

    }

    Func<MazeCell> GetTargetChaseFunctionWithGhostType(GhostType ghostType)
    {
        switch (ghostType)
        {
            case GhostType.Blinky:
                return GetPacmanTargetCell;
            case GhostType.Pinky:
                return GetFourUnitsAheadPacmanTargetCell;
            case GhostType.Inky:
                return AheadFromBlinkyTargetCell;
            case GhostType.Clyde:
                return DistanceBasedTargetCell;
        }
        return GetPacmanTargetCell;
    }

    MazeCell GetGhostSpotWith(GhostType ghostType)
    {
        switch (ghostType)
        {
            case GhostType.Blinky:
                return MazeManager.Instance.CellFromPosition(GhostSpot.blinky);

            case GhostType.Pinky:
                return MazeManager.Instance.CellFromPosition(GhostSpot.pinky);

            case GhostType.Inky:
                return MazeManager.Instance.CellFromPosition(GhostSpot.inky);

            case GhostType.Clyde:
                return MazeManager.Instance.CellFromPosition(GhostSpot.clyde);

        }
        return MazeManager.Instance.CellFromPosition(GhostSpot.blinky);

    }

    // Start is called before the first frame update
    void Start()
    {
        _ghostMovement = GetComponent<GhostMovement>();
        InitializeGhost();
    }

    void ChangeState()
    {
        if (_stateCycleCount > 7)
        {
            CurrentState = GhostState.Chase;
            return;
        }

        if (_stateCycleCount % 2 == 0)
        {
            changeStateCoroutine = StartCoroutine(ChangeState(GhostState.Scatter, _scatterTime));
        }
        else
        {
            changeStateCoroutine = StartCoroutine(ChangeState(GhostState.Chase, _chaseTime));
        }
        _stateCycleCount++;

    }
    public void SetFrightened()
    {
        if (CurrentState == GhostState.Dead)
            return;
        StopChangeStateCoroutine();
        changeStateCoroutine = StartCoroutine(ChangeState(GhostState.Frightened, _frightenedTime));
    }
    IEnumerator ChangeState(GhostState ghostState, float stateTime)
    {
        CurrentState = ghostState;

        StartCoroutine(CanReverse());

        yield return new WaitForSeconds(stateTime);

        ChangeState();
    }
    void StopChangeStateCoroutine()
    {
        if (changeStateCoroutine == null)
            return;
        StopCoroutine(changeStateCoroutine);
    }

    IEnumerator CanReverse()
    {
        _canReverse = true;

        yield return new WaitForSeconds(0.3f);
        _canReverse = false;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if(_isActive)
            ChaseAI();
        else
        {
            _ghostMovement.VerticalTilt();
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            SetFrightened();
        }
    }

    public void InitializeGhost()
    {
        SetGhostDirection(Vector2.left);
        ChangeState();

    }

    void ChaseAI()
    {
        _currentCell = MazeManager.Instance.CellFromPosition(positionInMaze);
        _targetCell = _targetFunction();

        if (!_ghostMovement.HasReachTarget())
        {
            _ghostMovement.MoveToTargetPosition();
            return;
        }
        ChooseDirection(CurrentState);
    }



    bool CellIsValid(MazeCell cell)
    {
        return cell != null && !cell.occupied;
    }

    private void ChooseDirection(GhostState ghostState)
    {
        float distanceUp, distanceDown, distanceLeft, distanceRight;

        switch (ghostState)
        {
            case GhostState.Frightened:
                distanceUp = distanceDown = distanceLeft = distanceRight = float.MinValue;
                CalculateDistancesFromCellNeighborToTarget(ref distanceUp, ref distanceDown, ref distanceLeft, ref distanceRight);
                ChooseMaxDistance(distanceUp, distanceDown, distanceLeft, distanceRight);
                break;
            default:
                distanceUp = distanceDown = distanceLeft = distanceRight = float.MaxValue;
                CalculateDistancesFromCellNeighborToTarget(ref distanceUp, ref distanceDown, ref distanceLeft, ref distanceRight);
                ChooseMinimumDistance(distanceUp, distanceDown, distanceLeft, distanceRight);
                break;
        }

    }

    private void ChooseMinimumDistance(float distanceUp, float distanceDown, float distanceLeft, float distanceRight)
    {
        var min = Mathf.Min(distanceUp, distanceDown, distanceLeft, distanceRight);

        if (min == distanceUp)
        {
            SetGhostDirection(Vector2.up);
        }

        else if (min == distanceDown)
        {
            SetGhostDirection(Vector2.down);
        }

        else if (min == distanceLeft)
        {
            SetGhostDirection(Vector2.left);
        }

        else if (min == distanceRight)
        {
            SetGhostDirection(Vector2.right);
        }
    }
    private void ChooseMaxDistance(float distanceUp, float distanceDown, float distanceLeft, float distanceRight)
    {
        var max = Mathf.Max(distanceUp, distanceDown, distanceLeft, distanceRight);

        if (max == distanceUp)
        {
            SetGhostDirection(Vector2.up);
        }

        else if (max == distanceDown)
        {
            SetGhostDirection(Vector2.down);
        }

        else if (max == distanceLeft)
        {
            SetGhostDirection(Vector2.left);
        }

        else if (max == distanceRight)
        {
            SetGhostDirection(Vector2.right);
        }
    }

    private void CalculateDistancesFromCellNeighborToTarget(ref float distanceUp, ref float distanceDown, ref float distanceLeft, ref float distanceRight)
    {
        if (CellIsValid(_currentCell.up) && (!(_ghostMovement.Direction.y < 0) || _canReverse) )
        {
            distanceUp = MazeManager.Instance.distance(_currentCell.up, _targetCell);
        }

        if (CellIsValid(_currentCell.down) && (!(_ghostMovement.Direction.y > 0) || _canReverse))
        {
            distanceDown = MazeManager.Instance.distance(_currentCell.down, _targetCell);
        }

        if (CellIsValid(_currentCell.left) && (!(_ghostMovement.Direction.x > 0) || _canReverse))
        {
            distanceLeft = MazeManager.Instance.distance(_currentCell.left, _targetCell);
        }

        if (CellIsValid(_currentCell.right) && (!(_ghostMovement.Direction.x < 0) || _canReverse))
        {
            distanceRight = MazeManager.Instance.distance(_currentCell.right, _targetCell);
        }
    }

    void SetGhostDirection(Vector2 direction)
    {
        _ghostMovement.Direction = direction;
        positionInMaze.x += (int)direction.x;
        positionInMaze.y += (int)direction.y;
        _ghostMovement.TargetPosition = positionInMaze; 
    }

    protected MazeCell GetPacmanTargetCell()
    {
        Vector2 targetPos = _target.position;
        _targetCell = MazeManager.Instance.CellFromPosition(targetPos);
        return _targetCell;
    }
    protected MazeCell GetFourUnitsAheadPacmanTargetCell()
    {
        Vector2 targetPos = _target.position;
        targetPos += 4 * GameManager.Instance.Pacman.Direction;
        _targetCell = MazeManager.Instance.CellFromPosition(targetPos);
        return _targetCell;
    }
    protected MazeCell AheadFromBlinkyTargetCell()
    {
        Vector2 targetPos = _target.position;
        targetPos += 2 * GameManager.Instance.Pacman.Direction;

        var blinkyPos = GameManager.Instance.Blinky.positionInMaze;

        var positionBasedOnBLinkyPosition = targetPos +  ( targetPos - blinkyPos);

        Debug.DrawLine(new Vector2(blinkyPos.x, blinkyPos.y), positionBasedOnBLinkyPosition, Color.red, 2);
        Debug.DrawLine(new Vector2(blinkyPos.x, blinkyPos.y), targetPos, Color.green, 2);

        _targetCell = MazeManager.Instance.CellFromPosition(positionBasedOnBLinkyPosition);

        return _targetCell;
    }
    protected MazeCell DistanceBasedTargetCell()
    {
        Vector2 targetPos = _target.position;
        var isDistanceEnough = Vector2.Distance(targetPos, transform.position) > 8;

        if (isDistanceEnough)
        {
            _targetCell = MazeManager.Instance.CellFromPosition(targetPos);
            return _targetCell;
        }

        return MazeManager.Instance.CellFromPosition(GhostSpot.clyde);
    }

    public enum GhostType
    {

        Blinky,
        Pinky,
        Inky,
        Clyde
    }

public enum GhostState
    {
        Chase,
        Scatter,
        Frightened,
        Dead
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("GhostHouse"))
        {
            if (CurrentState == GhostAI.GhostState.Dead)
            {
                Reset();
            }
            return;
        }
        if(_currentState == GhostState.Frightened)
        {
            CurrentState = GhostState.Dead;
            AudioManager.Instance.Play(AudioClipType.blueGhost);
            StartCoroutine(DramaticPause());
            StopChangeStateCoroutine();

        }
        else if(CurrentState != GhostAI.GhostState.Dead)
        {
            GameManager.Instance.PacmanDied();

        }
    }

    IEnumerator DramaticPause()
    {
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(0.5f);
        Time.timeScale = 1;
          
    }
}


public abstract class GhostSpot
{
    public static Vector2 blinky = new Vector2(27, 30);
    public static Vector2 pinky = new Vector2(2, 30);
    public static Vector2 clyde = new Vector2(2, 2);
    public static Vector2 inky = new Vector2(27, 2);
}






