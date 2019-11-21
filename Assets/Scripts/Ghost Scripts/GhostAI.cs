using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// The AI was implemented with the original in mind, from http://gameinternals.com/understanding-pac-man-ghost-behavior.
/// Based on pathfinding, the ghost pursue a target, when chase the target is the pacman, when frightened is away from pacman, when scatter is a position on the maze and when dead is the ghost house.
/// After deciding the target, the class GhostMovement will be encharged with physics and animations.
/// </summary>
public class GhostAI : MonoBehaviour
{
    GhostController _ghostController;
    Func<MazeCell> _targetFunction;
    MazeCell _currentCell, _targetCell;

    [SerializeField]
    Transform _target;

    [SerializeField]
    GhostType _ghostType;

    GhostState _currentState = GhostState.Scatter;

    Vector2Int positionInMaze = new Vector2Int(15, 20);

    Coroutine changeStateCoroutine;

    int _stateCycleCount;
    bool _canReverse;
    bool _isActive;

    float _scatterTime = 7;
    float _chaseTime = 20;
    float _frightenedTime = 7;

    /// <summary>
    /// Set CurrentState of the ghost and update the target that the ghost have to pursue
    /// </summary>
    public GhostState CurrentState
    {
        get => _currentState;
        set
        {
            _currentState = value;
            _ghostController.Frightened(_currentState == GhostState.Frightened);
            _ghostController.Dead(_currentState == GhostState.Dead);

            _targetFunction = GetTargetFunctionFromState(_currentState, _ghostType);
        }
    }

    /// <summary>
    /// Time of the states
    /// </summary>
    public float ScatterTime { get => _scatterTime; set => _scatterTime = value; }
    public float ChaseTime { get => _chaseTime; set => _chaseTime = value; }
    public float FrightenedTime { get => _frightenedTime; set => _frightenedTime = value; }


    /// <summary>
    /// Set the ghost is active, get outside of the ghosthouse
    /// </summary>
    /// <param name="option"></param>
    public void SetActive(bool option)
    {
        _isActive = option;
    }

    /// <summary>
    /// Default Ghost Position
    /// </summary>
    /// <returns></returns>
    MazeCell ReturnToStartPosition()
    {
        return MazeManager.Instance.CellFromPosition(new Vector2Int(15, 20));
    }

    /// <summary>
    /// Reset Ghost StateMachine to defaults
    /// </summary>
    public void Reset()
    {
        StopChangeStateCoroutine();
        CurrentState = GhostState.Scatter;
        SetActive(false);
        positionInMaze = new Vector2Int(15, 20);
        _ghostController.ResetPosition();
        _targetCell = MazeManager.Instance.CellFromPosition(positionInMaze);
        if(_ghostType == GhostType.Blinky)
        {
            SetActive(true);
        }
        ChangeState();

    }

    /// <summary>
    /// Every state has a unique target, and every ghostType has a unique behavior
    /// </summary>
    /// <param name="ghostState"></param>
    /// <param name="ghostType"></param>
    /// <returns>The unique Target behavior of the Ghost</returns>
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

    /// <summary>
    /// Every ghost has a strategy to chase the target
    /// </summary>
    /// <param name="ghostType"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Every ghost has a favorite spot to walk when in scatterState
    /// </summary>
    /// <param name="ghostType"></param>
    /// <returns></returns>
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
        _ghostController = GetComponent<GhostController>();
        InitializeGhost();
    }

    /// <summary>
    /// The ghosts change between chase and scatter 8 times, then enter in chase to the end
    /// </summary>
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

    /// <summary>
    /// When a ghost change his state, it can reverse its direction, its like a hint for the player to know when to runaway from chase, or to enjoy the scatter to eat dots
    /// </summary>
    IEnumerator CanReverse()
    {
        _canReverse = true;

        yield return new WaitForSeconds(0.3f);
        _canReverse = false;
    }

    /// <summary>
    /// If ghost is active, it pursues a target, if not, just tilt on ghosthouse
    /// </summary>
    void FixedUpdate()
    {
        if(_isActive)
            ChaseAI();
        else
        {
            _ghostController.VerticalTilt();
        }
    }

    /// <summary>
    /// Blinky is always outside the ghosthouse, Ghost goes left because player goes right
    /// </summary>
    public void InitializeGhost()
    {
        if(_ghostType == GhostType.Blinky)
        {
            SetActive(true);
        }
        SetGhostDirection(Vector2.left);
        ChangeState();
    }

    /// <summary>
    /// With the current cell on maze the ghost chooses a direction to reach the target, if it reached the target already set. This is needed to prevent from change the direction undesirable
    /// </summary>
    void ChaseAI()
    {
        _currentCell = MazeManager.Instance.CellFromPosition(positionInMaze);
        _targetCell = _targetFunction();

        if (!_ghostController.HasReachTarget())
        {
            _ghostController.MoveToTargetPosition();
            return;
        }
        ChooseDirection(CurrentState);
    }


    /// <summary>
    /// A valid cell is a cell the ghost can go without breaking the rules
    /// </summary>
    /// <param name="cell"></param>
    /// <returns></returns>
    bool CellIsValid(MazeCell cell)
    {
        return cell != null && !cell.Occupied;
    }

    /// <summary>
    /// When ghost is frightened, it chooses the furthest direction from the pacman, when chasingl, the nearest
    /// </summary>
    /// <param name="ghostState"></param>
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

    /// <summary>
    /// The distances to be calculated to help it choose wich one is better
    /// </summary>
    /// <param name="distanceUp"></param>
    /// <param name="distanceDown"></param>
    /// <param name="distanceLeft"></param>
    /// <param name="distanceRight"></param>
    private void CalculateDistancesFromCellNeighborToTarget(ref float distanceUp, ref float distanceDown, ref float distanceLeft, ref float distanceRight)
    {
        if (CellIsValid(_currentCell.up) && (!(_ghostController.Direction.y < 0) || _canReverse) )
        {
            distanceUp = MazeManager.Instance.distance(_currentCell.up, _targetCell);
        }

        if (CellIsValid(_currentCell.down) && (!(_ghostController.Direction.y > 0) || _canReverse))
        {
            distanceDown = MazeManager.Instance.distance(_currentCell.down, _targetCell);
        }

        if (CellIsValid(_currentCell.left) && (!(_ghostController.Direction.x > 0) || _canReverse))
        {
            distanceLeft = MazeManager.Instance.distance(_currentCell.left, _targetCell);
        }

        if (CellIsValid(_currentCell.right) && (!(_ghostController.Direction.x < 0) || _canReverse))
        {
            distanceRight = MazeManager.Instance.distance(_currentCell.right, _targetCell);
        }
    }

    /// <summary>
    /// Call ghost controller to set the direction and updates the position in maze
    /// </summary>
    /// <param name="direction"></param>
    void SetGhostDirection(Vector2 direction)
    {
        _ghostController.Direction = direction;
        positionInMaze.x += (int)direction.x;
        positionInMaze.y += (int)direction.y;
        _ghostController.TargetPosition = positionInMaze; 
    }

    protected MazeCell GetPacmanTargetCell()
    {
        Vector2 targetPos = _target.position;
        _targetCell = MazeManager.Instance.CellFromPosition(targetPos);
        return _targetCell;
    }

    /// <summary>
    /// Pinky has a unique strategy, it pursues 4 tiles ahead the pacman, trying to cut its path
    /// </summary>
    /// <returns></returns>
    protected MazeCell GetFourUnitsAheadPacmanTargetCell()
    {
        Vector2 targetPos = _target.position;
        targetPos += 4 * GameManager.Instance.Pacman.Direction;
        _targetCell = MazeManager.Instance.CellFromPosition(targetPos);
        return _targetCell;
    }
    /// <summary>
    /// Inky has a unique strategy, its target is the double length of vector from blinky to pacman
    /// </summary>
    /// <returns></returns>
    protected MazeCell AheadFromBlinkyTargetCell()
    {
        Vector2 targetPos = _target.position;
        targetPos += 2 * GameManager.Instance.Pacman.Direction;

        var blinkyPos = GameManager.Instance.Blinky.positionInMaze;

        var positionBasedOnBLinkyPosition = targetPos +  ( targetPos - blinkyPos);

        //Debug.DrawLine(new Vector2(blinkyPos.x, blinkyPos.y), positionBasedOnBLinkyPosition, Color.red, 2);
        //Debug.DrawLine(new Vector2(blinkyPos.x, blinkyPos.y), targetPos, Color.green, 2);

        _targetCell = MazeManager.Instance.CellFromPosition(positionBasedOnBLinkyPosition);

        return _targetCell;
    }
    /// <summary>
    /// Clyde has a unique strategy, it pursues the pacman, but when it is 8 tiles close, it moves to its spot
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// When dead and trigger the ghosthouse, ghost has to be reseted
    /// If frightened, it was eaten by the pacman
    /// But is it is not dead, the pacman is
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("GhostHouse"))
        {
            if (CurrentState == GhostState.Dead)
            {
                Reset();
            }
            return;
        }
        if(_currentState == GhostState.Frightened)
        {
            GameManager.Instance.GhostEaten(transform.position);

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

    /// <summary>
    /// When a ghost is eaten, the game pauses very fast to give a feel of accomplishment
    /// </summary>
    /// <returns></returns>
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






