using System;
using System.Collections;
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

    int _score = 0;
    int lives = 5;

    public PacmanController Pacman { get => _pacman; private set => _pacman = value; }
    public GhostAI Blinky { get => _blinky; private set => _blinky = value; }

    public int pacdotCount;
    // Start is called before the first frame update
    void Start()
    {
        StartGame();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetScore(int score)
    {
        pacdotCount++;
        CheckPelletsCollected();
        _score += score;
        _UIController.SetScore(_score);
        
    }

    void StartGame()
    {
        StartCoroutine(PauseTimeScale(5, null, () => _UIController.SetReadyPanel(false)));
    }

    IEnumerator PauseTimeScale(float time, Action setup = null, Action completition = null)
    {
        setup?.Invoke();

        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(time);
        Time.timeScale = 1;

        completition?.Invoke();
    }

    public void GotEnergizer()
    {
        foreach (var ghost in ghosts)
        {
            ghost.SetFrightened();
        }
    }

    private void CheckWin()
    {
        if(pacdotCount >= 333)
        {
            print("win");
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
       
    }

    void ResetPoints()
    {
        lives = 5;
        pacdotCount = 0;
        _score = 0;
        _UIController.SetScore(_score);
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
        lives--;
        if(lives <= 1)
        {
            RestartGame();
            return;
        }
        ResetGhosts();
        ResetPacman();
        _UIController.SetLives(lives);
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



}
