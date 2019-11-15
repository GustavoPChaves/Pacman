using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostAI : MonoBehaviour
{
    private Vector3 waypoint;			// AI-determined waypoint

    Queue<Vector2> waypoints;
    Vector2 _direction;
    GhostMovement _ghostMovement;

    [SerializeField]
    Transform target;
    [SerializeField]
    List<MazeCell> maze;

    MazeCell currentCell, targetCell, nextCell;

    private Vector3 _startPos = new Vector2(0f, 9f);

    // Start is called before the first frame update
    void Start()
    {
        _ghostMovement = GetComponent<GhostMovement>();
        maze = MazeManager.Instance.mazeCells;
        InitializeGhost();
    }

    // Update is called once per frame
    void Update()
    {
        ChaseAI();
    }
    public void InitializeGhost()
    {
        waypoint = transform.position;  // to avoid flickering animation
        _ghostMovement.SetRigidbodyVelocity(Vector2.right);
    }

    void ChaseAI()
    {

        // if not at waypoint, move towards it
        //if (Vector2.Distance(transform.position, waypoint) > 0.000000000001)
        //{
           // Vector2 p = Vector2.MoveTowards(transform.position, waypoint, 0.3f);
        //_ghostMovement.SetRigidbodyVelocity(p);
        //}

        // if at waypoint, run AI module
        //else 
        AILogic();

    }


    public void AILogic()
    {
        // get current tile
        Vector3 currentPos = new Vector3(transform.position.x + 0.499f, transform.position.y + 0.499f);
        currentCell = maze[MazeManager.Instance.Index((int)currentPos.x, (int)currentPos.y)];

        targetCell = GetTargetTilePerGhost();

        // get the next tile according to Direction
        if (_ghostMovement.Direction.x > 0)
        {
            nextCell = maze[MazeManager.Instance.Index((int)(currentPos.x + 1), (int)currentPos.y)];
        }

        if (_ghostMovement.Direction.x < 0)
        {
            nextCell = maze[MazeManager.Instance.Index((int)(currentPos.x - 1), (int)currentPos.y)];
        }

        if (_ghostMovement.Direction.y > 0)
        {
            nextCell = maze[MazeManager.Instance.Index((int)currentPos.x, (int)(currentPos.y + 1))];
        }

        if (_ghostMovement.Direction.y < 0)
        {
            nextCell = maze[MazeManager.Instance.Index((int)currentPos.x, (int)(currentPos.y - 1))];
        }
        print(nextCell);

        if (nextCell.occupied || currentCell.isIntersection)
        {
            //---------------------
            // IF WE BUMP INTO WALL
            if (nextCell.occupied && !currentCell.isIntersection)
            {
                // if _ghostMovement moves to right or left and there is wall next tile
                if (_ghostMovement.Direction.x != 0)
                {
                    if (currentCell.down == null)
                    {
                        SetGhostDirection(Vector3.up);
                    }
                    else
                    {
                        SetGhostDirection(Vector3.down);
                    }
                }

                // if _ghostMovement moves to up or down and there is wall next tile
                else if (_ghostMovement.Direction.y != 0)
                {
                    if (currentCell.left == null)
                    {
                        SetGhostDirection(Vector3.right);
                    }
                    else
                    {
                        SetGhostDirection(Vector3.left);
                    }
                }

            }

            //---------------------------------------------------------------------------------------
            // IF WE ARE AT INTERSECTION
            // calculate the distance to target from each available tile and choose the shortest one
            if (currentCell.isIntersection)
            {

                float dist1, dist2, dist3, dist4;
                dist1 = dist2 = dist3 = dist4 = 999999f;
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
                    SetGhostDirection(Vector3.up);
                }

                if (min == dist2)
                {
                    SetGhostDirection(Vector3.down);
                }

                if (min == dist3)
                {
                    SetGhostDirection(Vector3.left);
                }

                if (min == dist4)
                {
                    SetGhostDirection(Vector3.right);
                }
            }

        }

        // if there is no decision to be made, designate next waypoint for the _ghostMovement
        else
        {
            SetGhostDirection(_ghostMovement.Direction);  // setter updates the waypoint
        }
    }

    void SetGhostDirection(Vector2 direction)
    {

        Vector2 pos = new Vector3((int)transform.position.x, (int)transform.position.y, (int)transform.position.z);
        waypoint = pos + _direction;
        _ghostMovement.Direction = direction;
    }

    private MazeCell GetTargetTilePerGhost()
    {

        Vector3 targetPos;
        targetPos = new Vector3(target.position.x + 0.499f, target.position.y + 0.499f);
        targetCell = maze[MazeManager.Instance.Index((int)targetPos.x, (int)targetPos.y)];

        return targetCell;
    }
}
