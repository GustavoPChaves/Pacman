using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : GenericSingletonClass<GameManager>
{
    [SerializeField]
    UIController _UIController;

    [SerializeField]
    PacmanController _pacman;

    [SerializeField]
    GhostAI _blinky;

    [SerializeField]
    GhostAI[] ghosts;

    [SerializeField]
    bool _testMode = true;

    int _score;
    int _lives = 5;
    int _pelletCount;
    int _ghostsEaten;
    int _ghostBaseScore = 100;

    float _scatterTime = 7;
    float _chaseTime = 20;
    float _frightenedTime = 7;

    List<GameObject> _pelletsEaten, _energizersEaten;


    public PacmanController Pacman { get => _pacman; }

    public GhostAI Blinky { get => _blinky; }

    public int Score
    {
        get => _score;
        private set
        {
            _score = value;
            _UIController.UpdateScore(_score);
        }
    }

    public int Lives
    {
        get => _lives;
        private set
        {
            _lives = value;
            _UIController.UpdateLives(_lives);
        }
    }

    // TODO: level progression

    /// <summary>
    /// Calculate points based on GDD, and show the score on ghost position
    /// </summary>
    /// <param name="position"></param>
    public void GhostEaten(Vector2 position)
    {
        _ghostsEaten++;
        int score = _ghostBaseScore * (int)(Mathf.Pow(2, _ghostsEaten));
        Score += score;

        _UIController.SetGhostCanvasScore(position, score);
    }

    // Start is called before the first frame update
    void Start()
    {
        _pelletsEaten = new List<GameObject>();
        _energizersEaten = new List<GameObject>();
        StartGame();
    }

    // Update is called once per frame
    void Update()
    {
        if (_testMode)
        {
            HackMethodsForTest();
        }
    }

    public void SetScore(int score, GameObject pellet)
    {
        _pelletCount++;
        pellet?.SetActive(false);

        //Save pellet reference to restart the level without instantiate again
        _pelletsEaten.Add(pellet);

        CheckPelletsCollected();
        Score += score;
    }


    void StartGame()
    {
        StartCoroutine(UnityUtils.PauseTimeScale(2, null, () => _UIController.SetReadyPanel(false)));
        SetGhostStateTimes();
    }


    public void GotEnergizer(GameObject energizer = null)
    {
        _ghostsEaten = 0;

        energizer?.SetActive(false);

        //Save energizer reference to restart the level without instantiate again
        if (energizer != null)
            _energizersEaten.Add(energizer);

        foreach (var ghost in ghosts)
        {
            ghost.SetFrightened();
        }
    }

    /// <summary>
    /// Player pass a level, restart the game, increase difficult
    /// </summary>
    private void CheckWin()
    {
        if (_pelletCount >= 333)
        {
            //todo show intermission
            RestartGame();
        }
    }

    /// <summary>
    /// Check if the pelet count has triggered any action defined on GDD
    /// </summary>
    private void CheckPelletsCollected()
    {
        //Release Inky
        if (_pelletCount >= 1)
        {
            ghosts[1].SetActive(true);
        }
        //Release Pinky
        if (_pelletCount >= 30)
        {
            ghosts[2].SetActive(true);
        }
        //Release Clyde
        if (_pelletCount >= 100)
        {
            ghosts[3].SetActive(true);
        }

        CheckWin();
    }

    //Restart all changeble elements of the game, scores, positions, collectibles, without having to reload the scene
    void RestartGame()
    {
        ResetPoints();
        ResetGhosts();
        ResetPacman();
        ResetCollectibles();

    }

    void ResetPoints()
    {
        Lives = 5;
        _pelletCount = 0;
        Score = 0;
    }

    void ResetGhosts()
    {
        foreach (var ghost in ghosts)
        {
            ghost.Reset();
        }
    }

    void ResetPacman()
    {
        _pacman.Reset();
    }

    /// <summary>
    /// Lose a life and restart pacman and ghost positions
    /// </summary>
    void LostLife()
    {
        Lives--;
        if (Lives <= 0)
        {
            RestartGame();
            return;
        }

        ResetGhosts();
        ResetPacman();
        StartCoroutine(UnityUtils.PauseTimeScale(1));

    }

    public void PacmanDied()
    {
        StartCoroutine(Die());
    }


    IEnumerator Die()
    {
        _pacman.Die();
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(0.7f);
        Time.timeScale = 1;
        LostLife();
        yield break;
    }


    /// <summary>
    /// Active all collectibles eaten by pacman
    /// </summary>
    void ResetCollectibles()
    {
        foreach (var pellet in _pelletsEaten)
        {
            pellet.SetActive(true);
        }
        foreach (var energizer in _energizersEaten)
        {
            energizer.SetActive(true);
        }
        _pelletsEaten.Clear();
        _energizersEaten.Clear();
    }

    /// <summary>
    /// Set GhostState times to behave according with level progression
    /// </summary>
    void SetGhostStateTimes()
    {
        foreach (var ghost in ghosts)
        {
            ghost.FrightenedTime = _frightenedTime;
            ghost.ScatterTime = _scatterTime;
            ghost.ChaseTime = _chaseTime;
        }
    }

    /// <summary>
    /// Some shortcuts to test the game without realy having to play it
    /// </summary>
    public void HackMethodsForTest()
    {
        //Force ghotsÚ frightened mode
        if (Input.GetKeyDown(KeyCode.E))
        {
            GotEnergizer();
        }
        //UI doesnt show more than 5, but lives still count.
        if (Input.GetKeyDown(KeyCode.Plus))
        {
            Lives++;
        }
        if (Input.GetKeyDown(KeyCode.Minus))
        {
            Lives--;
        }
        //Force Clyde exit of the GhostHouse
        if (Input.GetKeyDown(KeyCode.C))
        {
            ghosts[3].SetActive(true);
        }
        //Force Inky exit of the GhostHouse
        if (Input.GetKeyDown(KeyCode.I))
        {
            ghosts[2].SetActive(true);
        }
        //Force Pinky exit of the GhostHouse
        if (Input.GetKeyDown(KeyCode.P))
        {
            ghosts[1].SetActive(true);
        }
    }
}
