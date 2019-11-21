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

    public GhostAI[] ghosts;

    [SerializeField]
    bool _testMode = true;

    int _score;
    int _lives = 5;

    public PacmanController Pacman { get => _pacman; private set => _pacman = value; }
    public GhostAI Blinky { get => _blinky; private set => _blinky = value; }
    public int Score { get => _score; set { _score = value; _UIController.SetScore(_score); } }

    public int Lives { get => _lives; set { _lives = value; _UIController.SetLives(_lives); } }

public int pacdotCount;

    int ghostsEaten;
    int ghostBaseScore = 100;

    List<GameObject> pacdotsEaten, energizersEaten;

    // TODO: level progression
    public void GhostEaten(Vector2 position)
    {
        ghostsEaten++;
        int score = ghostBaseScore * (int)(Mathf.Pow(2, ghostsEaten));
        Score += score;

        _UIController.SetGhostCanvasScore(position, score);
    }


    // Start is called before the first frame update
    void Start()
    {
        pacdotsEaten = new List<GameObject>();
        energizersEaten = new List<GameObject>();
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

    public void SetScore(int score, GameObject pacdot)
    {
        pacdotCount++;

        pacdot?.SetActive(false);
        pacdotsEaten.Add(pacdot);
        CheckPelletsCollected();
        Score += score;
        
    }

    void StartGame()
    {
        StartCoroutine(PauseTimeScale(2, null, () => _UIController.SetReadyPanel(false)));
    }

    IEnumerator PauseTimeScale(float time, Action setup = null, Action completition = null)
    {
        setup?.Invoke();

        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(time);
        Time.timeScale = 1;

        completition?.Invoke();
    }

    public void GotEnergizer(GameObject energizer)
    {
        ghostsEaten = 0;
        energizer?.SetActive(false);
        if(energizer != null)
            energizersEaten.Add(energizer);

        foreach (var ghost in ghosts)
        {
            ghost.SetFrightened();
        }
    }

    private void CheckWin()
    {
        if(pacdotCount >= 333)
        {
            RestartGame();
        }
    }

    private void CheckPelletsCollected()
    {
        if (pacdotCount >= 1)
        {
            ghosts[1].SetActive(true);
        }
        if (pacdotCount >= 30)
        {
            ghosts[2].SetActive(true);
        }
        if (pacdotCount >= 100)
        {
            ghosts[3].SetActive(true);
        }
        CheckWin();
    }

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
        pacdotCount = 0;
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
        StartCoroutine(PauseTimeScale(1));

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

    void ResetCollectibles()
    {
        foreach (var pacdot in pacdotsEaten)
        {
            pacdot.SetActive(true);
        }
        foreach (var energizer in energizersEaten)
        {
            energizer.SetActive(true);
        }
        pacdotsEaten.Clear();
        energizersEaten.Clear();


    }


    public void HackMethodsForTest()
    {
        //Força modo frightened dos fantasmas
        if (Input.GetKeyDown(KeyCode.E))
        {
            GotEnergizer(null);
        }
        //pode quebrar UI, mas é interessante para teste de gameplay
        if (Input.GetKeyDown(KeyCode.Plus))
        {
            Lives++;
        }
        if (Input.GetKeyDown(KeyCode.Minus))
        {
            Lives--;
        }
        //Força saida de Clyde
        if (Input.GetKeyDown(KeyCode.C)){
            ghosts[3].SetActive(true);

        }
        //Força saida de Inky
        if (Input.GetKeyDown(KeyCode.C))
        {
            ghosts[2].SetActive(true);

        }
        //Força saida de Blnky
        if (Input.GetKeyDown(KeyCode.B))
        {
            ghosts[2].SetActive(true);

        }
        //Aumenta velocidade de Pacman
        if (Input.GetKeyDown(KeyCode.P))
        {
            //todo speed global;

        }



    }



}
